using Caliburn.Micro;
using Microsoft.Win32;
using RecruTaskOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void LoadImage()
        {
            ProcessingTime = "30";
            ImagePath = "img path";
            TimeInfoContainerIsVisible = true;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
                AsynchronousProcessor.LoadImage(openFileDialog.FileName);
                ImageIsSelected = true;
            }
        }
    }
}
