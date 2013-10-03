var baseUrl = "";

function CheckAll() {
    var value = $('#checkAll')[0].checked;
    $("input[name=selected]").each(function (i) {
        this.checked = value;
    });
}

function Dialog(html, buttons, options) {
    options = options || { "classes": "winModal" };
    options.classes = options.classes || "winModal";
    bootbox.dialog(html, buttons, options);
}

//确认页面
function MsgConfirm() {
    if (arguments && arguments.length > 0) {
        var str = "确定此操作?";
        var cb = null;
        if (arguments.length == 1) {
            cb = arguments[0];
        } else if (arguments.length == 2) {
            str = arguments[1] || "确定此操作?";
            cb = arguments[0];
        }
        return bootbox.dialog(str, [{
            "label": "取消",
            "icon": "icon-remove-sign",
            "callback": function () {
                if (typeof cb == 'function') {
                    cb(false);
                }
            }
        }, {
            "label": "确认",
            "icon": 'icon-ok-sign',
            "callback": function () {
                if (typeof cb == 'function') {
                    cb(true);
                }
            }
        }],
    {
        "onEscape": function () {
            if (typeof cb == 'function') {
                cb(false);
            }
        },
        "classes": "alertModal"
    });
    } else
        return false;
}

//打开详细页面
function ShowDetails(el) {
    var title = $(el).text();
    var url = $(el).attr('href');
    GetAJAX(url, { rand: Math.random() }, function (r) {
        bootbox.dialog(r, [], { header: title });
    });
}

//提交新建或者修改页面
function FormSave(form) {
    if ($(form).valid()) {
        var data = $(form).serialize();
        var url = $(form).attr('action');
        $.post(url, data, function (result, textStatus, jqXHR) {
            if (result.Result) {
                $(form).parents('.modal').modal('hide');
                //刷新
                RefreshContent();
            } else
                bootbox.alert(result.MSG);
        });
    }
}

//刷新列表
function RefreshSearchContent(container) {
    var refUrl = $(".SearchContent .pagedlist a.active", container).attr('href');
    if (refUrl == undefined || refUrl.length < 2) {
        refUrl = $(".SearchContent table").attr('targeturl');
    }
    if (refUrl && refUrl.length > 2) {
        $.get(refUrl, { rand: Math.random() }, function (refList) {
            $('.SearchContent', container).html(refList);
        });
    }
}

function RefreshContent(container) {
    var refUrl = undefined;
    if (container) {
        refUrl = $(".SearchContent table", container).attr('targeturl');
    } else {
        refUrl = $(".SearchContent table").attr('targeturl');
    }
    if (refUrl) {
        RefreshSearchContent(container);
    } else {
        var msg = $("#SuccessMsg", container).text();
        ToSuccess(msg);
    }
}

//打开新建或者修改页面
function ShowCreateOrEdit(el) {
    var url = $(el).attr('href');
    var title = $(el).text();
    ShowDialogFormEdit(url, { rand: Math.random() }, title);
}

function ShowDialogFormEdit(url, data, title) {
    GetAJAX(url, data, function (r) {
        var dialog = bootbox.dialog(r, [{
            "label": "保存",
            "class": "btn-primary",
            "autoClose": false,
            "callback": function (ele) {
                var form = $(ele).parent().prev().find('form');
                FormSave(form);
                return false;
            }
        }, {
            "label": "取消",
            "class": "btn-default",
            "callback": function () { }
        }],
        { header: title });
        BindFormEvent();
    });
    return false;
}

function ClickOver(el) {
    var tr = $(el).parents('tr');
    if (tr.length > 0) {
        $(tr[0]).css('background-color', 'yellow');
    }
}

//打开选择页面,name=selected
function ShowSelect(url, callback) {
    var title = '请选择';
    $.get(url, { rand: Math.random() }, function (r) {
        bootbox.dialog(r, [{
            "label": "选择",
            "class": "btn-primary",
            "callback": function (ele) {
                var selected = $(ele).parent().prev().find('input[name=selected]:checked');
                if (typeof callback == 'function' && selected.length > 0)
                    callback(selected);
            }
        }],
        { header: title });
    });
    return false;
}

function disabledClick(el) {
    $(el).prop('disabled', true);
    setTimeout(function () { $(el).prop('disabled', false); }, 3000);
}

function DeleteItem(el) {
    MsgConfirm(function (isOk) {
        if (isOk) {
            var url = $(el).attr('href');
            var id = $(el).attr('targetID');
            $.post(url, function (result) {
                if (result.Result) {
                    $('#' + id).remove();
                    RefreshSearchContent();
                }
                bootbox.alert(result.MSG);
            });
        }
    });
    return false;
}

$(function () {
    $('#leftnav li a').bind('click', function (e) {
        e.preventDefault();
        OpenContent(this);
    });

    //$('.table tr td a').live("click", function () { ClickOver(this); });
    $('.datetime').live("focus", function () {
        $(this).datepicker();
    });

    $("input[name='Keyword'][type='text']").die("keydown").live("keydown", function (event) {
        if (event.which == 13) {
            $(this).next().click();
            return false;
        }
    });

    $('.SearchContent .pagedlist a').live("click", function (e) {
        var url = this.href;
        if (url.length > 5) {
            $.get(url, function (r) {
                $('.SearchContent').html(r);
            });
            e.preventDefault();
        }
    });
});

