
var userInfo;
var groupArray;
var groupArrayCustom;//仅含自定义项
var CheckSameResult = 0;

var Param;

//初始化
$(function () {
    Param = eval("(" + $('#paramJson').val() + ")");

    $('#searchGroup').combobox({
        valueField: 'GroupID',
        textField: 'GroupName',
        editable: false
    });
    $('#tree').tree({
        valueField: 'GroupID',
        textField: 'GroupName',
        editable: false
    });
    InitGroup();
    InitGrid();
});

function InitGroup() {
    var url = "/Enterprise/GetSMSContactGroupList";
    var sdata = {AccountCode: Param.AccountCode };
    $zh.ajax(sdata, url, function (data) {
        if (data.total) {
            groupArray = data.rows;
            groupArrayCustom = $.extend(true, [], data.rows);
            for (var i = 0; i < groupArrayCustom.length; i++) {
                if (groupArrayCustom[i].text.trim() == "未分组") {
                    groupArrayCustom.splice(i, 1);//删除
                }
            }

            //绑定组别数
            $('#tree').tree({
                checkbox: false,
                data: data.rows,
                onClick: function (node) {
                    loadgrid(node.id);
                }
            });

            var groupCombobox = $.extend(true, [], data.rows);
            groupCombobox.unshift({ id: -1, text: "请选择", selected:true });
            //绑定组别查询项
            $('#searchGroup').combobox({
                data: groupCombobox,
                valueField: 'id',
                textField: 'text',
                editable: false
            });

        }
    }
    );
}

//加载通讯录列表
function InitGrid() {
    $("#grid").datagrid({
        queryParams: { AccountCode: Param.AccountCode },
        frozenColumns:[[
           { field: 'UserName', title: '姓名', width: 100 }
        ]],
        columns: [[
           { field: 'UserSex', title: '性别', width: 50 },
           { field: 'UserBrithday', title: '生日', width: 100 },
           { field: 'TelPhoneNum', title: '电话号码', width: 100 },
           { field: 'CompanyEmail', title: 'Email', width: 100 },
           { field: 'QQ', title: 'QQ', width: 100 },
           { field: 'WebChat', title: '微信', width: 100 },
           { field: 'CompanyName', title: '公司名称', width: 100 },
           { field: 'ComPostion', title: '职位', width: 100 },
           { field: 'CompanyWeb', title: '公司网站', width: 150 },
           { field: 'AddTime', title: '创建时间', width: 150 },
           { field: 'TelPhoneGroupName', title: '通讯录组', width: 100 },
           {
               field: 'Operate', title: '操作', width: 200, formatter: function (value, row, index) {
                   var str = '<a href="javascript:void(0)"  onclick="deleteContact(' + index + ')"><span class="pure-button ismp-button4 btn">删除</span></a>&nbsp;';
                   str += '&nbsp;<a href="javascript:void(0)"  onclick="changeGroup(' + index + ')"><span class="pure-button ismp-button4 btn">分组</span></a>&nbsp;';
                   str += '&nbsp;<a href="javascript:void(0)"  onclick="sendMessage(' + index + ')"><span class="pure-button ismp-button4 btn">发短信</span></a>&nbsp;';
                   return str;
               }
           }
        ]],
        pageSize: 20
    });
}

//点击组别,加载通讯录列表
function loadgrid(groupID) {
    var page = $('#grid').datagrid('getPager');
    $(page).pagination({ pageNumber: 1 });
    $('#grid').datagrid('options').url = '../Enterprise/GetSMSContactList';
    $('#grid').datagrid('options').queryParams = { AccountCode: Param.AccountCode };
    var ds = $('#grid').datagrid('options').queryParams;
    ds["GroupID"] = groupID;

    reloadgrid();
}

//查询
function searchContact(groupID)
{
    var page = $('#grid').datagrid('getPager');
    $(page).pagination({ pageNumber: 1 });
    $('#grid').datagrid('options').url = '../Enterprise/GetSMSContactList';
    $('#grid').datagrid('options').queryParams = { AccountCode: Param.AccountCode };
    var ds = $('#grid').datagrid('options').queryParams;
    ds["GroupID"] = groupID ? groupID : $('#searchGroup').combobox("getValue");
    ds["Number"] = $('#searchNumber').val();//("getValue");

    reloadgrid();
}

