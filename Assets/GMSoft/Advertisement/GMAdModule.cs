using AZAdvertisement;
using UnityEngine;

namespace GMSoft.Advertisement
{
    public class GMAdModule : AdvertisementModuleBase
    {
        public override void Initialize()
        {
            base.Initialize();
            GMSDKAdvertisement.OnPauseGame += OnPause;
            GMSDKAdvertisement.OnResumeGame += OnResume;
            GMSDKAdvertisement.OnRewardGame += OnReward;
            GMSDKAdvertisement.OnRewardedVideoSuccess += OnRewardedSuccess;
            GMSDKAdvertisement.OnRewardedVideoFailure += OnRewardedFailure;
        }

        public override void PreloadRewardedAd()
        {
            preladRewardedVideo = true;
        }

        public override void ShowAd()
        {
            GMSDKAdvertisement.Instance.ShowAd();
        }

        public override void ShowRewardedAd()
        {
            GMSDKAdvertisement.Instance.ShowRewardedAd();
        }

        public override void CreateAdManager()
        {
            GameObject gmAdPrefab = Resources.Load<GameObject>("GMSDKAdvertisement");
            GameObject gmAd = GameObject.Instantiate(gmAdPrefab);
            gmAd.name = "GMSDKAdvertisement";
        }
    }
}