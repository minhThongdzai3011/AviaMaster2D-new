using System;
using UnityEngine;

namespace GMSoft.Advertisement
{
    public abstract class AdvertisementModuleBase
    {
        public Action OnResumeGame;
        public Action OnPauseGame;
        public Action OnRewardGame;
        public Action OnRewardedVideoSuccess;
        public Action OnRewardedVideoFailure;
        public Action<int> OnPreloadRewardedVideo;

        public bool preladRewardedVideo = false;

        protected void OnReadyRewardAd()
        {
            Debug.Log("[Ad Module] Rewarded video is ready");
            OnPreloadRewardedVideo?.Invoke(1);
        }

        protected void OnRewardedSuccess()
        {
            Debug.Log("[Ad Module] Rewarded video success");
            OnRewardedVideoSuccess?.Invoke();
            OnReward();
        }

        protected void OnRewardedFailure()
        {
            Debug.Log("[Ad Module] Rewarded video failure");
            OnRewardedVideoFailure?.Invoke();
        }

        protected void OnPause()
        {
            Debug.Log("[Ad Module] Game paused due to ad");
            OnPauseGame?.Invoke();
        }

        protected void OnResume()
        {
            Debug.Log("[Ad Module] Game resumed after ad");
            OnResumeGame?.Invoke();
        }

        protected void OnReward()
        {
            Debug.Log("[Ad Module] Player rewarded for watching ad");
            OnRewardGame?.Invoke();
        }

        protected void OnPreladRewardedAd(int value)
        {
            OnPreloadRewardedVideo?.Invoke(value);
            preladRewardedVideo = value == 1;
        }

        public virtual void Initialize() 
        {
            CreateAdManager();
        }

        public virtual void ShowAd() { }
        public virtual void ShowRewardedAd() { }
        public virtual void PreloadRewardedAd() { }
        public virtual void CreateAdManager() { }
    }
}
