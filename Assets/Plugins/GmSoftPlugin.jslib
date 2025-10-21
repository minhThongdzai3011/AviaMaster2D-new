var functions = {
    GM_SDK_GMEvent: function(eventName, msg) {
        gmEvent(UTF8ToString(eventName), UTF8ToString(msg));
    },
    GM_SDK_StartGame: function() {
        console.log("GM_SDK_StartGame");
        try {
            gmStartGame(function() {
                SendMessage("GmSoft", "SetGameInfoParam", JSON.stringify(window["GMSOFT_GAME_INFO"]));
            });
        } catch (err) {
            console.error("Error JS 12", err);
        }

        try {
            gmStartAds(function() {
                SendMessage("GmSoft", "SetAdParam", JSON.stringify(window["GMSOFT_ADS_INFO"]));
            });
        } catch (err) {
            console.error("Error JS 20", err);
        }

        try {
            document.addEventListener("gmsoftbanevent", (e) => {
                SendMessage("GmSoft", "LockGame", e.detail);
            });
        } catch (err) {
            console.error("Error JS 28", err);
        }
    },
    GM_SDK_InitParam: function() {
        let hostname = document.location.hostname;
        SendMessage("GmSoft", "SetUnityHostName", hostname);
        SendMessage("GmSoft", "SetSDKType", window["GMSOFT_SDKTYPE"]);
    },

    GM_SDK_GetVeryfiedSignature: function() {
        const key = "gmsdksigndomain";
        try {
            if (localStorage.hasOwnProperty(key)) {
                let signed_domain_save = localStorage.getItem(key);
                if (signed_domain_save && signed_domain_save.length > 10) {
                    window["GMSOFT_SIGNED"] = signed_domain_save;
                    console.log("Local storage GM_SDK_GetVeryfiedSignature: ", window["GMSOFT_SIGNED"]);
                    SendMessage("GmSoft", "OnGetVeryfiedSignature", window["GMSOFT_SIGNED"]);
                    return;
                }
            }
        } catch (e) {
            console.error("Error getting GM_SDK_GetVeryfiedSignature from local storage: ", e)
        }
        try {
            let raw_params = "hn=" + window.location.hostname;
            let encoded_params = btoa(raw_params);
            let request_signed_url_base = "https://api.cdnwave.com/sdkdom/keysigned";
            if (window["GMSOFT_OPTIONS"] && window["GMSOFT_OPTIONS"].signedurl) {
                request_signed_url_base = window["GMSOFT_OPTIONS"].signedurl;
            }
            let verify_license_url = request_signed_url_base + "?params=" + encoded_params;
            var xmlHttp = new XMLHttpRequest;
            let target = "";
            xmlHttp.open("GET", verify_license_url, false);
            xmlHttp.send(target);
            let data = xmlHttp.responseText; //signed code => json{domain: "xxx", sign: "xxxx"} . nếu thằng hostname end with domain thì ok 
            if (data && atob(data) === "bangame") {
                document.dispatchEvent(new CustomEvent("gmsoftbanevent", {
                    detail: "libevent"
                }));
                return;
            }
            window["GMSOFT_SIGNED"] = data;
            localStorage.setItem(key, data);
            console.log("send request GM_SDK_GetVeryfiedSignature: ", window["GMSOFT_SIGNED"]);
            SendMessage("GmSoft", "OnGetVeryfiedSignature", window["GMSOFT_SIGNED"])
        } catch (e) {
            console.error("Error fetching GM_SDK_GetVeryfiedSignature: ", e)
        }
    }
};

mergeInto(LibraryManager.library, functions);