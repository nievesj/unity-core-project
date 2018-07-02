using System;
using System.Threading.Tasks;

public static class UnityTaskExtensions
{
    /// <summary>
    /// Runs a task on main thread and calls onComplete when done.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="onComplete"></param>
    /// <typeparam name="T"></typeparam>
     public static void Run<T>(this Task<T> task, Action<T> onComplete = null)
     {
         Task.Run(async () =>
         {
             await Awaiters.WaitForUpdate; //Wait for main thread
             var val = await task;
             onComplete?.Invoke(val);
         });
     }

    /// <summary>
    /// Runs a task on main thread and calls onComplete when done.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="onComplete"></param>
    public static void Run(this Task task, Action<object> onComplete = null)
    {
        Task.Run(async () =>
        {
            await Awaiters.WaitForUpdate; //Wait for main thread
            await task;
            onComplete?.Invoke(null);
        });
    }
}