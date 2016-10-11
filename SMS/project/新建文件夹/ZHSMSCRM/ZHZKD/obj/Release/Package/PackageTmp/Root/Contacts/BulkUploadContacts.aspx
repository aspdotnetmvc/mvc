<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BulkUploadContacts.aspx.cs" Inherits="ZKD.Root.Contacts.BulkUploadContacts" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>批量通讯录导入</title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <link type="text/css" rel="stylesheet" href="../../JavaScript/Prompt/gridview.css" />
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
    <form id="form1" runat="server">
        <div class="navigation">首页 &gt; 通讯录 &gt; 批量通讯录导入</div>
        <div>
            <table class="form_table" cellspacing="1" cellpadding="0" width="100%" border="0">
                <tr>
                    <td>
                        <table cellspacing="0" cellpadding="0" width="100%" border="0">
                            <tr>
                                <th>模板:</th>
                                <td>
                                    <img src="1.jpg" /></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td style="color:red">导入数据的EXCEL必须标注表头，且数据从第二行开始</td>
                            </tr>
                              <tr>
                                <td></td>
                                <td style="color: red"><a href="通讯录模板.xls">通讯录模板下载</a></td>
                            </tr>
                        </table>
                    </td>

                </tr>
                <tr>

                    <td>
                        <table>
                            <tr>
                                <th>通讯录：</th>
                                <td style="width: 350px">
                                    <input id="FileExcel" style="width: 300px" type="file" size="42" name="FilePhoto" runat="server" />
                                </td>
                                <td>
                                    <asp:Button ID="BtnImport" Text="导 入" CssClass="btnSubmit" runat="server" OnClick="BtnImport_Click"></asp:Button>
                                </td>
                            </tr>
                            <tr>
                                <th>分组：</th>
                                <td style="width: 660px">
                                    <asp:DropDownList ID="dd_l" runat="server" Style="width: 34%">
                                    </asp:DropDownList>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="LblMessage" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

                <%--<tr>
                                <td colspan="2">
                                    <asp:GridView ID="GridView1" CssClass="gridview" runat="server" AutoGenerateColumns="False" 
                                        Width="100%" AllowPaging="True" 
                                         PageSize="15" OnDataBound="GridView1_DataBound" OnPageIndexChanging="GridView1_PageIndexChanging">
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
                                            <asp:BoundField DataField="AccountCode" HeaderText="企业帐号" />
                                            <asp:BoundField DataField="UserName" HeaderText="姓名" />
                                            <asp:BoundField DataField="UserBrithday" HeaderText="生日" />
                                            <asp:BoundField DataField="UserSex" HeaderText="性别" />
                                            <asp:BoundField DataField="CompanyName" HeaderText="公司" />
                                            <asp:BoundField DataField="ComPostion" HeaderText="职位" />
                                            <asp:BoundField DataField="TelPhoneNum" HeaderText="电话" />
                                            <asp:BoundField DataField="CompanyEmail" HeaderText="Email" />
                                            <asp:BoundField DataField="QQ" HeaderText="QQ" />
                                            <asp:BoundField DataField="WebChat" HeaderText="微信" />
                                            <asp:BoundField DataField="CompanyWeb" HeaderText="网站" />
                                        </Columns>
                                    </asp:GridView>
                                </td>
                            </tr>--%>
            </table>
        </div>
    </form>
</body>
</html>
