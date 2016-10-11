<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TempletAudit_Failure.aspx.cs" Inherits="ZHSMSPlatform.Root.SMSM.TempletAudit_Failure" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>短信模板不通过审核</title>
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
            <a href="javascript:history.go(-1);" class="back">后退</a>首页 &gt; 待审核短信模板管理 &gt; 短信模板审核
        </div>
        <div id="contentTab">
            <ul class="tab_nav">
                <li class="selected"><a onclick="tabs('#contentTab',0);" href="javascript:;">短信模板不通过审核</a></li>
            </ul>

            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>企业名称：</th>
                            <td>
                                <asp:Label ID="lbl_account" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>模板内容：</th>
                            <td>
                                <asp:Label ID="lbl_content" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>提交时间：</th>
                            <td>
                                <asp:Label ID="lbl_submit" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>原因：</th>
                            <td>
                                <asp:TextBox TextMode="MultiLine" ID="txt_cause" Text="" runat="server" CssClass="txtInput" MaxLength="256" Height="180" Width="320"></asp:TextBox>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <div class="foot_btn_box">
                    <asp:Button ID="btn_Submit" runat="server" Text="提交" CssClass="btnSubmit" OnClick="btnSubmit_Click" />
                    &nbsp;<input name="重置" type="reset" class="btnSubmit" value="重 置" /> &nbsp;
                    <asp:Button ID="btn_back" runat="server" Text="返回" CssClass="btnSubmit" OnClick="btn_back_Click" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>