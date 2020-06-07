using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace RecruTaskTwo.Utils
{
    public static class Extensions
    {
        public static BitmapImage ConvertToUiElement(this Bitmap bitmap)
        {
            BitmapImage bmImg = new BitmapImage();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0;

                bmImg.BeginInit();
                bmImg.CacheOption = BitmapCacheOption.OnLoad;
                bmImg.UriSource = null;
                bmImg.StreamSource = memoryStream;
                bmImg.EndInit();
            }

            return bmImg;
        }

    }
}
