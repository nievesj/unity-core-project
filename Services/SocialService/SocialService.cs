using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Core.Services.Social
{
    /// <summary>
    /// Wrapper service for the UnityEngine.Social class. Most methods are wrapped with Observables.
    /// </summary>
    public class SocialService : Service
    {
        private readonly SocialServiceConfiguration _configuration;

        public bool IsAuthenticated => UnityEngine.Social.localUser.authenticated;
        public string SocialUserName => UnityEngine.Social.localUser.userName;
        public bool IsUserUnderAge => UnityEngine.Social.localUser.underage;
        public UserState UserOnlineState => UnityEngine.Social.localUser.state;
        public Texture2D UserImage => UnityEngine.Social.localUser.image;

        public SocialService(ServiceConfiguration config)
        {
            _configuration = config as SocialServiceConfiguration;
        }

        /// <summary>
        /// Authenticate user. Opens System Authentication UI.
        /// </summary>
        /// <returns></returns>
        public IObservable<bool> Authenticate()
        {
            return Observable.Create<bool>(
                observer =>
                {
                    Action<bool> onAuthenticate = isAuthenticated =>
                    {
                        observer.OnNext(isAuthenticated);
                        observer.OnCompleted();
                    };
                    UnityEngine.Social.localUser.Authenticate(onAuthenticate);

                    return Disposable.Empty;
                });
        }

        /// <summary>
        /// Shows system achievement UI
        /// </summary>
        public void ShowAchievementsUI()
        {
            UnityEngine.Social.ShowAchievementsUI();
        }

        /// <summary>
        /// Shows system leaderboard UI
        /// </summary>
        public void ShowLeaderboardUI()
        {
            UnityEngine.Social.ShowLeaderboardUI();
        }

        /// <summary>
        /// Get local user friends
        /// </summary>
        /// <returns></returns>
        public List<IUserProfile> GetFriends()
        {
            return UnityEngine.Social.localUser.friends.ToList();
        }

        /// <summary>
        /// Get list of achievements
        /// </summary>
        /// <returns></returns>
        public IObservable<List<IAchievement>> GetAchievements()
        {
            return Observable.Create<List<IAchievement>>(
                observer =>
                {
                    Action<IAchievement[]> onLoadedAchievements = achievements =>
                    {
                        observer.OnNext(achievements.ToList());
                        observer.OnCompleted();
                    };
                    UnityEngine.Social.LoadAchievements(onLoadedAchievements);

                    return Disposable.Empty;
                });
        }

        /// <summary>
        /// Get achievements descriptions
        /// </summary>
        /// <returns></returns>
        public IObservable<List<IAchievementDescription>> GetAchievementDescriptions()
        {
            return Observable.Create<List<IAchievementDescription>>(
                observer =>
                {
                    Action<IAchievementDescription[]> onLoadedAchievements = achievements =>
                    {
                        observer.OnNext(achievements.ToList());
                        observer.OnCompleted();
                    };
                    UnityEngine.Social.LoadAchievementDescriptions(onLoadedAchievements);

                    return Disposable.Empty;
                });
        }

        /// <summary>
        /// Load leaderboard scores into a list
        /// </summary>
        /// <returns></returns>
        public IObservable<List<IScore>> LoadScores()
        {
            return Observable.Create<List<IScore>>(
                observer =>
                {
                    Action<IScore[]> onLoadedAchievements = achievements =>
                    {
                        observer.OnNext(achievements.ToList());
                        observer.OnCompleted();
                    };
                    UnityEngine.Social.LoadScores(_configuration.LeaderboardID, onLoadedAchievements);

                    return Disposable.Empty;
                });
        }

        /// <summary>
        /// Load users into a list
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public IObservable<List<IUserProfile>> LoadUsers(List<string> userIds)
        {
            return Observable.Create<List<IUserProfile>>(
                observer =>
                {
                    Action<IUserProfile[]> onLoadedAchievements = achievements =>
                    {
                        observer.OnNext(achievements.ToList());
                        observer.OnCompleted();
                    };
                    UnityEngine.Social.LoadUsers(userIds.ToArray(), onLoadedAchievements);

                    return Disposable.Empty;
                });
        }

        /// <summary>
        /// Unlock an achievement
        /// </summary>
        /// <param name="achievementId"></param>
        /// <param name="achievementProgress"></param>
        /// <returns></returns>
        public IObservable<bool> UnlockAchievement(string achievementId, double achievementProgress)
        {
            return Observable.Create<bool>(
                observer =>
                {
                    Action<bool> onProgressReported = success =>
                    {
                        observer.OnNext(success);
                        observer.OnCompleted();
                    };
                    UnityEngine.Social.ReportProgress(achievementId, achievementProgress, onProgressReported);

                    return Disposable.Empty;
                });
        }
        
        /// <summary>
        /// Report score
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public IObservable<bool> ReportScore(long score)
        {
            return Observable.Create<bool>(
                observer =>
                {
                    Action<bool> onScoreReported = success =>
                    {
                        observer.OnNext(success);
                        observer.OnCompleted();
                    };
                    UnityEngine.Social.ReportScore(score, _configuration.LeaderboardID, onScoreReported);
                    
                    return Disposable.Empty;
                });
        }
    }
}