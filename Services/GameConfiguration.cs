using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Service
{
	/// <summary>
	/// Game configuration. This is the main configuration for the game.
	/// If there are non service related elements that should go here for configuration
	/// such as asset URLs or whatnot, feel free to add them, but note that you will
	/// have to create a custom editor for them in GameConfigurationEditor.
	/// </summary>
	[CreateAssetMenu(fileName = "Game", menuName = "Game Configuration", order = 1)]
	public class GameConfiguration : ScriptableObject
	{
		public bool disableLogging = false;
		public List<ServiceConfiguration> services;

		public void CreateServices(ServiceFramework application, System.Action onServicesCreated)
		{
			if (disableLogging)
				Debug.unityLogger.logEnabled = false;

			Debug.Log(("GameConfiguration: Starting Services").Colored(Colors.lime));
			foreach (var service in services)
			{
				Debug.Log(("--- Starting Service: " + service.name).Colored(Colors.cyan));
				service.CreateService(application);
			}

			onServicesCreated();
		}
	}
}