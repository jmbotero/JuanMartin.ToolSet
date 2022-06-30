// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function SetRank() {
    var getSelectedValue = document.querySelector('input[name="ranking"]:checked');

    if (getSelectedValue != null) {
        var rank = 11 - parseInt(getSelectedValue.value);
        var rankText = document.getElementById('rank');

        rankText.value = rank;
        document.getElementById('selectedTagListAction').value = "none";
    }
}

function ClearTag() {
    document.getElementById('Tag').value = "";
}

function SetListAction(actionName) {
    document.getElementById('selectedTagListAction').value = actionName;
}

function SetTag(element) {
    var selectedText = element.value;

    //alert(selectedText);
    document.getElementById('Tag').value = selectedText;
}

function GetSelectedTagText(element) {
    var selectedText = element.options[element.selectedIndex].text;
    document.getElementById('Tag').value = selectedText;
    document.getElementById('hiddenTag').value = selectedText;
}
