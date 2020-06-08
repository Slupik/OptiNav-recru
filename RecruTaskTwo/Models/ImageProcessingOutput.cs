using RecruTaskTwo.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RecruTaskTwo.Models
{
    public readonly struct ImageProcessingOutput
    {
        public ImageProcessingOutput(Bitmap result, TimeSpan computatioSpan)
        {
            Image = result;
            ComputationTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                    computatioSpan.Hours, computatioSpan.Minutes, computatioSpan.Seconds, computatioSpan.Milliseconds);
        }

        public Bitmap Image { get; }
        public string ComputationTime { get; }

    }
}
