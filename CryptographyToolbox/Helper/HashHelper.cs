using System.Security.Cryptography;

namespace CryptographyToolbox.Helper
{
    class HashHelper
    {
        public static byte[] Md5(byte[] text)
        {
            using (MD5 md5 = MD5.Create())
            {
                return md5.ComputeHash(text);
            }
        }
        public static byte[] Sha1(byte[] text)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                return sha1.ComputeHash(text);
            }
        }
        public static byte[] Sha256(byte[] text)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(text);
            }
        }
        public static byte[] Sha384(byte[] text)
        {
            using (SHA384 sha = SHA384.Create())
            {
                return sha.ComputeHash(text);
            }
        }
        public static byte[] Sha512(byte[] text)
        {
            using (SHA512 sha = SHA512.Create())
            {
                return sha.ComputeHash(text);
            }
        }
        public static ulong Crc32(byte[] text)
        {
            return GetCRC32Str(text);
        }


        //生成CRC32码表  
        private static ulong[] GetCRC32Table()
        {
            ulong[] Crc32Table;
            ulong Crc;
            Crc32Table = new ulong[256];
            int i, j;
            for (i = 0; i < 256; i++)
            {
                Crc = (ulong)i;
                for (j = 8; j > 0; j--)
                {
                    if ((Crc & 1) == 1)
                        Crc = (Crc >> 1) ^ 0xEDB88320;
                    else
                        Crc >>= 1;
                }
                Crc32Table[i] = Crc;
            }
            return Crc32Table;
        }
        //获取字符串的CRC32校验值  
        private static ulong GetCRC32Str(byte[] buffer)
        {
            //生成码表  
            ulong[] Crc32Table = GetCRC32Table();
            ulong value = 0xffffffff;
            int len = buffer.Length;
            for (int i = 0; i < len; i++)
            {
                value = (value >> 8) ^ Crc32Table[(value & 0xFF) ^ buffer[i]];
            }
            return value ^ 0xffffffff;
        }
    }

}
