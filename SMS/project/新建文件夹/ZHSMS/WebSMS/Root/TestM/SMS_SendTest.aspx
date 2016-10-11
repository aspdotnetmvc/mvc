<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SMS_SendTest.aspx.cs" Inherits="WebSMS.Root.SMSM.SMS_SendTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>短信发送测试</title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
    <script>
        function countChars(textareaID, spanID, maxNum) {
            //得到输入的字符的长度
            var NowNum = document.getElementById(textareaID).value.length;
            document.getElementById(spanID).innerHTML = NowNum;
            var c = Math.round(NowNum / 67);
            var d = NowNum / 67;
            if (c < d) {
                c++;
            }
            document.getElementById(maxNum).innerHTML = c;
        }
        function PageInit() {
            countChars('txt_Content', 'counter', 'counter2');
        }
    </script>
</head>
<body class="mainbody" onload="PageInit()">
    <form id="form1" runat="server">
        <div id="contentTab">
            <div class="tab_con" style="display: block;">
                <table class="form_table">
                    <col width="150px">
                    <col>
                    <tbody>
                        <tr>
                            <th>电话号码：</th>
                            <td style="width: 600px">
                                <asp:TextBox ID="txt_phone" runat="server" CssClass="txtInput normal required" MaxLength="11" /><label></label>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <asp:FileUpload ID="FileUpload1" runat="server" />
                                <asp:Button ID="btn_import" runat="server" Text="增加" OnClick="btn_import_Click" />
                                <asp:Label ID="Label1" runat="server" Text="备注：选择文件为txt格式，每个号码之间用，隔开" ForeColor="#CC00FF"></asp:Label>
                            </td>
                            <td></td>
                        </tr>

                        <tr>
                            <th>短信内容：</th>
                            <td>目前已经输入<span span id="counter" style="COLOR: red; FONT-WEIGHT: bold">0</span>字 ;
                               将发送<span span id="counter2" style="COLOR: red; FONT-WEIGHT: bold">0</span>条短信；
                                <asp:TextBox ID="txt_Content" runat="server" MaxLength="25" CssClass="txtInput normal" Width="512px"
                                    TextMode="MultiLine" Height="345px" Style="WORD-BREAK: break-all" onkeydown="countChars('txt_Content','counter'，'counter2');"
                                    onkeyup="countChars('txt_Content','counter','counter2');"></asp:TextBox>
                            </td>
                            <td>
                                <asp:GridView ID="GridView1" SkinID="GridViewSkin" runat="server" AutoGenerateColumns="False"
                                    Width="95%" AllowPaging="True" OnDataBound="GridView1_DataBound"
                                    OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="15" CellPadding="3" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px">
                                    <FooterStyle BackColor="White" ForeColor="#000066" />
                                    <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
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
                                        <asp:BoundField DataField="phone" HeaderText="号码" />
                                    </Columns>
                                    <RowStyle ForeColor="#000066" />
                                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                                    <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                    <SortedAscendingHeaderStyle BackColor="#007DBB" />
                                    <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                    <SortedDescendingHeaderStyle BackColor="#00547E" />
                                </asp:GridView>
                            </td>
                        </tr>
                        <tr>
                            <th>状态报告：</th>
                            <td>
                                <asp:RadioButtonList ID="dd_StatusReport" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" Width="262px">
                                    <asp:ListItem Value="0">不发送</asp:ListItem>
                                    <asp:ListItem Value="1">发送</asp:ListItem>
                                    <asp:ListItem Value="2">推送</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <th>短信发送级别：</th>
                            <td>
                                <asp:RadioButtonList ID="dd_Level" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" Width="262px">
                                    <asp:ListItem Value="0">0</asp:ListItem>
                                    <asp:ListItem Value="1">1</asp:ListItem>
                                    <asp:ListItem Value="2">2</asp:ListItem>
                                    <asp:ListItem Value="3">3</asp:ListItem>
                                    <asp:ListItem Value="4">4</asp:ListItem>
                                    <asp:ListItem Value="5">5</asp:ListItem>
                                    <asp:ListItem Value="6">6</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <th>审核状态：</th>
                            <td>
                                <asp:RadioButtonList ID="dd_Audit" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" Width="262px">
                                    <asp:ListItem Value="1">自动审核</asp:ListItem>
                                    <asp:ListItem Value="0">人工审核</asp:ListItem>
                                    <asp:ListItem Value="2">企业鉴权</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <th>过滤方式：</th>
                            <td>
                                <asp:RadioButtonList ID="dd_ContentFilter" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" Width="262px">

                                    <asp:ListItem Value="0">替换</asp:ListItem>
                                    <asp:ListItem Value="1">审核失败</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <th>业务类型：</th>
                            <td>
                                <asp:RadioButtonList ID="dd_Busstype" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" Width="262px">
                                    <asp:ListItem Value="0">行业短信</asp:ListItem>
                                    <asp:ListItem Value="1">商业短信</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <th>签名：</th>
                            <td>
                                <asp:TextBox ID="txt_Signature" runat="server" CssClass="txtInput normal" Width="253px"></asp:TextBox>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <th style="color: red">测试数量级：</th>
                            <td>
                                <asp:TextBox ID="txt_c" runat="server" CssClass="txtInput normal required" MaxLength="10" />
                            </td>
                            <td></td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="foot_btn_box">
                <asp:Button ID="btnSubmit" runat="server" Text="发送" CssClass="btnSubmit" OnClick="btnSubmit_Click" />
                &nbsp;<input name="重置" type="reset" class="btnSubmit" value="重 置" />
            </div>
        </div>
    </form>
</body>
</html>
