using AForge.Video.DirectShow;

/*Bu sınıf, mevcut video giriş cihazlarını (genellikle web kameralarını) belirlemek için kullanılır. 
Bu cihazlar, bir QR kodunu okumak için kullanılabilir. 
İlk kez bir kamera listesi oluşturulduğunda, bu liste belirtilen bir dosyaya kaydedilir.*/

namespace QRCodeUI
{
    // Kamera cihazları ile ilgili işlemlerin yapıldığı sınıf
    public class CameraDevice
    {
        private readonly string _camerasFilePath; // Kameraya ait bilgilerin saklandığı dosya yolu

        public CameraDevice(string camerasFilePath)
        {
            _camerasFilePath = camerasFilePath;
        }

        // Sisteme bağlı olan tüm video giriş cihazlarını (kamera vb.) alır.
        public FilterInfoCollection GetCaptureDevices()
        {
            FilterInfoCollection devices = new FilterInfoCollection(FilterCategory.VideoInputDevice); // Video giriş cihazlarını alır
            if (!File.Exists(_camerasFilePath)) // Eğer dosya yoksa
            {
                // Dosyaya cihazların isimlerini yazdırır.
                File.WriteAllLines(_camerasFilePath, devices.Cast<FilterInfo>().Select(x => x.Name));
            }
            return devices; // Cihaz listesini döndürür.
        }
    }
}
