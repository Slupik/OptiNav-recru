using Caliburn.Micro;
using Microsoft.Win32;
using RecruTaskOne;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RecruTaskTwo.ViewModels
{
    public class MainViewModel : Screen
    {
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

        public async void LoadImage()
        {
            AllowToInteract = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
                await Task.Run(() =>
                {
                    StateInformation = "Wczytywanie pliku...";
                    SynchronousProcessor.LoadImage(openFileDialog.FileName);
                    ImageIsSelected = true;
                    AllowToInteract = true;
                });
            }
        }

        public async void ProcessImage()
        {
            await Task.Run(() =>
            {
                AllowToInteract = false;
                StateInformation = "Przetwarzanie danych...";

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                SynchronousProcessor.ToMainColors();
                stopWatch.Stop();

                TimeSpan ts = stopWatch.Elapsed;
                ProcessingTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

                TimeInfoContainerIsVisible = true;
                AllowToInteract = true;
            });
        }
    }
}
