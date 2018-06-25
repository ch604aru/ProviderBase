/// <reference path="jquery-2.1.4.js" />
/// <reference path="ProviderBase.Ajax.js" />
$(function() {
    if ($("#loginForm").length > 0) {
        $("#loginForm").on("submit", function(e) {
            e.preventDefault();
            Login();
        });
    }
});

function Login() {
    var requestUrl = "/WebControl.ch?Command=Login";
    var requestDataList = new Array();

    requestDataList.push(new RequestData("Username", $("#username").val()));
    requestDataList.push(new RequestData("Password", $("#password").val()));

    AjaxRequest(requestUrl, function(response) {
        if (response.Status == 1) {
            location.href = "";
        } else {
            $("#loginMessage").text(response.Message);
        }
    }, requestDataList);
}

function Logout() {
    var requestUrl = "/WebControl.ch?Command=Logout";
    
    AjaxRequest(requestUrl, function(response) {
        if (response.Status == 1) {
            location.href = "";
        } else {
            $("#loginMessage").text(response.Message);
        }
    });
}
