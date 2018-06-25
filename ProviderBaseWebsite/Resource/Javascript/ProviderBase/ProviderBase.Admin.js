/// <reference path="ProviderBase.js" />
/// <reference path="ProviderBase.Ajax.js" />

$(function () {
    var firstValue = ProviderBase.GetFirstValue();

    switch (firstValue.toLowerCase()) {
        case "": default:
            ProviderBaseAdmin.GetAdminIndex();
            break;

        case "formbuilder":
            ProviderBaseAdmin.GetFormBuilder();
            break;

        case "reportbuilder":
            ProviderBaseADmin.GetReportBuilder();
            break;
    }
});

(function (ProviderBaseAdmin, $, undefined) {
    var c_FormBuilderTemplateItemAreaTypeID = null;
    var c_FormBuilderDesignerExpandAll = false;
    var c_FormBuilderTableDefinitionFieldIDArray = new Array();
    var c_FormBuilderTableDefinitionFieldIDType = 1;

    FormBuilderTemplateItemAreaFieldMode = Object.freeze({ "Unassigned": 0, "Table": 1, "Object": 2, "Custom": 3 });

    ProviderBaseAdmin.FormBuilderTemplateItemAreaFieldMode = function (formBuilderTemplateItemAreaFieldMode) {
        c_FormBuilderTableDefinitionFieldIDType = formBuilderTemplateItemAreaFieldMode;
    };

    ProviderBaseAdmin.GetAdminIndex = function () {
        // Menu
    };

    ProviderBaseAdmin.GetFormBuilder = function () {
        if ($("#FormBuilder").length > 0) {
            var requestUrl = "/WebControl.ah?Command=AdminIndexGet";
            var requestDataList = new Array();

            ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
                ProviderBaseAjax.AjaxResponse(response, "FormBuilder");
            }, requestDataList, "FormBuilder");
        }

        if ($("#FormBuilder-Edit").length > 0) {
            FormBuilderEditItemGet();
            FormBuilderEditItemAreaGet();
            FormBuilderEditDesignerGet();
            FormBuilderEditFieldTableGet();
            FormBuilderEditFieldObjectGet();
            FormBuilderEditFieldCustomGet();
        }
    };

    ProviderBaseAdmin.GetReportBuilder = function () {

    };

    ProviderBaseAdmin.ToggleDesignerItemDrop = function () {
        $(".js-item-drop-toggle").toggle();
    };

    ProviderBaseAdmin.ToggleDesignerItemAreaDrop = function (formBuilderTemplateItemAreaTypeID) {
        c_FormBuilderTemplateItemAreaTypeID = formBuilderTemplateItemAreaTypeID;

        $(".js-item-area-drop-toggle").toggle();
    };

    ProviderBaseAdmin.ToggleDesignerItemAreaFieldDrop = function (tableDefinitionFieldID) {
        var formBuilderTableDefinitionFieldIDArrayIndex = 0;

        formBuilderTableDefinitionFieldIDArrayIndex = $.inArray(tableDefinitionFieldID, c_FormBuilderTableDefinitionFieldIDArray);

        if (formBuilderTableDefinitionFieldIDArrayIndex > -1) {
            c_FormBuilderTableDefinitionFieldIDArray.splice(formBuilderTableDefinitionFieldIDArrayIndex, 1);
        }
        else {
            c_FormBuilderTableDefinitionFieldIDArray.push(tableDefinitionFieldID);
        }

        if (c_FormBuilderTableDefinitionFieldIDArray.length == 0) {
            $(".js-item-area-field-drop-toggle").hide();
        }
        else {
            $(".js-item-area-field-drop-toggle").show();
        }
    };

    ProviderBaseAdmin.ToggleDesignerItem = function (formBuilderTemplateItemID, element) {
        $("#FormBuilder-Edit-Designer").find(`[data-FormBuilderTemplateItemID=${formBuilderTemplateItemID}]`).toggle();

        $(element).toggleClass("rotate90");
        $(element).toggleClass("rotate270");
    };

    ProviderBaseAdmin.ToggleDesignerItemArea = function (formBuilderTemplateItemAreaID, element) {
        $("#FormBuilder-Edit-Designer").find(`[data-FormBuilderTemplateItemAreaID=${formBuilderTemplateItemAreaID}]`).toggle();

        $(element).toggleClass("rotate90");
        $(element).toggleClass("rotate270");
    };

    ProviderBaseAdmin.ShowDesignerAll = function () {
        $(".js-item-toggle").show();
        $(".js-item-display-icon").removeClass("rotate90");
        $(".js-item-display-icon").addClass("rotate270");

        $(".js-item-area-toggle").show();
        $(".js-item-area-display-icon").removeClass("rotate90");
        $(".js-item-area-display-icon").addClass("rotate270");

        c_FormBuilderDesignerExpandAll = true;
    };

    ProviderBaseAdmin.HideDesignerAll = function () {
        $(".js-item-toggle").hide();
        $(".js-item-display-icon").removeClass("rotate270");
        $(".js-item-display-icon").addClass("rotate90");

        $(".js-item-area-toggle").hide();
        $(".js-item-display-icon").removeClass("rotate270");
        $(".js-item-area-display-icon").addClass("rotate90");

        c_FormBuilderDesignerExpandAll = false;
    };

    ProviderBaseAdmin.PickupArea = function (formBuilderTemplateItemAreaElement) {
        c_FormBuilderTemplateItemAreaTypeID = formBuilderTemplateItemAreaElement;

        ProviderBaseADmin.ShowDesignerAreaAll();
    };

    ProviderBaseAdmin.FormBuilderTemplateItemCreate = function (formBuilderTemplateID) {
        var requestUrl = "/WebControl.ah?Command=FormBuilderTemplateItemCreate";
        var requestDataList = new Array();

        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateID", formBuilderTemplateID));

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            if (response.Status == 1) {
                FormBuilderEditDesignerGet();
            } else {
                ProviderBaseAjax.AjaxResponse(response);
            }
        }, requestDataList);
    };

    ProviderBaseAdmin.FormBuilderTemplateItemDelete = function (formBuilderTemplateItemID) {
        var requestUrl = "/WebControl.ah?Command=FormBuilderTemplateItemDelete";
        var requestDataList = new Array();

        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemID", formBuilderTemplateItemID));

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            if (response.Status == 1) {
                FormBuilderEditDesignerGet();
            } else {
                ProviderBaseAjax.AjaxResponse(response);
            }
        }, requestDataList);
    };

    ProviderBaseAdmin.FormBuilderTemplateItemMoveUp = function (formBuilderTemplateItemID) {
        var requestUrl = "/WebControl.ah?Command=FormBuilderTemplateItemMoveUp";
        var requestDataList = new Array();

        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemID", formBuilderTemplateItemID));

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            if (response.Status == 1) {
                FormBuilderEditDesignerGet();
            } else {
                ProviderBaseAjax.AjaxResponse(response);
            }
        }, requestDataList);
    };

    ProviderBaseAdmin.FormBuilderTemplateItemMoveDown = function (formBuilderTemplateItemID) {
        var requestUrl = "/WebControl.ah?Command=FormBuilderTemplateItemMoveDown";
        var requestDataList = new Array();

        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemID", formBuilderTemplateItemID));

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            if (response.Status == 1) {
                FormBuilderEditDesignerGet();
            } else {
                ProviderBaseAjax.AjaxResponse(response);
            }
        }, requestDataList);
    };

    ProviderBaseAdmin.FormBuilderTemplateItemEdit = function (formBuilderTemplateItemID) {
        var requestUrl = "/WebControl.ah?Command=FormBuilderTemplateItemEdit";
        var requestDataList = new Array();

        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemID", formBuilderTemplateItemID));

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            if (response.Status == 1) {
                if (response.Data.length > 0) {
                    ProviderBase.ShowModal(response.Data[0], "Form Builder Template Item Edit", FormBuilderEditDesignerGet);
                } else {
                    ProviderBase.ShowModalAlert("Form Builder Template Item Edit - No Data", "Error");
                }
            } else {
                ProviderBaseAjax.AjaxResponse(response);
            }
        }, requestDataList);
    };

    ProviderBaseAdmin.FormBuilderTemplateItemAreaCreate = function (formBuilderTemplateItemID, formBuilderTemplateItemAreaID) {
        var requestUrl = "/WebControl.ah?Command=FormBuilderTemplateItemAreaCreate";
        var requestDataList = new Array();

        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemID", formBuilderTemplateItemID));
        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemAreaID", formBuilderTemplateItemAreaID));
        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemAreaTypeID", c_FormBuilderTemplateItemAreaTypeID));

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            if (response.Status == 1) {
                FormBuilderEditDesignerGet();
            } else {
                ProviderBaseAjax.AjaxResponse(response);
            }
        }, requestDataList);
    };

    ProviderBaseAdmin.FormBuilderTemplateItemAreaDelete = function (formBuilderTemplateItemAreaID) {
        var requestUrl = "/WebControl.ah?Command=FormBuilderTemplateItemAreaDelete";
        var requestDataList = new Array();

        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemAreaID", formBuilderTemplateItemAreaID));

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            if (response.Status == 1) {
                FormBuilderEditDesignerGet();
            } else {
                ProviderBaseAjax.AjaxResponse(response);
            }
        }, requestDataList);
    };

    ProviderBaseAdmin.FormBuilderTemplateItemAreaMoveUp = function (formBuilderTemplateItemAreaID) {
        var requestUrl = "/WebControl.ah?Command=FormBuilderTemplateItemAreaMoveUp";
        var requestDataList = new Array();

        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemAreaID", formBuilderTemplateItemAreaID));

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            if (response.Status == 1) {
                FormBuilderEditDesignerGet();
            } else {
                ProviderBaseAjax.AjaxResponse(response);
            }
        }, requestDataList);
    };

    ProviderBaseAdmin.FormBuilderTemplateItemAreaMoveDown = function (formBuilderTemplateItemAreaID) {
        var requestUrl = "/WebControl.ah?Command=FormBuilderTemplateItemAreaMoveDown";
        var requestDataList = new Array();

        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemAreaID", formBuilderTemplateItemAreaID));

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            if (response.Status == 1) {
                FormBuilderEditDesignerGet();
            } else {
                ProviderBaseAjax.AjaxResponse(response);
            }
        }, requestDataList);
    };

    ProviderBaseAdmin.FormBuilderTemplateItemAreaEdit = function (formBuilderTemplateItemAreaID) {
        var requestUrl = "/WebControl.ah?Command=FormBuilderTemplateItemAreaEdit";
        var requestDataList = new Array();

        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemAreaID", formBuilderTemplateItemAreaID));

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            if (response.Status == 1) {
                if (response.Data.length > 0) {
                    ProviderBase.ShowModal(response.Data[0], "Form Builder Template Item Area Edit", FormBuilderEditDesignerGet);
                } else {
                    ProviderBase.ShowModalAlert("Form Builder Template Item Area Edit - No Data", "Error");
                }
            } else {
                ProviderBaseAjax.AjaxResponse(response);
            }
        }, requestDataList);
    };

    ProviderBaseAdmin.FormBuilderTemplateItemAreaFieldCreate = function (formBuilderTemplateItemAreaID, formBuilderTemplateItemAreaFieldID) {
        var requestUrl = "/WebControl.ah?Command=FormBuilderTemplateItemAreaFieldCreate";
        var requestDataList = new Array();

        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemAreaID", formBuilderTemplateItemAreaID));
        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemAreaFieldID", formBuilderTemplateItemAreaFieldID));
        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTableDefinitionFieldID", c_FormBuilderTableDefinitionFieldIDArray.join(", ")));
        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemAreaFieldMode", c_FormBuilderTableDefinitionFieldIDType));

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            if (response.Status == 1) {
                FormBuilderEditDesignerGet();

                $("#FormBuilder-Edit-Field-Table").find("input:checkbox").prop("checked", false);
            } else {
                ProviderBaseAjax.AjaxResponse(response);
            }
        }, requestDataList);
    };

    ProviderBaseAdmin.FormBuilderTemplateItemAreaFieldDelete = function (formBuilderTemplateItemAreaFieldID) {
        var requestUrl = "/WebControl.ah?Command=FormBuilderTemplateItemAreaFieldDelete";
        var requestDataList = new Array();

        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemAreaID", formBuilderTemplateItemAreaID));

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            if (response.Status == 1) {
                FormBuilderEditDesignerGet();
            } else {
                ProviderBaseAjax.AjaxResponse(response);
            }
        }, requestDataList);
    };

    ProviderBaseAdmin.FormBuilderTemplateItemAreaFieldMoveUp = function (formBuilderTemplateItemAreaFieldID) {
        var requestUrl = "/WebControl.ah?Command=FormBuilderTemplateItemAreaFieldMoveUp";
        var requestDataList = new Array();

        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemAreaFieldID", formBuilderTemplateItemAreaFieldID));

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            if (response.Status == 1) {
                FormBuilderEditDesignerGet();
            } else {
                ProviderBaseAjax.AjaxResponse(response);
            }
        }, requestDataList);
    };

    ProviderBaseAdmin.FormBuilderTemplateItemAreaFieldMoveDown = function (formBuilderTemplateItemAreaFieldID) {
        var requestUrl = "/WebControl.ah?Command=FormBuilderTemplateItemAreaFieldMoveDown";
        var requestDataList = new Array();

        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemAreaFieldID", formBuilderTemplateItemAreaFieldID));

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            if (response.Status == 1) {
                FormBuilderEditDesignerGet();
            } else {
                ProviderBaseAjax.AjaxResponse(response);
            }
        }, requestDataList);
    };

    ProviderBaseAdmin.FormBuilderTemplateItemAreaFieldEdit = function (formBuilderTemplateItemAreaFieldID) {
        var requestUrl = "/WebControl.ah?Command=FormBuilderTemplateItemAreaFieldEdit";
        var requestDataList = new Array();

        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilderTemplateItemAreaFieldID", formBuilderTemplateItemAreaFieldID));

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            if (response.Status == 1) {
                if (response.Data.length > 0) {
                    ProviderBase.ShowModal(response.Data[0], "Form Builder Template Item Area Field Edit", FormBuilderEditDesignerGet);
                } else {
                    ProviderBase.ShowModalAlert("Form Builder Template Item Area Field Edit - No Data", "Error");
                }
            } else {
                ProviderBaseAjax.AjaxResponse(response);
            }
        }, requestDataList);
    };

    ProviderBaseAdmin.TableDefinitionSelect = function (tableDefinitionID) {
        var tableDefinitionIconElement = null;

        tableDefinitionIconElement = $(`#FormBuilder-Edit-TableDefinition-Icon-${tableDefinitionID}`);

        if ($(tableDefinitionIconElement).hasClass("icon-open_folder-w-20")) {
            $(tableDefinitionIconElement).removeClass("icon-open_folder-w-20");
            $(tableDefinitionIconElement).addClass("icon-close_folder-w-20");

            $(`#FormBuilder-Edit-TableDefinition-${tableDefinitionID}`).show();
        }
        else {
            $(tableDefinitionIconElement).removeClass("icon-close_folder-w-20");
            $(tableDefinitionIconElement).addClass("icon-open_folder-w-20");

            $(`#FormBuilder-Edit-TableDefinition-${tableDefinitionID}`).hide();
        }
    };

    ProviderBaseAdmin.CustomFieldSelect = function (CustomFieldID) {
        var tableDefinitionIconElement = null;

        tableDefinitionIconElement = $(`#FormBuilder-Edit-CustomField-Icon-${CustomFieldID}`);

        if ($(tableDefinitionIconElement).hasClass("icon-open_folder-w-20")) {
            $(tableDefinitionIconElement).removeClass("icon-open_folder-w-20");
            $(tableDefinitionIconElement).addClass("icon-close_folder-w-20");

            $(`#FormBuilder-Edit-CustomField-${CustomFieldID}`).show();
        }
        else {
            $(tableDefinitionIconElement).removeClass("icon-close_folder-w-20");
            $(tableDefinitionIconElement).addClass("icon-open_folder-w-20");

            $(`#FormBuilder-Edit-CustomField-${CustomFieldID}`).hide();
        }
    };

    function FormBuilderEditItemGet() {
        var requestUrl = "/WebControl.ah?Command=FormBuilderEditItemGet";
        var requestDataList = new Array();

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            ProviderBaseAjax.AjaxResponse(response, "FormBuilder-Edit-Item");
        }, requestDataList, "FormBuilder-Edit-Item");
    };

    function FormBuilderEditItemAreaGet() {
        var requestUrl = "/WebControl.ah?Command=FormBuilderEditItemAreaGet";
        var requestDataList = new Array();

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            ProviderBaseAjax.AjaxResponse(response, "FormBuilder-Edit-Item-Area");
        }, requestDataList, "FormBuilder-Edit-Item-Area");
    };

    function FormBuilderEditDesignerGet() {
        var ID = ProviderBase.GetUrlParameter("ID");
        var requestUrl = "/WebControl.ah?Command=FormBuilderEditDesignerGet";
        var requestDataList = new Array();

        requestDataList.push(new ProviderBaseAjax.RequestData("FormBuilder_GUID", ID));

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            ProviderBaseAjax.AjaxResponse(response, "FormBuilder-Edit-Designer");

            if (c_FormBuilderDesignerExpandAll) {
                ProviderBaseAdmin.ShowDesignerAll();
            }
        }, requestDataList, "FormBuilder-Edit-Designer");
    };

    function FormBuilderEditFieldTableGet() {
        var requestUrl = "/WebControl.ah?Command=FormBuilderEditFieldTableGet";
        var requestDataList = new Array();

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            ProviderBaseAjax.AjaxResponse(response, "FormBuilder-Edit-Field-Table", "FormBuilder-Edit-Field-Table-Paging");
        }, requestDataList, "FormBuilder-Edit-Field-Table");
    };

    function FormBuilderEditFieldObjectGet() {
        var requestUrl = "/WebControl.ah?Command=FormBuilderEditFieldObjectGet";
        var requestDataList = new Array();

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            ProviderBaseAjax.AjaxResponse(response, "FormBuilder-Edit-Field-Object", "FormBuilder-Edit-Field-Paging");
        }, requestDataList, "FormBuilder-Edit-Field-Object");
    };

    function FormBuilderEditFieldCustomGet() {
        var requestUrl = "/WebControl.ah?Command=FormBuilderEditFieldCustomGet";
        var requestDataList = new Array();

        ProviderBaseAjax.AjaxRequest(requestUrl, function (response) {
            ProviderBaseAjax.AjaxResponse(response, "FormBuilder-Edit-Field-Custom", "FormBuilder-Edit-Field-Paging");
        }, requestDataList, "FormBuilder-Edit-Field-Custom");
    };
}(window.ProviderBaseAdmin = window.ProviderBaseAdmin || {}, jQuery));
