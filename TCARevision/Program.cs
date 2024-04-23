using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace TCARevision
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Decrypt();

           
        }

        static void Decrypt()
        {
            //declaration of algorithm
            Aes myAlg = Aes.Create();

            //read the key and the iv
            byte[] key = new byte[32];
            byte[] iv = new byte[16];

            using (var file = File.OpenRead("aes_keys.bin"))
            {
                file.Read(key, 0, 32);
                file.Read(iv, 0, 16);
            } //this bracket will close the file we are reading from

             
            //setting up where to store the clear text
            MemoryStream msOutput = new MemoryStream();
            //reading the cipher and preparing to be decrypted
            //suggestion: always use MemoryStream to handle your data
            string cipher = File.ReadAllText("cipher.txt");

            //i chose convert.frombase64string (instead of utf32.getbytes) because the last conversion i executed
            //in the Encrypt method was Convert.Tobase64String
            byte[] cipherAsByte = Convert.FromBase64String(cipher);

            MemoryStream msInput = new MemoryStream(cipherAsByte);
            msInput.Position = 0;

            //configuring the engine which will actually decrypt the data
            //note: CryptoStreamMode determines what to do with the first parameter stream object
            CryptoStream myDecrypt = new CryptoStream(msInput, myAlg.CreateDecryptor(key, iv), CryptoStreamMode.Read);
            //this is where the encryption actually takes place
            myDecrypt.CopyTo(msOutput);
            //so if any bytes remain in the CryptoStream, they are flushed into the output stream i.e. msOutput
            myDecrypt.Flush();

            msOutput.Position = 0;

            //conversion i chose here is UTF32.GetString because in msOutput I have clear text represented as bytes
            string originalText = UTF32Encoding.UTF32.GetString(msOutput.ToArray());

            Console.WriteLine(originalText);
            Console.ReadLine();
        }

        static void  Encrypt()
        {
            //declaration of algorithm
            Aes myAlg = Aes.Create();

            //generation of random symmetric parameters/keys
            myAlg.GenerateIV(); myAlg.GenerateKey();

            //saving the symm parameters in a file
            MemoryStream ms = new MemoryStream();
            ms.Write(myAlg.Key, 0, myAlg.Key.Length); //32 bytes
            ms.Write(myAlg.IV, 0, myAlg.IV.Length); //16 bytes

            ms.Position = 0;
            File.WriteAllBytes("aes_keys.bin", ms.ToArray());


            //setting up where to store the cipher once its generated
            MemoryStream msOutput = new MemoryStream();
            //setting up what to encrypt
            //suggestion: always use MemoryStream to handle your data
            string toEncrypt = "hello world!";
            byte[] toEncryptAsBytes = UTF32Encoding.UTF32.GetBytes(toEncrypt);

            MemoryStream msInput = new MemoryStream(toEncryptAsBytes);
            msInput.Position = 0;

            //configuring the engine which will actually encrypt the data
            //note: CryptoStreamMode determines what to do with the first parameter stream object
            CryptoStream myEncryptor = new CryptoStream(msInput, myAlg.CreateEncryptor(), CryptoStreamMode.Read);
            //this is where the encryption actually takes place
            myEncryptor.CopyTo(msOutput);
            //so if any bytes remain in the CryptoStream, they are flushed into the output stream i.e. msOutput
            myEncryptor.Flush();

            msOutput.Position = 0;

            //converting the cipher which is being held as bytes in msOutput into an actual string to be stored in a text file
            string cipher = Convert.ToBase64String(msOutput.ToArray());

            //this is how you save in a text file
            File.WriteAllText("cipher.txt", cipher);
        }
    }
}