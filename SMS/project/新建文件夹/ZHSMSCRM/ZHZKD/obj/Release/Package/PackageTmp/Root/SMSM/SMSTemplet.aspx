<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SMSTemplet.aspx.cs" Inherits="ZKD.Root.SMSM.SMSTemplet" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>短信模板提交</title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <link type="text/css" rel="stylesheet" href="../../JavaScript/Prompt/gridview.css" />
    <link type="text/css" rel="stylesheet" href="../../css/msg.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.8.3.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script src="../../Script/formValidator/DateTimeMask.js" type="text/javascript"></script>
    <%--<script src="../../Script/formValidator/datepicker/calendar.js" type="text/javascript"></script>--%>
    <script src="../../Script/formValidator/datepicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
</head>
<body>
    <form id="frmSMSTemplet" runat="server">
        <div class="navigation">首页 &gt; 短信管理 &gt; 短信模板</div>
        <div class="tab_con" style="display: block;">
            <div class="msg_box">
                <br />
                <div class="msg_con line">
                    <div class="msg_text" style="margin-top: 60px"><span class="tit_text">短信模板：</span></div>
                    <div class="msg_area" style="margin-top: 60px">
                        <textarea runat="server" id="txt_TempletContent" rows="50" style="width:600px" ></textarea>
                        <p style="color:red">模板举例：亲爱的会员******，您好。您本次登录的密码是******，*分钟内有效。</p>
                    </div>
                </div>
                <div align="center">
                    <asp:Button ID="btnSubmit" runat="server" Text="提交" CssClass="btnSubmit" OnClick="btnSubmit_Click" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnReset" Text="重置" runat="server" CssClass="btnSubmit" OnClick="btnReset_Click" />
                </div>
                <div></div>
                <div>模板备案列表</div>
                <div>
                    <asp:GridView ID="gvSMSTemplet" runat="server" CssClass="gridview" AutoGenerateColumns="False" DataKeyNames="TempletID"
                        Width="90%" OnRowDataBound="gvSMSTemplet_RowDataBound" AllowPaging="True" OnDataBound="gvSMSTemplet_DataBound"
                        OnPageIndexChanging="gvSMSTemplet_PageIndexChanging" PageSize="15" OnRowCommand="gvSMSTemplet_RowCommand" OnRowDeleting="gvSMSTemplet_RowDeleting">
                        <PagerTemplate>
                            <table>
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="MessageLabel" ForeColor="black" Text="页码:" runat="server" />
                                        <asp:DropDownList ID="PageDropDownList" AutoPostBack="true" OnSelectedIndexChanged="PageDropDownList_SelectedIndexChanged"
                                            runat="server" />
                                        <asp:LinkButton CommandName="Page" CommandArgument="First" ID="linkBtnFirst" runat="server">首页</asp:LinkButton>
                                        <asp:LinkButton CommandName="Page" CommandArgument="Prev" ID="linkBtnPrev" runat="server">上一页</asp:LinkButton>
                                        <asp:LinkButton CommandName="Page" CommandArgument="Next" ID="linkBtnNext" runat="server">下一页</asp:LinkButton>
                                        <asp:LinkButton CommandName="Page" CommandArgument="Last" ID="linkBtnLast" runat="server">尾页</asp:LinkButton>
                                    </td>
                                    <td style="width: 460px"></td>
                                    <td>
                                        <asp:Label ID="CurrentPageLabel" ForeColor="black" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </PagerTemplate>
                        <Columns>
                            <asp:BoundField DataField="TempletID" Visible="false" HeaderText="编号">
                                <ItemStyle Width="5%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="TempletContent" HeaderText="报备内容">
                                <ItemStyle Width="30%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="SubmitTime" HeaderText="报备时间" />
                            <asp:BoundField DataField="AuditTime" HeaderText="审核时间" />
                            <asp:BoundField DataField="AuditState" HeaderText="审核状态" />
                            <asp:BoundField DataField="Remark" HeaderText="备注" />
                            <asp:CommandField HeaderText="删除" ShowDeleteButton="True" DeleteText="删除">
                                <ControlStyle ForeColor="Black" />
                            </asp:CommandField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
