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
        public static Task StartSafe(this Task task, Action<Exception> errorHandler = null)
        {
            try
            {
                task.Start();
                return task;
            }
            catch (Exception ex)
            {
                errorHandler?.Invoke(ex);
                return task;
            }
        }

        public static Task<T> StartSafe<T>(this Task<T> task, Action<Exception> errorHandler = null)
        {
            try
            {
                task.Start();
                return task;
            }
            catch (Exception ex)
            {
                errorHandler?.Invoke(ex);
                return task;
            }
        }
    }
}
