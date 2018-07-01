using System;
using System.Threading.Tasks;

public static class UnityTaskExtensions
{
    /// <summary>
    /// Runs a task and calls onComplete when done.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="onComplete"></param>
    /// <typeparam name="T"></typeparam>
     public static void Run<T>(this Task<T> task, Action<T> onComplete = null)
     {
         Func<Task> runTask = async () =>
         {
             var val = await task;
             onComplete?.Invoke(val);
         };
         runTask();
     }

    /// <summary>
    /// Runs a task and calls onComplete when done.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="onComplete"></param>
    public static void Run(this Task task, Action<object> onComplete = null)
    {
        Func<Task> runTask = async () =>
        {
            await task;
            onComplete?.Invoke(null);
        };
        runTask();
    }
}