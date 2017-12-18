using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Signals
{
	/// Base concrete form for a Signal with no parameters
	public class Signal : BaseSignal
	{
		public event Action Listener = delegate {};
		public event Action OnceListener = delegate {};

		public void Add(Action callback)
		{
			Listener = AddUnique(Listener, callback);
		}

		public void AddOnce(Action callback)
		{
			OnceListener = AddUnique(OnceListener, callback);
		}
		public void Remove(Action callback) { Listener -= callback; }
		public override List<Type> GetTypes()
		{
			return new List<Type>();
		}
		public void Dispatch()
		{
			Listener();
			OnceListener();
			OnceListener = delegate {};
			base.Dispatch(null);
		}

		private Action AddUnique(Action listeners, Action callback)
		{
			if (!listeners.GetInvocationList().Contains(callback))
			{
				listeners += callback;
			}
			return listeners;
		}
	}

	/// Base concrete form for a Signal with one parameter
	public class Signal<T> : BaseSignal
	{
		public event Action<T> Listener = delegate {};
		public event Action<T> OnceListener = delegate {};

		public void Add(Action<T> callback)
		{
			Listener = AddUnique(Listener, callback);
		}

		public void AddOnce(Action<T> callback)
		{
			OnceListener = AddUnique(OnceListener, callback);
		}

		public void Remove(Action<T> callback) { Listener -= callback; }
		public override List<Type> GetTypes()
		{
			List<Type> retv = new List<Type>();
			retv.Add(typeof(T));
			return retv;
		}
		public void Dispatch(T type1)
		{
			Listener(type1);
			OnceListener(type1);
			OnceListener = delegate {};
			object[] outv = { type1 };
			base.Dispatch(outv);
		}

		private Action<T> AddUnique(Action<T> listeners, Action<T> callback)
		{
			if (!listeners.GetInvocationList().Contains(callback))
			{
				listeners += callback;
			}
			return listeners;
		}
	}

	/// Base concrete form for a Signal with two parameters
	public class Signal<T, U> : BaseSignal
	{
		public event Action<T, U> Listener = delegate {};
		public event Action<T, U> OnceListener = delegate {};

		public void Add(Action<T, U> callback)
		{
			Listener = AddUnique(Listener, callback);
		}

		public void AddOnce(Action<T, U> callback)
		{
			OnceListener = AddUnique(OnceListener, callback);
		}

		public void Remove(Action<T, U> callback) { Listener -= callback; }
		public override List<Type> GetTypes()
		{
			List<Type> retv = new List<Type>();
			retv.Add(typeof(T));
			retv.Add(typeof(U));
			return retv;
		}
		public void Dispatch(T type1, U type2)
		{
			Listener(type1, type2);
			OnceListener(type1, type2);
			OnceListener = delegate {};
			object[] outv = { type1, type2 };
			base.Dispatch(outv);
		}
		private Action<T, U> AddUnique(Action<T, U> listeners, Action<T, U> callback)
		{
			if (!listeners.GetInvocationList().Contains(callback))
			{
				listeners += callback;
			}
			return listeners;
		}
	}

	/// Base concrete form for a Signal with three parameters
	public class Signal<T, U, V> : BaseSignal
	{
		public event Action<T, U, V> Listener = delegate {};
		public event Action<T, U, V> OnceListener = delegate {};

		public void Add(Action<T, U, V> callback)
		{
			Listener = AddUnique(Listener, callback);
		}

		public void AddOnce(Action<T, U, V> callback)
		{
			OnceListener = AddUnique(OnceListener, callback);
		}

		public void Remove(Action<T, U, V> callback) { Listener -= callback; }
		public override List<Type> GetTypes()
		{
			List<Type> retv = new List<Type>();
			retv.Add(typeof(T));
			retv.Add(typeof(U));
			retv.Add(typeof(V));
			return retv;
		}
		public void Dispatch(T type1, U type2, V type3)
		{
			Listener(type1, type2, type3);
			OnceListener(type1, type2, type3);
			OnceListener = delegate {};
			object[] outv = { type1, type2, type3 };
			base.Dispatch(outv);
		}
		private Action<T, U, V> AddUnique(Action<T, U, V> listeners, Action<T, U, V> callback)
		{
			if (!listeners.GetInvocationList().Contains(callback))
			{
				listeners += callback;
			}
			return listeners;
		}
	}

	/// Base concrete form for a Signal with four parameters
	public class Signal<T, U, V, W> : BaseSignal
	{
		public event Action<T, U, V, W> Listener = delegate {};
		public event Action<T, U, V, W> OnceListener = delegate {};

		public void Add(Action<T, U, V, W> callback)
		{
			Listener = AddUnique(Listener, callback);
		}

		public void AddOnce(Action<T, U, V, W> callback)
		{
			OnceListener = AddUnique(OnceListener, callback);
		}

		public void Remove(Action<T, U, V, W> callback) { Listener -= callback; }
		public override List<Type> GetTypes()
		{
			List<Type> retv = new List<Type>();
			retv.Add(typeof(T));
			retv.Add(typeof(U));
			retv.Add(typeof(V));
			retv.Add(typeof(W));
			return retv;
		}
		public void Dispatch(T type1, U type2, V type3, W type4)
		{
			Listener(type1, type2, type3, type4);
			OnceListener(type1, type2, type3, type4);
			OnceListener = delegate {};
			object[] outv = { type1, type2, type3, type4 };
			base.Dispatch(outv);
		}

		private Action<T, U, V, W> AddUnique(Action<T, U, V, W> listeners, Action<T, U, V, W> callback)
		{
			if (!listeners.GetInvocationList().Contains(callback))
			{
				listeners += callback;
			}
			return listeners;
		}
	}

}