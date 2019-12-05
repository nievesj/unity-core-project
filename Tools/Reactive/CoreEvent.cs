using System;
using UniRx;

namespace Core.Reactive
{
    public class CoreEvent<T> : IDisposable, IOptimizedObservable<T>
    {
        private Observer<T> _observable;
        private T _value;

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observable = new Observer<T>(observer);
            return _observable;
        }

        public void Broadcast(T value)
        {
            RaiseOnNext(ref value);
        }

        public void Dispose()
        {
            _observable?.OnCompleted();
        }

        private void RaiseOnNext(ref T value)
        {
            _observable?.OnNext(value);
        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
        }
    }
}