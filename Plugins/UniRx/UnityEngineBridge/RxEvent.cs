#if CSHARP_7_OR_LATER || (UNITY_2018_3_OR_NEWER && (NET_STANDARD_2_0 || NET_4_6))
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#endif

#if !UniRxLibrary
using System;
using System.Collections.Generic;
using UnityEngine;
#endif
#if CSHARP_7_OR_LATER || (UNITY_2018_3_OR_NEWER && (NET_STANDARD_2_0 || NET_4_6))
using UniRx.InternalUtil;

#endif

namespace UniRx
{
    public interface IRxEvent<T> : IReadOnlyReactiveProperty<T>
    {
        new T Value { get; }
        void Broadcast(T val);
    }

    /// <summary>
    /// Streamlined version of ReactiveProperty. Main difference is that the value can only be changed by triggering Broadcast
    /// and the value is not emitted on subscription.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class RxEvent<T> : IRxEvent<T>, IDisposable, IOptimizedObservable<T>, IObserverLinkedList<T>
    {
#if !UniRxLibrary
        static readonly IEqualityComparer<T> defaultEqualityComparer = UnityEqualityComparer.GetDefault<T>();
#else
        static readonly IEqualityComparer<T> defaultEqualityComparer = EqualityComparer<T>.Default;
#endif
        
        T value = default(T);
        ObserverNode<T> root;
        ObserverNode<T> last;
        bool isDisposed = false;

        private IEqualityComparer<T> EqualityComparer
        {
            get { return defaultEqualityComparer; }
        }

        public T Value
        {
            get { return value; }
        }

        // always true, allows empty constructor 'can' publish value on subscribe.
        // because sometimes value is deserialized from UnityEngine.
        public bool HasValue
        {
            get { return true; }
        }

        public RxEvent()
            : this(default(T)) { }

        public RxEvent(T initialValue)
        {
            value = initialValue;
        }

        public void Broadcast(T val)
        {
            value = val;
            RaiseOnNext(ref value);
        }

        private void RaiseOnNext(ref T value)
        {
            var node = root;
            while (node != null)
            {
                node.OnNext(value);
                node = node.Next;
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (isDisposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            // subscribe node, node as subscription.
            var next = new ObserverNode<T>(this, observer);
            if (root == null)
            {
                root = last = next;
            }
            else
            {
                last.Next = next;
                next.Previous = last;
                last = next;
            }

            return next;
        }

        void IObserverLinkedList<T>.UnsubscribeNode(ObserverNode<T> node)
        {
            if (node == root)
            {
                root = node.Next;
            }

            if (node == last)
            {
                last = node.Previous;
            }

            if (node.Previous != null)
            {
                node.Previous.Next = node.Next;
            }

            if (node.Next != null)
            {
                node.Next.Previous = node.Previous;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (isDisposed) return;

            var node = root;
            root = last = null;
            isDisposed = true;

            while (node != null)
            {
                node.OnCompleted();
                node = node.Next;
            }
        }

        public override string ToString()
        {
            return value == null ? "(null)" : value.ToString();
        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
        }
    }
}