using System;
using System.Collections.Generic;
using Core.Services.Assets;
using Core.Services.Audio;
using Core.Services.Factory;
using Core.Services.Levels;
using Core.Services.Scenes;
using Core.Services.UI;
using Core.Services.UpdateManager;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Services
{
	public class CoreFrameworkInstaller : MonoInstaller<CoreFrameworkInstaller>
	{
		[Inject]
		private LevelLoaderService _levelLoaderService;

		[Inject]
		private SceneLoaderService _sceneLoaderService;

		[Inject]
		private UIService _uiService;

		[Inject]
		private AudioService _audioService;

		[Inject]
		private UpdateService _updateService;

		[Inject]
		private FactoryService _factoryService;

		[Inject]
		private AssetService _assetService;

		private Subject<Unit> onGameStart = new Subject<Unit>();

		public override void InstallBindings()
		{
			Container.BindInstance(onGameStart).AsSingle();
		}

		protected void Awake()
		{
			_levelLoaderService.SetUp(Container);
			_sceneLoaderService.SetUp(Container);
			_uiService.SetUp(Container);
			_audioService.SetUp(Container);
			_updateService.SetUp(Container);
			_factoryService.SetUp(Container);
			_assetService.SetUp(Container);
		}

		public override void Start()
		{
			onGameStart.OnNext(new Unit());
			onGameStart.OnCompleted();
		}
	}
}