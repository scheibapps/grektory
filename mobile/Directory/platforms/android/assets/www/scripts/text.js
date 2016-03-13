(function () {
    document.addEventListener('deviceready', onDeviceReady.bind(this), false);
    var main = document.getElementById('main');
    var page = document.getElementById('page');
    var header = document.getElementById('header');
    var name = document.getElementById('name');
    var number = document.getElementById('number');
    var message = document.getElementById('message');
    var nameVal;
    var phoneVal;

    function onDeviceReady() {
        document.addEventListener('pause', onPause.bind(this), false);
        document.addEventListener('resume', onResume.bind(this), false);
        document.addEventListener('deviceready', setDisabled.bind($('#send')), false);
        $('#message').click(setEnabled);
        $('#send').click(send);
        $('#cancel').click(cancel);
        setHeader();
    };

    function setEnabled() {
        var elem = $(this);
        elem.data('oldVal', elem.val());
        elem.bind("propertychange change click keyup input paste", function (event) {
            var send = $('#send');
            if (elem.val().length > 0) {
                send[0].disabled = false;
            } else {
                send[0].disabled = true;
            }
        });
    }

    function setDisabled() {
        var elem = $(this);
        elem[0].disabled = true;
    }

    function send() {
        var app = {
            sendSms: function () {
                var number = localStorage.phone;
                var text = message.value;
                //CONFIGURATION
                var options = {
                    replaceLineBreaks: false, // true to replace \n by a new line, false by default
                    android: {
                        intent: 'INTENT'  // send SMS with the native android SMS messaging
                        //intent: '' // send SMS without open any other app
                    }
                };
                var success = function () {
                    console.log('Message sent successfully');
                };
                var error = function (e) {
                    console.log('Message Failed:' + e);
                };
                sms.send(number, text, options, success, error);
            }
        };
        app.sendSms();
    }

    function cancel() {
        window.history.back();
    }

    function setHeader() {
        name.innerHTML = nameVal = localStorage.name;
        //number.innerHTML = phoneVal = localStorage.phone;
    }

    function onPause() {
        // TODO: This application has been suspended. Save application state here.
    };

    function onResume() {
        // TODO: This application has been reactivated. Restore application state here.
    };
})();