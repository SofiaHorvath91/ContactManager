// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var collapsibles = document.getElementsByClassName("collapsible");
var powerCheckboxes = document.getElementsByClassName("selectedPowersCB");
var universesCheckboxes = document.getElementsByClassName("selectedUniversesCB");
var teamsCheckboxes = document.getElementsByClassName("selectedTeamsCB");
var opponentsSide1Checkboxes = document.getElementsByClassName("opponents-side1");
var opponentsSide2Checkboxes = document.getElementsByClassName("opponents-side2");
var opponentsDivs = document.getElementsByClassName("opponents-divs");

$(function () {

    for (i = 0; i < collapsibles.length; i++) {
        collapsibles[i].addEventListener("click", function () {
            this.classList.toggle("active");
            var content = this.nextElementSibling;
            if (content.style.display === "flex" || content.style.display === "block") {
                content.style.display = "none";
            } else {
                if (content.id.split("_")[1] != "table") {
                    content.style.display = "flex";
                }
                else {
                    content.style.display = "block";
                }
            }
        });
    }

    for (var i = 0; i < powerCheckboxes.length; i++) {

        if (powerCheckboxes[i].checked == true) {
            var divname = powerCheckboxes[i].id.split(" _")[0] + " _div";
            document.getElementById(divname).classList.remove("hidden");
        }
    }

    for (var i = 0; i < universesCheckboxes.length; i++) {

        if (universesCheckboxes[i].checked == true) {
            var divname = universesCheckboxes[i].id.split(" _")[0] + " _div";
            document.getElementById(divname).classList.remove("hidden");
        }
    }

    for (var i = 0; i < teamsCheckboxes.length; i++) {

        var selectname = teamsCheckboxes[i].id.split(" _")[0] + " _selectRole";
        if (teamsCheckboxes[i].checked == true) {
            document.getElementById(selectname).disabled = false;
        }
        else {
            document.getElementById(selectname).disabled = true;
        }
    }
});


$('.selectedPowersCB').click(function () {
    var id = $(this).attr('id');
    var divname = id.split(" _")[0] + " _div";
    var cb = document.getElementById(id);

    if (cb.checked == true) {
        document.getElementById(divname).classList.remove("hidden");
    }
    else {
        document.getElementById(divname).classList.add("hidden");
    }
});

$('.selectedUniversesCB').click(function () {
    var id = $(this).attr('id');
    var divname = id.split(" _")[0] + " _div";
    var cb = document.getElementById(id);

    if (cb.checked == true) {
        document.getElementById(divname).classList.remove("hidden");

        for (var i = 0; i < universesCheckboxes.length; i++) {
            if (universesCheckboxes[i] != cb) {
                universesCheckboxes[i].checked = false;

                if (!universesCheckboxes[i].classList.contains("hidden")) {
                    var div = universesCheckboxes[i].id.split(" _")[0] + " _div";
                    document.getElementById(div).classList.add("hidden");
                }
            }
        }
    }
    else {
        document.getElementById(divname).classList.add("hidden");
    }
});

$('.selectedTeamUniversesCB').click(function () {
    var id = $(this).attr('id');
    var cb = document.getElementById(id);

    if (cb.checked == true) {
        var checkboxes = document.getElementsByClassName("selectedTeamUniversesCB");
        for (var i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i] != cb) {
                checkboxes[i].checked = false;
            }
        }
    }
});

$('.selectedTeamsCB').click(function () {
    var id = $(this).attr('id');
    var selectname = id.split(" _")[0] + " _selectRole";
    var cb = document.getElementById(id);

    if (cb.checked == true) {
        document.getElementById(selectname).disabled = false;
    }
    else {
        document.getElementById(selectname).disabled = true;
    }
});

$('.opponents-side1').click(function () {
    var id = $(this).attr('id');
    var divname = id.split("_")[0] + "_div";
    var cb = document.getElementById(id);

    if (cb.checked == true) {
        document.getElementById(divname).classList.remove("hidden");

        for (var i = 0; i < opponentsSide1Checkboxes.length; i++) {
            if (opponentsSide1Checkboxes[i] != cb) {
                opponentsSide1Checkboxes[i].checked = false;

                var div = opponentsSide1Checkboxes[i].id.split("_")[0] + "_div";
                document.getElementById(div).classList.add("hidden");

                if (!document.getElementById(div).classList.contains("hidden")) {
                    document.getElementById(div).classList.add("hidden");
                }
            }
        }
    }
    else {
        document.getElementById(divname).classList.add("hidden");
    }
});

$('.opponents-side2').click(function () {
    var id = $(this).attr('id');
    var divname = id.split("_")[0] + "_div";
    var cb = document.getElementById(id);

    if (cb.checked == true) {
        document.getElementById(divname).classList.remove("hidden");

        for (var i = 0; i < opponentsSide2Checkboxes.length; i++) {
            if (opponentsSide2Checkboxes[i] != cb) {
                opponentsSide2Checkboxes[i].checked = false;

                var div = opponentsSide2Checkboxes[i].id.split("_")[0] + "_div";
                document.getElementById(div).classList.add("hidden");

                if (!document.getElementById(div).classList.contains("hidden")) {
                    document.getElementById(div).classList.add("hidden");
                }
            }
        }
    }
    else {
        document.getElementById(divname).classList.add("hidden");
    }
});