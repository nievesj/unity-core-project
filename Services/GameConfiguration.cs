using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services
{
	/// <summary>
	/// Game configuration. Contains the configuration of all the services to be started when the game starts.
	/// </summary>
	[CreateAssetMenu(fileName = "Game", menuName = "Game Configuration", order = 1)]
	public class GameConfiguration : ScriptableObject
	{
		public bool disableLogging = false;
		public List<ServiceConfiguration> services;
	}
}