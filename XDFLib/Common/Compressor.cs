using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XDFLib
{
    public static class Compressor
    {
        private const int DefaultBufferSize = 81920;

        // .NET Standard 2.1 中数组的最大长度。
        // 在 64 位环境下单个数组大小限制约为 2GB。
        private const int MaxArrayLength = 0X7FFFFFC7;

        #region 1. Stream 版本 (支持任意大小 + 进度 + 中断)

        /// <summary>
        /// 高性能异步压缩。适用于大文件，内存占用极低且恒定。
        /// </summary>
        public static async Task ZipAsync(Stream inputStream, Stream outputStream, CompressionLevel compLv = CompressionLevel.Optimal,
            IProgress<long> progress = null, CancellationToken ct = default)
        {
            // 写入 8 字节原始长度 Header
            long originalLength = inputStream.CanSeek ? inputStream.Length : -1;
            byte[] header = new byte[8];
            BinaryPrimitives.WriteInt64LittleEndian(header, originalLength);
            await outputStream.WriteAsync(header.AsMemory(), ct);

            // 使用 BrotliStream，设置 leaveOpen 为 true 以便外部管理流的生命周期
            using (var gs = new BrotliStream(outputStream, compLv, leaveOpen: true))
            {
                byte[] buffer = ArrayPool<byte>.Shared.Rent(DefaultBufferSize);
                long totalRead = 0;
                try
                {
                    int bytesRead;
                    // .NET Standard 2.1 原生支持 ReadAsync(Memory<byte>)
                    while ((bytesRead = await inputStream.ReadAsync(buffer.AsMemory(0, buffer.Length), ct)) > 0)
                    {
                        await gs.WriteAsync(buffer.AsMemory(0, bytesRead), ct);
                        totalRead += bytesRead;
                        progress?.Report(totalRead);

                        // 协作式取消检查
                        ct.ThrowIfCancellationRequested();
                    }
                    await gs.FlushAsync(ct);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }

        /// <summary>
        /// 高性能异步解压。
        /// </summary>
        public static async Task UnzipAsync(Stream compressedStream, Stream outputStream,
            IProgress<long> progress = null, CancellationToken ct = default)
        {
            byte[] header = new byte[8];
            int readHeader = 0;
            // 循环读取以确保读满 8 字节 Header
            while (readHeader < 8)
            {
                int r = await compressedStream.ReadAsync(header.AsMemory(readHeader, 8 - readHeader), ct);
                if (r <= 0) throw new InvalidDataException("压缩数据流已损坏或不完整");
                readHeader += r;
            }

            using (var gs = new BrotliStream(compressedStream, CompressionMode.Decompress, leaveOpen: true))
            {
                byte[] buffer = ArrayPool<byte>.Shared.Rent(DefaultBufferSize);
                long totalWritten = 0;
                try
                {
                    int bytesRead;
                    while ((bytesRead = await gs.ReadAsync(buffer.AsMemory(0, buffer.Length), ct)) > 0)
                    {
                        await outputStream.WriteAsync(buffer.AsMemory(0, bytesRead), ct);
                        totalWritten += bytesRead;
                        progress?.Report(totalWritten);
                        ct.ThrowIfCancellationRequested();
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }

        #endregion

        #region 2. Byte[] 版本 (处理 2GB 以内的内存块)

        /// <summary>
        /// 压缩字节数组。
        /// </summary>
        public static async Task<byte[]> ZipAsync(byte[] bytes, CompressionLevel compLv = CompressionLevel.Optimal, CancellationToken ct = default)
        {
            if (bytes == null || bytes.Length == 0) return Array.Empty<byte>();

            using (var mso = new MemoryStream())
            {
                byte[] header = new byte[8];
                BinaryPrimitives.WriteInt64LittleEndian(header, bytes.LongLength);
                await mso.WriteAsync(header.AsMemory(), ct);

                using (var gs = new BrotliStream(mso, compLv, leaveOpen: true))
                {
                    await gs.WriteAsync(bytes.AsMemory(), ct);
                }
                return mso.ToArray();
            }
        }

        /// <summary>
        /// 解压字节数组。利用 Header 信息预分配内存池，性能极高。
        /// </summary>
        public static async Task<byte[]> UnzipAsync(byte[] compressedBytes, CancellationToken ct = default)
        {
            if (compressedBytes == null || compressedBytes.Length < 8) return Array.Empty<byte>();

            // 利用 ReadOnlySpan 零拷贝读取 Header
            long originalSize = BinaryPrimitives.ReadInt64LittleEndian(compressedBytes.AsSpan(0, 8));

            // 如果原始大小在 2GB 的数组限制内，走优化路径
            if (originalSize > 0 && originalSize < MaxArrayLength)
            {
                using (var msi = new MemoryStream(compressedBytes, 8, compressedBytes.Length - 8))
                using (var gs = new BrotliStream(msi, CompressionMode.Decompress))
                {
                    byte[] destination = ArrayPool<byte>.Shared.Rent((int)originalSize);
                    try
                    {
                        int totalRead = 0;
                        int read;
                        while ((read = await gs.ReadAsync(destination.AsMemory(totalRead, (int)originalSize - totalRead), ct)) > 0)
                        {
                            totalRead += read;
                        }

                        // 将池中借用的数组拷贝到最终结果
                        byte[] result = new byte[totalRead];
                        Buffer.BlockCopy(destination, 0, result, 0, totalRead);
                        return result;
                    }
                    finally
                    {
                        ArrayPool<byte>.Shared.Return(destination);
                    }
                }
            }

            // fallback: 如果 Header 信息缺失或超大，回退到流式处理
            using (var msi = new MemoryStream(compressedBytes))
            using (var mso = new MemoryStream())
            {
                await UnzipAsync(msi, mso, null, ct);
                return mso.ToArray();
            }
        }
        #endregion

        #region 3. string <=> Byte[] 版本 (依然有 2GB 以内的限制)

        public static async Task<byte[]> ZipFromStringAsync(string text, CompressionLevel compLv = CompressionLevel.Optimal, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(text)) return Array.Empty<byte>();

            var bytes = Encoding.UTF8.GetBytes(text);
            return await ZipAsync(bytes, compLv, ct);
        }

        public static async Task<string> UnzipToStringAsync(byte[] compressedBytes, CancellationToken ct = default)
        {
            var bytes = await UnzipAsync(compressedBytes, ct);
            if (bytes.Length == 0) { return string.Empty; }

            return Encoding.UTF8.GetString(bytes);
        }
        #endregion
    }
}
