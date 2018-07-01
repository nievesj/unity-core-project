using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UniRx;

public static class ObservableExtensions
{
    /// <summary>
    /// AssetBundleRequest ToObservable extension
    /// </summary>
    /// <param name="asyncOperation"></param>
    /// <returns></returns>
    public static IObservable<UnityEngine.Object> ToObservable(this UnityEngine.AssetBundleRequest asyncOperation)
    {
        if (asyncOperation == null) throw new ArgumentNullException("asyncOperation");
        return Observable.FromCoroutine<UnityEngine.Object>((observer, cancellationToken) => RunAssetBundleRequestOperation(asyncOperation, observer, cancellationToken));
    }

    private static IEnumerator RunAssetBundleRequestOperation(UnityEngine.AssetBundleRequest asyncOperation, IObserver<UnityEngine.Object> observer, CancellationToken cancellationToken)
    {
        while (!asyncOperation.isDone && !cancellationToken.IsCancellationRequested)
            yield return null;

        if (!cancellationToken.IsCancellationRequested)
        {
            observer.OnNext(asyncOperation.asset);
            observer.OnCompleted();
        }
    }

    /// <summary>
    /// Converts a Task to an Observable, runs on main thread.
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public static IObservable<Unit> TaskToObservable(this Task task)
    {
        return Observable.Create<Unit>(
            (observer) =>
            {
                task.Run(_ =>
                {
                    observer.OnNext(new Unit());
                    observer.OnCompleted();
                });

                return Disposable.Empty;
            }
        );
    }

    /// <summary>
    /// Converts a Task<T/> to an Observable, runs on main thread.
    /// </summary>
    /// <param name="task"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IObservable<T> TaskToObservable<T>(this Task<T> task) where T : UnityEngine.Object
    {
        return Observable.Create<T>(
            (observer) =>
            {
                task.Run(x =>
                {
                    observer.OnNext(x);
                    observer.OnCompleted();
                });

                return Disposable.Empty;
            }
        );
    }
}