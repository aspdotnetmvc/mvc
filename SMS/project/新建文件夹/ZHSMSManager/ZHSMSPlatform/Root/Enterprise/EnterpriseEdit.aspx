<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EnterpriseEdit.aspx.cs" Inherits="ZHSMSPlatform.Root.Enterprise.EnterpriseEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>企业设置</title>
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
            <a href="javascript:history.go(-1);" class="back">后退</a>首页 &gt; 企业用户管理 &gt; 企业编辑
        </div>
        <div id="contentTab">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <td style="background-color: lavender; font-weight: bold">帐号信息</td>
                            <td style="background-color: lavender; text-align: right">
                                <%--<asp:Button ID="btn_BasicSave" CssClass="btnSubmit" runat="server" Text="保存" OnClick="btn_BasicSave_Click" />&nbsp;&nbsp;--%>
                            </td>
                        </tr>
                        <tr>
                            <th>企业帐号：</th>
                            <td>
                                <asp:Label ID="lbl_account" Text="" runat="server"></asp:Label>
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
                            <td style="background-color: lavender; font-weight: bold">企业基本资料</td>
                            <td style="background-color: lavender; text-align: right">
                                <asp:Button ID="btn_infoSave" CssClass="btnSubmit" runat="server" Text="保存" OnClick="btn_infoSave_Click" />&nbsp;&nbsp;
                            </td>
                        </tr>
                        <tr>
                            <th>企业名称：</th>
                            <td>
                                <asp:TextBox ID="txt_name" Text="" runat="server" CssClass="txtInput" Width="400" MaxLength="128"></asp:TextBox>*
                            </td>
                        </tr>
                        <tr>
                            <th>企业联系人：</th>
                            <td>
                                <asp:TextBox ID="txt_contact" Text="" runat="server" Width="200" CssClass="txtInput" MaxLength="32"></asp:TextBox>*
                            </td>
                        </tr>
                        <tr>
                            <th>手机号码：</th>
                            <td>
                                <asp:TextBox ID="txt_phone" Text="" runat="server" Width="200" CssClass="txtInput" MaxLength="11"></asp:TextBox>*
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
                                <asp:TextBox ID="txt_address" Text="" runat="server" CssClass="txtInput" Width="450" MaxLength="128"></asp:TextBox>
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
                            <td style="background-color: lavender; font-weight: bold">短信设置</td>
                            <td style="background-color: lavender; text-align: right">
                                <asp:Button ID="btn_SMSSave" CssClass="btnSubmit" runat="server" Text="保存" OnClick="btn_SMSSave_Click" />&nbsp;&nbsp;
                            </td>
                        </tr>
                        <tr>
                            <th>扩展码号：</th>
                            <td>
                                <asp:TextBox ID="txt_spNumber" Text="" runat="server" CssClass="txtInput" Width="200" MaxLength="5" onkeypress="if (event.keyCode < 48 || event.keyCode >57) event.returnValue = false;"></asp:TextBox>*<p style="color:red">（只填写扩展码，网关接入号码系统自动添加。扩展码号长度不能超过8位，建议设置4位扩展码。码号超过20位将导致发送短信失败。）</p>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                    ControlToValidate="txt_spNumber" Display="Dynamic"
                                    ErrorMessage="请填写企业接入号码">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>企业短信签名：</th>
                            <td>
                                <asp:TextBox ID="txt_smsSigure" Text="" runat="server" CssClass="txtInput" Width="200" MaxLength="10"></asp:TextBox>(无须加【】,系统自动添加。此项非必填，如果留空，用户需在短信内容里自己添加企业签名)
<%--                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server"
                                    ControlToValidate="txt_smsSigure" Display="Dynamic"
                                    ErrorMessage="请填写短信签名">
                                </asp:RequiredFieldValidator>--%>
                            </td>
                        </tr>
                        <tr>
                            <th>企业优先级：</th>
                            <td>
                                <asp:RadioButtonList ID="rb_accountLevel" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="0" Selected="True">0</asp:ListItem>
                                    <asp:ListItem Value="1">1</asp:ListItem>
                                    <asp:ListItem Value="2">2</asp:ListItem>
                                    <asp:ListItem Value="3">3</asp:ListItem>
                                </asp:RadioButtonList>(优先级越高,短信将优先发送)
                            </td>
                        </tr>
                        <tr>
                            <th>审核方式：</th>
                            <td>
                                <asp:RadioButtonList ID="rb_accountAudit" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="1">自动审核</asp:ListItem>
                                    <asp:ListItem Value="0">人工审核</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <th>二次开发接口使用许可：</th>
                            <td>
                                <asp:RadioButtonList ID="rb_isOpen" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="1">是</asp:ListItem>
                                    <asp:ListItem Value="0">否</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <th>状态报告接收方式：</th>
                            <td>
                                <asp:RadioButtonList ID="rb_SMSReportType" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="0">不发送</asp:ListItem>
                                    <asp:ListItem Value="1">发送</asp:ListItem>
                                    <asp:ListItem Value="2">推送</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <th>短信级别：</th>
                            <td>
                                <asp:RadioButtonList ID="rb_SMSLevel" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="0">0</asp:ListItem>
                                    <asp:ListItem Value="1">1</asp:ListItem>
                                    <asp:ListItem Value="2">2</asp:ListItem>
                                    <asp:ListItem Value="3">3</asp:ListItem>
                                    <asp:ListItem Value="4">4</asp:ListItem>
                                    <asp:ListItem Value="5">5</asp:ListItem>
                                    <asp:ListItem Value="6">6</asp:ListItem>
                                </asp:RadioButtonList>(短信级别越高,短信将优先发送)
                            </td>
                        </tr>
                        <tr>
                            <th>短信内容过滤方式：</th>
                            <td>
                                <asp:RadioButtonList ID="rb_SMSFilter" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="1">替换</asp:ListItem>
                                    <asp:ListItem Value="2">发送失败</asp:ListItem>
                                    <asp:ListItem Value="0">不过滤</asp:ListItem>
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
        </div>
    </form>
</body>
</html>
