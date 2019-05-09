using Core.Factory;
using UnityEngine;

namespace Core.Services.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class CoreAudioSource : CoreBehaviourPooled
    {
        public AudioSource AudioSource { get; private set; }

        private void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
        }

        public override void PoolElementWakeUp()
        {
            throw new System.NotImplementedException();
        }

        public override void PoolElementSleep()
        {
            throw new System.NotImplementedException();
        }
    }
}