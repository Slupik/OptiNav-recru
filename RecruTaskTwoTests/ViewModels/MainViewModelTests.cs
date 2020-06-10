using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RecruTaskTwo.Logic;
using RecruTaskTwo.Models;
using RecruTaskTwo.Utils;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RecruTaskTwo.ViewModels.Tests
{
    [TestClass()]
    public class MainViewModelTests
    {
        private static readonly BitmapImage EMPTY_IMAGE = new Bitmap(1, 1).ConvertToUiElement();

        private readonly Mock<IImageProcessingStrategy> imageProcessingStrategy = new Mock<IImageProcessingStrategy>();
        private readonly Mock<IFileChooser> fileChooser = new Mock<IFileChooser>();
        private readonly Mock<ITaskExecutor<Bitmap>> imageLoadingExecutor = new Mock<ITaskExecutor<Bitmap>>();
        private readonly Mock<ITaskExecutor<ImageProcessingOutput>> imageProcessingExecutor = new Mock<ITaskExecutor<ImageProcessingOutput>>();
        private MainViewModel sut;


        [TestInitialize()]
        public void Startup()
        {
            sut = new MainViewModel(
                    imageProcessingStrategy.Object,
                    fileChooser.Object,
                    imageLoadingExecutor.Object,
                    imageProcessingExecutor.Object
            );
        }

        [TestMethod()]
        public void WantToLoadFile_OpenFileChooser()
        {
            sut.LoadImage();

            fileChooser.Verify(chooser => chooser.GetFilePath(It.IsAny<Callback>()), Times.Once());
            fileChooser.VerifyNoOtherCalls();
        }

        [TestMethod()]
        public void SelectFile_ShowNecessaryInfo()
        {
            string path = "foo/bar.png";

            sut.OnFileSelect(path);

            Assert.AreEqual(path, sut.ImagePath);
        }

        [TestMethod()]
        public void SelectFile_ExecuteLoadingTask()
        {
            string path = "foo/bar.png";
            imageLoadingExecutor.Setup(executor => executor.Execute(It.IsAny<Task<Bitmap>>())).Verifiable();

            sut.OnFileSelect(path);

            imageLoadingExecutor.Verify(executor => executor.Execute(It.IsAny<Task<Bitmap>>()), Times.Once());
        }

        [TestMethod()]
        public void LoadImage_TrigerLoader()
        {
            string path = "foo/bar.png";

            sut.OnFileSelect(path);

            imageProcessingStrategy.Verify(loader => loader.LoadImage(path), Times.Once());
            imageProcessingStrategy.VerifyNoOtherCalls();
        }

        [TestMethod()]
        public void LoadingImage_SetLoadingState()
        {
            Action startCallback = () => Assert.Fail();
            imageLoadingExecutor.Setup(executor => executor.SetOnStart(It.IsAny<Action>()))
                .Callback<Action>(c =>
                {
                    startCallback = c;
                });
            Startup();
            imageLoadingExecutor.Setup(executor => executor.Execute(It.IsAny<Task<Bitmap>>()))
                .Callback(()=>
                {
                    startCallback();
                });
            string path = "foo/bar.png";

            sut.OnFileSelect(path);

            Assert.AreEqual("Wczytywanie pliku...", sut.StateInformation);
            Assert.AreEqual(false, sut.AllowToInteract);
        }

        [TestMethod()]
        public void LoadingImage_ShowOutput()
        {
            Action startCallback = () => Assert.Fail();
            imageLoadingExecutor.Setup(executor => executor.SetOnStart(It.IsAny<Action>()))
                .Callback<Action>(c => startCallback = c);
            Action<Bitmap> resultCallback = (_) => Assert.Fail();
            imageLoadingExecutor.Setup(executor => executor.SetOnResult(It.IsAny<Action<Bitmap>>()))
                .Callback<Action<Bitmap>>(c => resultCallback = c);
            Startup();
            Bitmap loadedImage = new Bitmap(2, 2);
            imageLoadingExecutor.Setup(executor => executor.Execute(It.IsAny<Task<Bitmap>>()))
                .Callback(() =>
                {
                    startCallback();
                    resultCallback(loadedImage);
                });
            string path = "foo/bar.png";

            sut.OnFileSelect(path);

            Assert.IsTrue(EMPTY_IMAGE.IsEqual(sut.OutputImage));
            Assert.IsTrue(loadedImage.ConvertToUiElement().IsEqual(sut.InputImage));
            Assert.AreEqual(true, sut.ImageIsSelected);
            Assert.AreEqual(true, sut.AllowToInteract);
        }
    }
    public static class BitmapImageExtensions
    {
        public static bool IsEqual(this BitmapImage image1, BitmapImage image2)
        {
            if (image1 == null || image2 == null)
            {
                return false;
            }
            return image1.ToBytes().SequenceEqual(image2.ToBytes());
        }

        public static byte[] ToBytes(this BitmapImage image)
        {
            byte[] data = new byte[] { };
            if (image != null)
            {
                try
                {
                    var encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        data = ms.ToArray();
                    }
                    return data;
                }
                catch (Exception ex)
                {
                }
            }
            return data;
        }
    }
}