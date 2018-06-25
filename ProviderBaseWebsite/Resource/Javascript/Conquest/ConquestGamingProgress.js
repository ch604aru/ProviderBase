/// <reference path="/Resource/Javascript/Jquery/jquery-2.1.4.js" />
/// <reference path="/Resource/Javascript/ProviderBase/ProviderBase.Ajax.js" />
/// <reference path="/Resource/Javascript/ProviderBase/ProviderBase.js" />
$(function () {
    Progress.GetProgressSummary();
});

(function (Progress, $, undefined) {
    //Private Property
    //var isHot = true;

    //Public Property
    //skillet.ingredient = "Bacon Strips";

    //Public Method
    Progress.GetProgressSummary = function () {
        if ($("#ProgressRepeat").length > 0) {
            var requestUrl = "/WebControl.ph?Command=GetProgressSummary";
            var requestDataList = new Array();

            requestDataList.push(new ProviderBaseAjax.RequestData("ProgressIDList", "10,11,12"));

            ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
                ProviderBaseAjax.AjaxResponse(response, "ProgressRepeat");
            }, requestDataList, "ProgressRepeat");
        }
    };

    //Private Method
    //function addItem(item) {
    //    if (item !== undefined) {
    //        console.log("Adding " + $.trim(item));
    //    }
    //}
}(window.Progress = window.Progress || {}, jQuery));
