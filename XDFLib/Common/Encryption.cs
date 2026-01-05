using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace XDFLib
{
    public static class Encryption
    {
        // --- 配置参数 ---
        private static readonly byte[] StaticSalt = Encoding.UTF8.GetBytes("SimpleAndFast_FixedSalt");
        private static readonly byte[] StaticIv = Encoding.UTF8.GetBytes("SimpleIV16_Bytes");

        private const int LowIterations = 100;
        private const int HighIterations = 50000;
        private const int SaltSize = 16;
        private const int IvSize = 16;

        #region 1. 高性能模式 (Fast - 适用于存档/配置)

        /// <summary>
        /// 将字符串加密为原始字节数组 (固定 Salt/IV)
        /// </summary>
        public static byte[] FastEncrypt(string plainText, string password)
        {
            if (string.IsNullOrEmpty(plainText)) return Array.Empty<byte>();
            byte[] data = Encoding.UTF8.GetBytes(plainText);
            return FastEncrypt(data, password);
        }

        /// <summary>
        /// 将字节数组解密为字符串 (固定 Salt/IV)
        /// </summary>
        public static string FastDecryptToString(byte[] cipherData, string password)
        {
            if (cipherData == null || cipherData.Length == 0) return string.Empty;
            byte[] decrypted = FastDecrypt(cipherData, password);
            return Encoding.UTF8.GetString(decrypted);
        }

        public static byte[] FastEncrypt(byte[] data, string password) =>
            AesProcess(data, password, StaticSalt, StaticIv, LowIterations, true);

        public static byte[] FastDecrypt(byte[] cipherData, string password) =>
            AesProcess(cipherData, password, StaticSalt, StaticIv, LowIterations, false);

        #endregion

        #region 2. 高安全模式 (Secure - 适用于账号/隐私)

        /// <summary>
        /// 将字符串加密为带 Salt/IV 头部的字节数组
        /// </summary>
        public static byte[] SecureEncrypt(string plainText, string password)
        {
            if (string.IsNullOrEmpty(plainText)) return Array.Empty<byte>();
            byte[] data = Encoding.UTF8.GetBytes(plainText);
            return SecureEncrypt(data, password);
        }

        /// <summary>
        /// 将带头部的字节数组解密为字符串
        /// </summary>
        public static string SecureDecryptToString(byte[] fullData, string password)
        {
            if (fullData == null || fullData.Length < (SaltSize + IvSize)) return string.Empty;
            byte[] decrypted = SecureDecrypt(fullData, password);
            return Encoding.UTF8.GetString(decrypted);
        }

        public static byte[] SecureEncrypt(byte[] data, string password)
        {
            if (data == null || data.Length == 0) return Array.Empty<byte>();

            byte[] dynamicSalt = new byte[SaltSize];
            byte[] dynamicIv = new byte[IvSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(dynamicSalt);
                rng.GetBytes(dynamicIv);
            }

            byte[] cipherText = AesProcess(data, password, dynamicSalt, dynamicIv, HighIterations, true);

            byte[] result = new byte[SaltSize + IvSize + cipherText.Length];
            Buffer.BlockCopy(dynamicSalt, 0, result, 0, SaltSize);
            Buffer.BlockCopy(dynamicIv, 0, result, SaltSize, IvSize);
            Buffer.BlockCopy(cipherText, 0, result, SaltSize + IvSize, cipherText.Length);

            return result;
        }

        public static byte[] SecureDecrypt(byte[] fullData, string password)
        {
            if (fullData == null || fullData.Length < (SaltSize + IvSize)) return Array.Empty<byte>();

            byte[] dynamicSalt = new byte[SaltSize];
            byte[] dynamicIv = new byte[IvSize];
            Buffer.BlockCopy(fullData, 0, dynamicSalt, 0, SaltSize);
            Buffer.BlockCopy(fullData, SaltSize, dynamicIv, 0, IvSize);

            int cipherLength = fullData.Length - SaltSize - IvSize;
            byte[] cipherText = new byte[cipherLength];
            Buffer.BlockCopy(fullData, SaltSize + IvSize, cipherText, 0, cipherLength);

            return AesProcess(cipherText, password, dynamicSalt, dynamicIv, HighIterations, false);
        }

        #endregion

        #region 3. 辅助接口 (手动调用)

        public static string ToBase64(byte[] data) => data == null ? string.Empty : Convert.ToBase64String(data);
        public static byte[] FromBase64(string base64) => string.IsNullOrEmpty(base64) ? Array.Empty<byte>() : Convert.FromBase64String(base64);

        #endregion

        #region 4. 核心逻辑

        private static byte[] AesProcess(byte[] data, string password, byte[] salt, byte[] iv, int iterations, bool isEncrypt)
        {
            if (data == null || data.Length == 0) return Array.Empty<byte>();

            using (var aes = Aes.Create())
            {
                using (var derive = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
                {
                    aes.Key = derive.GetBytes(32);
                }
                aes.IV = iv;

                using (var ms = new MemoryStream())
                {
                    var transform = isEncrypt ? aes.CreateEncryptor() : aes.CreateDecryptor();
                    using (var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                    }
                    return ms.ToArray();
                }
            }
        }

        #endregion
    }
}
