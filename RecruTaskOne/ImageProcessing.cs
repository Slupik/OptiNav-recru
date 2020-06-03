using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        public NotProcessedYetException() : base("Image must be processed first.") {}
    }

    public class SourceImageNotFoundException : Exception
    {
        public SourceImageNotFoundException() : base("Oryginal image must be provided before that operation.") {}
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

        public void SaveProcessed(string outputPath)
        {
            if(processed == null)
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
            if (processed == null)
            {
                throw new NotProcessedYetException();
            }
        }
    }

    public class AsynchronousImageProcessing : ImageProcessing
    {

        public override void Process()
        {
            if (processed == null)
            {
                throw new NotProcessedYetException();
            }
        }

    }

}
