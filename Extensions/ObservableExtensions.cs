﻿using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public static class ObservableExtensions
{
    /// <summary>
    /// AssetBundleRequest ToObservable extension
    /// </summary>
    /// <param name="asyncOperation"></param>
    /// <returns></returns>
    public static IObservable<UnityEngine.Object> ToObservable(this UnityEngine.AssetBundleRequest asyncOperation)
    {
        if (asyncOperation == null) throw new ArgumentNullException(nameof(asyncOperation));
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
        return Observable.Create<Unit>(observer =>
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
        return Observable.Create<T>(observer =>
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

    /// <summary>
    /// Simpler delay extension that just receives a float
    /// </summary>
    /// <param name="source"></param>
    /// <param name="seconds"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IObservable<T> Delay<T>(this IObservable<T> source, float seconds)
    {
        return source.Delay(TimeSpan.FromSeconds(seconds), Scheduler.DefaultSchedulers.TimeBasedOperations);
    }

    /// <summary>
    /// Starts a particle system, then observe when a it stops playing
    /// </summary>
    /// <param name="particle"></param>
    /// <returns></returns>
    public static IObservable<ParticleSystem> PlayRx(this ParticleSystem particle)
    {
        particle.Play();
        return Observable.FromCoroutine<ParticleSystem>(observer => OnParticleStopAsync(observer, particle));
    }

    /// <summary>
    /// Starts a particle system, waits for X seconds then stops it
    /// </summary>
    /// <param name="particle"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static IObservable<ParticleSystem> PlayRx(this ParticleSystem particle, float seconds)
    {
        particle.Play();
        return Observable.FromCoroutine<ParticleSystem>(observer => OnParticleStopAsync(observer, particle, seconds));
    }

    private static IEnumerator OnParticleStopAsync(IObserver<ParticleSystem> observer, ParticleSystem particle)
    {
        while (particle.isPlaying)
            yield return null;

        observer.OnNext(particle);
        observer.OnCompleted();
    }

    private static IEnumerator OnParticleStopAsync(IObserver<ParticleSystem> observer, ParticleSystem particle, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        particle.Stop();

        yield return new WaitForSeconds(1.5f);

        observer.OnNext(particle);
        observer.OnCompleted();
    }
}