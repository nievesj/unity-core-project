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

    public static IObservable<Unit> TaskToObservable(this Task task)
    {
        return Observable.Create<Unit>(
            (observer) =>
            {
                Func<Task> runTask = async () =>
                {
                    await task;
                    observer.OnNext(new Unit());
                    observer.OnCompleted();
                };
                runTask();
                return Disposable.Empty;
            }
        );
    }

    public static IObservable<T> TaskToObservable<T>(this Task<T> task) where T : UnityEngine.Object
    {
        return Observable.Create<T>(
            (observer) =>
            {
                Func<Task> runTask = async () =>
                {
                    var val = await task;
                    observer.OnNext(val);
                    observer.OnCompleted();
                };
                runTask();
                return Disposable.Empty;
            }
        );
    }
}