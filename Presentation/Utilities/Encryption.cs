using System.Security.Cryptography; //gives me access to the algorithms represented as classes
using System.IO; //enable to use Streams
using System.Text; //will enable me to convert the relevant data from various character sets
using System.Diagnostics;

namespace Presentation.Utilities
{
    public class Encryption
    {

        //used: to hash files
        public byte[]  Hash(byte[] input)
        {
            //md5, sha1 = broken ones, insecure
            //sha512, sha256 = secure ones
            //HMACSHA512 = you can use a salt with it

            SHA512 myAlg = SHA512.Create();
            byte[] digest = myAlg.ComputeHash(input);
            return digest;
        }

        //used: when hashing passwords
        public string Hash (string input)
        {
            //step 1 : convert from text >> byte []
            //step 2 : hash
            //step 3 : (if you intend to store the digest as a string) convert back from byte to base64 string

            byte [] inputAsBytes =UTF32Encoding.UTF32.GetBytes(input);
            byte[] digest = Hash(inputAsBytes); //if you would like to return an array of bytes return digest

            string output = Convert.ToBase64String(digest);
            return output;
        }

        byte[] salt = new byte[] { 45,2,1,49,60,100,100,2,200,178,190,35,150};

        public byte[] SymmetricEncrypt(byte[] input, string password)
        {
           Aes myAlg = Aes.Create();

            //approach 1 how to generate a key - then you need to extract the key and save it somewhere safe
            //myAlg.GenerateKey();
            //myAlg.GenerateIV();

            //approach 2 how to generate a key - this one uses an input from the user as a key + salt
                                             //- this will create a different secret key for every user
            Rfc2898DeriveBytes myKeyGenerator = new Rfc2898DeriveBytes(password, salt);
            byte[] secretKey = myKeyGenerator.GetBytes(myAlg.KeySize / 8);
            byte[] iv = myKeyGenerator.GetBytes(myAlg.BlockSize / 8);



            MemoryStream msIn= new MemoryStream(input); //Point A
            msIn.Position = 0;//ascertain myself that the CryptoStream (later on) will start from position 0

            MemoryStream msOut = new MemoryStream(); // Point B
            using(CryptoStream myCryptoStream = new CryptoStream(msIn,
                                                               myAlg.CreateEncryptor(secretKey, iv),
                                                                CryptoStreamMode.Read))
               {
                        myCryptoStream.CopyTo(msOut);
               }

            msOut.Position = 0;            
            return msOut.ToArray();            
        }

        public byte[] SymmetricDecrypt(byte[] cipher, string password)
        {
            Aes myAlg = Aes.Create();

            //approach 1 how to generate a key - then you need to extract the key and save it somewhere safe
            //myAlg.GenerateKey();
            //myAlg.GenerateIV();

            //approach 2 how to generate a key - this one uses an input from the user as a key + salt
            //- this will create a different secret key for every user
            Rfc2898DeriveBytes myKeyGenerator = new Rfc2898DeriveBytes(password, salt);
            byte[] secretKey = myKeyGenerator.GetBytes(myAlg.KeySize / 8);
            byte[] iv = myKeyGenerator.GetBytes(myAlg.BlockSize / 8);



            MemoryStream msIn = new MemoryStream(cipher); //Point A
            msIn.Position = 0;//ascertain myself that the CryptoStream (later on) will start from position 0

            MemoryStream msOut = new MemoryStream(); // Point B
            using (CryptoStream myCryptoStream = new CryptoStream(msIn,
                                                               myAlg.CreateDecryptor(secretKey, iv),
                                                                CryptoStreamMode.Read))
            {
                myCryptoStream.CopyTo(msOut);
            }

            msOut.Position = 0;
            return msOut.ToArray();
        }


        public AsymmetricParameters GenerateAsymmetricKeys()
        { 
            RSACryptoServiceProvider rsa= new RSACryptoServiceProvider();
            AsymmetricParameters myParams = new AsymmetricParameters()
            {
                PublicKey = rsa.ToXmlString(false),
                PrivateKey = rsa.ToXmlString(true)
            };

            return myParams;

        }

        public byte[] AsymmetricEncrypt(byte[] clearBytes, string publicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(); //it will generate another pair of keys
            rsa.FromXmlString(publicKey); //import the keys we created earlier

            byte[] cipher = rsa.Encrypt(clearBytes, true);
            return cipher;

        }

        public byte[] AsymmetricDecrypt(byte[] cipher, string privateKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(); //it will generate another pair of keys
            rsa.FromXmlString(privateKey); //import the keys we created earlier

            byte[] originalData = rsa.Decrypt(cipher, true);
            return originalData;

        }

        public byte[] DigitalSign(byte[] data, string privateKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);

            byte[] digest = Hash(data);
            
            byte[] signature =  rsa.SignHash(digest, new HashAlgorithmName("SHA512"), RSASignaturePadding.Pkcs1);
            return signature;

        }

        public bool DigitalVerify(byte[] data, byte[] signature, string publicKey)
        {
            //use rsa.VerifyHash(...);

            //return the output of VerifyHash;

            return false;
        
        }


        public MemoryStream HybridEncrypt(byte[] data, string publicKey) {

            Aes myAlg = Aes.Create();
            //step 2
            myAlg.GenerateKey();
            myAlg.GenerateIV();

            //step 3
            MemoryStream msIn = new MemoryStream(data); //Point A
            msIn.Position = 0;//ascertain myself that the CryptoStream (later on) will start from position 0

            MemoryStream msOut = new MemoryStream(); // Point B
            using (CryptoStream myCryptoStream = new CryptoStream(msIn,myAlg.CreateEncryptor(),CryptoStreamMode.Read))
            {
                myCryptoStream.CopyTo(msOut);
            }
            msOut.Position = 0;

            //msOut contains my encrypted data
            //step 4
            byte[] encryptedKey = AsymmetricEncrypt(myAlg.Key, publicKey);
            byte[] encryptedIv = AsymmetricEncrypt(myAlg.IV, publicKey);

            //step 5
            MemoryStream output = new MemoryStream();
            output.Write(encryptedKey, 0, encryptedKey.Length); //if here it wrote 128bytes, atm position =128
            output.Write(encryptedIv, 0, encryptedIv.Length);  //it continues writing from position 128

            //step 6
            msOut.CopyTo(output);
            msOut.Flush();

            return output;
        }



    }
}
