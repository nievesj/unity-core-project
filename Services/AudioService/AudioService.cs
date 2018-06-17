using System.Collections;
using System.Collections.Generic;
using Core.Services.Factory;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Core.Services.Audio
{
    public class AudioService : Service
    {
        [Inject]
        private FactoryService _factoryService;

        private Pooler<AudioSource> _pooler;
        private AudioServiceConfiguration _configuration;
        private List<AudioPlayer> _activeAudioPlayers;
        private List<AudioPlayer> _activeBackgroundMusicPlayers;

        //Global _mute
        private bool _mute;

        public bool Mute
        {
            get { return _mute; }
            set
            {
                _mute = value;
                foreach (var ap in _activeBackgroundMusicPlayers) ap.Player.mute = _mute;
                foreach (var ap in _activeAudioPlayers) ap.Player.mute = _mute;
            }
        }

        //Global _volume
        private float _volume;

        public float Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                foreach (var ap in _activeBackgroundMusicPlayers) ap.Player.volume = _volume;
                foreach (var ap in _activeAudioPlayers) ap.Player.volume = _volume;
            }
        }

        public AudioService(ServiceConfiguration config)
        {
            _configuration = config as AudioServiceConfiguration;
            _activeAudioPlayers = new List<AudioPlayer>();
            _activeBackgroundMusicPlayers = new List<AudioPlayer>();
        }

        public override void Initialize()
        {
            base.Initialize();

            if (_configuration.audioSourcePrefab)
            {
                _pooler = _factoryService.CreatePool<AudioSource>(_configuration.audioSourcePrefab, _configuration.poolAmount);
            }
            else
                Debug.LogError("AudioService : PlayClip - Failed to create pool. Configuration is missing the AudioSource prefab.");
        }

        public void PlayClip(AudioPlayer ap)
        {
            Play(ap);

            MainThreadDispatcher.StartCoroutine(WaitUntilDonePlaying(ap));
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
            MainThreadDispatcher.StartCoroutine(WaitUntilDonePlaying(ap));
        }

        public void PlayMusic(AudioPlayer ap)
        {
            _activeAudioPlayers.Add(ap);
            Play(ap);
        }

        public void PlayMusic(AudioClip clip, AudioMixerGroup mixerGroup)
        {
            MainThreadDispatcher.StartCoroutine(FadeMusicIn(clip, mixerGroup));
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

                Debug.Log(("AudioService: Playing Clip - " + ap.Clip.name).Colored(Colors.Magenta));
                ap.Player = _pooler.PopResize();

                ap.Player.volume = _volume;
                ap.Player.mute = _mute;

                if (mixerGroup)
                    ap.Player.outputAudioMixerGroup = mixerGroup;

                ap.Player.Play();
            }
        }

        public void StopClip(AudioPlayer ap)
        {
            if (_pooler != null && ap.Player)
            {
                Debug.Log(("AudioService: Stopping Clip - " + ap.Clip.name).Colored(Colors.Magenta));

                ap.Player.Stop();
                PushAudioSource(ap);
            }
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

        //TODO: stop using IEnumerator, replace with Async/Task
        private IEnumerator FadeMusicIn(AudioClip clip, AudioMixerGroup mixerGroup = null)
        {
            var volume = 0f;
            var audioPlayer = new AudioPlayer(clip) {Player = _pooler.PopResize()};

            if (mixerGroup)
                audioPlayer.Player.outputAudioMixerGroup = mixerGroup;

            MainThreadDispatcher.StartCoroutine(FadeMusicOut());
            //wait a tiny bit
            yield return new WaitForEndOfFrame();

            audioPlayer.Player.clip = clip;
            audioPlayer.Player.loop = true;
            audioPlayer.Player.volume = 0;
            audioPlayer.Player.mute = _mute;

            _activeBackgroundMusicPlayers.Add(audioPlayer);
            audioPlayer.Player.Play();
            while (volume <= _volume)
            {
                volume += _volume * Time.deltaTime / _configuration.crossfadeWait;
                audioPlayer.Player.volume = volume;
                yield return null;
            }
        }

        //TODO: stop using IEnumerator, replace with Async/Task
        private IEnumerator FadeMusicOut()
        {
            var volume = _volume;

            //flush all background music out
            var toFade = new List<AudioPlayer>();
            toFade.AddRange(_activeBackgroundMusicPlayers);
            _activeBackgroundMusicPlayers.Clear();

            foreach (var aPlayer in toFade)
            {
                while (volume > 0)
                {
                    volume -= _volume * Time.deltaTime / _configuration.crossfadeWait;
                    aPlayer.Player.volume = volume;
                    yield return null;
                }

                aPlayer.Player.Stop();
                _pooler.Push(aPlayer.Player);
                aPlayer.Player.clip = null;
                aPlayer.Player = null;
            }
        }

        //TODO: stop using IEnumerator, replace with Async/Task
        private IEnumerator WaitUntilDonePlaying(AudioPlayer ap)
        {
            CustomYieldInstruction wait = new WaitUntil(() => ap.Player.clip.loadState == AudioDataLoadState.Loaded);
            yield return wait;

            wait = new WaitWhile(() => ap.Player.isPlaying);
            yield return wait;

            if (ap.Clip)
                Debug.Log(("AudioService: Done Playing Clip - " + ap.Clip.name).Colored(Colors.Magenta));

            PushAudioSource(ap);
        }
    }
}