using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using System.Drawing;


/* Bu sınıf bir kamera cihazını kullanarak QR kodları okur. 
Yeni bir video çerçevesi her yakalandığında, 
çerçevedeki bir QR kodunu okur ve sonucu bir dosyaya yazar.  */
namespace QRCodeUI
{
    // QR kodlarını okumak için bir sınıf
    public class QrCodeReader
    {
        private VideoCaptureDevice _videoSource;
        private Result _result = null;
        private string _previousResult = null;

        // Capture device bilgisi alınarak video kaynağı oluşturulur.
        public QrCodeReader(FilterInfo captureDevice)
        {
            if (captureDevice != null)
            {
                _videoSource = new VideoCaptureDevice(captureDevice.MonikerString);
                _videoSource.NewFrame += VideoSource_NewFrame;  // Yeni frame event handlerı eklenir.
            }
            else
            {
                throw new ArgumentNullException(nameof(captureDevice), "Capture device is null.");
            }
        }

        // QR kod okumayı başlatır
        public void StartReading()
        {
            _videoSource.Start();
        }

        // QR kod okumayı durdurur
        public void StopReading()
        {
            _videoSource.SignalToStop();
            _videoSource.WaitForStop();
        }

        // Yeni frame oluştuğunda çağrılır. Frame içerisinde QR kod okuma işlemini gerçekleştirir.
        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                var barcodeReader = new ZXing.BarcodeReaderGeneric();
                var bitmap = (Bitmap)eventArgs.Frame.Clone();
                var luminanceSource = new BitmapLuminanceSource(bitmap);
                _result = barcodeReader.Decode(luminanceSource); // QR kod okuma
                if (_result != null) // QR kod okunduysa
                {
                    string filePath = Path.Combine("qrCodeResult.txt");

                    System.IO.File.WriteAllText(filePath, _result.Text); // Sonucu dosyaya yazdır

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata oluştu: {ex.Message}"); // Hata durumunda mesajı konsola yazdır
            }
        }
    }
}
