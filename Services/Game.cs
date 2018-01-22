using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Service
{
	public class Game : MonoBehaviour
	{
		[SerializeField]
		protected GameConfiguration configuration;
		public GameConfiguration GameConfiguration { get { return configuration; } }

		protected void Awake()
		{
			DontDestroyOnLoad(this.gameObject);
			Services.SetUp(this);
		}
	}
}