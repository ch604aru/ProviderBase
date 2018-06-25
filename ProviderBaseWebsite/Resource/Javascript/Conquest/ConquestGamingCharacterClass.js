/// <reference path="/Resource/Javascript/Jquery/jquery-2.1.4.js" />
/// <reference path="/Resource/Javascript/ProviderBase/ProviderBase.Ajax.js" />
/// <reference path="/Resource/Javascript/ProviderBase/ProviderBase.js" />
$(function () {
    CharacterClass.GetRecruitmentSummary();
});

(function(CharacterClass, $, undefined) {
    CharacterClass.GetRecruitmentSummary = function () {
        if ($("#RecruitmentRepeat").length > 0) {
            var requestUrl = "/WebControl.cch?Command=GetRecruitmentSummary";
            var requestDataList = new Array();

            requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilder_GUID", "91fe457a-ebae-430a-9d64-66d6955dcfad"));

            ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
                ProviderBaseAjax.AjaxResponse(response, "RecruitmentRepeat");
            }, requestDataList, "RecruitmentRepeat");
        }
    };

    CharacterClass.GetBlizzardCharacterProfile = function () {
        if ($("#CharacterProfile").length > 0) {
            var requestUrl = "/WebControl.cch?Command=GetBlizzardCharacterProfile";
            var requestDataList = new Array();
            var characterName = $("#CharacterUser_CharacterName").val();
            var realmName = $("#CharacterUser_CharacterServer").val();

            if (characterName.length > 0 && realmName.length > 0) {
                requestDataList.push(new ProviderBaseAjax.RequestData("CharacterName", characterName));
                requestDataList.push(new ProviderBaseAjax.RequestData("RealmName", realmName));
                requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilder_GUID", "DF444F45-D897-4296-BC08-1C43650E21E0"));

                ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
                    ProviderBaseAjax.AjaxResponse(response, "CharacterProfile");
                }, requestDataList, "CharacterProfile");
            }
        }
    };
}(window.CharacterClass = window.CharacterClass || {}, jQuery));
