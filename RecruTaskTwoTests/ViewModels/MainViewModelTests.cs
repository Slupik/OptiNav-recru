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
                .Callback(() =>
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

        [TestMethod()]
        public void ProcessImageAsync_SelectAsyncStrategy()
        {
            imageProcessingExecutor.Setup(executor => executor.Execute(It.IsAny<Task<ImageProcessingOutput>>())).Verifiable();

            sut.ProcessImageAsync();

            imageProcessingStrategy.VerifySet(strategy => strategy.AsyncStrategy = true, Times.Once());
        }

        [TestMethod()]
        public void ProcessImage_SelectSyncStrategy()
        {
            imageProcessingExecutor.Setup(executor => executor.Execute(It.IsAny<Task<ImageProcessingOutput>>())).Verifiable();

            sut.ProcessImageSync();

            imageProcessingStrategy.VerifySet(strategy => strategy.AsyncStrategy = false, Times.Once());
        }

        [TestMethod()]
        public void ProcessImageAsync_RunAsyncStrategy()
        {
            imageProcessingExecutor.Setup(executor => executor.Execute(It.IsAny<Task<ImageProcessingOutput>>())).Verifiable();

            sut.ProcessImageAsync();

            imageProcessingExecutor.Verify(executor => executor.Execute(It.IsAny<Task<ImageProcessingOutput>>()), Times.Once());
        }

        [TestMethod()]
        public void ProcessImage_RunSyncStrategy()
        {
            imageProcessingExecutor.Setup(executor => executor.Execute(It.IsAny<Task<ImageProcessingOutput>>())).Verifiable();

            sut.ProcessImageSync();

            imageProcessingExecutor.Verify(executor => executor.Execute(It.IsAny<Task<ImageProcessingOutput>>()), Times.Once());
        }

        [TestMethod()]
        public void ProcessImage_ShowProcessingState()
        {
            Action startCallback = () => Assert.Fail();
            imageProcessingExecutor.Setup(executor => executor.SetOnStart(It.IsAny<Action>()))
                .Callback<Action>(c => startCallback = c);
            Startup();
            Bitmap loadedImage = new Bitmap(2, 2);
            imageProcessingExecutor.Setup(executor => executor.Execute(It.IsAny<Task<ImageProcessingOutput>>()))
                .Callback(() =>
                {
                    startCallback();
                });

            sut.ProcessImageSync();

            Assert.AreEqual("Przetwarzanie zdjęcia...", sut.StateInformation);
            Assert.AreEqual(false, sut.AllowToInteract);
        }

        [TestMethod()]
        public void ProcessImage_ShowProcessingOutput()
        {
            Action startCallback = () => Assert.Fail();
            imageProcessingExecutor.Setup(executor => executor.SetOnStart(It.IsAny<Action>()))
                .Callback<Action>(c => startCallback = c);
            Action<ImageProcessingOutput> resultCallback = (_) => Assert.Fail();
            imageProcessingExecutor.Setup(executor => executor.SetOnResult(It.IsAny<Action<ImageProcessingOutput>>()))
                .Callback<Action<ImageProcessingOutput>>(c => resultCallback = c);
            Startup();
            Bitmap outputImage = new Bitmap(2, 2);
            TimeSpan processingTime = new TimeSpan(0, 0, 0, 0, 1);
            ImageProcessingOutput output = new ImageProcessingOutput(
                                                outputImage,
                                                processingTime
                                            );
            imageProcessingExecutor.Setup(executor => executor.Execute(It.IsAny<Task<ImageProcessingOutput>>()))
                .Callback(() =>
                {
                    startCallback();
                    resultCallback(output);
                });

            sut.ProcessImageSync();

            Assert.IsTrue(outputImage.ConvertToUiElement().IsEqual(sut.OutputImage));
            Assert.AreEqual("00:00:00.001", sut.ProcessingTime);
            Assert.AreEqual(true, sut.TimeInfoContainerIsVisible);
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