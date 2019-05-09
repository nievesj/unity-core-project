using System;
using UniRx;

namespace Core.Reactive
{
    /// <summary>
    /// Custom implementation of ReactiveProperty. Noticed that the UniRx version is kinda funky, so I created a trimmed down version.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReactiveProperty<T>
    {
        T Value { get; set; }
    }

    internal class Observer<T> : IObserver<T>, IDisposable
    {
        private readonly IObserver<T> _observer;

        public Observer(IObserver<T> observer)
        {
            _observer = observer;
        }

        public void OnCompleted()
        {
            _observer.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _observer.OnError(error);
        }

        public void OnNext(T value)
        {
            _observer.OnNext(value);
        }

        public void Dispose()
        {
            OnCompleted();
        }
    }

    public class CoreReactiveProperty<T> : IReactiveProperty<T>, IDisposable, IOptimizedObservable<T>
    {
        private T _value;
        private Observer<T> _observable;

        public T Value { get => _value; set => SetValue(value); }

        public CoreReactiveProperty()
            : this(default) { }

        public CoreReactiveProperty(T value)
        {
            _value = value;
        }

        private void SetValue(T value)
        {
            _value = value;
            RaiseOnNext(ref _value);
        }

        private void RaiseOnNext(ref T value)
        {
            _observable?.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observable = new Observer<T>(observer);
            return _observable;
        }

        public void Dispose()
        {
            _observable?.OnCompleted();
        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
        }
    }
}