using System;
using UniRx;
using UnityEngine;

namespace Core.Services
{
    public abstract class CoreBehaviour : MonoBehaviour
    {
        private readonly Subject<CoreBehaviour> _onDestroyed = new Subject<CoreBehaviour>();

        /// <summary>
        /// Triggered when the object is destroyed
        /// </summary>
        /// <returns></returns>
        public IObservable<CoreBehaviour> OnDestroyed()
        {
            return _onDestroyed;
        }

        protected virtual void OnDestroy()
        {
            _onDestroyed.OnNext(this);
            _onDestroyed.OnCompleted();
        }
    }
}