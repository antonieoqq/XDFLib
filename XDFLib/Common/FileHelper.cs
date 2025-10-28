using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XDFLib
{
    public static class FileHelper
    {
        public const int MaxSizeOfReadFileChunck = 100 * 1024 * 1024; // 100 MB

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

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

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

        public static byte[] Zip_FromString(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            return Zip(bytes);
        }

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