//重载
function reloadgrid() {
    $('#grid').datagrid('reload');
}

function addContact() {
    $("#frmedit").form("clear");

    $("#Radio_male").prop("checked", true);

    $("#winedit").window("open");
    //绑定组别查询项
    $('#GroupID').combobox({
        data: groupArray,
        valueField: 'id',
        textField: 'text',
        editable: false,
        onLoadSuccess: function () {
            if (groupArray && groupArray.length>0)
                $("#GroupID").combobox('select',groupArray[0].id);
        }
    });
    $("#frmedit").form({
        url: "/Enterprise/AddSMSContact",
        onSubmit: function (param) {
            param["AccountCode"] = Param.AccountCode;

            var isValid = $(this).form('validate');
            if (!isValid) {
                $.messager.progress('close');
                $.messager.alert("提示", "请先把数据填写完整！");
            }
            return isValid;
        },
        success: function (data) {
            var msg = eval("(" + data + ")");
            if (msg.success) {
                searchContact(msg.value);
                closeEdit();
            }
            //toastr[msg.type](msg.message);
            $.messager.alert("提示", msg.message, msg.type);
        }
    });
}

function submitAddContact() {
    $("#frmedit").submit();
}

function closeEdit() {
    $("#winedit").window("close");
}

function deleteContact(index) {
    var row = $('#grid').datagrid("getRows")[index];
    if (row) {
        $.messager.confirm('删除确认', '确定要删除该联系人吗?', function (r) {
            if (r) {
                //删除记录
                var jstr = $.ajax({
                    url: "/Enterprise/DeleteSMSContact",
                    data: { PID: row.PID, AccountCode: Param.AccountCode },
                    method: "post",
                    async: false
                }).responseText;

                var msg = eval("(" + jstr + ")");
                if (msg.success) {
                    reloadgrid();
                }
                $.messager.alert("提示", msg.message, msg.type);
            }
        });
    } else {
        $.messager.alert("提示", "请选择项！", "info");
    }
}

function addGroup() {
    $("#frmeditgroup").form("clear");
    $("#wineditgroup").window("open");

    $("#frmeditgroup").form({
        url: "/Enterprise/AddSMSContactGroup",
        onSubmit: function (param) {
            param["AccountCode"] = Param.AccountCode;

            var isValid = $(this).form('validate');
            if (!isValid) {
                $.messager.progress('close');
                $.messager.alert("提示", "请先把数据填写完整！");
            }
            return isValid;
        },
        success: function (data) {
            var msg = eval("(" + data + ")");
            if (msg.success) {
                InitGroup();
                closeEditGroup();
            }
            //toastr[msg.type](msg.message);
            $.messager.alert("提示", msg.message, msg.type);
        }
    });

    //名称重复判断
    CheckSameResult = 0;
    $("input", $("#GroupName").next("span")).blur(function () {
        if ($("#GroupName").val() == '') {
            $("#NameCheckResult").css("display", "none");
            return;
        }
        $zh.ajax({ GroupName: $("#GroupName").val(), AccountCode: Param.AccountCode }, "/Enterprise/IsSMSContactGroupSame", function (data) {
            if (data.success) {
                $("#NameCheckResult").css("display", "none");
                CheckSameResult = 0;
            }
            else {
                CheckSameResult = 1;
                $("#NameCheckResult").css("display", "block");
                $("#NameCheckResult").html("<span  style=\"color:red\">名称已用<span>");
            }
        }
   );
    });
}

function submitAddGroup() {
    if (CheckSameResult == 1) {
        $.messager.alert("提示", "名称已使用，换个试试！", "info");
        return;
    }
    $("#frmeditgroup").submit();
}

function closeEditGroup() {
    $("#NameCheckResult").css("display", "none");
    $("#wineditgroup").window("close");
}

