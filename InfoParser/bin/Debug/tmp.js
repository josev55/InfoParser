window.onload = function () {
    if (isIphone) {
        var iframe = document.createElement("IFRAME");
        setInputEnabled();
    } else {
        AndroidFunction.cargarDatos();
    }
}
var currentId = -1;
var maxid = -1;
var isIphone = false;
function setFecha(e) {
    if (!isIphone) {
        currentId = e.id;
        AndroidFunction.openDatePickerDialog();
    }
}

function setInputEnabled() {
    var inputs = document.getElementsByTagName("input");
    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].getAttribute("type") == "date") {
            inputs[i].removeAttribute("readonly");
        }
    }
}

function mostrarFecha(date) {
    document.getElementById(currentId).value = date;
}

function obtainMaxID() {
    var metaTags = document.getElementsByTagName("meta");
    for (var i = 0; i < metaTags.length; i++) {
        if (metaTags[i].getAttribute("property") == "maxid") {
            maxid = metaTags[i].getAttribute("content");
            break;
        }
    }
}

function setIphone() {
    isIphone = true;
    console.log("IPHONE SETEADO");
}

function save() {
    var metaTags = document.getElementsByTagName("meta");
    var data = "<?xml version='1.0' ?><formData>";

    for (var i = 0; i < metaTags.length; i++) {
        if (metaTags[i].getAttribute("property") == "maxid") {
            maxid = metaTags[i].getAttribute("content");
            break;
        }
    }

    for (var i = 0; i < maxid; i++) {
        var element = document.getElementById("" + i);
        if (element.getAttribute("type") != "button") {
            var xdBinding = element.getAttribute("xd:binding");
            var nodeName = xdBinding.substring(3);
            var nodeValue = element.value;
            data = data + "<" + nodeName + " id='" + i + "'>" + nodeValue + "</" + nodeName + ">";
        }

    }

    data = data + "</formData>";
    if (isIphone) {
        var iframe = document.createElement("IFRAME");
        iframe.setAttribute("width", "1");
        iframe.setAttribute("height", "1");
        iframe.setAttribute("src", "js-frame:save:" + data);
        document.documentElement.appendChild(iframe);
        iframe.parentNode.removeChild(iframe);
        iframe = null;
    } else {
        AndroidFunction.saveXML(data);
    }
}

function loadData(json) {
    try {
        console.log("ENTRA ACA");
        obtainMaxID();

        var jsonData = JSON.parse(json);
        console.log(jsonData);
        for (var i = 0; i < jsonData.length; i++) {
            var id = jsonData[i].guid;
            var value = jsonData[i].valor;
            var element = document.getElementById(id);
            console.log(id);
            if (element.getAttribute("type") != "button") {
                if (element.getAttribute("xd:xctname") == "dropdown") {
                    var optionList = element.options;
                    for (var j = 0; j < optionList.length; j++) {
                        if (optionList[j].value == value) {
                            element.selectedIndex = j;
                            break;
                        }
                    }
                } else {
                    element.value = value;
                }
            }
        }
    } catch (err) {
        console.log(err);
    }
}