using System.Collections.Generic;
using Core.Common.Extensions.String;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;
using Logger = UnityLogger.Logger;

namespace Core.Systems
{
    /// <summary>
    /// Service plays audio from a central place.
    /// </summary>
    public class AudioCoreSystem : CoreSystem
    {
        [Inject]
        private FactoryCoreSystem _factory;

        [InjectOptional]
        private PersistentDataSystem _persistentData;

        [SerializeField]
        private AudioSource AudioSourcePrefab;

        [SerializeField]
        private float CrossfadeWait = 1.5f;

        [SerializeField]
        private int PoolAmount = 10;

        private ComponentPool<AudioSource> _pooler;
        private List<AudioPlayer> _activeAudioPlayers;
        private List<AudioPlayer> _activeBackgroundMusicPlayers;

        //Global _mute
        private bool _mute;

        public bool Mute
        {
            get => _mute;
            set
            {
                _mute = value;
                foreach (var ap in _activeBackgroundMusicPlayers) ap.Player.mute = _mute;
                foreach (var ap in _activeAudioPlayers) ap.Player.mute = _mute;
            }
        }

        //Global _volume
        private float _musicVolume;

        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = value;
                foreach (var ap in _activeBackgroundMusicPlayers) ap.Player.volume = _musicVolume;
            }
        }

        private float _fxVolume;

        public float FxVolume
        {
            get => _fxVolume;
            set
            {
                _fxVolume = value;
                foreach (var ap in _activeAudioPlayers) ap.Player.volume = _fxVolume;
            }
        }

        public override async void Initialize()
        {
            base.Initialize();

            _activeAudioPlayers = new List<AudioPlayer>();
            _activeBackgroundMusicPlayers = new List<AudioPlayer>();

            if (AudioSourcePrefab)
                _pooler = _factory.CreateComponentPool<AudioSource>(AudioSourcePrefab, PoolAmount, transform);
            else
                Logger.LogError("AudioService : PlayClip - Failed to create pool. Configuration is missing the AudioSource prefab.");

            if (_persistentData != null)
            {
                var prefs = await GetPreferences();
                if (prefs.Equals(default(UserPreferences)))
                {
                    Logger.Log("AudioService: No UserPreferences set. Creating default.", Colors.Magenta);
                    SaveInitialPreferences();
                }
                else
                {
                    _musicVolume = prefs.MusicVolume;
                    _fxVolume = prefs.FxVolume;
                }
            }
        }

        public void PlayClip(AudioPlayer ap)
        {
            Play(ap);
            WaitUntilDonePlaying(ap);
        }

        public void PlayClip(AudioClip clip)
        {
            var ap = new AudioPlayer(clip);
            PlayClip(ap);
        }

        public void PlayClip(AudioClip clip, AudioMixerGroup mixerGroup)
        {
            var ap = new AudioPlayer(clip);

            Play(ap, mixerGroup);
            WaitUntilDonePlaying(ap);
        }

        public void PlayMusic(AudioClip clip, AudioMixerGroup mixerGroup = null)
        {
            FadeMusicIn(clip, mixerGroup);
        }

        private void Play(AudioPlayer ap, AudioMixerGroup mixerGroup = null)
        {
            if (_pooler != null && (!ap.Player || !ap.Player.gameObject.activeSelf))
            {
                _activeAudioPlayers.Add(ap);

                if (ap.PlayFrom)
                {
                    ap.Player.transform.SetParent(ap.PlayFrom);
                    ap.Player.transform.localPosition = Vector3.zero;
                }

                Logger.Log("AudioService: Playing Clip - " + ap.Clip.name, Colors.Magenta);
                var audioSource = _pooler.PopResize();
                ap.Player = _pooler.PopResize();
                ap.Player.volume = _fxVolume;
                ap.Player.mute = _mute;
                ap.Player.outputAudioMixerGroup = mixerGroup;
                ap.Player.Play();
            }
        }

        public void StopMusic()
        {
            FadeMusicOut();
        }

        private void PushAudioSource(AudioPlayer ap)
        {
            if (ap.PlayFrom)
            {
                ap.Player.transform.SetParent(_pooler.PoolerTransform);
                ap.Player.transform.localPosition = Vector3.zero;
            }

            _activeAudioPlayers.Remove(ap);
            _pooler.Push(ap.Player);
            ap.Player.clip = null;
            ap.Player = null;
        }

        private async void FadeMusicIn(AudioClip clip, AudioMixerGroup mixerGroup = null)
        {
            await FadeMusicInAsync(clip, mixerGroup);
        }

        private async UniTask FadeMusicInAsync(AudioClip clip, AudioMixerGroup mixerGroup = null)
        {
            var volume = 0f;
            var audioPlayer = new AudioPlayer(clip) {Player = _pooler.PopResize()};

            audioPlayer.Player.outputAudioMixerGroup = mixerGroup;

            await FadeMusicOutAsync();

            //wait a tiny bit
            await UniTask.Yield();

            audioPlayer.Player.clip = clip;
            audioPlayer.Player.loop = true;
            audioPlayer.Player.volume = 0;
            audioPlayer.Player.mute = _mute;

            _activeBackgroundMusicPlayers.Add(audioPlayer);
            audioPlayer.Player.Play();
            while (volume <= _musicVolume)
            {
                volume += _musicVolume * Time.deltaTime / CrossfadeWait;
                audioPlayer.Player.volume = volume;
                await UniTask.Yield();
            }
        }

        private async void FadeMusicOut()
        {
            await FadeMusicOutAsync();
        }

        private async UniTask FadeMusicOutAsync()
        {
            var volume = _musicVolume;

            //flush all background music out
            var toFade = new List<AudioPlayer>();
            toFade.AddRange(_activeBackgroundMusicPlayers);
            _activeBackgroundMusicPlayers.Clear();

            foreach (var aPlayer in toFade)
            {
                while (volume > 0)
                {
                    volume -= _musicVolume * Time.deltaTime / CrossfadeWait;
                    aPlayer.Player.volume = volume;
                    await UniTask.Yield();
                }

                aPlayer.Player.Stop();
                _pooler.Push(aPlayer.Player);
                aPlayer.Player.clip = null;
                aPlayer.Player = null;
            }
        }

        private async void WaitUntilDonePlaying(AudioPlayer ap)
        {
            await WaitUntilDonePlayingAsync(ap);
        }

        private async UniTask WaitUntilDonePlayingAsync(AudioPlayer ap)
        {
            CustomYieldInstruction wait = new WaitUntil(() => ap.Player.clip.loadState == AudioDataLoadState.Loaded);
            await wait;

            wait = new WaitWhile(() => ap.Player.isPlaying);
            await wait;

            if (ap.Clip)
                Logger.Log("AudioService: Done Playing Clip - " + ap.Clip.name, Colors.Magenta);

            PushAudioSource(ap);
        }

        private async UniTask<UserPreferences> GetPreferences()
        {
            return await _persistentData.Load<UserPreferences>();
        }

        private void SaveInitialPreferences()
        {
            var pref = new UserPreferences
            {
                MusicVolume = 1,
                FxVolume = 1,
                UseGameCenter = false
            };

            _persistentData.Save(pref);
        }
    }
}