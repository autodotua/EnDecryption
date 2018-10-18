
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using static CryptographyToolbox.Tools;
using Windows.Storage.AccessCache;
using Windows.Storage;
using System.Xml;

namespace CryptographyToolbox.Helper
{
    public static class RsaHelper
    {
        public static byte[] Encrypte(byte[] source, string publicKeyXml, int keyLength, bool oaep)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keyLength))
            {
                try
                {
                    rsa.InitializeFromXmlString(publicKeyXml);
                }
                catch(Exception ex)
                {
                    ShowError("密钥格式有误", "加密失败", ex);
                }
                return Encrypte(source, rsa,oaep);
            }
        }
        
        public static byte[] Encrypte(byte[] source, RSAParameters parameters, bool oaep)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(parameters.Modulus.Length * 8))
            {
                rsa.ImportParameters(parameters);
                return Encrypte(source, rsa, oaep);
            }

        }

        private static byte[] Encrypte(byte[] source, RSACryptoServiceProvider rsa, bool oaep)
        {
            List<byte> encrypted = new List<byte>();
            MemoryStream msInput = new MemoryStream(source);
            int bufferSize = rsa.KeySize / 8 - 11;
            byte[] buffer = new byte[bufferSize];
            int length = msInput.Read(buffer, 0, bufferSize);
            try
            {
                while (length > 0)
                {
                    if (length == bufferSize)
                    {
                        encrypted.AddRange(rsa.Encrypt(buffer, oaep));
                    }
                    else
                    {
                        byte[] current = new byte[length];
                        Array.Copy(buffer, current, length);
                        encrypted.AddRange(rsa.Encrypt(current, oaep));
                    }
                    length = msInput.Read(buffer, 0, bufferSize);

                }
                return encrypted.ToArray();
            }
            catch (Exception ex)
            {
                ShowError("加密失败：" + Environment.NewLine + ex.ToString());
                return new byte[0];
            }
        }







        public static byte[] Decrypte(byte[] source, string privateKeyXml,int keyLength, bool oaep)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keyLength))
            {
                try
                { 
                rsa.InitializeFromXmlString(privateKeyXml);
                }
                catch (Exception ex)
                {
                    ShowError("密钥格式有误", "解密失败", ex);
                }
                return Decrypte(source, rsa, oaep);
            }

        }

        
        public static byte[] Decrypte(byte[] source, RSAParameters parameters, bool oaep)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(parameters.D.Length * 8))
            {
                rsa.ImportParameters(parameters);
             return   Decrypte(source, rsa, oaep);
                
              
            }

        }
        public static byte[] Decrypte(byte[] source, RSACryptoServiceProvider rsa,bool oaep)
        {
            List<byte> decrypted = new List<byte>();
            MemoryStream msInput = new MemoryStream(source);
            int bufferSize = rsa.KeySize / 8;
            byte[] buffer = new byte[bufferSize];
            int length = msInput.Read(buffer, 0, bufferSize);
            try
            {
                //while (length > 0)
                //{
                //    decrypted.AddRange(rsa.Decrypt(buffer, oaep));
                //    length = msInput.Read(buffer, 0, bufferSize);
                //}

                while (length > 0)
                {
                    if (length == bufferSize)
                    {
                        decrypted.AddRange(rsa.Decrypt(buffer, oaep));
                    }
                    else
                    {
                        byte[] current = new byte[length];
                        Array.Copy(buffer, current, length);
                        decrypted.AddRange(rsa.Decrypt(current, oaep));
                    }
                    length = msInput.Read(buffer, 0, bufferSize);

                }
                return decrypted.ToArray();
                //return decrypted.ToArray();
                //int count = 0;
                //length = decrypted.Count;
                //while(decrypted[length-count-1]==(char)0)
                //{
                //    count++;
                //}
                //decrypted.RemoveRange(length - count, count);
                //return decrypted.ToArray();
            }
            catch (Exception ex)
            {
                ShowError("解密失败：" + Environment.NewLine + ex.ToString());
                return new byte[0];
            }
        }

        public static string ToPriavateXmlString(this RSACryptoServiceProvider rsa)
        {
            RSAParameters parameters = rsa.ExportParameters(true);

            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                Convert.ToBase64String(parameters.Modulus),
                Convert.ToBase64String(parameters.Exponent),
                Convert.ToBase64String(parameters.P),
                Convert.ToBase64String(parameters.Q),
                Convert.ToBase64String(parameters.DP),
                Convert.ToBase64String(parameters.DQ),
                Convert.ToBase64String(parameters.InverseQ),
                Convert.ToBase64String(parameters.D));
        }
        public static string ToPublicXmlString(this RSACryptoServiceProvider rsa)
        {
            RSAParameters parameters = rsa.ExportParameters(false);

            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                Convert.ToBase64String(parameters.Modulus),
                Convert.ToBase64String(parameters.Exponent));
        }

        public static void InitializeFromXmlString(this RSACryptoServiceProvider rsa, string xmlString)
        {
            RSAParameters parameters = new RSAParameters();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus":
                            parameters.Modulus = Convert.FromBase64String(node.InnerText);
                            break;
                        case "Exponent":
                            parameters.Exponent = Convert.FromBase64String(node.InnerText);
                            break;
                        case "P":
                            parameters.P = Convert.FromBase64String(node.InnerText);
                            break;
                        case "Q":
                            parameters.Q = Convert.FromBase64String(node.InnerText);
                            break;
                        case "DP":
                            parameters.DP = Convert.FromBase64String(node.InnerText);
                            break;
                        case "DQ":
                            parameters.DQ = Convert.FromBase64String(node.InnerText);
                            break;
                        case "InverseQ":
                            parameters.InverseQ = Convert.FromBase64String(node.InnerText);
                            break;
                        case "D":
                            parameters.D = Convert.FromBase64String(node.InnerText);
                            break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            rsa.ImportParameters(parameters);
        }
    }
}