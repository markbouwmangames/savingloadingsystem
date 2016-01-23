using System.Security.Cryptography;
using System.IO;
using System;
using System.Text;

//Using an algorithm called Rijndael for encrypting/decrypting. See the following websites.
//http://www.c-sharpcorner.com/UploadFile/4d9083/encrypt-in-javascript-and-decrypt-in-C-Sharp-with-aes-algorithm/
//http://www.codeproject.com/Tips/704372/How-to-use-Rijndael-ManagedEncryption-with-Csharp
public class Cryptographer {
    public static const string key = "<notgoingtoshowthis>";
    public static const string iv = "<notgoingtoshowthis>";

    public static string Encrypt(string data) {
        byte[] key = Encoding.UTF8.GetBytes(key);
        byte[] iv = Encoding.UTF8.GetBytes(iv);

        // Encrypt the string to an array of bytes.
        byte[] encrypted = EncryptStringToBytes(data, key, iv);
        return Convert.ToBase64String(encrypted);
    }

    public static string Decrypt(string data) {
        byte[] key = Encoding.UTF8.GetBytes(key);
        byte[] iv = Encoding.UTF8.GetBytes(iv);

        // Decrypt the string to an array of bytes.
        byte[] encrypted = Convert.FromBase64String(data);
        return DecryptStringFromBytes(encrypted, key, iv);
    }


    private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv) {
        // Check arguments.  
        if (plainText == null || plainText.Length <= 0) {
            throw new ArgumentNullException("plainText");
        }
        if (key == null || key.Length <= 0) {
            throw new ArgumentNullException("key");
        }
        if (iv == null || iv.Length <= 0) {
            throw new ArgumentNullException("key");
        }
        byte[] encrypted;
        // Create a RijndaelManaged object  
        // with the specified key and IV.  
        using (var rijAlg = new RijndaelManaged()) {
            rijAlg.Mode = CipherMode.CBC;
            rijAlg.Padding = PaddingMode.PKCS7;
            rijAlg.FeedbackSize = 128;

            rijAlg.Key = key;
            rijAlg.IV = iv;

            // Create a decrytor to perform the stream transform.  
            var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

            // Create the streams used for encryption.  
            using (var msEncrypt = new MemoryStream()) {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                    using (var swEncrypt = new StreamWriter(csEncrypt)) {
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

    private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv) {
        // Check arguments.  
        if (cipherText == null || cipherText.Length <= 0) {
            throw new ArgumentNullException("cipherText");
        }
        if (key == null || key.Length <= 0) {
            throw new ArgumentNullException("key");
        }
        if (iv == null || iv.Length <= 0) {
            throw new ArgumentNullException("key");
        }

        // Declare the string used to hold  
        // the decrypted text.  
        string plaintext = null;

        // Create an RijndaelManaged object  
        // with the specified key and IV.  
        using (var rijAlg = new RijndaelManaged()) {
            //Settings  
            rijAlg.Mode = CipherMode.CBC;
            rijAlg.Padding = PaddingMode.PKCS7;
            rijAlg.FeedbackSize = 128;

            rijAlg.Key = key;
            rijAlg.IV = iv;

            // Create a decrytor to perform the stream transform.  
            var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

            try {
                // Create the streams used for decryption.  
                using (var msDecrypt = new MemoryStream(cipherText)) {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {

                        using (var srDecrypt = new StreamReader(csDecrypt)) {
                            // Read the decrypted bytes from the decrypting stream  
                            // and place them in a string.  
                            plaintext = srDecrypt.ReadToEnd();

                        }

                    }
                }
            } catch {
                plaintext = "keyError";
            }
        }

        return plaintext;
    }
}
