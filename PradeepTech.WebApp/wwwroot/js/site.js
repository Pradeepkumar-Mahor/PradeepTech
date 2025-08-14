//const { min } = require("../lib/moment.js/moment-with-locales");

$(document).ready(function () {
    if (typeof initToastrMessage == 'function') {
        initToastrMessage();
    }
    else {
        console.log("InitToastr NotFound");
    }
    loadScript();
});
function cardTileTooltips() {
    $(".card.card-tpl").each(function () {
        var elementh5 = $(this).find(".card-header").find("h5");
        if (isEllipsisActive(elementh5[0])) {
            var tooltip_text = $(this).find(".card-header").find("h5").html();
            $(this).attr("data-title", tooltip_text);
        }
        else {
            $(this).attr("data-title", null);
        }
    });
}
function isEllipsisActive(element) {
    if (element.clientWidth < element.scrollWidth) {
        var style = element.currentStyle || window.getComputedStyle(element);
        return style.textOverflow === 'ellipsis'
    }
    return false;
}
function loadScript() {
    openModalListner();
    checkFilteredMarker();
    binaryDownloadListner();
    batchProcessListner();
    $('#modalcontainer, #modalreadonlycontainer').off('shown.bs.modal').on('shown.bs.modal', function () {
        var form = $(this).find("form");
        $.validator.unobtrusive.parse(form);
    });
    $('.nav-link').off('click').on('click', function () {
        loadView(this);
    });
}
function openModalListner() {
    $('*[data-jqueryselector$=openmodal]').off('click').on('click', function () {
        var modalcontainer = $(this).data('modalcontainer');
        modalcontainer = $('#' + modalcontainer);
        var modalcontent = modalcontainer.find('#modal-content');
        var modaldismiss = $(this).data('modaldismiss');
        if (modaldismiss == 'undefined')
            modaldismiss = 'false';
        var modalpopupwidth = $(this).data('modalpopupwidth');
        if (modalpopupwidth != null) {
            modalcontainer.removeClass("rightw25");
            modalcontainer.removeClass("rightw35");
            modalcontainer.removeClass("rightw50");
            modalcontainer.removeClass("rightw75");
            modalcontainer.addClass(modalpopupwidth);
        }
        var cache = $(this).data('cache');
        if (cache != true) {
            cache = false;
        }
        var modaltitle = $(this).data('modaltitle');
        var modalheader = $(this).data('modalheader');
        modalcontainer.find('#modaltitle').text(modaltitle);
        modalcontainer.find('#modalheader').text(modalheader);
        var staticHTMLSourceDivId = $(this).data('statichtmlsourcedivid');
        if (staticHTMLSourceDivId) {
            staticHTMLSourceDivId = '#' + staticHTMLSourceDivId;
            var staticHTML = $(staticHTMLSourceDivId).html();
            modalcontent.html(staticHTML);
            modalcontainer.modal();
            return;
        }
        var jsUrl = $(this).data('url');
        var callBack = $(this).data('callback');
        var dataParams = $(this).data();
        delete dataParams["modalheader"];
        delete dataParams["modaltitle"];
        delete dataParams["url"];
        delete dataParams["statichtmlsourcedivid"];
        delete dataParams["cache"];
        delete dataParams["modalcontainer"];
        delete dataParams["jqueryselector"];
        delete dataParams["modalpopupwidth"];
        delete dataParams["callBack"];
        $.ajax({
            url: jsUrl,
            type: "GET",
            data: dataParams,
            success: function (data) {
                modalcontent.html(data);
                if (modaltitle !== null && modaltitle !== "undefined")
                    modalcontainer.find('#modaltitle').text(modaltitle);
                if (modalheader !== null && modalheader !== "undefined")
                    modalcontainer.find('#modalheader').text(modalheader);
                if (modalcontainer[0].id == "modalcontainer" && modaldismiss != "auto")
                    modalcontainer.modal({ backdrop: 'static', keyboard: false });
                else
                    modalcontainer.modal();
                if (typeof localScript == 'function') {
                    localScript();
                }
                if (callBack) {
                    window[callBack]();
                }
            },
            error: function (result) {
                errorText = result.responseText.indexOf("divErrorMessage") == -1 ? result.responseText : ($(result.responseText).find("div[id*=divErrorMessage]").text()).replace("text-danger", "text-white");
                notify("error", errorText, 'Error');
            }
        });
    });
}
function binaryDownloadListner() {
    $('*[data-jqueryselector$=binarydownload]').off('click').on('click', function () {
        let anchorBtn = this;
        var cache = $(this).data('cache');
        if (cache != true) {
            cache = false;
        }
        var jsUrl = $(this).data('url');
        var callBack = $(this).data('callback');
        var requestVerificationToken = $('input:hidden[name="__RequestVerificationToken"]').val();
        var dataParams = $(this).data();
        delete dataParams["url"];
        delete dataParams["cache"];
        delete dataParams["jqueryselector"];
        delete dataParams["antiforgerytoken"];
        delete dataParams["callBack"];
        var xlbtnclasses = "";
        $.ajax({
            url: jsUrl,
            type: "GET",
            data: dataParams,
            beforeSend: function (xhr) {
                if (requestVerificationToken)
                    xhr.setRequestHeader('RequestVerificationToken', requestVerificationToken);
                $(anchorBtn).prop("disabled", true);
                $(anchorBtn).addClass('not-allowed');
                if ($(anchorBtn).find('.tileLoader').length > 0 || $(anchorBtn).find('.loaderElement').length > 0) {
                    $($(anchorBtn).find('.tileLoader')).removeClass("hidden");
                    $($(anchorBtn).find('.loaderElement')).removeClass("hidden");
                }
                else {
                    xlbtnclasses = $(anchorBtn).find('i').attr('class');
                    $(anchorBtn).find('i').attr('class', 'spinner-border spinner-border-sm');
                }
            },
            success: function (data, status, xhr) {
                if (data.messageType == "OK") {
                    var blob = convertToBlob(data.messageFileContent.fileContents, data.messageFileContent.contentType);
                    var link = document.createElement('a');
                    link.href = window.URL.createObjectURL(blob);
                    link.download = data.messageFileContent.fileDownloadName;
                    link.click();
                    notify("success", data.messageText, 'Success');
                }
                if (data.messageType == "KO") {
                    notify("error", data.messageText, 'Error');
                }
            },
            error: function (result) {
                errorText = result.responseText.indexOf("divErrorMessage") == -1 ? result.responseText : ($(result.responseText).find("div[id*=divErrorMessage]").text()).replace("text-danger", "text-white");
                notify("error", errorText, 'Error');
            },
            complete: function () {
                $(anchorBtn).prop("disabled", false);
                $(anchorBtn).removeClass('not-allowed');
                if ($(anchorBtn).find('.tileLoader').length > 0 || $(anchorBtn).find('.loaderElement').length > 0) {
                    $($(anchorBtn).find('.tileLoader')).addClass("hidden");
                    $($(anchorBtn).find('.loaderElement')).addClass("hidden");
                }
                else {
                    $(anchorBtn).find('i').attr('class', xlbtnclasses);
                    $(anchorBtn).find('i').removeClass('spinner-border spinner-border-sm');
                }
            }
        });
    });
}
function batchProcessListner() {
    $('*[data-jqueryselector$=batchprocessgenerate]').off('click').on('click', function () {
        let anchorBtn = this;
        console.log(anchorBtn);
        var cache = $(this).data('cache');
        if (cache != true) {
            cache = false;
        }
        var jsUrl = $(this).data('url');
        var requestVerificationToken = $('input:hidden[name="__RequestVerificationToken"]').val();
        var dataParams = $(this).data();
        delete dataParams["url"];
        delete dataParams["cache"];
        delete dataParams["jqueryselector"];
        delete dataParams["antiforgerytoken"];
        delete dataParams["callBack"];
        delete dataParams["actiontype"];
        $.ajax({
            url: jsUrl,
            type: "GET",
            data: dataParams,
            beforeSend: function (xhr) {
                if (requestVerificationToken)
                    xhr.setRequestHeader('RequestVerificationToken', requestVerificationToken);
                $(anchorBtn).prop("disabled", true);
            },
            success: function (data, status, xhr) {
                if (data.messageType == "OK") {
                    notify("success", data.messageText, 'Success');
                    if (typeof setBatchProcessStatus == 'function')
                        setBatchProcessStatus();
                }
                if (data.messageType == "KO") {
                    notify("error", data.messageText, 'Error');
                }
            },
            error: function (result) {
                errorText = result.responseText.indexOf("divErrorMessage") == -1 ? result.responseText : ($(result.responseText).find("div[id*=divErrorMessage]").text()).replace("text-danger", "text-white");
                notify("error", errorText, 'Error');
            },
            complete: function () {
                $(anchorBtn).prop("disabled", false);
            }
        });
    });
}
function showDeleteConfirmationPopup(event, obj) {
    event.preventDefault();
    event.stopPropagation();
    var data = $(obj).data();
    var strData = JSON.stringify(data);
    showDeleteConfirmation(strData);
    return false;
}
function showDeleteConfirmation(jsonString) {
    var dataParams = JSON.parse(jsonString);
    var sTitle = dataParams["modaltitle"];
    delete dataParams["modaltitle"];
    var sConfirmationText = dataParams["confirmationtext"];
    delete dataParams["confirmationtext"];
    var sConfirmationType = dataParams["confirmationtype"]
    delete dataParams["confirmationtype"];
    var sConfirmButtonClass = "btn-danger btn-sm";
    if (dataParams["confirmbuttonclass"] != undefined) {
        sConfirmButtonClass = dataParams["confirmbuttonclass"]
    }
    delete dataParams["confirmbuttonclass"];
    var sConfirmButtonText = dataParams["confirmbuttontext"]
    delete dataParams["confirmbuttontext"];
    var sUrl = dataParams["url"]
    delete dataParams["url"];
    delete dataParams["jqueryselector"];
    var sRedirectUrl = dataParams["redirecturl"];
    delete dataParams["redirecturl"];
    var sPostSaveReloadDataTable = dataParams["postsavereloaddatatable"];
    delete dataParams["postsavereloaddatatable"];
    var sPostDeleteReLoadDataTables = dataParams["postdeletereloaddatatables"];
    delete dataParams["postdeletereloaddatatables"];
    var sPostLockUnlockReLoadOSCMhrsSummaryDataTables = dataParams["postlockunlockreloadoscmhrssummarydatatables"];
    delete dataParams["postlockunlockreloadoscmhrssummarydatatables"];
    var sCallback = dataParams["callback"];
    delete dataParams["callback"];
    var bDoRedirect = sRedirectUrl != undefined;
    if (bDoRedirect)
        bDoRedirect = sRedirectUrl.length > 0;
    swal({
        title: sTitle,
        text: sConfirmationText,
        type: sConfirmationType, // warning / error / success /info
        showCancelButton: true,
        confirmButtonClass: sConfirmButtonClass, // "btn-danger btn-sm"/"btn-info btn-sm"/"btn-success btn-sm"
        cancelButtonClass: "btn-sm",
        confirmButtonText: sConfirmButtonText,
        closeOnconfirm: true
    }, function () {
        $.ajax({
            type: "POST",
            url: sUrl,
            data: dataParams,
            beforeSend: function () {
                showLoader();
            },
            success: function (data) {
                if (data.success || data.messageType == "OK" || data.messageType == "OO") {
                    if (bDoRedirect)
                        window.location = sRedirectUrl;
                    else if (sPostSaveReloadDataTable == "OK") {
                        if (typeof PostSaveReLoadDataTable == 'function') {
                            PostSaveReLoadDataTable(data);
                        }
                    }
                    else if (sPostDeleteReLoadDataTables == "OK") {
                        if (typeof PostDeleteReLoadDataTables == 'function') {
                            PostDeleteReLoadDataTables(data);
                        }
                    }
                    else if (sPostLockUnlockReLoadOSCMhrsSummaryDataTables == "OK") {
                        if (typeof PostLockUnlockReLoadOSCMhrsSummaryDataTables == 'function') {
                            PostLockUnlockReLoadOSCMhrsSummaryDataTables(data);
                        }
                    }
                    else if (sCallback) {
                        {
                            window[sCallback](data);
                        }
                    }
                    else {
                        notify("success", data.message, "Success")
                        hideLoader();
                        localScript();
                    }
                } else {
                    hideLoader();
                    notify("error", data.message, "Error")
                }
            },
            error: function (xhr) {
                hideLoader();
                showError(xhr);
            }
        });
    });
}
function showLoader() {
    $(".load-bar").removeClass('d-none');
}
function showModalLoader() {
    $(".load-bar-modal").removeClass('d-none');
}
function hideLoader() {
    $(".load-bar").addClass('d-none');
}
function hideModalLoader() {
    $(".load-bar-modal").addClass('d-none');
}
function initSelect2() {
    $('.Select2').select2({
        placeholder: "Select an option",
        minimumResultsForSearch: -1,
        placeholder: function () {
            $(this).data('placeholder');
        },
        allowClear: true,
        width: "100%"
    });
    $('.chosen-select').select2({
        placeholder: "Select an option",
        minimumResultsForSearch: -1,
        placeholder: function () {
            $(this).data('placeholder');
        },
        allowClear: true,
        width: "100%"
    });
    $('.chosen-select-multiple').select2({
        placeholder: "Select an option",
        allowClear: true,
        width: "100%"
    });
}

