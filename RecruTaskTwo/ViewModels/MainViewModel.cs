using Caliburn.Micro;
using Microsoft.Win32;
using RecruTaskTwo.Logic;
using RecruTaskTwo.Models;
using RecruTaskTwo.Utils;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static RecruTaskTwo.Utils.FileChooser;

namespace RecruTaskTwo.ViewModels
{
    public class MainViewModel : Screen
    {
        private static readonly BitmapImage EMPTY_IMAGE = new Bitmap(1, 1).ConvertToUiElement();
        private readonly ImageProcessingStrategy imageProcessor;
        private readonly FileChooser fileChooser;

        public MainViewModel(ImageProcessingStrategy imageProcessingStrategy, FileChooser fileChooser)
        {
            imageProcessor = imageProcessingStrategy;
            this.fileChooser = fileChooser;
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

        async void OnFileSelect(string path)
        {
            AllowToInteract = false;
            TimeInfoContainerIsVisible = false;
            ImagePath = path;
            StateInformation = "Wczytywanie pliku...";

            Bitmap result = await Task.Run(() =>
            {
                return imageProcessor.LoadImage(path);
            });

            InputImage = result.ConvertToUiElement();
            OutputImage = EMPTY_IMAGE;
            ImageIsSelected = true;
            AllowToInteract = true;
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

        private async void ProcessImage()
        {
            AllowToInteract = false;

            ImageProcessingOutput result = await Task.Run(() =>
            {
                return imageProcessor.ProcessLoadedImage();
            });

            ProcessingTime = result.ComputationTime;
            TimeInfoContainerIsVisible = true;
            AllowToInteract = true;
            OutputImage = result.Image.ConvertToUiElement();
        }
    }
}
