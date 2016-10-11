<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EnterpriseAdd.aspx.cs" Inherits="ZKD.Root.Enterprise.EnterpriseAdd" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>企业开户</title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.8.3.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script src="../../Script/formValidator/DateTimeMask.js" type="text/javascript"></script>
    <%--<script src="../../Script/formValidator/datepicker/calendar.js" type="text/javascript"></script>--%>
    <script src="../../Script/formValidator/datepicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
</head>
<body class="mainbody">
    <form id="form1" runat="server">
        <div id="contentTab">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <div id="Div1" runat="server" class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <td colspan="2" style="background-color: lavender; font-weight: bold">企业基本资料</td>
                        </tr>
                        <tr>
                            <th>企业名称：</th>
                            <td>
                                <asp:TextBox ID="txt_name" Text="" runat="server" CssClass="txtInput normal required"></asp:TextBox>*
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server"
                                    ControlToValidate="txt_name" Display="Dynamic" ForeColor="Red"
                                    ErrorMessage="请填写企业名称">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>企业联系人：</th>
                            <td>
                                <asp:TextBox ID="txt_contact" Text="" runat="server" CssClass="txtInput normal required"></asp:TextBox>*
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server"
                                    ControlToValidate="txt_contact" Display="Dynamic" ForeColor="Red"
                                    ErrorMessage="请填写企业联系人">
                                </asp:RequiredFieldValidator>

                            </td>
                        </tr>
                        <tr>
                            <th>联系电话：</th>
                            <td>
                                <asp:TextBox ID="txt_phone" Text="" runat="server" CssClass="txtInput normal required"></asp:TextBox>*
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server"
                                    ControlToValidate="txt_phone" Display="Dynamic" ForeColor="Red"
                                    ErrorMessage="请填写联系电话">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>地区：</th>
                            <td>
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="dd_province" runat="server" OnSelectedIndexChanged="dd_province_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                                        <asp:DropDownList ID="dd_city" runat="server"></asp:DropDownList>*
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <th>详细地址：</th>
                            <td>
                                <asp:TextBox ID="txt_address" Text="" runat="server" CssClass="txtInput normal required"></asp:TextBox>*
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server"
                                    ControlToValidate="txt_address" Display="Dynamic" ForeColor="Red"
                                    ErrorMessage="请填写企业详细地址">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <td colspan="2" style="background-color: lavender; font-weight: bold">帐号设置</td>
                        </tr>

                        <tr>
                            <th>企业帐号：</th>
                            <td>
                                <asp:TextBox ID="txt_account" Text="" runat="server" CssClass="txtInput normal required"></asp:TextBox>*
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                                    ControlToValidate="txt_account" Display="Dynamic" ForeColor="Red"
                                    ErrorMessage="请填写企业帐号"> 
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>企业密码：</th>
                            <td>
                                <asp:TextBox ID="txt_pass" Text="" runat="server" CssClass="txtInput normal required" TextMode="Password"></asp:TextBox>*
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                    ControlToValidate="txt_pass" Display="Dynamic" ForeColor="Red"
                                    ErrorMessage="请填写企业密码">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                       
                        <tr>
                            <td colspan="2" style="background-color: lavender; font-weight: bold">短信设置</td>
                        </tr>
                         <tr>
                            <th>接入号码尾号：</th>
                            <td>
                                <asp:TextBox ID="txt_spNumber" Text="" runat="server" CssClass="txtInput normal required" Enabled="false"></asp:TextBox>
                                <asp:TextBox ID="txt_Wei" Text="" runat="server" CssClass="txtInput normal required" MaxLength="2" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;"></asp:TextBox>*<p style="color:red">码号总长度不能超过20位。如果不知道如何设置，请咨询客服人员。</p>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                    ControlToValidate="txt_Wei" Display="Dynamic" ForeColor="Red"
                                    ErrorMessage="请填写企业接入号码尾号">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>企业短信签名：</th>
                            <td>
                                <asp:TextBox ID="txt_smsSigure" Text="" runat="server" CssClass="txtInput normal required"></asp:TextBox>*(无须加【】,系统自动添加)
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server"
                                    ControlToValidate="txt_smsSigure" Display="Dynamic" ForeColor="Red"
                                    ErrorMessage="请填写短信签名">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>状态报告接收方式：</th>
                            <td>
                                <asp:RadioButtonList ID="rb_SMSReportType" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="0">不发送</asp:ListItem>
                                    <asp:ListItem Value="1" Selected="True">发送</asp:ListItem>
                                    <asp:ListItem Value="2">推送</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <th>短信发送通道：</th>
                            <td>
                                <asp:RadioButtonList ID="rb_SMSChannel" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="-1-">所有通道</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
<%--            <div class="tab_con" style="display: block;">
                <asp:CheckBox ID="chkLicence" runat="server" /><a href="http://www.baidu.com">短信使用许可条款</a>
            </div>--%>
            <div class="foot_btn_box">
                <asp:Button ID="btnSubmit" runat="server" Text="提交保存" CssClass="btnSubmit" OnClick="btnSubmit_Click" />
                &nbsp;<input name="重置" type="reset" class="btnSubmit" value="重 置" />
            </div>
        </div>
    </form>
</body>
</html>
