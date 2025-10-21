using UnityEngine;

namespace GMSoft.Advertisement
{
    public class GameDistributionAdModule : AdvertisementModuleBase
    {
        public override void Initialize()
        {
            base.Initialize();
            GameDistribution.OnPauseGame += OnPause;
            GameDistribution.OnResumeGame += OnResume;
            GameDistribution.OnRewardGame += OnReward;
            GameDistribution.OnRewardedVideoSuccess += OnRewardedSuccess;
            GameDistribution.OnRewardedVideoFailure += OnRewardedFailure;
            GameDistribution.OnPreloadRewardedVideo += OnPreladRewardedAd;
        }

        public override void PreloadRewardedAd()
        {
            GameDistribution.Instance.PreloadRewardedAd();
        }

        public override void ShowAd()
        {
            GameDistribution.Instance.ShowAd();
        }

        public override void ShowRewardedAd()
        {
            GameDistribution.Instance.ShowRewardedAd();
        }

        public override void CreateAdManager()
        {
            GameObject gdPrefab = Resources.Load<GameObject>("Prefabs/GameDistribution");
            GameObject gd = GameObject.Instantiate(gdPrefab);
            gd.name = "GameDistribution";
        }

        public void SetGameKey(string gameKey)
        {
            if (GameDistribution.Instance != null)
            {
                GameDistribution.Instance.GAME_KEY = gameKey;
            }
            else
            {
                Debug.Log("GameDistribution instance is not initialized.");
            }
        }
    }
}
