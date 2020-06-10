using RecruTaskTwo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruTaskTwo.Logic
{

    public interface ITaskExecutor<TResult>
    {
        Task<TResult> Task { get; }

        void SetOnStart(Action callback);
        void SetOnResult(Action<TResult> callback);
        void SetOnError(Action callback);
        void Execute(Task<TResult> task);
    }

    public class TaskExecutor<TResult> : ITaskExecutor<TResult>
    {
        public Action StartCallback;
        public Action<TResult> ResultCallback;
        public Action ErrorCallback;

        public Task<TResult> Task { get; private set; }
        private TResult Result
        {
            get
            {
                return (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default;
            }
        }

        public void Execute(Task<TResult> task)
        {
            if (Task == null || Task.IsCompleted)
            {
                Task = task;
                if (!task.IsCompleted)
                {
                    var _ = WatchTaskAsync(Task);
                }
            }
        }

        public void SetOnError(Action callback)
        {
            ErrorCallback = callback;
        }

        public void SetOnResult(Action<TResult> callback)
        {
            ResultCallback = callback;
        }

        public void SetOnStart(Action callback)
        {
            StartCallback = callback;
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
