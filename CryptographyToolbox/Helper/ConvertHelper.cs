using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CryptographyToolbox.Tools;

namespace CryptographyToolbox.Helper
{
    public static class ConvertHelper
    {
        public async static Task<string> GetString(byte[] bytes, BinaryMode type)
        {
            try
            {
                switch (type)
                {
                    case BinaryMode.String:
                        throw new Exception("无法转换为字符串");
                    case BinaryMode.Base64:
                        return Convert.ToBase64String(bytes);
                    case BinaryMode.DecWithComma:
                        return GetDecWithCommaString(bytes);
                    case BinaryMode.HexWithDash:
                        return GetHexWithDashString(bytes);
                    case BinaryMode.Hex:
                        return GetHexString(bytes);
                    default:
                        throw new Exception("GetBytes二进制类型不存在");
                }
            }
            catch (Exception ex)
            {
                await ShowError("分析失败", ex);
                return null;
            }
        }

        public async static Task<byte[]> GetBytes(string text, BinaryMode type)
        {
            try
            {
                switch (type)
                {
                    case BinaryMode.String:
                        return Settings.Encoding.GetBytes(text);
                    case BinaryMode.Base64:
                        return Convert.FromBase64String(text);
                    case BinaryMode.DecWithComma:
                        return GetBytesFromDecWithComma(text);
                    case BinaryMode.HexWithDash:
                        return GetBytesFromHexWithDash(text);
                    case BinaryMode.Hex:
                        return GetBytesFromHex(text);
                    default:
                        throw new Exception("GetBytes二进制类型不存在");
                }
            }
            catch (Exception ex)
            {
                await ShowError("分析失败", ex);
                return null;
            }
        }

        private static byte[] GetBytesFromDecWithComma(string text)
        {
            try
            {
                return text.Split(',').Select(p => (byte)int.Parse(p)).ToArray();
            }
            catch
            {
                return null;
            }
        }
        private static string GetDecWithCommaString(byte[] bytes)
        {
            return string.Join(',', bytes);
        }
        private static byte[] GetBytesFromHexWithDash(string text)
        {
            try
            {
                return text.Split('-').Select(p => Convert.ToByte(p, 16)).ToArray();

            }
            catch
            {
                return null;
            }
        }
        private static string GetHexWithDashString(byte[] bytes)
        {
            return BitConverter.ToString(bytes);
        }

        private static byte[] GetBytesFromHex(string text)
        {
            try
            {
                if (text.Length % 2 == 1)
                {
                    return null;
                }
                List<byte> bytes = new List<byte>();
                for (int i = 0; i < text.Length; i += 2)
                {
                    bytes.Add(Convert.ToByte(text.Substring(i, 2)));
                }
                return bytes.ToArray();
            }
            catch
            {
                return null;
            }
        }

        private static string GetHexString(byte[] bytes)
        {
            return GetHexWithDashString(bytes).Replace("-", "");
        }
    }
}
