using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RecruTaskOne
{
    public interface IImageProcessing
    {
        void LoadImage(string path);
        void LoadImage(Image image);

        void Process();

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

        public abstract void Process();

        protected Color ProcessPixel(Color pixel)
        {
            if (pixel.R >= pixel.G && pixel.R >= pixel.B)
            {
                return Color.FromArgb(pixel.A, 255, 0, 0);
            }
            if (pixel.G >= pixel.R && pixel.G >= pixel.B)
            {
                return Color.FromArgb(pixel.A, 0, 255, 0);
            }
            return Color.FromArgb(pixel.A, 0, 0, 255);
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

        public override void Process()
        {
            if (oryginal == null)
            {
                throw new SourceImageNotFoundException();
            }
            Bitmap source = new Bitmap(oryginal);
            Bitmap output = new Bitmap(source.Width, source.Height);
            for (int x = 0; x < source.Width; x++)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    Color sourceColor = source.GetPixel(x, y);
                    output.SetPixel(x, y, ProcessPixel(sourceColor));
                }
            }
            processed = output;
        }
    }

    public class AsynchronousImageProcessing : ImageProcessing
    {

        public override void Process()
        {
            if (oryginal == null)
            {
                throw new SourceImageNotFoundException();
            }
            Bitmap source = new Bitmap(oryginal);
            Bitmap dest = new Bitmap(source.Width, source.Height);

            unsafe
            {
                BitmapData srcData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadWrite, source.PixelFormat);
                BitmapData destData = dest.LockBits(new Rectangle(0, 0, dest.Width, dest.Height), ImageLockMode.ReadWrite, dest.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(source.PixelFormat) / 8;
                int widthInBytes = srcData.Width * bytesPerPixel;
                int heightInPixels = srcData.Height;

                byte* srcStartPointer = (byte*)srcData.Scan0;
                byte* destStartPointer = (byte*)destData.Scan0;

                Parallel.For(0, heightInPixels, y =>
                {
                    byte* srcCurrentLine = srcStartPointer + (y * srcData.Stride);
                    byte* destCurrentLine = destStartPointer + (y * destData.Stride);
                    for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                    {
                        ChangePixel(
                                    ref srcCurrentLine[x + 2],
                                    ref srcCurrentLine[x + 1],
                                    ref srcCurrentLine[x],
                                    ref destCurrentLine[x + 2],
                                    ref destCurrentLine[x + 1],
                                    ref destCurrentLine[x]
                            );
                        destCurrentLine[x + 3] = (byte)255;
                    }
                });
                source.UnlockBits(srcData);
                dest.UnlockBits(destData);
            }

            processed = dest;
        }

        private void ChangePixel(ref byte redSrc, ref byte greenSrc, ref byte blueSrc,
            ref byte red, ref byte green, ref byte blue)
        {
            if (redSrc >= greenSrc && redSrc >= blueSrc)
            {
                red = (byte)255;
                green = (byte)0;
                blue = (byte)0;
            }
            else if (greenSrc >= redSrc && greenSrc >= blueSrc)
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

    }

}
