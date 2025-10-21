using CrazyGames;

namespace GMSoft.Advertisement
{
    public class CrazyGameAdModule : AdvertisementModuleBase
    {
        public bool checkAdblock = true;

        public override void Initialize()
        {
        }

        public override void PreloadRewardedAd()
        {
            preladRewardedVideo = true;
        }

        public override void ShowAd()
        {
            if (checkAdblock)
            {
                CrazySDK.Ad.HasAdblock((hasAdblock) =>
                {
                    if (!hasAdblock)
                    {
                        CrazySDK.Ad.RequestAd(CrazyGames.CrazyAdType.Midgame, OnPause, null, OnResume);
                    }
                    else
                    {
                        OnResume();
                    }
                });
            }
            else
            {
                CrazySDK.Ad.RequestAd(CrazyGames.CrazyAdType.Midgame, OnPause, null, OnResume);
            }
        }

        public override void ShowRewardedAd()
        {
            if (checkAdblock)
            {
                CrazySDK.Ad.HasAdblock((hasAdblock) =>
                {
                    if (!hasAdblock)
                    {
                        CrazySDK.Ad.RequestAd(CrazyAdType.Rewarded, OnPause, null, OnRewardedSuccess);
                    }
                    else
                    {
                        OnResume();
                    }
                });
            }
            else
            {
                CrazySDK.Ad.RequestAd(CrazyAdType.Rewarded, OnPause, null, OnRewardedSuccess);
            }
        }

        private void OnRewardSuccess()
        {
            OnResumeGame?.Invoke();
            OnRewardedVideoSuccess?.Invoke();
        }
    }
}
