$(function () {
    
});

(function (ProviderBase, $, undefined) {
    var c_ModalVisibleCount = 0;
    var c_ModalCurrent = 1;
    var c_ModalStack = new Array();
    var c_PageLast = {};
    var c_TabLast = {};
    var c_FormSearchSubmit = {};

    ProviderBase.ClearMessage = function (elementName) {
        $("#" + elementName).removeClass("fail");
        $("#" + elementName).removeClass("pass");
        $("#" + elementName).text("");
    }

    ProviderBase.ShowFailMessage = function (elementName, message) {
        $("#" + elementName).addClass("fail");
        $("#" + elementName).removeClass("pass");
        $("#" + elementName).text(message);
    }

    ProviderBase.ShowPassMessage = function (elementName, message) {
        $("#" + elementName).addClass("pass");
        $("#" + elementName).removeClass("fail");
        $("#" + elementName).text(message);
    }

    ProviderBase.GetUrlParameter = function (parameter) {
        return ProviderBase.GetQueryValue(parameter, window.location.search.substring(1));
    }

    ProviderBase.GetQueryValue = function (parameter, queryString) {
        var urlParameters = queryString;
        var urlVariables = urlParameters.split('&');

        for (var i = 0; i < urlVariables.length; i++) {
            var parameterName = urlVariables[i].split('=');

            if (parameterName[0].toLowerCase() == parameter.toLowerCase()) {
                return parameterName[1];
            }
        }
    };

    ProviderBase.GetFirstValue = function () {
        var urlpath = window.location.pathname.substring(1);
        var urlValues = urlpath.split('/');

        if (urlValues.length > 0) {
            return urlValues[urlValues.length - 1];
        }
        else {
            return "";
        }
    }

    ProviderBase.TabToggle = function (newTabID, tabIDPrefix, element) {
        if (c_TabLast[tabIDPrefix] == null) {
            var pageElementVisible = $("[id^=" + tabIDPrefix + "]").filter("div").filter(":visible");
            if (pageElementVisible.length > 0) {
                c_TabLast[tabIDPrefix] = pageElementVisible[0].id;
            }
        }

        $(`#${c_TabLast[tabIDPrefix]}-Tab`).removeClass("selected");
        $(`#${c_TabLast[tabIDPrefix]}`).hide();
        $(`#${newTabID}-Tab`).addClass("selected");
        $(`#${newTabID}`).show();

        c_TabLast[tabIDPrefix] = newTabID;
    };

    ProviderBase.PageToggle = function (newPageID, pagePrefix, validatePage) {
        var validationPassed = true;

        if (c_PageLast[pagePrefix] == null) {
            var pageElementVisible = $("[id^=" + pagePrefix + "]").filter(":visible");
            if (pageElementVisible.length > 0) {
                var pageDefaultID = pageElementVisible[0].id.replace(pagePrefix, "");

                c_PageLast[pagePrefix] = pageDefaultID;
            }
        }

        if (validatePage) {
            var currentPage = $("#" + pagePrefix + c_PageLast[pagePrefix]);
            var currentPageRequired = $(currentPage).find(".required");

            $(currentPageRequired).each(function () {
                switch (this.type) {
                    case "text": default:
                        if (this.value.length == 0) {
                            $(this).addClass("error");
                            $("#" + this.id + "ERR").html("Required field");
                            $("#" + this.id + "ERR").show();

                            validationPassed = false;
                        }
                        break;

                    case "radio": case "checkbox":
                        if (this.checked == false) {
                            $(this).addClass("error");
                            $("#" + this.id + "ERR").html("Required field");
                            $("#" + this.id + "ERR").show();

                            validationPassed = false;
                        }
                        break;
                }
            });
        }

        if (validationPassed && newPageID != c_PageLast[pagePrefix]) {
            $("#" + pagePrefix + c_PageLast[pagePrefix]).hide();
            $("#" + pagePrefix + newPageID).show();

            c_PageLast[pagePrefix] = newPageID;
        }
    }

    ProviderBase.SplitToElement = function (splitElement, delimiter, targetArray) {
        var value = $("#" + splitElement).val();

        if (value.length > 0) {
            var splitValue = value.split(delimiter);

            for (var i = 0; (i < targetArray.length && i < splitValue.length) ; i++) {
                if (splitValue[i].charAt(0) == "0") {
                    var splitValueTemp = splitValue[i].substring(1);

                    $("#" + targetArray[i]).val(splitValueTemp);
                }
                else {
                    $("#" + targetArray[i]).val(splitValue[i]);
                }
            }
        }
    }

    ProviderBase.AppendToElement = function (appendArray, delimiter, elementTarget) {
        if (appendArray.length > 0) {
            var appendString = "";

            for (var i = 0; i < appendArray.length; i++) {
                var appendValue = "";

                if (appendString.length > 0) {
                    appendString += delimiter;
                }

                appendValue = $("#" + appendArray[i]).val();

                if (appendValue != null && appendValue.length == 1) {
                    appendValue = "0" + appendValue;
                }

                appendString += appendValue;
            }

            $("#" + elementTarget).val(appendString);
        }
    }

    ProviderBase.HtmlEncode = function (value) {
        return $('<div/>').text(value).html();
    }

    ProviderBase.HtmlDecode = function (value) {
        return $('<div/>').html(value).text();
    }

    ProviderBase.ShowModalAlert = function (content, title) {
        title = (title == undefined) ? "Error" : title;

        $("#ModalAlert").find(".js-modal-body").html(content);
        $("#ModalAlert").find(".js-modal-header-text").html(title);

        ShowModal("ModalAlert");
    };

    ProviderBase.HideModalAlert = function () {
        HideModal("ModalAlert");
    };

    ProviderBase.ShowModalException = function (content, title) {
        title = (title == undefined) ? "Error" : title;

        $("#ModalException").find(".js-modal-body").html(content);
        $("#ModalException").find(".js-modal-header-text").html(title);

        ShowModal("ModalException");
    };

    ProviderBase.HideModalException = function () {
        HideModal("ModalException");
    };

    ProviderBase.ShowModal = function (content, title, formComplete) {
        var formElement = null;
        var currentModal = `Modal${c_ModalCurrent}`;

        title = (title == undefined) ? "" : title;

        $(`#${currentModal}`).find(".js-modal-body").html(content);
        $(`#${currentModal}`).find(".js-modal-header-text").html(title);

        ShowModal(`${currentModal}`);
        
        formElement = $(`#${currentModal}`).find("form");

        if (formElement.length > 0) {
            ProviderBase.SetupFormModal(formElement, currentModal, formComplete);
        }

        c_ModalCurrent++;
    };

    ProviderBase.HideModal = function (name) {
        c_ModalCurrent--;
        HideModal(name);
    };

    ProviderBase.SubmitModal = function (name) {
        $(`#${name}`).find("form").submit();
    };

    ProviderBase.ShowModalInfo = function (content, icon) {
        var newModalInfo = "";
        var newModal = [];
        var modalInfoIcon = "";
        var modalInfoID = ProviderBase.GenerateGUID();

        icon = (icon == undefined) ? "flag" : icon;

        modalInfoIcon = `<div class='icon-${icon}-g-30'></div>`;

        newModalInfo = $("#ModalInfo-Template").html();
        newModal = $.parseHTML(newModalInfo);

        $(newModal).find(".js-modal-info-icon").first().html(modalInfoIcon);
        $(newModal).find(".js-modal-info-text").first().html(content);
        $(newModal).prop("id", modalInfoID);

        $("#ModalInfo").append(newModal);

        ShowModal(modalInfoID);
        
        setTimeout(function () {
            HideModal(modalInfoID);

            setTimeout(function () {
                DeleteElement(modalInfoID);
            }, 2000);
        }, 5000);
    };

    ProviderBase.HideModalInfo = function (element) {
        var modalInfoID = "";

        modalInfoID = $(element).find(".js-modal-info-item").id();

        HideModal(modalInfoID);

        setTimeout(function () {
            DeleteElement(ModalInfoID);
        }, 4000);
    };

    ProviderBase.ShowLoading = function (name) {
        var loading = "<div class='loadingcontainer'><div class='loading'></div></div>";

        $(`#${name}`).html(loading);
    }

    ProviderBase.HideLoading = function (name) {
        $(`#${name}`).find(".loadingcontainer").remove();
    }

    ProviderBase.ShowLoadingOverlay = function (element) {
        var loading = "<div class='loadingcontaineroverlay'><div class='loadingoverlay'></div></div>";

        $(element).prepend(loading);
    };

    ProviderBase.HideLoadingOverlay = function (element) {
        $(element).find(".loadingcontaineroverlay").remove();
    };

    ProviderBase.ShowTooltip = function (element) {
        $(`#${element.id}Tooltip`).show();
    };

    ProviderBase.HideTooltip = function (element) {
        $(`#${element.id}Tooltip`).hide();
    };

    ProviderBase.SetupFormModal = function (formElement, modalName, completeEvent) {
        $(formElement).ajaxForm(options = {
            beforeSubmit: function () {
                ProviderBase.ShowLoadingOverlay($(formElement).find(".js-modal-content"));
            },
            success: function (response) {
                ProviderBase.HideLoadingOverlay($(formElement).find(".js-modal-content"));

                if (response.Status == 1) {
                    ProviderBase.HideModal(modalName);
                    ProviderBase.ShowModalInfo(response.Message);

                    if (completeEvent) {
                        completeEvent();
                    }
                }
                else {
                    ProviderBase.ShowModalAlert(response.Message);
                }
            },
            dataType: "json"
        });
    };

    ProviderBase.SetupPaging = function (responseElement, responsePagingElement) {
        $(`#${responsePagingElement}`).ajaxForm(options = {
            beforeSubmit: function () {
                ProviderBase.ShowLoading(responseElement);
            },
            success: function (response) {
                ProviderBase.HideLoading(responseElement);

                ProviderBaseAjax.AjaxResponse(response, responseElement);

                ProviderBase.SetupPaging(responseElement, responsePagingElement);
            },
            dataType: "json"
        });
    };

    ProviderBase.ToggleCheckbox = function (element) {
        element.prop("checked", !element.prop("checked"));
        element.trigger("change");
    };

    ProviderBase.Search = function (element, minLength) {
        minLength = (minLength == undefined) ? 3 : minLength;

        if (element.value.length > minLength || element.value.length == 0) {
            var elementForm = null;
            var elementFormID = "";
            var elementFormSubmit = null;

            elementForm = $(element).closest("form");
            elementFormID = $(elementForm).get(0).id;

            if (elementFormID != undefined && elementFormID.length > 0 && c_FormSearchSubmit[elementFormID]) {
                var elementFormSubmitData = null;

                elementFormSubmitData = c_FormSearchSubmit[elementFormID].data("jqxhr");

                if (elementFormSubmitData.status == undefined) {
                    c_FormSearchSubmit[elementFormID].data("jqxhr").abort();
                }
            }

            elementFormSubmit = $(elementForm).submit();

            c_FormSearchSubmit[elementFormID] = elementFormSubmit;
        }
    };

    ProviderBase.ArrayRemove = function (array, value) {
        var index = array.indexOf(value);

        if (index !== -1) {
            array.splice(index, 1);
        }

        return array;
    };

    ProviderBase.GenerateGUID = function () {
        var lut = [];

        for (var i = 0; i < 256; i++) {
            lut[i] = ((i < 16) ? '0' : '') + (i).toString(16);
        }

        var d0 = Math.random() * 0xffffffff | 0;
        var d1 = Math.random() * 0xffffffff | 0;
        var d2 = Math.random() * 0xffffffff | 0;
        var d3 = Math.random() * 0xffffffff | 0;
        return lut[d0 & 0xff] + lut[d0 >> 8 & 0xff] + lut[d0 >> 16 & 0xff] + lut[d0 >> 24 & 0xff] + '-' +
          lut[d1 & 0xff] + lut[d1 >> 8 & 0xff] + '-' + lut[d1 >> 16 & 0x0f | 0x40] + lut[d1 >> 24 & 0xff] + '-' +
          lut[d2 & 0x3f | 0x80] + lut[d2 >> 8 & 0xff] + '-' + lut[d2 >> 16 & 0xff] + lut[d2 >> 24 & 0xff] +
          lut[d3 & 0xff] + lut[d3 >> 8 & 0xff] + lut[d3 >> 16 & 0xff] + lut[d3 >> 24 & 0xff];
    };

    function ShowModal(name) {
        $(`#${name}`).fadeIn();
    };

    function HideModal(name) {
        $(`#${name}`).fadeOut();
    };

    function DeleteElement(name) {
        $(`#${name}`).remove();
    };
}(window.ProviderBase = window.ProviderBase || {}, jQuery));
