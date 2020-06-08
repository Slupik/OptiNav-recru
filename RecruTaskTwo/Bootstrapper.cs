using Caliburn.Micro;
using RecruTaskTwo.Logic;
using RecruTaskTwo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TaskLibrary;

namespace RecruTaskTwo
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }

        protected override void Configure()
        {
            _container.Singleton<IWindowManager, WindowManager>();
            _container.RegisterSingleton(typeof(IImageProcessing), "sync", typeof(SynchronzousImageProcessing));
            _container.RegisterSingleton(typeof(IImageProcessing), "async", typeof(AsynchronousImageProcessing));
            _container.RegisterHandler(typeof(ImageProcessingStrategy), null, container =>
            {
                var sync = IoC.Get<IImageProcessing>("sync");
                var async = IoC.Get<IImageProcessing>("async");
                return new ImageProcessingStrategy(sync, async);
            });
            _container.Singleton<MainViewModel>();
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            return _container.GetInstance(serviceType, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

    }
}
