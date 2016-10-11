<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FailAudit.aspx.cs" Inherits="ZHCRM.Root.SMSM.FailAudit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>审核失败原因</title>
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
          <div class="navigation">
            <a href="javascript:history.go(-1);" class="back">后退</a>首页 &gt; 短信审核 &gt; 审核失败原因
        </div>
        </div>
        <div id="contentTab">
            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>失败原因：</th>
                            <td>
                                <asp:TextBox ID="txt_reason" runat="server" TextMode="MultiLine" Width="300px" Height="100px"></asp:TextBox>
                            </td>
                        </tr>

                    </tbody>
                </table>
                <div class="foot_btn_box">
                    <asp:Button ID="btn_Submit" runat="server" Text="提交审核结果" CssClass="btnSubmit" OnClick="btnSubmit_Click" />
                    &nbsp;<input name="重置" type="reset" class="btnSubmit" value="重 置" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