function toggleMenuState() {
    /*
    var r = document.querySelector(':root');
    var oObjects = getComputedStyle(r);
    var navBarWidth = oObjects.getPropertyValue('--navbar-width');
    if (navBarWidth == '264px')
        r.style.setProperty('--navbar-width', '80px');
    else {
        r.style.setProperty('--navbar-width', '264px');
    }
      */
    var r = document.querySelector(':root');
    var oObjects = getComputedStyle(r);
    var navBarWidth = oObjects.getPropertyValue('--navbar-width');
    var navBarOpenWidth = oObjects.getPropertyValue('--navbar-open-width');
    var navBarCloseWidth = oObjects.getPropertyValue('--navbar-close-width');
    var navBarHeaderHeight = oObjects.getPropertyValue('--app-header-height');
    var deviceRatio = window.devicePixelRatio;
    let aryDeviceRatios = [1, 1.25, 1.5];
    console.log(navBarWidth);
    console.log(navBarOpenWidth);
    console.log(navBarCloseWidth);
    console.log(navBarHeaderHeight);
    if (navBarWidth == navBarOpenWidth) {
        r.style.setProperty('--navbar-width', navBarCloseWidth);
        console.log("Step1")
    } else {
        r.style.setProperty('--navbar-width', navBarOpenWidth);
        console.log("Step2")
    }
}
function ofbfileDownload(event, object) {
    event.preventDefault();
    event.stopPropagation();
    var data = $(object).data();
    var strData = JSON.stringify(data);
    FileDownload(strData);
    return false;
}
function FileDownload(jsonString) {
    var dataParams = JSON.parse(jsonString);
    var sUrl = dataParams["url"]
    delete dataParams["url"];
    var sClientFileName = dataParams["clientfilename"]
    delete dataParams["clientfilename"];
    var sKeyId = dataParams["keyid"]
    delete dataParams["keyid"];
    $.ajax({
        url: sUrl,
        type: "GET",
        cache: false,
        data: {
            keyid: sKeyId
        },
        xhr: function () {
            var xhr = new XMLHttpRequest();
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 2) {
                    if (xhr.status == 200) {
                        xhr.responseType = "blob";
                    }
                }
            };
            return xhr;
        },
        beforeSend: function () {
            showLoader();
        },
        success: function (data) {
            var link = document.createElement('a');
            var url = window.URL.createObjectURL(data);
            link.href = window.URL.createObjectURL(data);
            link.download = sClientFileName;
            link.click();
            link.remove();
            window.URL.revokeObjectURL(url);
            hideLoader();
            toastr.success("File downloaded successfully.");
        },
        error: function (xhr) {
            showError(xhr);
            hideLoader();
        }
    });
}
function onBegin(btn) {
    if (btn == null)
        btn = $('#btnconfirm');
    btnDisable(btn);
    showModalLoader();
    $("#modal-loader").show();
}
function onComplete(btn) {
    if (btn == null)
        btn = $('#btnconfirm');
    btnEnable(btn);
    hideModalLoader();
    $("#modal-loader").hide();
    loadScript();
}
function onSuccess() {
    $('#modalcontainer').modal('hide');
}
function onSuccessTab(tabname) {
    console.log("onSuccessTab(tabname)");
    $('#modalcontainer').modal('hide');
    $('#tabs-' + tabname + '-tab').click();
}
function onSuccessRedirect(redirecturl) {
    $('#modalcontainer').modal('hide');
    window.location = redirecturl;
}
/*
function reDirectURL(reDirectUrl) {
    if (reDirectUrl == "/HRMasters/CostCenter/DeactivateCostCenter") {
        window.location = "/HRMasters/CostCenter";
    }
}
*/
function onError(xhr) {
    errorText = xhr.responseText.indexOf("divErrorMessage") == -1 ? xhr.responseText : ($(xhr.responseText).find("div[id*=divErrorMessage]").text()).replace("text-danger", "text-white");
    notify("error", errorText, 'Error');
}
function showError(xhr) {
    console.log(xhr);
    let errorText = "";
    console.log('typeof-' + typeof xhr);
    if (typeof xhr === "string") {
        errorText = xhr;
    }
    else {
        console.log(xhr.responseText);
        if (xhr.responseText != undefined) {
            errorText = xhr.responseText.indexOf("divErrorMessage") == -1 ? xhr.responseText : ($(xhr.responseText).find("div[id*=divErrorMessage]").text()).replace("text-danger", "text-white");
        } else if (xhr.messageText !== undefined) {
            errorText = xhr.messageText;
        } else
            errorText = "Undefined error. Please contact administrator.";
    }
    notify("error", errorText, 'Error');
}
function onDataTableReloadSuccess() {
    location.reload();
}
function btnDisable(btn) {
    btn.prop("disabled", true);
    btn.html(btn.data('loading-text'));
}
function btnEnable(btn) {
    btn.prop("disabled", false);
    btn.html(btn.data('text'));
}
function loadView(element) {
    var divid = $(element).data('divid');
    if (divid == null)
        return;
    var id = $(element).data('id');
    var guidid = $(element).data('guidid');
    var parameter = $(element).data('parameter');
    var headertitle = $(element).data('headertitle');
    var containerid = $(element).data('divcontainerid');
    var div = $('#' + divid);
    var area = $(element).data('area');
    var controller = $(element).data('controller');
    var action = $(element).data('action');
    var partialviewname = $(element).data('partialviewname');
    var divcontainer = $('#tabs-' + containerid);
    var callback = $(element).data('callback');
    var jsUrl = $(element).data('url');
    var cache = $(element).data('cache');
    if (cache != true) {
        cache = false;
    }
    $.ajax({
        url: jsUrl,
        dataType: 'html',
        type: 'GET',
        cache: cache,
        data: {
            'id': id,
            'guidid': guidid,
            'headertitle': headertitle,
            'parameter': parameter,
            'partialviewname': partialviewname
        },
        beforeSend: function () {
            showLoader();
        },
        success: function (data) {
            $(divcontainer).find('div[id^="pw"]').each(function () {
                $(this).html('');
            }); div.html(data); loadScript();
            if (typeof localScript == 'function') {
                localScript();
                if (typeof loadTimesheetLockedDataTable == 'function' && action == 'EmployeeLockedIndex') {
                    loadTimesheetLockedDataTable();
                }
                if (typeof loadTimesheetApprovedDataTable == 'function' && action == 'EmployeeApprovedIndex') {
                    loadTimesheetApprovedDataTable();
                }
                if (typeof loadTimesheetPostedDataTable == 'function' && action == 'EmployeePostedIndex') {
                    loadTimesheetPostedDataTable();
                }
                if (typeof loadTimesheetNotfilledDataTable == 'function' && action == 'EmployeeNotfilledIndex') {
                    loadTimesheetNotfilledDataTable();
                }
            } if (callback) {
                var aryCallBack = (callback + ";").split(";");
                for (i = 0; i < aryCallBack.length; i++) {
                    if (aryCallBack[i]) {
                        var callBackFn = aryCallBack[i];
                        if (callBackFn.indexOf("(") > -1) {
                            callBackFn = callBackFn.split("(")[0];
                        }
                        var fn = window[callBackFn];
                        if (typeof fn === "function") fn.apply(null)
                        else
                            console.log("'" + callBackFn + "' - Callback function not found.")
                                ;
                    }
                }
            } hideLoader();
        },
        complete: function () {
            hideLoader();
        },
        error: function (xhr) {
            hideLoader();
            showError(xhr);
        }
    });
}
function resetFilter(event, element) {
    event.preventDefault();
    var actionId = $(element).data('actionid');
    var url = $(element).data('url');
    $.ajax({
        url: url,
        type: 'GET',
        cache: false,
        data: {
            actionId: actionId
        },
    }).done(function (result) {
        window.location.reload();
        /*
        if (typeof resetFilterDatePicker === "function") {
            resetFilterDatePicker();
        }
        $("input[name^='FilterDataModel.'], input[name^='FilterDataModel_']").each(function (index, item) {
            $(item).val('');
        });
        checkFilteredMarker();
        $('#buttonSearch').click();*/
    });
}
function minutesToHours(minutes) {
    let bIsNegative = minutes < 0;
    let positiveMinutes = bIsNegative ? minutes * -1 : minutes;
    let HH = Math.floor(positiveMinutes / 60);
    let MI = Math.floor(positiveMinutes % 60);
    let retVal = bIsNegative ? "-" : "" + ((HH < 10) ? ("0" + HH) : HH) + ":" + ((MI < 10) ? ("0" + MI) : MI);
    return retVal;
}
function checkFilteredMarker() {
    var result = false;
    $("input[name^='FilterDataModel.'], input[name^='FilterDataModel_'], input[name^='GenericSearch']").each(function (index, item) {
        if ($(item).val().length > 0) {
            result = true;
        }
    });
    if (result == true) {
        $('.filteredmarker-border').removeClass('border-white');
        $('.filteredmarker-visibility').show();
        $('#isFiltered').addClass('text-c-red');
    } else {
        $('.filteredmarker-border').addClass('border-white');
        $('.filteredmarker-visibility').hide();
        $('#isFiltered').removeClass('text-c-red');
    }
}
function GetProcessQueueList(processUrl) {
    $.ajax({
        url: processUrl,
        type: 'GET',
        beforeSend: function () {
            showLoader();
        },
        success: function (data) {
            $(data).each(function () {
                ProcessQueueListProcessShowHide(this);
            });
            hideLoader();
        },
        error: function (result) {
            hideLoader();
            notify($.i18n('Error'), result.responseText, 'danger');
        }
    });
}
function ProcessQueueListProcessShowHide(thisData) {
    if ($('[data-processitemid^=' + thisData.processItemId + ']').length == 0)
        return;
    $('[data-processitemid^=' + thisData.processItemId + ']').each(function () {
        $(this).hide();
    });
    if (thisData.status == 0 || thisData.status == 1) {
        $('[data-processitemid^=' + thisData.processItemId + '][data-actiontype^=showprogress]').show();
    }
    else if (thisData.status == 2) {
        $('[data-processitemid^=' + thisData.processItemId + '][data-actiontype^=download]').show();
        $('[data-processitemid^=' + thisData.processItemId + '][data-actiontype^=doprocess]').show();
    }
    else if (thisData.status == -1) {
        $('[data-processitemid^=' + thisData.processItemId + '][data-actiontype^=doprocess]').show();
        $('[data-processitemid^=' + thisData.processItemId + '][data-actiontype^=showerror]').show();
        $('[data-processitemid^=' + thisData.processItemId + '][data-actiontype^=showerror]').attr("title", thisData.processLog);
    }
}
function functionPpenModalListner(obj) {
    var modalcontainer = $(obj).data('modalcontainer');
    modalcontainer = $('#' + modalcontainer);
    var modalcontent = modalcontainer.find('#modal-content');
    var modaldismiss = $(obj).data('modaldismiss');
    if (modaldismiss == 'undefined')
        modaldismiss = 'false';
    var modalpopupwidth = $(obj).data('modalpopupwidth');
    if (modalpopupwidth != null) {
        modalcontainer.removeClass("rightw25");
        modalcontainer.removeClass("rightw35");
        modalcontainer.removeClass("rightw50");
        modalcontainer.removeClass("rightw75");
        modalcontainer.addClass(modalpopupwidth);
    }
    var cache = $(obj).data('cache');
    if (cache != true) {
        cache = false;
    }
    var modaltitle = $(obj).data('modaltitle');
    var modalheader = $(obj).data('modalheader');
    modalcontainer.find('#modaltitle').text(modaltitle);
    modalcontainer.find('#modalheader').text(modalheader);
    var staticHTMLSourceDivId = $(obj).data('statichtmlsourcedivid');
    if (staticHTMLSourceDivId) {
        staticHTMLSourceDivId = '#' + staticHTMLSourceDivId;
        var staticHTML = $(staticHTMLSourceDivId).html();
        modalcontent.html(staticHTML);
        modalcontainer.modal();
        return;
    }
    var jsUrl = $(obj).data('url');
    var dataParams = $(obj).data();
    delete dataParams["modalheader"];
    delete dataParams["modaltitle"];
    delete dataParams["url"];
    delete dataParams["statichtmlsourcedivid"];
    delete dataParams["cache"];
    delete dataParams["modalcontainer"];
    delete dataParams["jqueryselector"];
    delete dataParams["modalpopupwidth"];
    console.info(jsUrl);
    $.ajax({
        url: jsUrl,
        type: "GET",
        data: dataParams,
        success: function (data) {
            modalcontent.html(data);
            if (modaltitle !== null && modaltitle !== "undefined")
                modalcontainer.find('#modaltitle').text(modaltitle);
            if (modalheader !== null && modalheader !== "undefined")
                modalcontainer.find('#modalheader').text(modalheader);
            if (modalcontainer[0].id == "modalcontainer" && modaldismiss != "auto")
                modalcontainer.modal({ backdrop: 'static', keyboard: false });
            else
                modalcontainer.modal();
            if (typeof localScript == 'function') {
                localScript();
            }
        },
        error: function (result) {
            errorText = result.responseText.indexOf("divErrorMessage") == -1 ? result.responseText : ($(result.responseText).find("div[id*=divErrorMessage]").text()).replace("text-danger", "text-white");
            notify("error", errorText, 'Error');
        }
    });
}
function genericLoadDataTable({ pDataTableId, pColumns, pUrl, pUrlParams, pRequestVerificationToken, PStateSave = true, pCallBackHeaderData, pPagination = true }) {
    dataTable = $(pDataTableId).DataTable({
        drawCallback: function (settings) {
            loadScript();
            if (pCallBackHeaderData !== null && pCallBackHeaderData !== "undefined") {
                if (pCallBackHeaderData) {
                    window[pCallBackHeaderData](settings.json);
                }
            }
        },
        destroy: true,
        order: [[1, "asc"]],
        sort: false,
        responsive: true,
        autoWidth: false,
        pageLength: 25,
        lengthMenu: [25, 50, 100, 200],
        processing: true,
        serverSide: true,
        stateSave: PStateSave,
        info: false,
        filter: false,
        paginate: pPagination,
        lengthChange: pPagination,
        dom: '<"headerDetails">rt<"row"<"col-1"l><"col"p>><"clear">',
        'columnDefs': [
            { orderable: false, targets: 0 }
        ],
        columns: pColumns,
        ajax: {
            headers: { "RequestVerificationToken": pRequestVerificationToken },
            url: pUrl,
            type: 'GET',
            cache: true,
            dataType: "json",
            data: function (d) {
                d.columns = null;
                d.order = null;
                d.search = null;
                d = { ...d, ...pUrlParams };
                return { "paramJson": JSON.stringify(d) };
            },
            error: function (request, status, error) {
                showError('error', request, 'danger');
            },
            complete: function () {
                loadScript();
            }
        }
    });
}
function AvoidSpace(event) {
    var k = event ? event.which : window.event.keyCode;
    if (k == 32) return false;
}
function RestrictAlphabets(event) {
    var regex = new RegExp("^[0-9-!@#$%*?,./():'|~`_=+]");
    var key = String.fromCharCode(event.charCode ? event.which : event.charCode);
    if (!regex.test(key)) {
        event.preventDefault();
        return false;
    }
}
function AllowNumbersOnly(event) {
    var regex = new RegExp("^[0-9]");
    var key = String.fromCharCode(event.charCode ? event.which : event.charCode);
    if (!regex.test(key)) {
        event.preventDefault();
        return false;
    }
}
function AllowDecimalNumbersOnly(event) {
    var regex = new RegExp("^[0-9.]");
    var key = String.fromCharCode(event.charCode ? event.which : event.charCode);
    if (!regex.test(key)) {
        event.preventDefault();
        return false;
    }
}
function convertToBlob(b64Data, contentType = '', sliceSize = 512) {
    const byteCharacters = atob(b64Data);
    const byteArrays = [];
    for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
        const slice = byteCharacters.slice(offset, offset + sliceSize);
        const byteNumbers = new Array(slice.length);
        for (let i = 0; i < slice.length; i++) {
            byteNumbers[i] = slice.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        byteArrays.push(byteArray);
    }
    const blob = new Blob(byteArrays, { type: contentType });
    return blob;
};
function RegularExpressionValidationCheck(event) {
    var regex = new RegExp("^[a-zA-Z0-9, ]");
    var key = String.fromCharCode(event.charCode ? event.which : event.charCode);
    var k = event ? event.which : window.event.keyCode;
    if (!regex.test(key) && k != 13) {
        event.preventDefault();
        return false;
    }
}
function validatePaste(el, e) {
    var regex = /^[a-zA-Z0-9, \r\n]+$/;
    var key = e.clipboardData.getData('text')
    if (!regex.test(key)) {
        e.preventDefault();
        alert("Found Invalid corrector");
        return false;
    }
}
function loadProcessLogDataTable(vVUloadProcessLogList) {
    genericLoadDataTable({
        pDataTableId: "#tbGridDetail",
        pColumns: datatableColumns,
        pUrl: vVUloadProcessLogList,
        pUrlParams: {
            id: $("#formProgressDetail input[id=KeyId]").val()
        },
        pRequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
    });
};