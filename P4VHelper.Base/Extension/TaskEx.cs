// jdyun 24/04/06(토)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.Extension
{
    public static class TaskEx
    {
        public static Task StartSafe(this Task _task, Action<Exception> _errorHandler = null)
        {
            try
            {
                _task.Start();
                return _task;
            }
            catch (Exception ex)
            {
                _errorHandler?.Invoke(ex);
                return _task;
            }
        }

        public static Task<T> StartSafe<T>(this Task<T> _task, Action<Exception> _errorHandler = null)
        {
            try
            {
                _task.Start();
                return _task;
            }
            catch (Exception ex)
            {
                _errorHandler?.Invoke(ex);
                return _task;
            }
        }
    }
}
