<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CsvUpload.aspx.cs" Inherits="ZKD.Root.Contacts.CsvUpload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>通讯录CSV导入</title>
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
        <div class="navigation">首页 &gt; 通讯录 &gt; 通讯录CSV导入</div>
        <div>
        </div>
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
                                <td style="color: red">导入数据的CSV必须标注表头，且数据从第二行开始</td>

                            </tr>
                            <tr>
                                <td></td>
                                <td style="color: red"><a href="通讯录CSV模板.csv">通讯录CSV模板下载</a></td>
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
                                    <asp:FileUpload ID="FileUpload1" runat="server" />
                                    &nbsp;</td>
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
            </table>
        </div>
    </form>
</body>
</html>
