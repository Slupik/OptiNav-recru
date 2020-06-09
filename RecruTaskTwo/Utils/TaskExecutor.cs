using RecruTaskTwo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruTaskTwo.Logic
{

    public class TaskExecutor<TResult>
    {
        public delegate void OnStart();
        public delegate void OnResult(TResult output);
        public delegate void OnError();

        public OnStart StartCallback;
        public OnResult ResultCallback;
        public OnError ErrorCallback;

        public Task<TResult> Task { get; private set; }
        public TResult Result
        {
            get
            {
                return (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default;
            }
        }

        public TaskExecutor(Task<TResult> task)
        {
            Task = task;
        }

        public void Execute()
        {
            if (!Task.IsCompleted)
            {
                var _ = WatchTaskAsync(Task);
            }
        }

        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                StartCallback();
                await task;
                ResultCallback(Result);
            }
            catch
            {
                ErrorCallback();
            }
        }
    }

}
