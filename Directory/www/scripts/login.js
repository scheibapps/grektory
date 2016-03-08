(function () {
    "use strict";
    var main = document.getElementById('main');
    var page = document.getElementById('page');
    var header = document.getElementById('header');
    var key = document.getElementById('key');
    var login = document.getElementById('login');
    var url = document.getElementById('url');
    var load = document.getElementById('load');
    document.addEventListener('deviceready', onDeviceReady.bind(this), false);

    function onDeviceReady() {
        //load.style.visibility = "hidden";
        if (localStorage.keyVal != null) {
            key.value = localStorage.keyVal;
        }
        Parse.initialize('hehueu8y283yu3hlj14k3h4j1');
        Parse.serverURL = 'https://grektory.herokuapp.com/parse';
        document.addEventListener('pause', onPause.bind(this), false);
        document.addEventListener('resume', onResume.bind(this), false);
        $('#url').click(loadUrl);
        $('#login').click(queryDBS);
    };

    function queryDBS() {
        load.style.visibility = 'visible'
        var query = new Parse.Query(Parse.Object.extend("DBS"));
        query.equalTo("public", key.value);
        query.find({
            success: function (dbs) {
                if (dbs[0]) {
                    load.style.visible = 'hidden';
                    console.log(load.style.display);
                    localStorage.keyVal = key.value;
                    localStorage.dbname = dbs[0].get("name");
                    localStorage.dbval = dbs[0].get("val");
                    //AdMob.showInterstitial();
                    window.location = './main.html';
                } else {
                    load.style.visibility = 'hidden';
                    alert("No such directory with public key: " + key.value);
                }
            },
            error: function (err) {
                load.style.visibility = 'hidden';
                alert("An error has occurred. Please check your network or try again later.");
            }
        })
    }

    function loadUrl() {
        window.open('https://scheibapps.herokuapp.com/Grektory/Add');
    }

    function onPause() {

    }

    function onResume() {

    }

})();