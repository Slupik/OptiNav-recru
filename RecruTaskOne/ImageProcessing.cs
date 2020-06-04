using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace RecruTaskOne
{
    public interface IImageProcessing
    {
        void LoadImage(string path);
        void LoadImage(Image image);

        void ToMainColors();

        void SaveProcessed(string outputPath);
    }

    public class NotProcessedYetException : Exception
    {
        public NotProcessedYetException() : base("Image must be processed first.") { }
    }

    public class SourceImageNotFoundException : Exception
    {
        public SourceImageNotFoundException() : base("Oryginal image must be provided before that operation.") { }
    }

    public abstract class ImageProcessing : IImageProcessing
    {
        protected Image oryginal = null;
        protected Image processed = null;

        public void LoadImage(string path)
        {
            LoadImage(Image.FromFile(path));
        }

        public void LoadImage(Image image)
        {
            oryginal = image;
        }

        public abstract void ToMainColors();

        protected void ChangePixel(ref byte red, ref byte green, ref byte blue)
        {
            if (red >= green && red >= blue)
            {
                red = (byte)255;
                green = (byte)0;
                blue = (byte)0;
            }
            else if (green >= red && green >= blue)
            {
                red = (byte)0;
                green = (byte)255;
                blue = (byte)0;
            }
            else
            {
                red = (byte)0;
                green = (byte)0;
                blue = (byte)255;
            }
        }

        public void SaveProcessed(string outputPath)
        {
            if (processed == null)
            {
                throw new NotProcessedYetException();
            }
            processed.Save(outputPath);
        }
    }

    public class SynchronzousImageProcessing : ImageProcessing
    {

        public override void ToMainColors()
        {
            if (oryginal == null)
            {
                throw new SourceImageNotFoundException();
            }
            Bitmap source = new Bitmap(oryginal);

            unsafe
            {
                BitmapData imageData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadWrite, source.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(source.PixelFormat) / 8;
                int widthInBytes = imageData.Width * bytesPerPixel;
                int heightInPixels = imageData.Height;

                byte* startPointer = (byte*)imageData.Scan0;

                for (int y = 0; y < heightInPixels; y++)
                {
                    byte* srcCurrentLine = startPointer + (y * imageData.Stride);
                    for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                    {
                        ChangePixel(
                                    ref srcCurrentLine[x + 2],
                                    ref srcCurrentLine[x + 1],
                                    ref srcCurrentLine[x]
                            );
                    }
                };
                source.UnlockBits(imageData);
            }

            processed = source;
        }

    }

    public class AsynchronousImageProcessing : ImageProcessing
    {

        public override void ToMainColors()
        {
            if (oryginal == null)
            {
                throw new SourceImageNotFoundException();
            }
            Bitmap source = new Bitmap(oryginal);

            unsafe
            {
                BitmapData imageData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadWrite, source.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(source.PixelFormat) / 8;
                int widthInBytes = imageData.Width * bytesPerPixel;
                int heightInPixels = imageData.Height;

                byte* startPointer = (byte*)imageData.Scan0;

                Parallel.For(0, heightInPixels, y =>
                {
                    byte* srcCurrentLine = startPointer + (y * imageData.Stride);
                    for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                    {
                        ChangePixel(
                                    ref srcCurrentLine[x + 2],
                                    ref srcCurrentLine[x + 1],
                                    ref srcCurrentLine[x]
                            );
                    }
                });
                source.UnlockBits(imageData);
            }

            processed = source;
        }

    }

}
