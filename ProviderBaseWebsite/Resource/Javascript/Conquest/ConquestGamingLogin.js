/// <reference path="/Resource/Javascript/Jquery/jquery-2.1.4.js" />
/// <reference path="/Resource/Javascript/ProviderBase/ProviderBase.Ajax.js" />
/// <reference path="/Resource/Javascript/ProviderBase/ProviderBase.js" />
$(function () {
    Login.SetupLoginForm();
    Login.GetUserCharacterSummary();
});

(function (login, $, undefined) {
    Login.SetupLoginForm = function () {
        if ($("#Login").length > 0) {
            $("#Login").ajaxForm(options = {
                beforeSubmit: function () {
                    ProviderBase.ClearMessage("LoginMessage");
                },
                success: function (response) {
                    if (response.Status == 1) {
                        location.href = "";
                    } else {
                        ProviderBase.ShowFailMessage("LoginMessage", response.Message);
                    }
                },
                dataType: "json"
            });
        }
    };

    Login.Login = function () {
        var requestUrl = "/WebControl.ch?Command=Login";
        var requestDataList = new Array();

        requestDataList.push(new ProviderBaseAjax.RequestData("Username", $("#username").val()));
        requestDataList.push(new ProviderBaseAjax.RequestData("Password", $("#password").val()));

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            if (response.Status == 1) {
                location.href = "";
            } else {
                if ($("#LoginMessage").length > 0) {
                    ProviderBase.ShowFailMessage("LoginMessage", response.Message);
                } else {
                    alert(response.Message);
                }
            }
        }, requestDataList);
    };

    Login.logout = function() {
        var requestUrl = "/WebControl.ch?Command=Logout";

        ProviderBaseAjax.AjaxRequest(requestUrl, function(response) {
            if (response.Status == 1) {
                location.href = "";
            } else {
                if ($("#LoginMessage").length > 0) {
                    ProviderBase.ShowFailMessage("LoginMessage", response.Message);
                } else {
                    alert(response.Message);
                }
            }
        });
    };

    Login.GetUserCharacterSummary = function () {
        if ($("#HeaderbarRepeat").length > 0) {
            var requestUrl = "/WebControl.cch?Command=GetUserCharacterSummary";
            var requestDataList = new Array();

            ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
                ProviderBaseAjax.AjaxResponse(response, "HeaderbarRepeat");
            }, requestDataList);
        }
    };
}(window.Login = window.Login || {}, jQuery));
