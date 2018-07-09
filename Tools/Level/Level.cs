using Core.Services.Assets;
using Core.Services.Audio;
using Core.Services.UI;
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
		public LevelState LevelState => _levelState;

		[SerializeField]
		protected AudioPlayer _backgroundMusic;

		protected LevelState _levelState;
		
		[Inject]
		protected AudioService _audioService;

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

			if (_audioService != null && _backgroundMusic != null && _backgroundMusic.Clip != null)
				_audioService.PlayMusic(_backgroundMusic);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			
			if (_audioService != null && _backgroundMusic != null && _backgroundMusic.Clip != null)
				_audioService.StopClip(_backgroundMusic);
		}
	}
}