function changeGroup(index) {
    $("#frmselectgroup").form("clear");
    var row = $('#grid').datagrid("getRows")[index];
    if (!row) $.messager.alert("提示", "请选择项！", "info");
    $("#PIDSelect").val(row.PID);
    //绑定组别查询项
    $('#GroupSelect').combobox({
        data: groupArray,
        valueField: 'id',
        textField: 'text',
        editable: false,
        onLoadSuccess: function () {
            if (groupArray && groupArray.length > 0)
                $("#GroupSelect").combobox('select', groupArray[0].id);
        }
    });

    $("#winselectgroup").window("open"); 

    $("#frmselectgroup").form({
        url: "/Enterprise/ChangeSMSContactGroup",
        onSubmit: function (param) {
            param["AccountCode"] = Param.AccountCode;

            var isValid = $(this).form('validate');
            if (!isValid) {
                $.messager.progress('close');
                $.messager.alert("提示", "请先把数据填写完整！");
            }
            return isValid;
        },
        success: function (data) {
            var msg = eval("(" + data + ")");
            if (msg.success) {
                reloadgrid();
                closeSelectGroup();
            }
            //toastr[msg.type](msg.message);
            $.messager.alert("提示", msg.message, msg.type);
        }
    });
}

function submitSelectGroup() {
    var selectValue = $('#GroupSelect').combobox("getValue");
    if (!selectValue || selectValue == "") {
        $.messager.alert("提示", "请先选择分组！");
        return;
    }
    $("#frmselectgroup").submit();
}

function closeSelectGroup() {
    $("#winselectgroup").window("close");
}

function sendMessage(index) {
    var row = $('#grid').datagrid("getRows")[index];
    if (row) {
        window.location.href = "/Enterprise/SendSMS?Numbers=" + row.TelPhoneNum;
    } else {
        $.messager.alert("提示", "请选择项！", "info");
    }
}

function sendGroupSMS() {
    $("#frmselectgroupsendsms").form("clear");

    //绑定组别查询项
    $('#GroupSelectSendSMS').combobox({
        data: groupArray,
        valueField: 'id',
        textField: 'text',
        editable: false,
        onLoadSuccess: function () {
            if (groupArray && groupArray.length > 0)
                $("#GroupSelectSendSMS").combobox('select', groupArray[0].id);
        }
    });

    $("#winselectgroupsendsms").window("open");

    $("#frmselectgroupsendsms").form({
        url: "/Enterprise/GetGroupSMSContact",
        onSubmit: function (param) {
            param["AccountCode"] = Param.AccountCode;

            var isValid = $(this).form('validate');
            if (!isValid) {
                $.messager.progress('close');
                $.messager.alert("提示", "请先把数据填写完整！");
            }
            return isValid;
        },
        success: function (data) {
            var msg = eval("(" + data + ")");
            if (msg.success) {
                closeSelectGroupSendSMS();
                //window.location.href = "/Enterprise/SendSMS?Numbers=" + msg.value;
                var postForm = document.createElement("form");
                postForm.method = "post";
                postForm.action = "/Enterprise/SendSMS";

                var input = document.createElement("textarea");
                input.type = "hidden";
                input.name = "Numbers";
                input.innerHTML = msg.value;
                postForm.appendChild(input);
                document.body.appendChild(postForm);
                postForm.submit();
                //用完之后，不要忘记删掉
                postForm.removeChild(input);
                document.body.removeChild(postForm);
            } else {
                //toastr[msg.type](msg.message);
                $.messager.alert("提示", msg.message, msg.type);
            }
        }
    });
}

function submitSelectGroupSendSMS() {
    var selectValue = $('#GroupSelectSendSMS').combobox("getValue");
    if (!selectValue || selectValue == "") {
        $.messager.alert("提示", "请先选择分组！");
        return;
    }
    $("#frmselectgroupsendsms").submit();
}

function closeSelectGroupSendSMS() {
    $("#winselectgroupsendsms").window("close");
}

