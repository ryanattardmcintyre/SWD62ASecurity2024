using System.Security.Cryptography; //gives me access to the algorithms represented as classes
using System.IO; //enable to use Streams
using System.Text; //will enable me to convert the relevant data from various character sets



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






    }
}
