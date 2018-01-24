using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Events
{
	public class Event<T> : IDisposable
	{
		readonly Subject<T> _subject = new Subject<T>();

		public IDisposable Subscribe(Action<T> action)
		{
			return _subject
				.AsObservable()
				.Subscribe(action);
		}

		public void Publish(T o)
		{
			_subject.OnNext(o);
		}

		public void OnComplete()
		{
			_subject.OnCompleted();
		}

		public void OnError(Exception ex)
		{
			_subject.OnError(ex);
		}

		public void Dispose()
		{
			_subject.Dispose();
		}
	}
}