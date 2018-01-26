using System.Collections;
using System.Collections.Generic;
using Core.LevelLoaderService;
using Core.UI;
using UnityEngine;

namespace Core.Service
{
	public class Game : MonoBehaviour
	{
		[SerializeField]
		protected GameConfiguration configuration;
		public GameConfiguration GameConfiguration { get { return configuration; } }

		protected ILevelLoaderService LevelLoader { get { return ServiceLocator.GetService<ILevelLoaderService>(); } }
		protected IUIService UILoader { get { return ServiceLocator.GetService<IUIService>(); } }

		protected virtual void Awake()
		{
			DontDestroyOnLoad(this.gameObject);
			ServiceLocator.SetUp(this);
		}
	}
}