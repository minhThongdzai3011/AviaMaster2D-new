using UnityEngine;

namespace GMSoft.Advertisement
{
    public class WeeGooAdModule : AdvertisementModuleBase
    {
        public override void Initialize()
        {
            base.Initialize();
            WeeGooAdManager.Instance.OnReady.AddListener(OnReadyRewardAd);
            WeeGooAdManager.Instance.OnSuccess.AddListener(OnRewardedSuccess);
            WeeGooAdManager.Instance.OnPause.AddListener(OnPause);
            WeeGooAdManager.Instance.OnResume.AddListener(OnResume);
            WeeGooAdManager.Instance.OnReady.AddListener(() =>
            {
                OnPreladRewardedAd(1);
            });
        }

        public override void PreloadRewardedAd()
        {
            WeeGooAdManager.Instance.FetchNewReward();
        }

        public override void ShowAd()
        {
            WeeGooAdManager.Instance.GetAd();
        }

        public override void ShowRewardedAd()
        {
            WeeGooAdManager.Instance.ShowRewardAd();
        }

        public override void CreateAdManager()
        {
            GameObject wgPrefab = Resources.Load<GameObject>("WeeGooAdManager");
            GameObject wg = GameObject.Instantiate(wgPrefab);
            wg.name = "WeeGooAdManager(Clone)";
        }
    }
}