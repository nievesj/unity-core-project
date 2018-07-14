using Core.Services.Audio;
using UnityEngine;
using UnityEngine.Audio;
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
        protected AudioClip _backgroundMusic;

        [SerializeField]
        protected AudioMixerGroup _audioMixer;

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

            if (_audioService != null && _backgroundMusic != null && _backgroundMusic != null)
                _audioService.PlayMusic(_backgroundMusic, _audioMixer);
        }
    }
}