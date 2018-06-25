/// <reference path="ProviderBase.js" />
/// <reference path="ProviderBase.Ajax.js" />


$(function () {

});

(function (ProviderBaseFormBuilder, $, undefined) {
    ProviderBaseFormBuilder.GetFormBuilder = function (formBuilderGUID, targetElementID) {
        if (formBuilderGUID.length > 0 && $("#" + targetElementID).length > 0) {
            var requestUrl = "/WebControl.fbh?Command=FormBuilderGet";
            var requestDataList = new Array();

            requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilder_GUID", formBuilderGUID));

            ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
                ProviderBaseAjax.AjaxResponse(response, targetElementID);

                var form = $("#" + targetElementID).find("form");

                if (form.length > 0) {
                    form.ajaxForm(options = {
                        beforeSubmit: function () {
                            ProviderBase.ClearMessage(targetElementID + "Message");
                        },
                        success: function (response) {
                            if (response.Status == -2) {
                                if ($("#ModalException").length > 0 && response.Data.length > 0) {
                                    $("#ModalException").find(".modal-body").html(response.Data[0]);
                                    $("#ModalException").find(".providerbase-modal-content").width("800px").height("600px");
                                    ProviderBase.ShowModal("ModalException");
                                } else {
                                    ProviderBase.ShowFailMessage(targetElementID + "Message", response.Message);
                                }
                            } else if (response.Status == 2) {
                                if (response.Redirect.length > 0) {
                                    location.href = response.Redirect;
                                } else {
                                    ProviderBase.ShowPassMessage(targetElementID + "Message", response.Message);
                                }

                            } else if (response.Status == 1) {
                                ProviderBase.ShowPassMessage(targetElementID + "Message", response.Message);
                            } else {
                                ProviderBase.ShowFailMessage(targetElementID + "Message", response.Message);
                            }
                        },
                        dataType: "json"
                    });
                }
            }, requestDataList, targetElementID);
        }
    };
}(window.ProviderBaseFormBuilder = window.ProviderBaseFormBuilder || {}, jQuery));
