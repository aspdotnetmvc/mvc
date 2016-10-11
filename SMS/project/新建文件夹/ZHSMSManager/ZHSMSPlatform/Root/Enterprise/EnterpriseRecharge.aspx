<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EnterpriseRecharge.aspx.cs" Inherits="ZHSMSPlatform.Root.Enterprise.EnterpriseRecharge" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>企业充值</title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script src="../../Script/formValidator/DateTimeMask.js" type="text/javascript"></script>
    <script src="../../Script/formValidator/datepicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
</head>
<body class="mainbody">
    <form id="form1" runat="server">
        <div class="navigation">
            <a href="javascript:history.go(-1);" class="back">后退</a>首页 &gt; 企业用户管理 &gt; 企业设置
        </div>
        <div id="contentTab">
            <ul class="tab_nav">
                <li><a onclick="tabs('#contentTab',0);" href="javascript:;">企业设置</a></li>
            </ul>
            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>企业帐号：</th>
                            <td>
                                <asp:Label ID="lbl_account" Text="" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>可发送短信条数：</th>
                            <td>
                                <asp:Label ID="lbl_Old" Text="" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>充值方式：</th>
                            <td>
                                <asp:RadioButtonList ID="rb_type" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="0" >金额</asp:ListItem>
                                    <asp:ListItem Value="1" Selected="True">短信条数</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <th>充值金额（条数）：</th>
                            <td>
                                <asp:TextBox ID="txt_sms" Text="" runat="server" CssClass="txtInput" Width="360"></asp:TextBox>*
                            </td>
                        </tr>
                        <tr>
                            <th>短信费率（元）：</th>
                            <td>
                                <asp:TextBox ID="txt_SMSRate" Text="" runat="server" CssClass="txtInput"  Width="360"></asp:TextBox>*
                            </td>
                        </tr>
                        <tr>
                            <th>备注：</th>
                            <td>
                                <asp:TextBox ID="txt_remark" TextMode="MultiLine" Width="360" Height ="180" runat="server" CssClass="txtInput"></asp:TextBox>*
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="foot_btn_box">
                <asp:Button ID="btnSubmit" runat="server" Text="提交保存" CssClass="btnSubmit" OnClick="btnSubmit_Click" />
                &nbsp;<input name="重置" type="reset" class="btnSubmit" value="重 置" />
            </div>
        </div>
    </form>
</body>
</html>

