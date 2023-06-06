using AForge.Video.DirectShow;

namespace QRCodeUI
{
    // QR kodunu okumak için kullanılan scanner sınıfı
    public class QRScanner
    {
        private readonly string _camerasFilePath; // Kameraya ait bilgilerin saklandığı dosya yolu
        private readonly CameraDevice _cameraDevice; // Kamera cihazını temsil eder

        public QRScanner(string camerasFilePath)
        {
            _camerasFilePath = camerasFilePath;
            _cameraDevice = new CameraDevice(camerasFilePath); // Kamera cihazını oluşturur
        }

        // Belirtilen index'teki cihazı döndürür
        public static FilterInfo GetDeviceByIndex(int index, string camerasFilePath)
        {
            var cameraDevice = new CameraDevice(camerasFilePath); // Kamera cihazını oluşturur
            var devices = cameraDevice.GetCaptureDevices(); // Tüm cihazları alır

            if (index >= -1 && index < devices.Count) // Eğer index geçerli bir aralıktaysa
            {
                return devices[index + 1]; // Belirtilen index'teki cihazı döndür
            }

            throw new ArgumentOutOfRangeException(nameof(index), "Invalid index value."); // Geçersiz index durumunda hata fırlat
        }
    }
}
