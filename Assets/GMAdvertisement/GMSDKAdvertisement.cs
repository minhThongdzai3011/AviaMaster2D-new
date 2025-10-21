using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AZAdvertisement
{
    public class GMSDKAdvertisement : MonoBehaviour
    {
        public enum GMAdType
        {
            Midgame,
            Rewarded,
        }

        public static GMSDKAdvertisement Instance;

        public static Action OnResumeGame;
        public static Action OnPauseGame;
        public static Action OnRewardGame;
        public static Action OnRewardedVideoSuccess;
        public static Action OnRewardedVideoFailure;

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void GMRequestAd(string adType);
#else
        private void GMRequestAd(string adType) { }
#endif

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Initialize()
        {
        }

        public void ShowAd()
        {
            GMRequestAd(GMAdType.Midgame.ToString().ToLower());
        }

        public void ShowRewardedAd()
        {
            GMRequestAd(GMAdType.Rewarded.ToString().ToLower());
        }

        private void JSLibCallback_AdStarted()
        {
            Debug.Log("GM Ad started");
            OnPauseGame?.Invoke();
        }

        private void JSLibCallback_AdFinished()
        {
            Debug.Log("GM Ad finished");
            OnResumeGame?.Invoke();
            OnRewardedVideoSuccess?.Invoke();
        }

        private void JSLibCallback_AdError(string errorJson)
        {
            OnResumeGame?.Invoke();
            Debug.LogError("Ad Error: " + errorJson);
        }
    }
}
