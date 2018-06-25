/// <reference path="/Resource/Javascript/Jquery/jquery-2.1.4.js" />
/// <reference path="/Resource/Javascript/ProviderBase/ProviderBase.Ajax.js" />
/// <reference path="/Resource/Javascript/ProviderBase/ProviderBase.js" />
$(function () {
    var firstValue = ProviderBase.GetFirstValue();

    switch (firstValue.toLowerCase()) {
        case "": default:
            Forum.GetForumIndex();
            break;

        case "area":
            Forum.GetForumArea();
            break;

        case "group":
            Forum.GetForumGroup();
            break;

        case "thread":
            Forum.GetForumThread();
            break;

        case "threadnew":
            Forum.GetForumThreadNew();
            break;
    }
});

(function (forum, $, undefined) {
    Forum.GetForumIndex = function () {
        if ($("#ForumRepeat").length > 0) {
            var requestUrl = "/WebControl.fh?Command=GetForumIndex";
            var requestDataList = new Array();

            ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
                ProviderBaseAjax.AjaxResponse(response, "ForumRepeat");
                $('.forum-border').last().remove();
            }, requestDataList, "ForumRepeat");
        }
    };

    Forum.GetForumArea = function () {
        if ($("#ForumRepeat").length > 0) {
            var requestUrl = "/WebControl.fh?Command=GetForumArea";
            var requestDataList = new Array();

            requestDataList.push(new ProviderBaseAjax.RequestData("ID", ProviderBase.GetUrlParameter("ID")));
            requestDataList.push(new ProviderBaseAjax.RequestData("PageSize", "20"));

            ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
                ProviderBaseAjax.AjaxResponse(response, "ForumRepeat");
                $('.forum-border').last().remove();

                SetupForumAreaPagingForm();
            }, requestDataList, "ForumRepeat");
        }
    };

    Forum.GetForumGroup = function () {
        if ($("#ForumRepeat").length > 0) {
            var requestUrl = "/WebControl.fh?Command=GetForumGroup";
            var requestDataList = new Array();

            requestDataList.push(new ProviderBaseAjax.RequestData("ID", ProviderBase.GetUrlParameter("ID")));

            ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
                ProviderBaseAjax.AjaxResponse(response, "ForumRepeat");
                $('.forum-border').last().remove();
            }, requestDataList, "ForumRepeat");
        }
    };

    Forum.GetForumThread = function () {
        if ($("#ForumRepeat").length > 0) {
            var requestUrl = "/WebControl.fh?Command=GetForumThread";
            var requestDataList = new Array();

            requestDataList.push(new ProviderBaseAjax.RequestData("ID", ProviderBase.GetUrlParameter("ID")));
            requestDataList.push(new ProviderBaseAjax.RequestData("PageSize", "10"));

            ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
                ProviderBaseAjax.AjaxResponse(response, "ForumRepeat");
                $('.forum-border').last().remove();
                if ($("#ForumThreadMessage").length > 0) {
                    CKEDITOR.replace("ForumThreadMessage", {
                        customConfig: "/Resource/Javascript/ProviderBase/ProviderBase.CkeditorConfig.js"
                    });

                    SetupNewForumThreadMessageForm();
                }

                SetupForumThreadPagingForm();
            }, requestDataList, "ForumRepeat");
        }
    };

    Forum.GetForumThreadNew = function () {
        if ($("#ForumRepeat").length > 0) {
            var requestUrl = "/WebControl.fh?Command=GetForumThreadNew";
            var requestDataList = new Array();

            requestDataList.push(new ProviderBaseAjax.RequestData("ID", ProviderBase.GetUrlParameter("ID")));

            ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
                ProviderBaseAjax.AjaxResponse(response, "ForumRepeat");
                $('.forum-border').last().remove();
                if ($("#ForumThreadMessage").length > 0) {
                    CKEDITOR.replace("ForumThreadMessage", {
                        customConfig: "/Resource/Javascript/ProviderBase/ProviderBase.CkeditorConfig.js"
                    });

                    SetupNewForumThreadForm();
                }
            }, requestDataList, "ForumRepeat");
        }
    }

    function SetupNewForumThreadMessageForm() {
        if ($("#NewForumThreadMessage").length > 0) {
            $("#NewForumThreadMessage").ajaxForm(options = {
                beforeSerialize: function(){
                    CKEDITOR.instances["ForumThreadMessage"].updateElement();

                    $("#ForumThreadMessage").val(ProviderBase.HtmlEncode($("#ForumThreadMessage").val()));
                },
                beforeSubmit: function () {
                    ProviderBase.ClearMessage("ForumThreadNewMessage");
                },
                success: function (response) {
                    if (response.Status == 1) {
                        if (response.Action.toLowerCase() == "submit") {
                            Forum.GetForumThread();
                        }
                        else if (response.Action.toLowerCase() == "preview") {
                            if (response.Data.length > 0 && response.Data[0].length > 0) {
                                $("#ForumThreadMessagePreview").html(response.Data[0]);
                            } else {
                                alert(response.Message);
                            }
                        }
                    } else {
                        ProviderBase.ShowFailMessage("ForumThreadNewMessage", response.Message);
                    }
                },
                dataType: "json"
            });
        }
    };

    function SetupNewForumThreadForm() {
        if ($("#NewForumThread").length > 0) {
            $("#NewForumThread").ajaxForm(options = {
                beforeSerialize: function () {
                    CKEDITOR.instances["ForumThreadMessage"].updateElement();

                    $("#ForumThreadMessage").val(ProviderBase.HtmlEncode($("#ForumThreadMessage").val()));
                },
                beforeSubmit: function () {
                    ProviderBase.ClearMessage("ForumThreadNewMessage");
                },
                success: function (response) {
                    if (response.Status == 1) {
                        if (response.Action.toLowerCase() == "submit") {
                            if (response.Data.length > 0 && response.Data[0].length > 0) {
                                location.href = "/Forum/Thread?id=" + response.Data[0];
                            } else {
                                alert(response.Message);
                            }
                        }
                        else if (response.Action.toLowerCase() == "preview") {
                            if (response.Data.length > 0 && response.Data[0].length > 0) {
                                $("#ForumThreadMessagePreview").html(response.Data[0]);
                            } else {
                                alert(response.Message);
                            }
                        }
                    } else {
                        ProviderBase.ShowFailMessage("ForumThreadNewMessage", response.Message);
                    }
                },
                dataType: "json"
            });
        }
    };

    function SetupForumAreaPagingForm() {
        if ($("#ForumAreaPaging").length > 0) {
            $("#ForumAreaPaging").ajaxForm(options = {
                success: function (response) {
                    ProviderBaseAjax.AjaxResponse(response, "ForumRepeat");
                    $('.forum-border').last().remove();

                    SetupForumAreaPagingForm();
                },
                dataType: "json"
            });
        }
    }

    function SetupForumThreadPagingForm() {
        if ($("#ForumThreadPaging").length > 0) {
            $("#ForumThreadPaging").ajaxForm(options = {
                success: function (response) {
                    ProviderBaseAjax.AjaxResponse(response, "ForumRepeat");
                    $('.forum-border').last().remove();
                    if ($("#ForumThreadMessage").length > 0) {
                        CKEDITOR.replace("ForumThreadMessage", {
                            customConfig: "/Resource/Javascript/ProviderBase/ProviderBase.CkeditorConfig.js"
                        });

                        SetupNewForumThreadMessageForm();
                    }

                    SetupForumThreadPagingForm();
                },
                dataType: "json"
            });
        }
    }
}(window.Forum = window.Forum || {}, jQuery));
