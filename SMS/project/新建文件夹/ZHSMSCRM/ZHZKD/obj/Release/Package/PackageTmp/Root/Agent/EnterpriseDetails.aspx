<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EnterpriseDetails.aspx.cs" Inherits="ZKD.Root.Agent.EnterpriseDetails" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>企业用户详情</title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.8.3.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script src="../../Script/formValidator/DateTimeMask.js" type="text/javascript"></script>
<%--    <script src="../../Script/formValidator/datepicker/calendar.js" type="text/javascript"></script>--%>
    <script src="../../Script/formValidator/datepicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
</head>
<body class="mainbody">
    <form id="form1" runat="server">
        <div class="navigation">
            <a href="javascript:history.go(-1);" class="back">后退</a>首页 &gt; 企业用户管理 &gt; 企业开户
        </div>
        <div id="contentTab">
            <ul class="tab_nav">
                <li class="selected"><a onclick="tabs('#contentTab',0);" href="javascript:;">企业资料</a></li>
                <li><a onclick="tabs('#contentTab',1);" href="javascript:;">企业设置</a></li>
                <li><a onclick="tabs('#contentTab',2);" href="javascript:;">企业短信设置</a></li>
            </ul>

            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>企业名称：</th>
                            <td>
                                <asp:Label ID="lbl_name" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>企业联系人：</th>
                            <td>
                                <asp:Label ID="lbl_contact" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>联系电话：</th>
                            <td>
                                <asp:Label ID="lbl_phone" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>地区：</th>
                            <td>
                                <asp:DropDownList ID="dd_province" runat="server" AutoPostBack="True"></asp:DropDownList>
                                <asp:DropDownList ID="dd_city" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <th>详细地址：</th>
                            <td>
                                <asp:Label ID="lbl_address" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="tab_con" style="display: none;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>分配企业帐号：</th>
                            <td>
                                <asp:Label ID="lbl_account" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>接入号码：</th>
                            <td>
                                <asp:Label ID="lbl_spNumber" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>企业优先级：</th>
                            <td>
                                <asp:Label ID="lbl_accountLevel" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>审核方式：</th>
                            <td>
                                <asp:Label ID="lbl_accountAudit" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>是否启用：</th>
                            <td>
                                <asp:Label ID="lbl_accountEnable" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="tab_con" style="display: none;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>状态报告接收方式：</th>
                            <td>
                                <asp:Label ID="lbl_smsReport" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>短信级别：</th>
                            <td>
                                <asp:Label ID="lbl_smsLevel" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>短信内容过滤方式：</th>
                            <td>
                                <asp:Label ID="lbl_smsFilter" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th>短信发送通道：</th>
                            <td>
                                <asp:Label ID="lbl_smsChannel" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </form>
</body>
</html>

