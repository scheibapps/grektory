(function(){
    document.addEventListener('deviceready', onDeviceReady.bind(this), false);

    function onDeviceReady(){
        initAd();
        if (/(ipod|iphone|ipad)/i.test(navigator.userAgent)) { // for ios
            StatusBar.hide();
        }
        window.location = './login.html';
    }

    function initAd() {
        var admobid = {};
        if (/(android)/i.test(navigator.userAgent)) { // for android & amazon-fireos
            admobid = {
                banner: 'ca-app-pub-9006593988246578/9846507040',
                interstitial: 'ca-app-pub-9006593988246578/3799973441'
            };
        } else if (/(ipod|iphone|ipad)/i.test(navigator.userAgent)) { // for ios
            admobid = {
                banner: 'ca-app-pub-9006593988246578/6753439845',
                interstitial: 'ca-app-pub-9006593988246578/8230173044'
            };
        } else { // for windows phone
            admobid = {
                banner: 'ca-app-pub-9006593988246578/2183639441',
                interstitial: 'ca-app-pub-9006593988246578/3660372645'
            };
        }
        localStorage.bannerId = admobid.banner;
        localStorage.interstitialId = admobid.interstitial;
        if (AdMob) {
            AdMob.prepareInterstitial({ adId: admobid.interstitial, autoShow: false });
        }
        if (!(/(ipod|iphone|ipad)/i.test(navigator.userAgent))) {
            initBanner();
        }
    }

    function initBanner() {
        if (AdMob) {
            AdMob.createBanner({
                adId: localStorage.bannerId,
                position: AdMob.AD_POSITION.BOTTOM_CENTER,
                autoShow: true,
                adSize: 'SMART_BANNER',
            });
        }
    }
})();