using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XDFLib.Collections;

namespace XDFLib
{
    public static class FileHelper
    {
        public const int MaxSizeOfReadFileChunck = 100 * 1024 * 1024; // 100 MB

        private const int BufferSize = 128 * 1024; // 128KB
        private const int SampleSize = 4096;      // 4KB

        public static void TryOpenDirAndSelectFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                Process.Start("explorer", "/select,\"" + filePath + "\"");
            }
        }

        public static void MakeSureSaveDirectory(string directory)
        {
            var dir = new DirectoryInfo(directory);
            if (dir.Exists)
                return;
            dir.Create();
        }

        public static string JoinPathes(params string[] pathes)
        {
            int totalLength = pathes.Length;
            foreach (var p in pathes)
            {
                totalLength += p.Length;
            }

            StringBuilder strB = new StringBuilder(totalLength);
            bool isFirst = true;
            char prevLastChar = char.MinValue;
            foreach (var p in pathes)
            {
                if (p.Length == 0)
                {
                    continue;
                }
                char currFirstChar = p[0];

                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    if (prevLastChar != '\\' && prevLastChar != '/'
                        && currFirstChar != '\\' && currFirstChar != '/')
                    {
                        strB.Append('/');
                    }
                }

                strB.Append(p);
                prevLastChar = p[p.Length - 1];
            }

            strB = strB.Replace('\\', '/');
            return strB.ToString();
        }

        public static void ReadLinesToCollection(string path, ICollection<string> dest)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    dest.Add(line);
                }
            }
        }

        public static async Task ReadLinesToCollectionAsync(string path, ICollection<string> dest)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while (true)
                {
                    line = await sr.ReadLineAsync();
                    if (line == null)
                    {
                        break;
                    }
                    dest.Add(line);
                }
            }
        }

        public static string ReadStringFromFile(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                return sr.ReadToEnd();
            }
        }

        public static void SaveStringToFile(string path, string content)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(content);
            }
        }

        public static bool TryCreateDirectoryAndSaveStringToFile(string path, string content)
        {
            var dir = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(dir)) { return false; }
            MakeSureSaveDirectory(dir);
            SaveStringToFile(path, content);
            return true;
        }

        public static async Task<string> ReadStringFromFileAsync(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string text = await sr.ReadToEndAsync();
                return text;
            }
        }

        public static async Task SaveStringToFileAsync(string path, string content)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                await sw.WriteAsync(content);
            }
        }

        public static void CopyFile(string sourcePath, string destinationPath)
        {
            var dir = Path.GetDirectoryName(destinationPath);
            Directory.CreateDirectory(dir);
            using (var source = File.Open(sourcePath, FileMode.Open))
            {
                using (var dest = File.Create(destinationPath))
                {
                    source.CopyTo(dest);
                }
            }
        }

        public static async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            var dir = Path.GetDirectoryName(destinationPath);
            Directory.CreateDirectory(dir);

            using (var source = File.Open(sourcePath, FileMode.Open))
            {
                using (var dest = File.Create(destinationPath))
                {
                    await source.CopyToAsync(dest);
                }
            }
        }

        /// <summary>
        /// 注意，不支持超过2G的单个文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] ReadByteArrayFromFile(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                byte[] fileBytes = new byte[fs.Length];
                fs.Read(fileBytes);
                return fileBytes;
            }
        }

        public static byte[] ComputeHash(byte[] bytes)
        {
            var hashBytes = new MD5CryptoServiceProvider().ComputeHash(bytes);
            return hashBytes;
        }

        public static byte[] ComputeHash(Stream stream)
        {
            var hashBytes = new MD5CryptoServiceProvider().ComputeHash(stream);
            return hashBytes;
        }

        public static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }


        public static bool AreTwoByteArraysEqual(byte[] arr1, byte[] arr2)
        {
            bool isEqual = false;
            if (arr1.Length == arr2.Length)
            {
                int i = 0;
                while ((i < arr1.Length) && (arr1[i] == arr2[i]))
                {
                    i += 1;
                }
                if (i == arr1.Length)
                {
                    isEqual = true;
                }
            }
            return isEqual;
        }

        public static byte[] ComputeHashOfFile(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var hashBytes = ComputeHash(fs);
                return hashBytes;
            }
        }

        public static bool ComapreTwoFilesViaHash(string filePath1, string filePath2)
        {
            var fileHash1 = ComputeHashOfFile(filePath1);
            var fileHash2 = ComputeHashOfFile(filePath2);
            return AreTwoByteArraysEqual(fileHash1, fileHash2);
        }

        public static string ComputeHashStringOfFile(string path)
        {
            var fileHash = ComputeHashOfFile(path);
            var fileHashString = ByteArrayToString(fileHash);
            return fileHashString;
        }

        public static bool AreFilesEqual(string path1, string path2)
        {
            var file1 = new FileInfo(path1);
            var file2 = new FileInfo(path2);

            // 1. 基础长度校验
            if (file1.Length != file2.Length) return false;
            if (string.Equals(file1.FullName, file2.FullName, StringComparison.OrdinalIgnoreCase)) return true;
            if (file1.Length == 0) return true;

            // 2. 抽样校验 (头、中、尾) - 极速排除差异文件
            if (!PassSampleCheck(file1, file2)) return false;

            // 3. 完整校验 (使用 ArrayPool)
            return PassFullCheck(file1, file2);
        }

        private static bool PassSampleCheck(FileInfo f1, FileInfo f2)
        {
            // 如果文件太小，采样没意义，直接走全量比对
            if (f1.Length <= SampleSize * 3) return true;

            using var s1 = new RentedArray<byte>(SampleSize);
            using var s2 = new RentedArray<byte>(SampleSize);

            using var fs1 = f1.OpenRead();
            using var fs2 = f2.OpenRead();

            long[] offsets = { 0, f1.Length / 2 - SampleSize / 2, f1.Length - SampleSize };

            foreach (var offset in offsets)
            {
                fs1.Seek(offset, SeekOrigin.Begin);
                fs2.Seek(offset, SeekOrigin.Begin);

                // 使用 Span 确保读取长度准确
                int read1 = fs1.Read(s1.Span);
                int read2 = fs2.Read(s2.Span);

                if (read1 != read2 || !s1.Span.SequenceEqual(s2.Span))
                    return false;
            }

            return true;
        }

        private static bool PassFullCheck(FileInfo f1, FileInfo f2)
        {
            using var b1 = new RentedArray<byte>(BufferSize);
            using var b2 = new RentedArray<byte>(BufferSize);

            using var fs1 = f1.OpenRead();
            using var fs2 = f2.OpenRead();

            while (true)
            {
                // 在 .NET Standard 2.1 中，FileStream.Read(Span<byte>) 是内置支持的
                int read1 = fs1.Read(b1.Span);
                int read2 = fs2.Read(b2.Span);

                if (read1 != read2) return false;
                if (read1 == 0) break; // 读取完毕

                // 使用 Span 进行矢量化比对
                if (!b1.Span.Slice(0, read1).SequenceEqual(b2.Span.Slice(0, read2)))
                    return false;
            }

            return true;
        }

        public static async Task<bool> AreFilesEqualAsync(string path1, string path2, CancellationToken cancellationToken = default)
        {
            var file1 = new FileInfo(path1);
            var file2 = new FileInfo(path2);

            // 1. 物理检查 (同步即可)
            if (file1.Length != file2.Length) return false;
            if (string.Equals(file1.FullName, file2.FullName, StringComparison.OrdinalIgnoreCase)) return true;
            if (file1.Length == 0) return true;

            // 2. 采样检查 (异步)
            if (!await PassSampleCheckAsync(file1, file2, cancellationToken)) return false;

            // 3. 全量检查 (异步)
            return await PassFullCheckAsync(file1, file2, cancellationToken);
        }

        private static async Task<bool> PassSampleCheckAsync(FileInfo f1, FileInfo f2, CancellationToken ct)
        {
            if (f1.Length <= SampleSize * 3) return true;

            byte[] s1 = ArrayPool<byte>.Shared.Rent(SampleSize);
            byte[] s2 = ArrayPool<byte>.Shared.Rent(SampleSize);
            try
            {
                using var fs1 = f1.OpenRead();
                using var fs2 = f2.OpenRead();

                long[] offsets = { 0, f1.Length / 2 - SampleSize / 2, f1.Length - SampleSize };

                foreach (var offset in offsets)
                {
                    fs1.Seek(offset, SeekOrigin.Begin);
                    fs2.Seek(offset, SeekOrigin.Begin);

                    // .NET Standard 2.1: ReadAsync 接受 Memory<byte>
                    int read1 = await fs1.ReadAsync(s1.AsMemory(0, SampleSize), ct);
                    int read2 = await fs2.ReadAsync(s2.AsMemory(0, SampleSize), ct);

                    if (read1 != read2 || !s1.AsSpan(0, read1).SequenceEqual(s2.AsSpan(0, read2)))
                        return false;
                }
                return true;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(s1);
                ArrayPool<byte>.Shared.Return(s2);
            }
        }

        private static async Task<bool> PassFullCheckAsync(FileInfo f1, FileInfo f2, CancellationToken ct)
        {
            byte[] b1 = ArrayPool<byte>.Shared.Rent(BufferSize);
            byte[] b2 = ArrayPool<byte>.Shared.Rent(BufferSize);

            try
            {
                // 使用 FileOptions.Asynchronous 优化大文件 IO
                using var fs1 = new FileStream(f1.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, FileOptions.Asynchronous);
                using var fs2 = new FileStream(f2.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, FileOptions.Asynchronous);

                while (true)
                {
                    // 并行发出读取请求，提高 IO 利用率
                    var t1 = fs1.ReadAsync(b1.AsMemory(0, BufferSize), ct);
                    var t2 = fs2.ReadAsync(b2.AsMemory(0, BufferSize), ct);

                    int r1 = await t1;
                    int r2 = await t2;

                    if (r1 != r2) return false;
                    if (r1 == 0) break;

                    // 比较时使用 Span 触发 SIMD 优化
                    if (!b1.AsSpan(0, r1).SequenceEqual(b2.AsSpan(0, r2)))
                        return false;
                }
                return true;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(b1);
                ArrayPool<byte>.Shared.Return(b2);
            }
        }

        [Obsolete("Use AreFilesEqual(Async) instead")]
        public static bool CompareTwoFilesStraightforward(string filePath1, string filePath2)
        {
            if (filePath1 == filePath2)
            {
                return true;
            }

            var fi1 = new FileInfo(filePath1);
            var fi2 = new FileInfo(filePath2);

            if (fi1.Length != fi2.Length)
            {
                return false;
            }

            using (var fs1 = new FileStream(filePath1, FileMode.Open))
            using (var fs2 = new FileStream(filePath2, FileMode.Open))
            {
                if (fi1.Length < MaxSizeOfReadFileChunck)
                {
                    byte[] fBytes1 = new byte[fs1.Length];
                    fs1.Read(fBytes1);

                    byte[] fBytes2 = new byte[fs2.Length];
                    fs2.Read(fBytes2);

                    return AreTwoByteArraysEqual(fBytes1, fBytes2);
                }
                else
                {
                    byte[] fBytes1 = new byte[MaxSizeOfReadFileChunck];
                    byte[] fBytes2 = new byte[MaxSizeOfReadFileChunck];

                    while (true)
                    {
                        int readLen1 = fs1.Read(fBytes1, 0, MaxSizeOfReadFileChunck);
                        int readlen2 = fs2.Read(fBytes2, 0, MaxSizeOfReadFileChunck);
                        if (readLen1 != readlen2 || readLen1 == 0)
                        {
                            return false;
                        }
                        else
                        {
                            return AreTwoByteArraysEqual(fBytes1, fBytes2);
                        }
                    }
                }
            }
        }

        [Obsolete("Use Compressor instead")]
        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        [Obsolete("Use Compressor instead")]
        public static byte[] Zip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        [Obsolete("Use Compressor instead")]
        public static byte[] Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    CopyTo(gs, mso);
                }

                return mso.ToArray();
            }
        }

        [Obsolete("Use Compressor.ZipFromStringAsync instead")]
        public static byte[] Zip_FromString(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            return Zip(bytes);
        }

        [Obsolete("Use Compressor.UnzipToStringAsync instead")]
        public static string Unzip_ToString(byte[] bytes)
        {
            var unziped = Unzip(bytes);
            return Encoding.UTF8.GetString(unziped);
        }

        /// <summary>
        /// Compresses the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        [Obsolete("Use Compressor.ZipFromStringAsync instead")]
        public static string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            using (var memoryStream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gZipStream.Write(buffer, 0, buffer.Length);
                }

                memoryStream.Position = 0;

                var compressedData = new byte[memoryStream.Length];
                memoryStream.Read(compressedData, 0, compressedData.Length);

                var gZipBuffer = new byte[compressedData.Length + 4];
                Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
                return Convert.ToBase64String(gZipBuffer);
            }
        }

        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns></returns>
        [Obsolete("Use Compressor.UnzipToStringAsync instead")]
        public static string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        #region Encrypt
        private static readonly byte[] EncryptKey = new byte[32] { 121, 123, 23, 14, 161, 141, 37, 68, 126, 213, 16, 134, 168, 217, 58, 108, 46, 218, 5, 78, 28, 128, 113, 208, 61, 56, 10, 87, 187, 162, 233, 38 };
        private static readonly byte[] EncryptIV = new byte[16] { 32, 241, 16, 11, 108, 13, 14, 248, 4, 86, 56, 5, 60, 76, 16, 191 };

        /// <summary>
        /// 把字符串加密并返回字节串
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        [Obsolete("Use XDFLib.Encryption instead")]
        public static byte[] EncryptStringToBytes(string plainText)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            //if (Key == null || Key.Length <= 0)
            //    throw new ArgumentNullException("Key");
            //if (IV == null || IV.Length <= 0)
            //    throw new ArgumentNullException("IV");

            byte[] encrypted;

            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = EncryptKey;
                rijAlg.IV = EncryptIV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        /// <summary>
        /// 把字节串解密并返回字符串
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>        
        [Obsolete("Use XDFLib.Encryption instead")]
        public static string DecryptStringFromBytes(byte[] cipherText, Action onException = null)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                return null;
            //if (Key == null || Key.Length <= 0)
            //    throw new ArgumentNullException("Key");
            //if (IV == null || IV.Length <= 0)
            //    throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = EncryptKey;
                rijAlg.IV = EncryptIV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            try
                            {
                                plaintext = srDecrypt.ReadToEnd();
                            }
                            catch
                            {
                                plaintext = null;
                                if (onException != null)
                                {
                                    onException();
                                }
                            }
                        }
                    }
                }
            }
            return plaintext;
        }
        #endregion

    }
}
