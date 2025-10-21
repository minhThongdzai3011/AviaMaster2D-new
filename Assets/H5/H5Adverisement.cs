using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace H5
{
    public class H5Adverisement : MonoBehaviour
    {
        public static H5Adverisement Instance;

        public static Action OnResumeGame;
        public static Action OnPauseGame;
        public static Action OnRewardGame;
        public static Action OnRewardedVideoSuccess;
        public static Action OnRewardedVideoFailure;
        public static Action<int> OnPreloadRewardedVideo;

        [DllImport("__Internal")]
        private static extern void H5ADS_Init();

        [DllImport("__Internal")]
        private static extern void H5_PreloadAd();

        [DllImport("__Internal")]
        private static extern void H5_ShowAd();

        [DllImport("__Internal")]
        private static extern void H5_ShowRewardAd();

        private bool _isRewardedVideoLoaded = false;

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

        void Init()
        {
            try
            {
                H5ADS_Init();
            }
            catch (EntryPointNotFoundException e)
            {
                Debug.LogWarning("H5 initialization failed. Make sure you are running a WebGL build in a browser:" + e.Message);
            }
        }

        internal void ShowAd()
        {
            try
            {
                H5_ShowAd();
            }
            catch (EntryPointNotFoundException e)
            {
                Debug.LogWarning("H5 ShowAd failed. Make sure you are running a WebGL build in a browser:" + e.Message);
            }
        }

        internal void ShowRewardedAd()
        {
            try
            {
                H5_ShowRewardAd();
            }
            catch (EntryPointNotFoundException e)
            {
                Debug.LogWarning("H5 ShowAd failed. Make sure you are running a WebGL build in a browser:" + e.Message);
            }
        }

        internal void PreloadRewardedAd()
        {
            try
            {
                Debug.Log("H5 PreloadAd called");
                H5_PreloadAd();
            }
            catch (EntryPointNotFoundException e)
            {
                Debug.LogWarning("H5 Preload failed. Make sure you are running a WebGL build in a browser:" + e.Message);
            }
        }

        /// <summary>
        /// It is being called by HTML5 SDK when the game should start.
        /// </summary>
        void ResumeGameCallback()
        {
            if (OnResumeGame != null) OnResumeGame();
        }

        /// <summary>
        /// It is being called by HTML5 SDK when the game should pause.
        /// </summary>
        void PauseGameCallback()
        {
            if (OnPauseGame != null) OnPauseGame();
        }

        /// <summary>
        /// It is being called by HTML5 SDK when the game should should give reward.
        /// </summary>
        void RewardedCompleteCallback()
        {
            if (OnRewardGame != null) OnRewardGame();
        }

        /// <summary>
        /// It is being called by HTML5 SDK when the rewarded video succeeded.
        /// </summary>
        void RewardedVideoSuccessCallback()
        {
            _isRewardedVideoLoaded = false;

            if (OnRewardedVideoSuccess != null) OnRewardedVideoSuccess();
        }

        /// <summary>
        /// It is being called by HTML5 SDK when the rewarded video failed.
        /// </summary>
        void RewardedVideoFailureCallback()
        {
            _isRewardedVideoLoaded = false;

            if (OnRewardedVideoFailure != null) OnRewardedVideoFailure();
        }

        /// <summary>
        /// It is being called by HTML5 SDK when it preloaded rewarded video
        /// </summary>
        void PreloadRewardedVideoCallback(int loaded)
        {
            _isRewardedVideoLoaded = (loaded == 1);

            if (OnPreloadRewardedVideo != null) OnPreloadRewardedVideo(loaded);
        }
    }
}
