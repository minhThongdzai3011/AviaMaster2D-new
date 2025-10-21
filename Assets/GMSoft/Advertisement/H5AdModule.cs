using H5;
using UnityEngine;

namespace GMSoft.Advertisement
{
    public class H5AdModule : AdvertisementModuleBase
    {
        public override void Initialize()
        {           
            base.Initialize();
            H5Adverisement.OnPauseGame += OnPause;
            H5Adverisement.OnResumeGame += OnResume;
            H5Adverisement.OnRewardGame += OnReward;
            H5Adverisement.OnRewardedVideoSuccess += OnRewardedSuccess;
            H5Adverisement.OnRewardedVideoFailure += OnRewardedFailure;
            H5Adverisement.OnPreloadRewardedVideo += OnPreladRewardedAd;
        }

        public override void PreloadRewardedAd()
        {
            H5Adverisement.Instance.PreloadRewardedAd();
        }

        public override void ShowAd()
        {
            H5Adverisement.Instance.ShowAd();
        }

        public override void ShowRewardedAd()
        {
            H5Adverisement.Instance.ShowRewardedAd();
            Debug.Log("H5AdModule: ShowRewardedAd called.");
        }

        override public void CreateAdManager()
        {
            GameObject h5Prefab = Resources.Load<GameObject>("H5Adverisement");
            GameObject h5 = GameObject.Instantiate(h5Prefab);
            h5.name = "H5Adverisement";
        }
    }
}
