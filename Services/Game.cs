using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Service
{
	public class Game : MonoBehaviour
	{
		[SerializeField]
		protected GameConfiguration configuration;

		protected ServiceFramework core;

		protected void Awake()
		{
			core = ServiceFramework.Instance;
			DontDestroyOnLoad(this.gameObject);

			core.SetUp(configuration);
		}
	}
}