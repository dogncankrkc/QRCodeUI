using System.Drawing;
using System.Drawing.Imaging;

/*Bu sınıf, bir bitmap üzerindeki piksellerin luminans değerlerini (ışık yoğunlukları) 
hesaplar ve saklar. Bu, bir QR kodunun tarama işlemi sırasında kullanılabilir.*/

// ZXing.LuminanceSource'dan türetilen özel bir sınıf. Bitmap'ten bir luminans kaynağı oluşturur.
public class BitmapLuminanceSource : ZXing.LuminanceSource
{
    // Bitmap'ten bir luminans kaynağı oluşturur.
    public BitmapLuminanceSource(Bitmap bitmap) : base(bitmap.Width, bitmap.Height)
    {
        // Bitmap'in raw verilerine erişim sağlar.
        var lockData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        var stride = lockData.Stride;
        var dataPointer = lockData.Scan0;

        // Raw verileri bir byte dizisine kopyalar.
        var imageData = new byte[stride * Height];
        System.Runtime.InteropServices.Marshal.Copy(dataPointer, imageData, 0, imageData.Length);
        bitmap.UnlockBits(lockData);

        // Luminans verilerini hesaplar.
        var luminanceData = new byte[Width * Height];
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var offset = y * stride + x * 4;
                var r = imageData[offset + 2];
                var g = imageData[offset + 1];
                var b = imageData[offset];
                // Luminans = 0.299*R + 0.587*G + 0.114*B
                luminanceData[y * Width + x] = (byte)((RChannelWeight * r + GChannelWeight * g + BChannelWeight * b) >> ChannelWeight);
            }
        }

        luminances = luminanceData;
    }

    // Kanal ağırlıkları ve toplam ağırlığı belirtir
    private const int ChannelWeight = 16;
    private const int RChannelWeight = 19562; // 0.299
    private const int GChannelWeight = 38550; // 0.587
    private const int BChannelWeight = 7424;  // 0.114

    // Belirtilen satırdaki piksellerin luminans değerlerini döndürür.
    public override byte[] getRow(int y, byte[] row)
    {
        if (row == null || row.Length != Width)
        {
            row = new byte[Width];
        }
        for (var x = 0; x < Width; x++)
        {
            row[x] = luminances[y * Width + x];
        }
        return row;
    }

    // Tüm luminans verilerini döndürür.
    public override byte[] Matrix => luminances;

    // Luminans verilerini saklar.
    private readonly byte[] luminances;
}