function Search(el) {
    var url = $(el).attr('url');
    var searchForm = $(el).parents('form')[0];
    if (typeof (url) == 'undefined' || url.length == 0) {
        url = $(searchForm).attr('action');
    }

    var data = $(searchForm).serialize() + "&rand=" + Math.random();
    var UpateID = $(searchForm).attr('update-target') || '.SearchContent';

    GetAJAX(url, data, function (r) {
        $(UpateID).html(r);
    });
}

//分页方法
function PageClick(url) {
    $.ajax({
        dataType: "html",
        url: url,
        success: function (msg) {
            $('.SearchContent').html(msg);
        },
        error: function (msg) {
            alert("Error!");
        }
    });
}

//替代内容区域
function OpenContent(el) {
    var url = $(el).attr('href');
    if (url != "#") {
        url += "?rand=" + Math.random();
        GetAJAX(url, {}, function (msg) {
            $('#maincontent').html(msg);
        });
    }
    var $li = $(el).parent('.active');
    if ($li.length) {
        $li.removeClass('active');
    }
    else {
        $(el).parent().addClass('active');
    }
}

//绑定表单事件
function BindFormEvent() {
    jQuery.validator.unobtrusive.parse('form');
}

//保留两位小数
//功能：将浮点数四舍五入，取小数点后2位
function toDecimal(x) {
    var f = parseFloat(x);
    if (isNaN(f)) {
        return;
    }
    f = Math.round(x * 100) / 100;
    return f;
}

function SelectIt(el) {
    var value = $(el).attr('value');
    $($(el).parents('.btn-group')[0]).find('input:text').val(value);
}

function ShowNotice(h) {
    if ($('#dvnotice').length > 0) {
        $('#dvnotice').remove();
    }

    var html = $('<div class="well alert alert-error" id="dvnotice"><button type="button" class="close" data-dismiss="alert">×</button></div>').append(h);
    $('body').append(html);
}

function HideNotice() {
    if ($('#dvnotice').length > 0) {
        $('#dvnotice').remove();
    }
}

function StartLoading() {
    if ($('#loading').length == 0) {
        var html = '<div class="well" id="loading">加载中，请稍等...... <img src="' + baseUrl + '/Content/img/loading.gif" alt="loading.." /></div>';
        $('body').append(html);
    } else {
        $('#loading').show();
    }
}

function EndLoading() {
    if ($('#loading').length > 0) {
        $('#loading').hide();
    }
}

function GetAJAX(url, data, call) {
    $.ajax({
        url: url,
        data: data,
        success: call,
        cache: false,
        beforeSend: function () {
        },
        complete: function () {
        },
        error: function (rep) {
            alert("错误！");
        }
    });
}

//项目
function ToSuccess(msg) {
    var url = baseUrl + '/Home/Success';
    window.location.href = url + '?msg=' + msg;
}

function ToError(msg) {
    var url = baseUrl + '/Home/Error';
    window.location.href = url + '?msg=' + msg;
}

function GoHome() {
    var url = baseUrl + '/';
    GetAJAX(url, { rand: Math.random() }, function (msg) {
        $('#MainContent').html(msg);
    });
}

function ListClickAfter(el) {
    $(el).css('backgroud-color', 'yellow');
}

function keyWordEnter() {
    $('#Keyword').unbind().keydown(function (e) {
        if (e.keyCode == 13) {
            e.preventDefault();
            $('#btnSearchCondition').click();
        }
    });
}

function ShortcutLink(el, call) {
    bootbox.confirm("确定此操作吗?", function (result) {
        if (result) {
            var url = $(el).attr('href');
            $.post(url, function (r) {
                if (r.Result) {
                    RefreshSearchContent();
                    if (call) {
                        call(el);
                    }
                } else
                    bootbox.alert(r.MSG);
            });
        }
    });
}

function DeleteTr(el) {
    $(el).parents("tr").remove();
}

function Upload(url) {
    $.ajaxFileUpload({
        url: url,
        secureuri: false,
        fileElementId: 'upload',
        dataType: 'json',
        data: {//加入的文本参数
            //"fileDes": $("#filedes").val()
        },
        beforeSend: function () {
            $('#fileloading').show();
        },
        complete: function () {
            $('#fileloading').hide();
        },
        success: function (data, status) {
            if (data.Result) {
                $('#uploadresult').text("上传成功");
                var val = $('input[name=FileName]').val();
                val += data.MSG + ";";
                $('input[name=FileName]').val(val);
                $('#fileList').append('<tr><td>' + data.MSG + '</td></tr>');
            } else {
                alert(data.MSG);
            }
        },
        error: function (data, status, e) {
            alert('上传错误：' + e);
        }
    });
    return false;
}

Date.prototype.format = function (format) {
    /*
    * eg:format="yyyy-MM-dd hh:mm:ss";
    */
    var o = {
        "M+": this.getMonth() + 1,  //month
        "d+": this.getDate(),     //day
        "h+": this.getHours(),    //hour
        "m+": this.getMinutes(),  //minute
        "s+": this.getSeconds(), //second
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter
        "S": this.getMilliseconds() //millisecond
    }

    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }

    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
}

function ConertJsonTimeAndFormat(jsonTime, format) {
    return new Date(eval(jsonTime.replace(/\/Date\((\d+)\)\//gi, "new Date($1)"))).format(format);
}