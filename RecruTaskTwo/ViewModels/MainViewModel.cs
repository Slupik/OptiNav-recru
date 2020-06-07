using Caliburn.Micro;
using Microsoft.Win32;
using TaskLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using RecruTaskTwo.Utils;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing.Imaging;

namespace RecruTaskTwo.ViewModels
{
    public class MainViewModel : Screen
    {
        private static readonly BitmapImage EMPTY_IMAGE = new Bitmap(1, 1).ConvertToUiElement();
        private readonly IImageProcessing AsynchronousProcessor = new AsynchronousImageProcessing();
        private readonly IImageProcessing SynchronousProcessor = new SynchronzousImageProcessing();

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

        public async void LoadImage()
        {
            AllowToInteract = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
                StateInformation = "Wczytywanie pliku...";
                await Task.Run(() =>
                {
                    SynchronousProcessor.LoadImage(openFileDialog.FileName);
                });
                ImageIsSelected = true;
                AllowToInteract = true;
                InputImage = SynchronousProcessor.Oryginal.ConvertToUiElement();
                OutputImage = EMPTY_IMAGE;
            }
        }

        public async void ProcessImage()
        {
            AllowToInteract = false;
            StateInformation = "Przetwarzanie danych...";
            await Task.Run(() =>
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                SynchronousProcessor.ToMainColors();
                stopWatch.Stop();

                TimeSpan ts = stopWatch.Elapsed;
                ProcessingTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            });
            TimeInfoContainerIsVisible = true;
            AllowToInteract = true;
            OutputImage = SynchronousProcessor.Result.ConvertToUiElement();
        }
    }
}
