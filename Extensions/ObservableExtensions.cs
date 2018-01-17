using System;
using System.Collections;
using UniRx;

public static class ObservableExtensions
{
	// public static IObservable<float> ToObservable(this UnityEngine.AsyncOperation asyncOperation)
	// {
	// 	if (asyncOperation == null) throw new ArgumentNullException("asyncOperation");

	// 	return Observable.FromCoroutine<float>((observer, cancellationToken) => RunAsyncOperation(asyncOperation, observer, cancellationToken));
	// }

	// static IEnumerator RunAsyncOperation(UnityEngine.AsyncOperation asyncOperation, IObserver<float> observer, CancellationToken cancellationToken)
	// {
	// 	while (!asyncOperation.isDone && !cancellationToken.IsCancellationRequested)
	// 	{
	// 		observer.OnNext(asyncOperation.progress);
	// 		yield return null;
	// 	}
	// 	if (!cancellationToken.IsCancellationRequested)
	// 	{
	// 		observer.OnNext(asyncOperation.progress); // push 100%
	// 		observer.OnCompleted();
	// 	}
	// }

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

	static IEnumerator RunAssetBundleRequestOperation(UnityEngine.AssetBundleRequest asyncOperation, IObserver<UnityEngine.Object> observer, CancellationToken cancellationToken)
	{
		while (!asyncOperation.isDone && !cancellationToken.IsCancellationRequested)
			yield return null;

		if (!cancellationToken.IsCancellationRequested)
		{
			observer.OnNext(asyncOperation.asset);
			observer.OnCompleted();
		}
	}
}