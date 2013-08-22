var currentId = -1;
function setFecha(e) {
    currentId = e.id;
    AndroidFunction.openDatePickerDialog();
}

function mostrarFecha(date) {
    document.getElementById(currentId).value = date;
}

function save() {

    var maxid = -1;
    var metaTags = document.getElementsByTagName("meta");
    AndroidFunction.showFecha("1");
    var data = "<?xml version=1.0 ?><formData>";

    for (var i = 0; i < metaTags.length; i++) {
        if (metaTags[i].getAttribute("property") == "maxid") {
            maxid = metaTags[i].getAttribute("content");
            AndroidFunction.showFecha("Max ID: " + maxid);
            break;
        }
    }

    for (var i = 0; i < maxid; i++) {
        AndroidFunction.showFecha("" + i);
        var element = document.getElementById("" + i);

        var xdBinding = element.getAttribute("xd:binding");
        AndroidFunction.showFecha(xdBinding);
        var nodeName = xdBinding.substring(3);
        AndroidFunction.showFecha(nodeName);
        var nodeValue = element.value;
        AndroidFunction.showFecha(nodeValue);
        data = data + "<" + nodeName + ">" + nodeValue + "</" + nodeName + ">";

    }

    data = data + "</formData>";

    AndroidFunction.saveXml(data);
}