var functions = {
    H5ADS_Init: function() {
        // var script = document.createElement('script');
        // script.setAttribute('crossorigin', 'anonymous');
        // // Comment out to disable test mode
        // script.setAttribute('data-adbreak-test', 'on');
        // // Ad frequency is user configuble
        // // https://developers.google.com/ad-placement/docs/ad-rate
        // script.setAttribute('data-ad-frequency-hint', '30s');
        // // Comment in if you would like to use a specific channel for ad reporting
        // // https://developers.google.com/ad-placement/docs/advanced-reporting
        // script.setAttribute('data-ad-channel', "demo-h5");
        // script.src = 'https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js?client=pub-3940256099942544';
        // document.head.appendChild(script);
        // this._showRewardAdFn = null;
        // window.adsbygoogle = window.adsbygoogle || [];
        // var afg = {};
        // afg.adBreak = window.adConfig = function(o) {
        //     adsbygoogle.push(o);
        // };
        // afg.ready = false;

        // var onReady = function() {
        //     afg.ready = true;
        // };
        // // Config the ad setup
        // adConfig({
        //     // https://developers.google.com/ad-placement/docs/preload-ads
        //     preloadAdBreaks: 'on',
        //     onReady: onReady,
        // });
        // var beforeAdCalled = false;
        // afg.onBeforeAd = function() {
        //     beforeAdCalled = true;
        // };
        // afg.onAfterAd = function() {
        //     beforeAdCalled = false;
        // };
        // window.afg = afg;
    },

    H5_PreloadAd: function() {
        console.log("afg.ready: " + afg.ready);
        if (afg.ready) {
            SendMessage("H5Adverisement", "PreloadRewardedVideoCallback", 1);
            afg.adBreak({
                type: 'reward',
                name: 'reward ads',
                beforeReward: function(showAdFn) {
                    this._showRewardAdFn = showAdFn;
                    console.log("[H5 Ads] before reward");
                }.bind(this),
                adViewed: function() {
                    this._showRewardAdFn = null;
                    SendMessage("H5Adverisement", "RewardedVideoSuccessCallback");
                    console.log("[H5 Ads] ad viewed");
                }.bind(this),
                adDismissed: function() {
                    this._showRewardAdFn = null;
                    SendMessage("H5Adverisement", "RewardedVideoFailureCallback");
                    console.log("[H5 Ads] ad failure");
                }.bind(this),
                adBreakDone: function(placementInfo) {
                    console.log("[H5 Ads] Reward break done");
                    SendMessage("H5Adverisement", "ResumeGameCallback");
                }.bind(this)
            });
        } else {
            console.log("[H5 Ads] no reward ads");
            SendMessage("H5Adverisement", "PreloadRewardedVideoCallback", 0);
        }
    },

    H5_ShowAd: function() {
        if (afg.ready) {
            afg.adBreak({
                type: "next",
                name: "next",
                beforeAd: function() {
                    afg.onBeforeAd();
                    console.log("[H5 Ads] Before ad");
                    SendMessage("H5Adverisement", "PauseGameCallback");
                }.bind(this),
                adBreakDone: function() {
                    console.log("[H5 Ads] Ad break done");
                    SendMessage("H5Adverisement", "ResumeGameCallback");
                }.bind(this)
            });
        } else {
            console.log("no " + adType + " ads");
            SendMessage("H5Adverisement", "ResumeGameCallback");
        }
    },

    H5_ShowRewardAd: function() {
        if (this._showRewardAdFn) {
            SendMessage("H5Adverisement", "PauseGameCallback");
            this._showRewardAdFn();
        } else {
            SendMessage("H5Adverisement", "ResumeGameCallback");
        }
    }
}
mergeInto(LibraryManager.library, functions);