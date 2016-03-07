// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=397704
// To debug code on page load in Ripple or on Android devices/emulators: launch your app, set breakpoints, 
// and then run "window.location.reload()" in the JavaScript Console.
(function () {
    "use strict";
    var directory_input = new Array();
    document.addEventListener('deviceready', onDeviceReady.bind(this), false);
    var main = document.getElementById('main');
    var page = document.getElementById('page');
    var header = document.getElementById('header');
    var footer = document.getElementById('footer');
    var directory = document.getElementById('directory');
    var selected = -1;
    var oldNode;

    function onDeviceReady() {
        Parse.initialize('hehueu8y283yu3hlj14k3h4j1');
        Parse.serverURL = 'https://grektory.herokuapp.com/parse';
        document.addEventListener( 'pause', onPause.bind( this ), false );
        document.addEventListener('resume', onResume.bind(this), false);
        document.addEventListener('deviceready', setDisabled.bind($('button')), false); //default buttons to disabled
        document.addEventListener('deviceready', setConstraints.bind($('#main')), false);
        requestData();
        $('#search').click(setDisabled);
        $('#call').click(call);
        $('#text').click(text);
        $('#email').click(email);
        $('#search').click(search);
    };

    function setConstraints() {
        var height = page.offsetHeight - (header.offsetHeight + footer.offsetHeight);
        $('#main').attr("style", "height: " + height + "px");
    }

    //Enables buttons if table is click
    function setEnabled() {
        var elem = $('button');
        for (var i = 0; i < elem.length; i++) {
            elem[i].disabled = false;
        }
    }

    //Disables buttons if Search or Head is clicked
    function setDisabled() {
        var elem = $('button');
        for (var i = 0; i < elem.length; i++) {
            elem[i].disabled = true;
        }
        if (oldNode != null) {
            oldNode.style.backgroundColor = directory.style.backgroundColor;
        }
    }

    //SEARCHBAR FUNCTION TO FILTER
    function search() {
        var elem = $(this);
        elem.data('oldVal', elem.val());
        elem.bind("propertychange change click keyup input paste", function (event) {
            if (elem.data('oldVal') != elem.val()) {
                elem.data('oldVal', elem.val());
                filterTable(elem.val());
            }
        });
        setConstraints();
    }

    //FILTERS TABLE, USED BY SEARCHBAR
    function filterTable(filter) {
        $('#directory').empty();
        if (filter.length > 0) {
            for (var i = 0; i < directory_input.length; i++) {
                if (verifyFilter(directory_input[i].name,filter)) {
                    var node = document.createElement("LI");
                    node.textContent = directory_input[i].name;
                    node.name = directory_input[i].name;
                    node.email = directory_input[i].email;
                    node.phone = directory_input[i].phone;
                    node.index = directory_input[i].index;
                    node.setAttribute("style", "padding-left:25px;");
                    node.addEventListener("click", rowClick.bind(node), false);
                    directory.appendChild(node);
                }
            }
        } else {
            loadTable();
        }
    }

    //BOOLEAN FUNCTION TO ADD FILTERED DATA TO TABLE
    function verifyFilter(name, filter) {
        name = name.toLowerCase();
        filter = filter.toLowerCase();
        var charName = name.split('');
        var charFilter = filter.split('');
        if (charFilter.length > charName.length) {
            return false;
        }
        for (var i = 0; i < filter.length; i++) {
            if (charName[i] != charFilter[i]) {
                return false;
            }
        }
        return true;
    }

    //CALL FUNCTION
    function call() {
        window.open('tel:'+directory_input[selected].phone, '_top');
    }

    //TEXT FUNCTION
    function text() {
        localStorage.name = directory_input[selected].name;
        localStorage.phone = directory_input[selected].phone;
        window.location = './text.html';
    }

    //EMAIL FUNCTION
    function email() {
        window.open('mailto:' + directory_input[selected].email, '_top');
    }

    //REQUESTS SYNCHRONISE DATA FROM XML
    function requestData() {
            var load = document.getElementById("load");
            load.style.visibility = "visible";
            var query = new Parse.Query(Parse.Object.extend(localStorage.db));
            query.ascending("name");
            query.limit(1000);
            query.find({
                success: function (results) {
                    for (var i in results) {
                        var obj = results[i];
                        directory_input.push({ name: obj.get("name"), email: obj.get("email"), phone: obj.get("phone"), index: i });
                    }
                    loadTable();
                    load.style.visibility = "hidden";
                },
                error: function (error) {
                    load.style.visibility = "hidden";
                    alert(error.message);
                }
            });
        //JSON REQUEST (JQUERY)
        /*
        $.getJSON("./json/directory_data.json", function (data) {
            $.each(data, function (key,val) {
                directory_input.push({ name: val.member.name, phone: val.member.phone, email: val.member.email, index: 0});
            });
            directory_input.sort(function (a, b) {
                if (a.name < b.name) return -1;
                if (a.name > b.name) return 1;
                return 0;
            });
            for (var i in directory_input) {
                directory_input[i].index = i;
            }
            loadTable();
        });
        */
        //XML REQUEST
        /*
        var request = new XMLHttpRequest();
        request.open("GET", "./xml/directory.xml", false);
        request.send();
        var xml = request.responseXML;
        var members = xml.getElementsByTagName("member");
        for (var i = 0; i < members.length; i++) {
            var member = members[i];
            var name = member.getElementsByTagName("name")[0].childNodes[0].nodeValue;
            var phone = member.getElementsByTagName("phone")[0].childNodes[0].nodeValue;
            directory_input.push({name:name,phone:phone});
        }
        */
    }

    //ROW CLICK FUNCTION
    function rowClick() {
        setEnabled();
        if (oldNode != null) {
            oldNode.style.backgroundColor = directory.style.backgroundColor;
        }
        oldNode = this;
        this.style.backgroundColor = "#f2e6ff";
        console.log();
        selected = this.index;
    }

    //LOAD TABLE FUNCTION
    function loadTable() {
        $('#directory').empty();
        var header = '';
        for (var i = 0; i < directory_input.length; i++) {
            var new_header = directory_input[i].name.charAt(0);
            if (header != new_header) { //Adds alphabetical header to row
                header = new_header;
                var head_node = document.createElement("li");
                head_node.textContent = new_header;
                head_node.setAttribute("data-role", "list-divider");
                head_node.setAttribute("class", "ui-li-divider ui-bar-a");
                head_node.addEventListener("click", setDisabled.bind(node), false);
                directory.appendChild(head_node);
            }
            var node = document.createElement("li");
            node.textContent = directory_input[i].name;
            node.name = directory_input[i].name;
            node.email = directory_input[i].email;
            node.phone = directory_input[i].phone;
            node.index = directory_input[i].index;
            node.setAttribute("style", "padding-left:25px;");
            node.addEventListener("click", rowClick.bind(node), false);
            directory.appendChild(node);
        }
    }

    function onPause() {
        // TODO: This application has been suspended. Save application state here.
    };

    function onResume() {
        // TODO: This application has been reactivated. Restore application state here.
    };
} )();