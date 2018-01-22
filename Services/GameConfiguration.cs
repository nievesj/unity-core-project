using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Service
{
	[CreateAssetMenu(fileName = "Game", menuName = "Game Configuration", order = 1)]
	public class GameConfiguration : ScriptableObject
	{
		public bool disableLogging = false;
		public List<ServiceConfiguration> services;
	}
}