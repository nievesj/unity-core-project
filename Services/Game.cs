using System.Collections;
using System.Collections.Generic;
using Core.LevelLoaderService;
using Core.Scenes;
using Core.UI;
using UniRx;
using UnityEngine;

namespace Core.Service
{
	public class Game : MonoBehaviour
	{
		[SerializeField]
		protected GameConfiguration configuration;
		public GameConfiguration GameConfiguration { get { return configuration; } }

		protected ILevelLoaderService LevelLoader { get { return ServiceLocator.GetService<ILevelLoaderService>(); } }
		protected ISceneLoaderService SceneLoader { get { return ServiceLocator.GetService<ISceneLoaderService>(); } }
		protected IUIService UILoader { get { return ServiceLocator.GetService<IUIService>(); } }

		protected virtual void Awake()
		{
			DontDestroyOnLoad(this.gameObject);
			ServiceLocator.SetUp(this)
				.DoOnError(e => Debug.LogError("Catastrophic error, services couldn't be created. " + e.Message))
				.Subscribe(OnGameStart);
		}

		protected virtual void OnGameStart(ServiceLocator locator) {}
	}
}