function importContactExcel() {
    $("#frmimportcontacts").form("clear");

    //绑定组别查询项
    $('#GroupSelectImportExcel').combobox({
        data: groupArray,
        valueField: 'id',
        textField: 'text',
        editable: false,
        onLoadSuccess: function () {
            if (groupArray && groupArray.length > 0)
                $("#GroupSelectImportExcel").combobox('select', groupArray[0].id);
        }
    });

    $("#winimportcontacts").window("open");

    $("#frmimportcontacts").form({
        url: "/Enterprise/ImportSMSContactExcel",
        onSubmit: function (param) {
            param["AccountCode"] = Param.AccountCode;

            var isValid = $(this).form('validate');
            if (!isValid) {
                $.messager.progress('close');
                $.messager.alert("提示", "请先把数据填写完整！");
            }
            return isValid;
        },
        success: function (data) {
            var msg = eval("(" + data + ")");
            if (msg.success) {
                searchContact(msg.value);
                closeImportContact();
            }
            //toastr[msg.type](msg.message);
            $.messager.alert("提示", msg.message, msg.type);
        }
    });
}

function submitImportContact() {
    var selectFile = $('#SMSContactExcelFile').val();
    if (!selectFile || selectFile == "") {
        $.messager.alert("提示", "请先选择Excel文件！");
        return;
    }
    var selectValue = $('#GroupSelectImportExcel').combobox("getValue");
    if (!selectValue || selectValue == "") {
        $.messager.alert("提示", "请先选择分组！");
        return;
    }
    $("#frmimportcontacts").submit();
}

function closeImportContact() {
    $("#winimportcontacts").window("close");
}

function delGroup() {
    $("#frmselectgroupdel").form("clear");
    //绑定组别查询项
    $('#GroupSelectDel').combobox({
        data: groupArrayCustom,
        valueField: 'id',
        textField: 'text',
        editable: false,
        onLoadSuccess: function () {
            if (groupArrayCustom && groupArrayCustom.length > 0)
                $("#GroupSelectDel").combobox('select', groupArrayCustom[0].id);
        }
    });

    $("#winselectgroupdel").window("open");

    $("#frmselectgroupdel").form({
        url: "/Enterprise/DeleteSMSContactGroup",
        onSubmit: function (param) {
            param["AccountCode"] = Param.AccountCode;

            var isValid = $(this).form('validate');
            if (!isValid) {
                $.messager.progress('close');
                $.messager.alert("提示", "请先把数据填写完整！");
            }
            return isValid;
        },
        success: function (data) {
            var msg = eval("(" + data + ")");
            if (msg.success) {
                InitGroup();
                searchContact(msg.value);
                closeSelectGroupDel();
            }
            //toastr[msg.type](msg.message);
            $.messager.alert("提示", msg.message, msg.type);
        }
    });
}

function submitSelectGroupDel() {
    var selectValue = $('#GroupSelectDel').combobox("getValue");
    if (!selectValue || selectValue == "") {
        $.messager.alert("提示", "请先选择分组！");
        return;
    }
    $.messager.confirm('删除确认', '确定要删除该分组吗?', function (r) {
        if (r) {
            //删除记录
            $("#frmselectgroupdel").submit();
        }
    });
}

function closeSelectGroupDel() {
    $("#winselectgroupdel").window("close");
}

function exportGroup() {
    $("#frmselectgroupexport").form("clear");

    //绑定组别查询项
    $('#GroupSelectExport').combobox({
        data: groupArray,
        valueField: 'id',
        textField: 'text',
        editable: false,
        onLoadSuccess: function () {
            if (groupArray && groupArray.length > 0)
                $("#GroupSelectExport").combobox('select', groupArray[0].id);
        }
    });

    $("#winselectgroupexport").window("open");

    $("#frmselectgroupexport").form({
        url: "/Enterprise/ExportSMSContactGroup",
        onSubmit: function (param) {
            param["GroupSelectExportName"] = $('#GroupSelectExport').combobox("getText");
            param["AccountCode"] = Param.AccountCode;

            var isValid = $(this).form('validate');
            if (!isValid) {
                $.messager.progress('close');
                $.messager.alert("提示", "请先把数据填写完整！");
            }
            return isValid;
        }
    });
}

function submitSelectGroupExport() {
    var selectValue = $('#GroupSelectExport').combobox("getValue");
    if (!selectValue || selectValue == "") {
        $.messager.alert("提示", "请先选择分组！");
        return;
    }
    $("#frmselectgroupexport").submit();
    closeSelectGroupExport();
}

function closeSelectGroupExport() {
    $("#winselectgroupexport").window("close");
}
