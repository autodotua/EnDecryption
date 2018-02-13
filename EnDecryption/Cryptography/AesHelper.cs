using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using static EnDecryption.Tools;
using Windows.Storage.AccessCache;
using Windows.Storage;

namespace EnDecryption
{
    public static class AesHelper
    {
        public static byte[] Encrypte(byte[] source,byte[] key,byte[] iv, CipherMode mode, PaddingMode padding)
        {
            using (var aes = Aes.Create())
            {
                //byte[] encrypted;
                aes.Key = key;

                aes.Mode = mode;
                aes.Padding = padding;
                aes.IV = mode==CipherMode.ECB?new byte[16]: iv;

                //using (MemoryStream memory = new MemoryStream())
                //{
                //    using (CryptoStream crypto = new CryptoStream(memory, encryptor, CryptoStreamMode.Write))
                //    {
                //        using (StreamWriter writer = new StreamWriter(crypto))
                //        {
                //            writer.Write(text);
                //        }
                //        encrypted = memory.ToArray();
                //    }
                //}
                //MemoryStream memory = new MemoryStream();
                //CryptoStream crypto = new CryptoStream(memory, aes.CreateEncryptor(), CryptoStreamMode.Write);
                try
                {
                    return aes.CreateEncryptor().TransformFinalBlock(source, 0, source.Length);
                    //crypto.Write(source, 0, source.Length);
                    //crypto.FlushFinalBlock();
                    //encrypted = memory.ToArray();
                    //return encrypted;
                }
                catch (Exception ex)
                {
                    ShowError("加密失败：" + Environment.NewLine + ex.ToString());
                    return new byte[0];
                }
            }
            
        }
        //public static MemoryStream EncrypteToStream(byte[] text, byte[] key, byte[] iv, CipherMode mode, PaddingMode padding)
        //{
        //    var aes = Aes.Create();
        //    aes.Key = key;

        //    aes.Mode = mode;
        //    aes.Padding = padding;
        //    aes.IV = iv;

        //    //using (MemoryStream memory = new MemoryStream())
        //    //{
        //    //    using (CryptoStream crypto = new CryptoStream(memory, encryptor, CryptoStreamMode.Write))
        //    //    {
        //    //        using (StreamWriter writer = new StreamWriter(crypto))
        //    //        {
        //    //            writer.Write(text);
        //    //        }
        //    //        encrypted = memory.ToArray();
        //    //    }
        //    //}
        //    MemoryStream memory = new MemoryStream();
        //    CryptoStream crypto = new CryptoStream(memory, aes.CreateEncryptor(), CryptoStreamMode.Write);
        //    try
        //    {
        //        crypto.Write(text, 0, text.Length);
        //        crypto.FlushFinalBlock();
        //        return memory;
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowError("加密失败：" + Environment.NewLine + ex.ToString());
        //        memory.Dispose();
        //        crypto.Dispose();
        //        return null;
        //    }
        //    finally
        //    {
               
        //    }

        //}

        public static byte[] Decrypte(byte[] encrypted, byte[] key, byte[] iv, CipherMode mode, PaddingMode padding)
        {
            //byte[] decrypted;
            using (var aes = Aes.Create())
            {
                aes.Key = key;

                aes.Mode = mode;
                aes.Padding = padding;
                aes.IV = mode == CipherMode.ECB ? new byte[16] : iv;

                //using (MemoryStream memory = new MemoryStream())
                //{
                //    using (CryptoStream crypto = new CryptoStream(memory, encryptor, CryptoStreamMode.Write))
                //    {
                //        using (StreamWriter writer = new StreamWriter(crypto))
                //        {
                //            writer.Write(text);
                //        }
                //        encrypted = memory.ToArray();
                //    }
                //}
                //MemoryStream memory = new MemoryStream(text);
                //CryptoStream crypto = new CryptoStream(memory, aes.CreateEncryptor(), CryptoStreamMode.Read);
                try
                {
                    //crypto.Read(text, 0, text.Length);
                    // crypto.FlushFinalBlock();
                    //decrypted = memory.ToArray();
                    //return decrypted;
                    return aes.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
                }
                catch (Exception ex)
                {
                    ShowError("解密失败：" + Environment.NewLine + ex.ToString());
                    return new byte[0];
                }
            }

        }

    }
    
}
