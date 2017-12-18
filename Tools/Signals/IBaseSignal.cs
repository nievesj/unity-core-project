using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Signals
{
	public interface IBaseSignal
	{
		/// Instruct a Signal to call on all its registered listeners
		void Dispatch(object[] args);

		/// Attach a callback to this Signal
		/// The callback parameters must match the Types and order which were
		/// originally assigned to the Signal on its creation
		void Add(Action<IBaseSignal, object[]> callback);

		/// Attach a callback to this Signal for the duration of exactly one Dispatch
		/// The callback parameters must match the Types and order which were
		/// originally assigned to the Signal on its creation, and the callback
		/// will be removed immediately after the Signal dispatches
		void AddOnce(Action<IBaseSignal, object[]> callback);

		/// Remove a callback from this Signal
		void Remove(Action<IBaseSignal, object[]> callback);

		/// Returns a List<System.Type> representing the Types bindable to this Signal
		List<Type> GetTypes();
	}
}
