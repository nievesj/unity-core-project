using System;
using System.Threading.Tasks;
using UniRx.Async;

public static class UnityTaskExtensions
{
    /// <summary>
    /// Runs a Task on main thread and calls onComplete when done.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="onComplete"></param>
    /// <typeparam name="T"></typeparam>
    public static Task Run<T>(this Task<T> task, Action<T> onComplete = null)
    {
        return Task.Run(async () =>
        {
            await UniTask.Yield();
            var val = await task;
            onComplete?.Invoke(val);
        });
    }

    /// <summary>
    /// Runs a Task on main thread and calls onComplete when done.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="onComplete"></param>
    public static Task Run(this Task task, Action<object> onComplete = null)
    {
        return Task.Run(async () =>
        {
            await UniTask.Yield();
            await task;
            onComplete?.Invoke(null);
        });
    }

    /// <summary>
    /// Runs a UniTask and calls onComplete when done.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="onComplete"></param>
    public static void Run(this UniTask task, Action<object> onComplete = null)
    {
        TaskRunner.RunTask(task, onComplete);
    }

    /// <summary>
    ///  Runs a UniTask and calls onComplete when done.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="onComplete"></param>
    /// <typeparam name="T"></typeparam>
    public static void Run<T>(this UniTask<T> task, Action<T> onComplete = null)
    {
        TaskRunner<T>.RunTask(task, onComplete);
    }

    private struct TaskRunner
    {
        public static async UniTask RunTask(UniTask task, Action<object> onComplete)
        {
            await task;
            onComplete?.Invoke(null);
        }
    }

    private struct TaskRunner<T>
    {
        public static async UniTask RunTask(UniTask<T> task, Action<T> onComplete)
        {
            var result = await task;
            onComplete?.Invoke(result);
        }
    }
}