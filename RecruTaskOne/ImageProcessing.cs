using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruTaskOne
{
    public interface IImageProcessing
    {

    }

    public abstract class ImageProcessing : IImageProcessing
    {

    }

    public class SynchronousImageProcessing : ImageProcessing
    {

    }

    public class AsynchronousImageProcessing : ImageProcessing
    {

    }

}
