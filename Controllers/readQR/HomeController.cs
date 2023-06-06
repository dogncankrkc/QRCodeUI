using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QRCodeUI.Models;
namespace QRCodeUI.Controllers
{
    // HomeController sınıfı, web uygulamanızın temel işlevselliğini kontrol eder.
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; // Logging için kullanılır
        private readonly string _camerasFilePath; // Kameraya ait bilgilerin saklandığı dosya yolu

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _camerasFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cameras.txt"); // Kameraya ait bilgilerin saklandığı dosya yolunu belirler
        }

        // Ana sayfayı getirir
        public IActionResult Index()
        {
            return View();
        }
        // QR kodunu tarar
        public IActionResult ScanQr()
        {
            if (!System.IO.File.Exists(_camerasFilePath)) // Eğer kamera bilgileri dosyası yoksa
            {
                var devices = new CameraDevice(_camerasFilePath).GetCaptureDevices(); // Kamera cihazlarını alır
            }
            var cameraList = System.IO.File.ReadLines(_camerasFilePath).ToList(); // Kamera listesini okur

            var model = new ScanQrViewModel { CameraList = cameraList, QrCodeResult = null }; // Modeli oluşturur ve view'a gönderir
            return View(model);
        }

        // QR kodunu oluşturur
        public IActionResult CreateQr()
        {
            return View();
        }

        // Hata sayfasını getirir
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Seçilen kamerayla QR Scanner'ı çalıştırır
        [HttpPost]
        public IActionResult RunQRScanner(string cameraSelection)
        {
            if (System.IO.File.Exists("qrCodeResult.txt")) // Eğer sonuç dosyası varsa
            {
                System.IO.File.Delete("qrCodeResult.txt"); // Sonuç dosyasını sil
            }
            int selectedIndex = int.Parse(cameraSelection) - 1; // Seçilen index'i belirler
            var qrCodeReader = new QrCodeReader(QRScanner.GetDeviceByIndex(selectedIndex, "cameras.txt")); // QR Kod okuyucusunu oluşturur
            qrCodeReader.StartReading(); // QR kod okuma işlemine başlar

            // Check every second if the file is created.
            while (!System.IO.File.Exists("qrCodeResult.txt")) // Dosya oluşana kadar her saniye kontrol et
            {
                System.Threading.Thread.Sleep(1000);
            }

            return RedirectToAction("DisplayQRResult"); // Sonucu göstermek için DisplayQRResult action'ına yönlendirir
        }

        // QR kod okuma sonucunu gösterir
        public IActionResult DisplayQRResult()
        {
            string qrCodeResult = System.IO.File.Exists("qrCodeResult.txt")
                                ? System.IO.File.ReadAllText("qrCodeResult.txt")
                                : ""; // Sonucu okur

            if (System.IO.File.Exists("qrCodeResult.txt")) // Eğer sonuç dosyası varsa
            {
                System.IO.File.Delete("qrCodeResult.txt"); // Sonuç dosyasını sil
            }

            var cameraList = System.IO.File.Exists(_camerasFilePath)
                                ? System.IO.File.ReadLines(_camerasFilePath).ToList()
                                : new List<string>(); // Kamera listesini okur

            var model = new ScanQrViewModel { CameraList = cameraList, QrCodeResult = qrCodeResult }; // Modeli oluşturur ve view'a gönderir
            return View("ScanQr", model);
        }

        // Kullanıcının girdiği bilgilere göre QR kodunu oluşturur
        public IActionResult RunCreateQr(UserInput userInput)
        {
            string qrCodeResult = QRCreator.CreateQr(userInput); // QR kodunu oluşturur
            ViewBag.BarcodeImage = qrCodeResult; // Oluşturulan QR kodunu ViewBag'a ekler
            ViewBag.FormSubmitted = "True"; // Formun gönderildiğini belirler
            return View("Index"); // Ana sayfaya yönlendirir
        }

        // Ana sayfayı getirir
        public IActionResult HomePage()
        {
            return View();
        }
    }
}
