<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="KeywordsGroupTypeBind.aspx.cs" Inherits="ZHSMSPlatform.Root.Keywords.KeywordsGroupTypeBind" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>敏感词组与类型关联</title>
 <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <link type="text/css" rel="stylesheet" href="../../JavaScript/Prompt/gridview.css" />
    <script type="text/javascript" src="../js/function.js"></script>
    <script src="../../Script/formValidator/DateTimeMask.js" type="text/javascript"></script>
    <script src="../../Script/formValidator/datepicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
</head>
<body class="mainbody">
    <form id="form1" runat="server">
         <div class="navigation">首页 &gt; 敏感词管理 &gt; 敏感词类型与词组关联</div>
        <div class="tools_box">
            <div class="tools_bar">
                <a href="KeywordsMange.aspx" class="tools_btn"><span><b class="all">返回敏感词管理</b></span></a>
            </div>
        </div>
        <div id="contentTab">
            <ul class="tab_nav">
                <li class="selected"><a onclick="tabs('#contentTab',0);" href="javascript:;">词组与类别关联</a></li>
            </ul>
            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>敏感词组：</th>
                            <td>
                                <asp:DropDownList ID="dd_groups" runat="server" OnSelectedIndexChanged="dd_groups_SelectedIndexChanged"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <th>敏感词类型：</th>
                            <td>
                                <asp:CheckBoxList ID="cb_types" runat="server" RepeatColumns="6" RepeatDirection="Horizontal"></asp:CheckBoxList>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <asp:HiddenField ID="hf_code" runat="server" />
            <div class="foot_btn_box">
                <asp:Button ID="btnSubmit" runat="server" Text="提交保存" CssClass="btnSubmit" OnClick="btnSubmit_Click" />
                &nbsp;<input name="重置" type="reset" class="btnSubmit" value="重 置" />
            </div>
        </div>
    </form>
</body>

