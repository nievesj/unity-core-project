using UnityEngine;

namespace Core.Services.Audio
{
    public class AudioServiceConfiguration : ServiceConfiguration
    {
        public override Service ServiceClass => new AudioService(this);

        public AudioSource AudioSourcePrefab;
        public float CrossfadeWait = 1.5f;
        public int PoolAmount = 10;
    }
}