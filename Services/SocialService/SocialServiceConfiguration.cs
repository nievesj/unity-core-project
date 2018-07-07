using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.Social
{
    public class SocialServiceConfiguration : ServiceConfiguration
    {
        [SerializeField]
        private string _LeaderboardID = "com.domain.app.leaderboardid"; //Same as Leaderboard ID you setup in iTunes Connect.
        
        [SerializeField]
        private List<string> _achievementIDs;

        public string LeaderboardID => _LeaderboardID;
        public List<string> AchievementIDs => _achievementIDs; //TODO improve this with scriptable objects...
        public override Service ServiceClass => new SocialService(this);
    }
}