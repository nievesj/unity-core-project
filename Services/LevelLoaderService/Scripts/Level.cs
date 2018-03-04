using Core.Services;
using Core.Services.Audio;
using Core.Services.UI;
using System.Collections;
using System.Collections.Generic;
using Core.Services.Assets;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Services.Levels
{
	public enum LevelState
	{
		Loaded,
		Started,
		InProgress,
		Completed
	}

	/// <summary>
	/// A level has the same purpose of a scene, but we can change them without having to load a
	/// scene. This works well on most plaforms except for WebGL where loading scenes also clears the memory.
	/// </summary>
	public abstract class Level : CoreBehaviour
	{
		public AudioPlayer backgroundMusic;

		[Inject]
		protected LevelLoaderService _levelService;

		[Inject]
		protected AudioService _audioService;

		[Inject]
		protected UIService uiService;

		[Inject]
		protected AssetService _assetService;

		protected LevelState _levelState;

		public LevelState State { get { return _levelState; } }

		protected override void Awake()
		{
			base.Awake();
			Debug.Log(("Level: " + name + " loaded").Colored(Colors.LightBlue));

			_levelState = LevelState.Loaded;
		}

		protected override void Start()
		{
			base.Start();

			Debug.Log(("Level: " + name + " started").Colored(Colors.LightBlue));

			_levelState = LevelState.Started;
			_levelState = LevelState.InProgress;

			if (_audioService != null && backgroundMusic != null && backgroundMusic.Clip != null)
				_audioService.PlayMusic(backgroundMusic);
		}

		public virtual void Unload()
		{
			if (_audioService != null && backgroundMusic != null && backgroundMusic.Clip != null)
				_audioService.StopClip(backgroundMusic);
		}

		protected override void OnDestroy()
		{
			if (_audioService != null && backgroundMusic != null && backgroundMusic.Clip != null)
				_audioService.StopClip(backgroundMusic);
		}
	}
}