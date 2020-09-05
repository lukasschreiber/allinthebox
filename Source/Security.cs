using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace allinthebox
{
    public class Security
    {
        public static string KEY = "IPWsVf7t54CmSHAcitf2hLsTkxSckRcL";
        public static string IV = "jqk8gyF1KoH8T0D1";


        public static byte[] Encrypt(string plainText)
        {
            // Check arguments. 
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");

            byte[] encrypted;
            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (var rijAlg = new RijndaelManaged())
            {
                var enc = new ASCIIEncoding();
                rijAlg.Key = enc.GetBytes(KEY);
                rijAlg.IV = enc.GetBytes(IV);

                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption. 
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
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
    }
}