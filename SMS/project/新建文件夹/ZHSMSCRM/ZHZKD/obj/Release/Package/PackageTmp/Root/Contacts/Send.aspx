<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Send.aspx.cs" Inherits="ZKD.Root.Contacts.Send" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>短信发送</title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <link type="text/css" rel="stylesheet" href="../../css/msg.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.8.3.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script src="../../Script/formValidator/DateTimeMask.js" type="text/javascript"></script>
    <%--<script src="../../Script/formValidator/datepicker/calendar.js" type="text/javascript"></script>--%>
    <script src="../../Script/formValidator/datepicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />
    <script type="text/javascript">
        function contentChange() {
            var idValue = $('#txt_Content').val().length;
            var id1Value = $('#txt_Content').val();
            var id2Value = $('#txt_Signature').val();
            $.ajax({
                url: 'SendHandler.ashx',
                type: 'POST',
                data: { id: id1Value, id2: id2Value },
                success: function (json) {
                    $('#p1').html(idValue);
                    $('#div2').html(json);
                },
                error: function (json) {
                    $('#div2').html(json);
                }
            });
        }
    </script>

</head>
<body class="mainbody">
    <form id="form1" runat="server">

        <div class="navigation">首页 &gt; 短信管理 &gt; 短信发送</div>
        <div id="contentTab">
            <br />
            <div class="tab_con" style="display: block;">
                <div class="msg_box">

                    <div class="msg_con line">

                        <div class="msg_text"><span class="tit_text">电话号码：</span></div>
                        <div class="tit_text">
                            <asp:CheckBoxList ID="CheckBoxList1" runat="server" RepeatColumns="10" RepeatDirection="Horizontal" DataTextField="phone" DataValueField="ID"></asp:CheckBoxList>
                        </div>
                    </div>
                    <br />
                    <div class="msg_con line">

                        <div class="msg_text" style="margin-top: 60px"><span class="tit_text">短信内容：</span></div>
                        <div class="msg_area" style="margin-top: 60px">
                            <textarea runat="server" id="txt_Content" onchange="contentChange();"></textarea>当前输入的字符数是: <span style="color: red;">
                                <asp:Label ID="p1" Style="color: red; font-weight: bolder" runat="server">0</asp:Label></span> 要发送的短信条数为: <span style="color: red">
                                    <asp:Label ID="div2" Style="color: red; font-weight: bolder" runat="server">0</asp:Label></span> 条
                        </div>
                        <div class="msg_area" style="margin-left: 100px; margin-top: 0px">
                            <h5 style="color: red">注意：</h5>
                            1.	单次提交最多不超过1000个号码
                            <br />
                            2.	多个手机号码请以英文逗号隔开
                            <br />
                            3.	短信内容最多为 500 字,不包含签名
                            <br />
                            4.     短信计费长度=内容+追加的字符+签名
                            <br />
                            5.	计费条数指当前信息是按几条短信计费，
                            <br />
                            6.	普通短信计费条数为 70 字，长短信为 67 字
                        </div>
                    </div>

                    <div class="time_pass line"><span class="tit_text">定时发送：</span><input type="text" id="txt_SendTime" runat="server" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})" />&nbsp;&nbsp; 不选择时间，默认立即发送</div>

                    <div class="url line"><span class="tit_text">WapURL：</span><input type="text" id="txt_wapURL" runat="server" />地址为空，不发送WAP推送信息</div>
                    <div align="center">
                        <asp:Button ID="btnSubmit" runat="server" Text="发送" CssClass="btnSubmit" OnClick="btnSubmit_Click" />
                        &nbsp;<input name="重置" type="reset" class="btnSubmit" value="重 置" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
