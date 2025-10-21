mergeInto(LibraryManager.library, {
    ShowRewardAdCallback: function() {
        window[preroll.config.loaderObjectName].showRewardAd();
    },

    RegisterRewardCallbacks__deps: ['DoRegisterRewardCallbacks'],
    RegisterRewardCallbacks: function() {
        setTimeout(function() {
            _DoRegisterRewardCallbacks();
        }, 250);
    },

    DoRegisterRewardCallbacks__deps: ['RegisterRewardCallbacks'],
    DoRegisterRewardCallbacks: function() {
        try {
            window[preroll.config.loaderObjectName].log("WGSDK: Reward callbacks registered.");
            window[window.preroll.config.loaderObjectName].registerRewardCallbacks({
                onReady: "OnReadyMethod",
                onSuccess: "OnSuccessMethod",
                onFail: "OnFailMethod"
            });
        } catch (e) {
            console.log("WGSDK: Can not register reward ad callbacks.");
            _RegisterRewardCallbacks();
        }
    },

    RegisterGameControls: function() {
        try {
            window[preroll.config.loaderObjectName].log("WGSDK: Game controls registered.");
            window[window.preroll.config.loaderObjectName].registerGameControls(Module);
        } catch (e) {
            console.log("WGSDK: Can not register game controls.");
            setTimeout(function() {
                _RegisterGameControls();
            }, 250);
        }
    },

    FetchAd: function() {
        try {
            window[preroll.config.loaderObjectName].log("WGSDK: Getting WeeGoo InGame Ad");
            window[preroll.config.loaderObjectName].refetchAd();
        } catch (e) {
            console.log("No WeeGooAFG implementation found!");
        }
    },
    GameEvent: function(gameEventName) {
        try {
            window[preroll.config.loaderObjectName].log("WGSDK: Sending game event");
            window[preroll.config.loaderObjectName].GameEvent(gameEventName);
        } catch (e) {
            console.log("No WeeGooAFG implementation found!");
        }
    },
    Ping__deps: ['DoPing'],
    Ping: function(WgObjectName, isString) {
        var objName;
        if (isString === true) {
            objName = WgObjectName;
        } else {
            objName = UTF8ToString(WgObjectName);
        }
        setTimeout(function() {
            _DoPing(objName);
        }, 250);
    },
    DoPing__deps: ['Ping'],
    DoPing: function(WgObjectName) {
        window['wgUnityInstance'] = Module;
        try {
            window[preroll.config.loaderObjectName].log("WGSDK: WeeGoo InGame Ad, pinging.");
            window[preroll.config.loaderObjectName].ping(WgObjectName);
        } catch (e) {
            console.log("WGSDK: No production WGSDK resources found, loading development resources.");
            var wgConf = document.createElement('script');
            wgConf.addEventListener('load', function(ev) {
                var wgLibrary = document.createElement('script');
                wgLibrary.addEventListener('load', function(ev) {
                    console.log("WGSDK: Development resources loaded.");
                    _Ping(WgObjectName, true);
                }.bind(this));
                document.getElementsByTagName('head')[0].appendChild(wgLibrary);
                wgLibrary.src = window["GMSOFT_ADS_INFO"].wgLibrary; //
            }.bind(this));
            document.getElementsByTagName('head')[0].appendChild(wgConf);
            wgConf.src = window["GMSOFT_ADS_INFO"].wgConf;
        }
    },
    RefetchReward: function() {
        try {
            window[preroll.config.loaderObjectName].log("WGSDK: Refetching reward ad.");
            window[preroll.config.loaderObjectName].rewarded.initSlot.call(window[preroll.config.loaderObjectName]);
        } catch (e) {
            console.log("No WeeGooAFG implementation found!");
        }
    },
    Log: function(s) {
        try {
            window[preroll.config.loaderObjectName].log(UTF8ToString(s));
        } catch (e) { console.log("WGSDK: Can't log: " + e); }
    }
});