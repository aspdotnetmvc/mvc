<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChannelGateway.aspx.cs" Inherits="ZHSMSPlatform.Root.GatewayConfig.ChannelGateway" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>通道网关绑定</title>
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
        <div class="navigation">
            <a href="javascript:history.go(-1);" class="back">后退</a>首页 &gt; 通道管理 &gt;通道网关绑定
        </div>
        <div id="contentTab">
            <ul class="tab_nav">
                <li class="selected"><a onclick="tabs('#contentTab',0);" href="javascript:;">网关绑定</a></li>
            </ul>
            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <tr>
                        <th>通道编号：</th>
                        <td>
                            <asp:Label ID="lbl_channelID" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <th>通道名称：</th>
                        <td>
                            <asp:Label ID="lbl_channelName" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <th>可用网关：</th>
                        <td>
                            <asp:DataList ID="DataListGateway" runat="server" Width="688px" OnItemDataBound="DataListGateway_ItemDataBound">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_operators" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"Code") %>'
                                        Visible="False"></asp:Label>
                                    <asp:Label ID="lbl_title" runat="server" ForeColor="#000" Width="100%" Font-Bold="true"
                                        Text='<%# DataBinder.Eval(Container.DataItem,"Name")%>'></asp:Label>
                                    <asp:CheckBoxList ID="Application_ID" runat="server" Visible="true" RepeatColumns="5">
                                    </asp:CheckBoxList>
                                    <div style="margin-left: 15px">
                                        <asp:Label ID="lbl_message" runat="server" Visible="False" ForeColor="Red" Width="260"></asp:Label>
                                    </div>
                                </ItemTemplate>
                            </asp:DataList>
                        </td>
                    </tr>
                </table>
                <div class="foot_btn_box">
                    <asp:Button ID="btnSubmit" runat="server" Text="提交保存" CssClass="btnSubmit" OnClick="btnSubmit_Click" />
                    &nbsp;<input name="重置" type="reset" class="btnSubmit" value="重 置" />
                </div>
            </div>
        </div>
    </form>
</body>
