(function () {
    "use strict";
    var directory_input_all;
    var directory_input_active;
    var directory_input_alumni;
    var current_directory = new Array();
    var main = document.getElementById('main');
    var page = document.getElementById('page');
    var header = document.getElementById('header');
    var footer = document.getElementById('footer');
    var directory = document.getElementById('directory');
    var title = document.getElementById('title');
    var load = document.getElementById('load');
    var menu_list = document.getElementById('menu_list');
    var menu_dead_space = document.getElementById('menu_dead_space');
    var selected = -1;
    var oldNode;
    var banner;
    document.addEventListener('deviceready', onDeviceReady.bind(this), false);

    function onDeviceReady() {
        try{
            directory_input_all = JSON.parse(localStorage.getItem("directory_input_all")); //checks for stored data
            directory_input_active = JSON.parse(localStorage.getItem("directory_input_active")); //checks for stored data
            directory_input_alumni = JSON.parse(localStorage.getItem("directory_input_alumni")); //checks for stored data
        } catch (e){
            directory_input_all = new Array(); //no data, initialize to empty
            directory_input_active = new Array(); //no data, initialize to empty
            directory_input_alumni = new Array(); //no data, initialize to empty
        }
        Parse.initialize('hehueu8y283yu3hlj14k3h4j1');
        Parse.serverURL = 'https://grektory.herokuapp.com/parse';
        document.addEventListener( 'pause', onPause.bind( this ), false );
        document.addEventListener('resume', onResume.bind(this), false);
        document.addEventListener('deviceready', setDisabled.bind($('button')), false); //default buttons to disabled
        document.addEventListener('deviceready', setConstraints.bind($('#main')), false);
        title.textContent = localStorage.dbname;
        requestData();
        $('#menu').change(onMenuChange);
        $('#search').click(setDisabled);
        $('#call').click(call);
        $('#text').click(text);
        $('#email').click(email);
        $('#search').click(search);
        $('#add').click(add);
    };

    function onMenuChange() {
        var selected = $(this).children(":selected").html();
        console.log(selected);
        setCurrentDirectory(selected);
    }

    function setCurrentDirectory(selected) {
        if (selected == "ACTIVE") {
            current_directory = directory_input_active;
        } else if (selected == "ALUMNI") {
            current_directory = directory_input_alumni;
        } else if (selected == "ALL MEMBERS") {
            current_directory = directory_input_all;
        }
        loadTable();
    }

    function setConstraints() {
        var height = page.offsetHeight - (header.offsetHeight);
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
            for (var i = 0; i < current_directory.length; i++) {
                if (verifyFilter(current_directory[i].name, filter)) {
                    var node = document.createElement("LI");
                    node.textContent = current_directory[i].name;
                    node.name = current_directory[i].name;
                    node.email = current_directory[i].email;
                    node.phone = current_directory[i].phone;
                    node.index = current_directory[i].index;
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
        window.open('tel:' + current_directory[selected].phone, '_top');
    }

    //TEXT FUNCTION
    function text() {
        localStorage.name = current_directory[selected].name;
        localStorage.phone = current_directory[selected].phone;
        window.location = './text.html';
    }

    //EMAIL FUNCTION
    function email() {
        window.open('mailto:' + current_directory[selected].email, '_top');
    }

    function add() {
        function onSuccess(contacts) {
            if (contacts[0] == null) {
                newContact();
            } else {
                jAlert("this is a test", "with dialog");
                alert("Contact with this name is already present on your mobile device.");
            }
        };

        function onError(contactError) {
            alert("An error has occurred. Please try again later.");
        };

        var options = new ContactFindOptions();
        options.filter = current_directory[selected].name;
        options.hasPhoneNumber = true;
        var fields = [navigator.contacts.fieldType.displayName, navigator.contacts.fieldType.name];
        navigator.contacts.find(fields, onSuccess, onError, options);
    }

    function newContact() {
        function onSuccess(contact) {
            alert("Contact " + current_directory[selected].name + " successfully created.");
        }

        function onError(contactError) {
            alert("An error has occurred. Please try again later.");
        }

        var contact = navigator.contacts.create();
        contact.displayName = current_directory[selected].name;
        contact.name = current_directory[selected].name;
        contact.emails = [new ContactField('emails', current_directory[selected].email, true)];
        contact.phoneNumbers = [new ContactField('mobile', current_directory[selected].phone, true)];
        contact.note = "Added via Grektory app";
        contact.save(onSuccess, onError);
    }

    //REQUESTS SYNCHRONISE DATA FROM XML
    function requestData() {
            load.style.visibility = 'visible';
            if (directory_input_all.length < 1) {
                queryDB();
            } else {
                setCurrentDirectory($('#menu').children(":selected").html());
                load.style.visibility = 'hidden';
                loadTable();
            }
    }

    function queryDB() {
        console.log("request...");
        var query = new Parse.Query(Parse.Object.extend(localStorage.dbval));
        query.ascending("name");
        query.limit(1000);
        query.find({
            success: function (results) {
                for (var i in results) {
                    var obj = results[i];
                    directory_input_all.push({ name: obj.get("name"), email: obj.get("email"), phone: obj.get("phone"), index: directory_input_all.length });
                    if (obj.get("alumni")) {
                        directory_input_alumni.push({ name: obj.get("name"), email: obj.get("email"), phone: obj.get("phone"), index: directory_input_alumni.length });
                    } else {
                        directory_input_active.push({ name: obj.get("name"), email: obj.get("email"), phone: obj.get("phone"), index: directory_input_active.length });
                    }
                }
                localStorage.setItem("directory_input_all", JSON.stringify(directory_input_all)); //stops requerying
                localStorage.setItem("directory_input_active", JSON.stringify(directory_input_active)); //stops requerying
                localStorage.setItem("directory_input_alumni", JSON.stringify(directory_input_alumni)); //stops requerying
                setCurrentDirectory($('#menu').children(":selected").html());
                loadTable();
                load.style.visibility = 'hidden';
            },
            error: function (error) {
                alert(error.message);
            }
        });
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
        for (var i = 0; i < current_directory.length; i++) {
            var new_header = current_directory[i].name.charAt(0);
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
            node.textContent = current_directory[i].name;
            node.name = current_directory[i].name;
            node.email = current_directory[i].email;
            node.phone = current_directory[i].phone;
            node.index = current_directory[i].index;
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