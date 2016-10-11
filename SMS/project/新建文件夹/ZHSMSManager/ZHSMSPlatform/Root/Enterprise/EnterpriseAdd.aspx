<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EnterpriseAdd.aspx.cs" Inherits="ZHSMSPlatform.Root.Enterprise.EnterpriseAdd" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>企业开户</title>
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
                                <asp:TextBox ID="txt_name" Text="" runat="server" CssClass="txtInput" Width="350" MaxLength="256"></asp:TextBox>*
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server"
                                    ControlToValidate="txt_name" Display="Dynamic" ForeColor="Red"
                                    ErrorMessage="请填写企业名称">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>企业联系人：</th>
                            <td>
                                <asp:TextBox ID="txt_contact" Text="" runat="server" CssClass="txtInput" Width="200" MaxLength="32"></asp:TextBox>*
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server"
                                    ControlToValidate="txt_contact" Display="Dynamic" ForeColor="Red"
                                    ErrorMessage="请填写企业联系人">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>手机号码：</th>
                            <td>
                                <asp:TextBox ID="txt_phone" Text="" runat="server" CssClass="txtInput" Width="200" MaxLength="16"></asp:TextBox>*
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server"
                                    ControlToValidate="txt_phone" Display="Dynamic"  ForeColor="Red"
                                    ErrorMessage="请输入手机号码">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>地区：</th>
                            <td>
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="dd_province" runat="server" OnSelectedIndexChanged="dd_province_SelectedIndexChanged" AutoPostBack="True">
                                        </asp:DropDownList>
                                        <asp:DropDownList ID="dd_city" runat="server">
                                        </asp:DropDownList>*
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <th>详细地址：</th>
                            <td>
                                <asp:TextBox ID="txt_address" Text="" runat="server" CssClass="txtInput" Width="350" MaxLength="256"></asp:TextBox>
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
                                <asp:TextBox ID="txt_account" Text="" runat="server" CssClass="txtInput" Width="200" MaxLength="32"></asp:TextBox>*
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                                    ControlToValidate="txt_account" Display="Dynamic" ForeColor="Red"
                                    ErrorMessage="请填写企业帐号">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>企业密码：</th>
                            <td>
                                <asp:TextBox ID="txt_pass" TextMode="Password" runat="server" Width="200" CssClass="txtInput" MaxLength="16"></asp:TextBox>*
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                    ControlToValidate="txt_pass" Display="Dynamic" ForeColor="Red"
                                    ErrorMessage="请填写企业密码">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>确认密码：</th>
                            <td>
                                <asp:TextBox ID="TextBox1" TextMode="Password" runat="server" Width="200" CssClass="txtInput" MaxLength="16"></asp:TextBox>*
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server"
                                    ControlToValidate="TextBox1" Display="Dynamic" ForeColor="Red"
                                    ErrorMessage="请填写确认密码">
                                </asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CompareValidator1" runat="server" Display="Dynamic" ControlToCompare="txt_pass" ControlToValidate="TextBox1" ForeColor="Red" ErrorMessage="两次输入的密码不一致"></asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>开户类别：</th>
                            <td>
                                <asp:RadioButtonList ID="rb_IsAgent" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="1" Selected="True">代理商</asp:ListItem>
                                    <asp:ListItem Value="0">终端用户</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <th>是否启用：</th>
                            <td>
                                <asp:RadioButtonList ID="rb_accountEnable" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="1" Selected="True">是</asp:ListItem>
                                    <asp:ListItem Value="0">否</asp:ListItem>
                                </asp:RadioButtonList>
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
                            <th>扩展码号：</th>
                            <td>
                                <asp:TextBox ID="txt_spNumber" Text="" runat="server" CssClass="txtInput" Width="200" MaxLength="5" onkeyup="if(isNaN(value))execCommand('undo')" onafterpaste="if(isNaN(value))execCommand('undo')"></asp:TextBox>*
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                    ControlToValidate="txt_spNumber" Display="Dynamic" ForeColor="Red"
                                    ErrorMessage="请填写企业接入号码">
                                </asp:RequiredFieldValidator>
                                <p style="color:red">（只填写扩展码，网关接入号码系统自动添加。扩展码号长度不能超过8位，建议设置4位扩展码。码号超过20位将导致发送短信失败。）</p>
                            </td>
                        </tr>
                        <tr>
                            <th>企业短信签名：</th>
                            <td>
                                <asp:TextBox ID="txt_smsSigure" Text="" runat="server" CssClass="txtInput" Width="200" MaxLength="10"></asp:TextBox>(无须加【】,系统自动添加。此项非必填，如果留空，用户需在短信内容里自己添加企业签名)
<%--                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server"
                                    ControlToValidate="txt_smsSigure" Display="Dynamic" ForeColor="Red"
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
                                    <asp:ListItem Value="0" Selected="True">人工审核</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <th>二次开发接口使用许可：</th>
                            <td>
                                <asp:RadioButtonList ID="rb_isOpen" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="1">是</asp:ListItem>
                                    <asp:ListItem Value="0" Selected="True">否</asp:ListItem>
                                </asp:RadioButtonList>
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
                            <th>短信级别：</th>
                            <td>
                                <asp:RadioButtonList ID="rb_SMSLevel" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="0">0</asp:ListItem>
                                    <asp:ListItem Value="1">1</asp:ListItem>
                                    <asp:ListItem Value="2" Selected="True">2</asp:ListItem>
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
                                    <asp:ListItem Value="1" Selected="True">替换</asp:ListItem>
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
            <div class="foot_btn_box">
                <asp:Button ID="btnSubmit" runat="server" Text="提交保存" CssClass="btnSubmit" OnClick="btnSubmit_Click" />
                &nbsp;<input name="重置" type="reset" class="btnSubmit" value="重 置" />
            </div>
        </div>
    </form>
</body>
</html>
