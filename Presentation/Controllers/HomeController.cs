using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using Presentation.Utilities;
using System.Diagnostics;
using System.Text;

namespace Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string ipaddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation("Starting to test encryption...");

            Encryption myEncryption = new Encryption();

            
            //NOTE: Asymmetric encryption works only with base-64 data. Hence...
            string myPassword = "Hello World!";
            string myPasswordIntoBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(myPassword)); //SGVsbG8gV29ybGQhCg==

            //1. generate a public-private pair of keys
            var myKeys = myEncryption.GenerateAsymmetricKeys();

            //2. encrypt the base64 data
            byte[] clearDataAsBytesBase64 = Convert.FromBase64String(myPasswordIntoBase64);
            byte [] cipher = myEncryption.AsymmetricEncrypt(clearDataAsBytesBase64, myKeys.PublicKey);

            //-----------------------------------------------------------------------------------------------------------

            //Note: that i am using the same keys as in the encryption

            byte[] originalDataInBase64= myEncryption.AsymmetricDecrypt(cipher, myKeys.PrivateKey);


            string originalData = Encoding.UTF8.GetString(originalDataInBase64);


            var myOutputThatsGoingToBeSavedInAFile =
                myEncryption.HybridEncrypt(Encoding.UTF32.GetBytes(myPassword), myKeys.PublicKey);

            //System.IO.File.WriteAllBytes("", myOutputThatsGoingToBeSavedInAFile.ToArray());


            try
            {
                throw new Exception("raising an error on purpose");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "message occurred on an error thrown on purpose", null);
            }


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}