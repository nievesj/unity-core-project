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
        public LevelState LevelState { get; private set; }

        [SerializeField]
        protected AudioClip BackgroundMusic;

        [SerializeField]
        protected AudioMixerGroup AudioMixer;

        protected override void Awake()
        {
            base.Awake();
            Debug.Log(("Level: " + name + " loaded").Colored(Colors.LightBlue));

            LevelState = LevelState.Loaded;
        }

        protected override void Start()
        {
            base.Start();

            Debug.Log(("Level: " + name + " started").Colored(Colors.LightBlue));

            LevelState = LevelState.Started;
            LevelState = LevelState.InProgress;

            if (AudioService != null && BackgroundMusic != null && BackgroundMusic != null)
                AudioService.PlayMusic(BackgroundMusic, AudioMixer);
        }
    }
}