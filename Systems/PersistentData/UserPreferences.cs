using System;

namespace Core.Systems
{
    [Serializable]
    public struct UserPreferences : IStorable
    {
        public string FileName => nameof(UserPreferences);
        public float MusicVolume { get; set; }
        public float FxVolume { get; set; }
        public bool UseGameCenter { get; set; }
    }
}