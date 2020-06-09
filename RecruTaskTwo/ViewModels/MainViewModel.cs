using Caliburn.Micro;
using Microsoft.Win32;
using RecruTaskTwo.Logic;
using RecruTaskTwo.Models;
using RecruTaskTwo.Utils;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
//using static RecruTaskTwo.Logic.TaskExecutor<RecruTaskTwo.Models.ImageProcessingOutput>;
using static RecruTaskTwo.Utils.FileChooser;

namespace RecruTaskTwo.ViewModels
{
    public class MainViewModel : Screen
    {
        private static readonly BitmapImage EMPTY_IMAGE = new Bitmap(1, 1).ConvertToUiElement();
        private readonly ImageProcessingStrategy imageProcessor;
        private readonly FileChooser fileChooser;
        private readonly TaskExecutor<Bitmap> imageLoadingExecutor;
        private readonly TaskExecutor<ImageProcessingOutput> imageProcessingExecutor;

        public MainViewModel(ImageProcessingStrategy imageProcessingStrategy, ImageFilesChooser fileChooser, TaskExecutor<Bitmap> imageLoadingExecutor,
            TaskExecutor<ImageProcessingOutput> imageProcessingExecutor)
        {
            imageProcessor = imageProcessingStrategy;
            this.fileChooser = fileChooser;
            this.imageLoadingExecutor = imageLoadingExecutor;
            this.imageProcessingExecutor = imageProcessingExecutor;

            SetupImageLoadingExecutor();
            SetupImageProcessingExecutor();
        }

        private void SetupImageLoadingExecutor()
        {
            imageLoadingExecutor.StartCallback = new TaskExecutor<Bitmap>.OnStart(() =>
            {
                AllowToInteract = false;
                StateInformation = "Wczytywanie pliku...";
            });
            imageLoadingExecutor.ResultCallback = new TaskExecutor<Bitmap>.OnResult(result =>
            {
                InputImage = result.ConvertToUiElement(); ;
                OutputImage = EMPTY_IMAGE;
                ImageIsSelected = true;
                AllowToInteract = true;
            });
        }

        private void SetupImageProcessingExecutor()
        {
            imageProcessingExecutor.StartCallback = new TaskExecutor<ImageProcessingOutput>.OnStart(() =>
            {
                AllowToInteract = false;
            });
            imageProcessingExecutor.ResultCallback = new TaskExecutor<ImageProcessingOutput>.OnResult(result =>
            {
                ProcessingTime = result.ComputationTime;
                TimeInfoContainerIsVisible = true;
                AllowToInteract = true;
                OutputImage = result.Image.ConvertToUiElement();
            });
        }

        private bool _timeInfoContainerIsVisible = false;
        public bool TimeInfoContainerIsVisible
        {
            get { return _timeInfoContainerIsVisible; }
            set
            {
                _timeInfoContainerIsVisible = value;
                NotifyOfPropertyChange(() => TimeInfoContainerIsVisible);
            }
        }

        private bool _imageIsSelected = false;
        public bool ImageIsSelected
        {
            get { return _imageIsSelected; }
            set
            {
                _imageIsSelected = value;
                NotifyOfPropertyChange(() => ImageIsSelected);
            }
        }

        private bool _stateInformationIsVisible = false;
        public bool StateInformationIsVisible
        {
            get { return _stateInformationIsVisible; }
            set
            {
                _stateInformationIsVisible = value;
                NotifyOfPropertyChange(() => StateInformationIsVisible);
            }
        }

        private bool _allowToInteract = true;
        public bool AllowToInteract
        {
            get { return _allowToInteract; }
            set
            {
                _allowToInteract = value;
                if (value) StateInformationIsVisible = false;
                NotifyOfPropertyChange(() => AllowToInteract);
            }
        }

        private string _stateInformation;
        public string StateInformation
        {
            get { return _stateInformation; }
            set
            {
                _stateInformation = value;
                StateInformationIsVisible = true;
                NotifyOfPropertyChange(() => StateInformation);
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                NotifyOfPropertyChange(() => ImagePath);
            }
        }

        private string _processingTime;
        public string ProcessingTime
        {
            get { return _processingTime; }
            set
            {
                _processingTime = value;
                NotifyOfPropertyChange(() => ProcessingTime);
            }
        }

        private BitmapImage _inputImage;
        public BitmapImage InputImage
        {
            get { return _inputImage; }
            set
            {
                _inputImage = value;
                NotifyOfPropertyChange(() => InputImage);
            }
        }

        private BitmapImage _outputImage;
        public BitmapImage OutputImage
        {
            get { return _outputImage; }
            set
            {
                _outputImage = value;
                NotifyOfPropertyChange(() => OutputImage);
            }
        }

        public void LoadImage()
        {
            fileChooser.GetFilePath(new Callback(OnFileSelect));
        }

        public void OnFileSelect(string path)
        {
            ImagePath = path;
            TimeInfoContainerIsVisible = false;
            imageLoadingExecutor.Execute(GetFileReadingTask(path));
        }

        async Task<Bitmap> GetFileReadingTask(string path)
        {
            return await Task.Run(() =>
            {
                return imageProcessor.LoadImage(path);
            });
        }

        public void ProcessImageAsync()
        {
            imageProcessor.AsyncStrategy = true;
            ProcessImage();
        }

        public void ProcessImageSync()
        {
            imageProcessor.AsyncStrategy = false;
            ProcessImage();
        }

        public void ProcessImage()
        {
            imageProcessingExecutor.Execute(GetProcessingTask());
        }

        async Task<ImageProcessingOutput> GetProcessingTask()
        {
            return await Task.Run(() =>
            {
                return imageProcessor.ProcessLoadedImage();
            });
        }
    }
}
