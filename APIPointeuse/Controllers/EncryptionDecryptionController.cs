using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace APIPointeuse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptionDecryptionController : ControllerBase
    {
        private static string key = "8lftofMewLXqPHH5Uk5tkDtO5X0lsDlItm1FYQWAcdw=";

        [HttpGet]
        [Route("encrypt")]
        public IActionResult Encrypt(string plainText)
        {
            try
            {
                string encryptedText = EncryptData(plainText, Convert.FromBase64String(key));
                return Ok(encryptedText);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("decrypt")]
        public IActionResult Decrypt(string cipherText)
        {
            try
            {
                string decryptedText = DecryptData(cipherText, Convert.FromBase64String(key));
                return Ok(decryptedText);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("generateKey")]
        public IActionResult GenerateKey()
        {
            try
            {
                string generatedKey = GenerateKeyData();
                return Ok(generatedKey);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private static string EncryptData(string plainText, byte[] key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = aesAlg.Key.Take(16).ToArray();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        private static string DecryptData(string cipherText, byte[] key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = aesAlg.Key.Take(16).ToArray();

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        private static string GenerateKeyData()
        {
            using (AesCryptoServiceProvider aesCrypto = new AesCryptoServiceProvider())
            {
                aesCrypto.KeySize = 256;
                aesCrypto.GenerateKey();
                byte[] key = aesCrypto.Key;

                string keyBase64 = Convert.ToBase64String(key);
                return keyBase64;
            }
        }
    }
}