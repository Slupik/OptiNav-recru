using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TaskLibrary.Tests
{
    [TestClass()]
    public abstract class ImageProcessingTests
    {

        [TestMethod()]
        public void AsyncProcess_MostRed_MaxRed()
        {
            ImageProcessing sut = GetSut();
            Bitmap input = new Bitmap(1, 1);
            input.SetPixel(0, 0, Color.FromArgb(255, 200, 3, 4));
            sut.LoadImage(input);

            sut.ToMainColors();

            Assert.AreEqual(Color.FromArgb(255, 255, 0, 0), sut.Result.GetPixel(0, 0));
        }

        [TestMethod()]
        public void AsyncProcess_MostGreen_MaxGreen()
        {
            ImageProcessing sut = GetSut();
            Bitmap input = new Bitmap(1, 1);
            input.SetPixel(0, 0, Color.FromArgb(255, 3, 200, 4));
            sut.LoadImage(input);

            sut.ToMainColors();

            Assert.AreEqual(Color.FromArgb(255, 0, 255, 0), sut.Result.GetPixel(0, 0));
        }

        [TestMethod()]
        public void AsyncProcess_MostBlue_MaxBlue()
        {
            ImageProcessing sut = GetSut();
            Bitmap input = new Bitmap(1, 1);
            input.SetPixel(0, 0, Color.FromArgb(255, 3, 4, 200));
            sut.LoadImage(input);

            sut.ToMainColors();

            Assert.AreEqual(Color.FromArgb(255, 0, 0, 255), sut.Result.GetPixel(0, 0));
        }

        [TestMethod()]
        public void AsyncProcess_AllColorsEqual_MaxRed()
        {
            ImageProcessing sut = GetSut();
            Bitmap input = new Bitmap(1, 1);
            input.SetPixel(0, 0, Color.FromArgb(255, 200, 200, 200));
            sut.LoadImage(input);

            sut.ToMainColors();

            Assert.AreEqual(Color.FromArgb(255, 255, 0, 0), sut.Result.GetPixel(0, 0));
        }

        [TestMethod()]
        public void AsyncProcess_RedAndGeenEqual_MaxRed()
        {
            ImageProcessing sut = GetSut();
            Bitmap input = new Bitmap(1, 1);
            input.SetPixel(0, 0, Color.FromArgb(255, 200, 200, 0));
            sut.LoadImage(input);

            sut.ToMainColors();

            Assert.AreEqual(Color.FromArgb(255, 255, 0, 0), sut.Result.GetPixel(0, 0));
        }

        [TestMethod()]
        public void AsyncProcess_RedAndBlueEqual_MaxRed()
        {
            ImageProcessing sut = GetSut();
            Bitmap input = new Bitmap(1, 1);
            input.SetPixel(0, 0, Color.FromArgb(255, 200, 0, 200));
            sut.LoadImage(input);

            sut.ToMainColors();

            Assert.AreEqual(Color.FromArgb(255, 255, 0, 0), sut.Result.GetPixel(0, 0));
        }

        [TestMethod()]
        public void AsyncProcess_GreenAndBlueEqual_MaxGreen()
        {
            ImageProcessing sut = GetSut();
            Bitmap input = new Bitmap(1, 1);
            input.SetPixel(0, 0, Color.FromArgb(255, 0, 200, 200));
            sut.LoadImage(input);

            sut.ToMainColors();

            Assert.AreEqual(Color.FromArgb(255, 0, 255, 0), sut.Result.GetPixel(0, 0));
        }

        [TestMethod()]
        public void AsyncProcess_CheckAlpha_AlphaTheSame()
        {
            ImageProcessing sut = GetSut();
            Bitmap input = new Bitmap(1, 1);
            input.SetPixel(0, 0, Color.FromArgb(127, 50, 123, 200));
            sut.LoadImage(input);

            sut.ToMainColors();

            Assert.AreEqual(127, sut.Result.GetPixel(0, 0).A);
        }

        public abstract ImageProcessing GetSut();
    }

    [TestClass()]
    public class AsynchronzousImageProcessingTest : ImageProcessingTests
    {
        public override ImageProcessing GetSut() => new AsynchronousImageProcessing();
    }

    [TestClass()]
    public class SynchronzousImageProcessingTest : ImageProcessingTests
    {
        public override ImageProcessing GetSut() => new SynchronzousImageProcessing();
    }
}