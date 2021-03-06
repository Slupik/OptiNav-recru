﻿using RecruTaskTwo.Models;
using System;
using System.Diagnostics;
using System.Drawing;
using TaskLibrary;

namespace RecruTaskTwo.Logic
{

    public interface IImageProcessingStrategy
    {
        bool AsyncStrategy { get; set; }

        Bitmap LoadImage(String path);
        ImageProcessingOutput ProcessLoadedImage();
    }

    public class ImageProcessingStrategy : IImageProcessingStrategy
    {
        private IImageProcessing Sync { get; }
        private IImageProcessing Async { get; }

        public bool AsyncStrategy { get; set; }

        public ImageProcessingStrategy(IImageProcessing sync, IImageProcessing async)
        {
            AsyncStrategy = true;
            Sync = sync;
            Async = async;
        }

        public Bitmap LoadImage(String path)
        {
            if (AsyncStrategy)
            {
                Async.LoadImage(path);
                Sync.LoadImage(Async.Oryginal);
            }
            else
            {
                Sync.LoadImage(path);
                Async.LoadImage(Sync.Oryginal);
            }
            return Async.Oryginal;
        }

        public ImageProcessingOutput ProcessLoadedImage()
        {
            IImageProcessing strategy = GetStrategy();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            strategy.ToMainColors();
            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;
            return new ImageProcessingOutput(strategy.Result, ts);
        }

        private IImageProcessing GetStrategy() => AsyncStrategy? Async : Sync;
    }
}
