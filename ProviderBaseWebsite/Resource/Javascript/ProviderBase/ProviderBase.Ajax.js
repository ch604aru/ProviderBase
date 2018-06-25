/// <reference path="/Resource/Javascript/ProviderBase/ProviderBase.js" />
(function (ProviderBaseAjax, $, undefined) {
    var c_AjaxSubmit = {};

    ProviderBaseAjax.AjaxRequest = function(requestUrl, requestResponse, requestData, responseElement) {
        var ajaxDataArray = {};
        var command = "";
        var ajaxSubmit = null;

        if (requestData != null && requestData.length > 0) {
            for (var i = 0; i < requestData.length; i++) {
                ajaxDataArray[requestData[i].ParameterName] = requestData[i].ParameterValue;
            }
        }

        command = ProviderBase.GetQueryValue("Command", requestUrl.split("?").pop());

        if (command != undefined && command.length > 0 && c_AjaxSubmit[command]) {
            var ajaxSubmitData = null;

            ajaxSubmitData = c_AjaxSubmit[command];

            if (ajaxSubmitData != undefined) {
                ajaxSubmitData.abort();
            }
        }

        ajaxSubmit = $.ajax({
            method: "POST",
            url: requestUrl,
            data: ajaxDataArray,
            dataType: "json",
            cache: false,
            beforeSend: function () {
                if (responseElement != undefined) {
                    ProviderBase.ShowLoading(responseElement);
                }
            }
        })
        .done(function (response) {
            if (responseElement != undefined) {
                ProviderBase.HideLoading(responseElement);
            }

            if (requestResponse) {
                requestResponse(response);
            } else {
                ProviderBaseAjax.AjaxResponse(response, responseElement);
            }
        })
        .fail(function (jqXHR, textStatus) {
            if (jqXHR.status > 0) {
                alert(`Error: ${jqXHR.statusText}`);
            }
        });

        c_AjaxSubmit[command] = ajaxSubmit;
    }

    ProviderBaseAjax.AjaxResponse = function(response, elementName, elementPaging) {
        var element = $(`#${elementName}`);

        switch (response.Status) {
            case -2: // Exception
                if ($("#ModalException").length > 0 && response.Data.length > 0) {
                    ProviderBase.ShowModalException(response.Data[0]);
                } else {
                    alert(response.Message);
                }
                break;

            case -1: // Failed
                if ($("#ModalAlert").length > 0) {
                    ProviderBase.ShowModalAlert(response.Message);
                } else {
                    alert(response.Message);
                }
                break;

            case 0: // Unassigned
                if (element != undefined) {
                    element.html(response.Message);
                }
                break;

            case 1: // Success
                if (response.Data.length > 0 && response.Data[0].length > 0) {
                    element.html(response.Data[0]);

                    if (elementPaging != undefined) {
                        ProviderBase.SetupPaging(elementName, elementPaging);
                    }
                } else if (element != undefined) {
                    element.html(response.Message);
                }
                break;

            case 2: // Redirect
                if (response.Redirect.length > 0) {
                    location.href = response.Redirect;
                }
                break;
        }
    }

    ProviderBaseAjax.RequestData = function(parameterName, parameterValue) {
        this.ParameterName = parameterName;
        this.ParameterValue = parameterValue;
    }
    
    ProviderBaseAjax.AjaxResult = function() {
        this.AjaxData = new Array();
        this.Data = new Array();
        this.Status = 0;
        this.Message = "";
    }
}(window.ProviderBaseAjax = window.ProviderBaseAjax || {}, jQuery));
