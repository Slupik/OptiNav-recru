using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruTaskTwo.Utils
{
    public class FileChooser
    {

        public delegate void Callback(String path);
        private readonly OpenFileDialog OpenFileDialog;

        public FileChooser(string filter)
        {
            OpenFileDialog = new OpenFileDialog
            {
                Filter = filter
            };
        }

        public void GetFilePath(Callback callback)
        {
            if (OpenFileDialog.ShowDialog() == true)
            {
                callback.Invoke(OpenFileDialog.FileName);
            }
        }

    }
}
