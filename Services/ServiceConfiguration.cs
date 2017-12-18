using System;
using System.Collections;
using UnityEngine;

namespace Core.Service
{
	/// <summary>
	/// Service configuration. Inherit from this to define the configuration for a given service.
	/// The GetServiceClass and ShowEditorUI methods must be implemented in child classes.
	/// 
	/// Note: this class MUST be named the same as the service + "Config" or "Configuration".
	/// </summary>
	public abstract class ServiceConfiguration : ScriptableObject
	{
		/// <summary>
		/// Create a new instance of the service for this configuration.
		/// You must override this in each child class.
		/// </summary>
		/// <returns>The newly created service instance.</returns>
		protected abstract IService GetServiceClass();

		/// <summary>
		/// Creates the service. This is called during game startup to construct and configure the service.
		/// </summary>
		/// <returns>The interface to the newly created service.</returns>
		/// <param name="application">Application.</param>
		public IService CreateService(ServiceFramework application)
		{
			IService service = GetServiceClass();
			if (service == null) return null;
			service.Configure(this);
			application.AddService(name, service);

			return service;
		}

		/// <summary>
		/// Shows the editor UI. This is used for the custom service editor. You must override this in each child class.
		/// </summary>
		public abstract void ShowEditorUI();
	}

}