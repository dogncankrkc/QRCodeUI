using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.QrCode;
using QRCodeUI.Models;

// QR kod oluşturma işlemlerini yürüten sınıf
namespace QRCodeUI
{
    public class QRCreator
    {
        // UserInput modeline dayalı olarak bir QR kod oluşturan ve base64 string formatında döndüren fonksiyon
        public static string CreateQr(UserInput userInput)
        {
            // ZXing.Net kütüphanesini kullanarak bir BarcodeWriterPixelData nesnesi oluştur
            var qrCodeWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Width = 300,  // QR kodunun genişliği
                    Height = 300, // QR kodunun yüksekliği
                    Margin = 4,   // QR kodunun etrafındaki boşluk
                    CharacterSet = "UTF-8" // QR kodunun karakter kümesi
                }
            };

            // userInput modelinden alınan değerler
            string url= userInput.Url;
            string name = userInput.Name;
            string date = userInput.Date;
            string gender = userInput.Gender;
            string address = userInput.Address;
            string email = userInput.Email;
            string tel = userInput.Tel;
            string job = userInput.Job;
            string education = userInput.Education;
            string extraField = userInput.ExtraField;

            // QR kodunun oluşturulması
            var pixelData = qrCodeWriter.Write($"Name: {name}, Date: {date}, Gender: {gender}, Address: {address}, Email: {email}, Tel: {tel}, Job: {job}, Education: {education}, Extra: {extraField}, URL: {url}");

            // QR kodunun bir bitmap'e dönüştürülmesi
            var barcodeBitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppArgb);

            // Bitmap üzerindeki her pikseli ayarlama
            for (int y = 0; y < pixelData.Height; y++)
            {
                for (int x = 0; x < pixelData.Width; x++)
                {
                    // Piksel verilerinin ofsetini hesapla
                    int offset = (y * pixelData.Width + x) * 4;
                    int alpha = pixelData.Pixels[offset + 3];
                    int red = pixelData.Pixels[offset + 2];
                    int green = pixelData.Pixels[offset + 1];
                    int blue = pixelData.Pixels[offset];

                    // Eğer alpha değeri 0 ise pikseli beyaz olarak ayarla
                    if (alpha == 0)
                    {
                        barcodeBitmap.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        // Aksi takdirde, pikseli belirli bir renk olarak ayarla
                        Color color = Color.FromArgb(alpha, red, green, blue);
                        barcodeBitmap.SetPixel(x, y, color);
                    }
                }
            }

            // QR kodunun base64 formatında bir string olarak dönüştürülmesi
            MemoryStream ms = new MemoryStream();
            barcodeBitmap.Save(ms, ImageFormat.Png);
            byte[] barcodeBytes = ms.ToArray();
            string base64String = Convert.ToBase64String(barcodeBytes);

            return base64String;
        }
    }
}
