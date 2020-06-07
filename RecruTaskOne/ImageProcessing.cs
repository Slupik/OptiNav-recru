using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace TaskLibrary
{

    public interface IImageProcessing
    {
        Bitmap Oryginal { get;}
        Bitmap Result { get;}

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
        protected Bitmap _oryginal = null;
        public Bitmap Oryginal
        {
            get
            {
                return _oryginal;
            }
        }

        protected Bitmap _result = null;
        public Bitmap Result
        {
            get
            {
                return _result;
            }
        }

        protected bool processed = false;

        public void LoadImage(string path)
        {
            LoadImage(Image.FromFile(path));
        }

        public void LoadImage(Image image)
        {
            if (image is Bitmap bitmap)
                _oryginal = bitmap;
            else
                _oryginal = new Bitmap(image);
            _result = new Bitmap(_oryginal);
            processed = false;
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
            if (!processed)
            {
                throw new NotProcessedYetException();
            }
            _result.Save(outputPath);
        }
    }

    public class SynchronzousImageProcessing : ImageProcessing
    {

        public override void ToMainColors()
        {
            if (_oryginal == null)
            {
                throw new SourceImageNotFoundException();
            }
            if (processed) return;

            unsafe
            {
                BitmapData imageData = _result.LockBits(new Rectangle(0, 0, _result.Width, _result.Height), ImageLockMode.ReadWrite, _result.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(_result.PixelFormat) / 8;
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
                _result.UnlockBits(imageData);
                processed = true;
            }
        }

    }

    public class AsynchronousImageProcessing : ImageProcessing
    {

        public override void ToMainColors()
        {
            if (_oryginal == null)
            {
                throw new SourceImageNotFoundException();
            }
            if (processed) return;

            unsafe
            {
                BitmapData imageData = _result.LockBits(new Rectangle(0, 0, _result.Width, _result.Height), ImageLockMode.ReadWrite, _result.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(_result.PixelFormat) / 8;
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
                _result.UnlockBits(imageData);
                processed = true;
            }
        }

    }

}
