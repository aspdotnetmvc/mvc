<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SMS_Send.aspx.cs" Inherits="ZKD.Root.SMSM.SMS_Send" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>短信发送</title>
    <link type="text/css" rel="stylesheet" href="../../scripts/ui/skins/Aqua/css/ligerui-all.css" />
    <link type="text/css" rel="stylesheet" href="../images/style.css" />
    <link type="text/css" rel="stylesheet" href="../../css/pagination.css" />
    <link type="text/css" rel="stylesheet" href="../../JavaScript/Prompt/gridview.css" />
    <link type="text/css" rel="stylesheet" href="../../css/msg.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.8.3.min.js"></script>
    <script type="text/javascript" src="../../scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="../js/function.js"></script>
    <script src="../../Script/formValidator/DateTimeMask.js" type="text/javascript"></script>
  <!--  <script src="../../Script/formValidator/datepicker/calendar.js" type="text/javascript"></script>-->
    <script src="../../Script/formValidator/datepicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../JavaScript/Prompt/ymPrompt.js"></script>
    <link rel="stylesheet" type="text/css" href="../../JavaScript/Prompt/skin/qq/ymPrompt.css" />

    <style>
        .lblMsg {
            margin-left:30px;
            font-size:large;
            color:red;
        }
    </style>

    <script type="text/javascript">
        function contentChange() {
            var idValue = $('#txt_Content').val().length;
            var id1Value = $('#txt_Content').val();
            var id2Value = $('#txt_Signature').val();
            $.ajax({
                url: 'Hand.ashx',
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
                    <div class="tel_numb line">
                        <span class="tit_text">输入电话号码：</span><input type="text" id="txt_phone" style="width: 700px" runat="server" maxlength="1199" />
                    </div>
                    <div class="tel_numb line">
                        <span class="tit_text">或批量导入：</span>
                        <asp:FileUpload ID="FileUpload1" runat="server" Width="260px" />
                        <asp:Button ID="btn_import" runat="server" Text="批量导入" OnClick="btn_import_Click" Width="100px" />
                    </div>
                    <div>
                        <table style="margin-left: 118px; width: 25%">
                            <tr>
                                <td>
                                    <asp:CheckBoxList ID="CheckBoxList1" runat="server" RepeatColumns="10" RepeatDirection="Horizontal" DataTextField="phone" DataValueField="ID"></asp:CheckBoxList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <br />  
                    <div class="lblMsg">
                    <asp:Label ID="lblMsg" runat="server" Text="" ></asp:Label>
                     </div>
                    <br />
                    <div class="msg_con line">
                        <div class="msg_text" style="margin-top: 60px"><span class="tit_text">短信内容：</span></div>
                        <div class="msg_area" style="margin-top: 60px">
                            <textarea runat="server" id="txt_Content" rows="50" onchange="contentChange();"></textarea>当前输入的字符数是: <span style="color: red;">
                                <asp:Label ID="p1" Style="color: red; font-weight: bolder" runat="server">0</asp:Label></span> 要发送的短信条数为: <span style="color: red">
                                    <asp:Label ID="div2" Style="color: red; font-weight: bolder" runat="server">0</asp:Label></span> 条
                        </div>
                        <div class="msg_area" style="margin-left: 100px; margin-top: 0px">
                            <h5 style="color: red">注意：</h5>
                          <%--  1.	单次提交最多不超过100个号码--%>
                            <br />
                            1.	多个手机号码请以英文逗号隔开
                            <br />
                            2.	短信内容最多为 500 字，不包含签名。编辑短信内容时不要添加签名，系统预设，发送短信时自动添加
                            <br />
                            3.     短信计费长度含：内容+追加的字符+签名
                            <br />
                            4. 计费条数指当前信息是按几条短信计费<br />
                            5.	普通短信计费条数为 70 字，长短信为 67 字<br />
                            6. 按行业标准，短信发送最迟72小时后准确显示发送结果。
                        </div>
                    </div>

                    <%--                    <div class="signature_pass line">
                        <span class="tit_text">企业签名：</span><input type="text" id="signature" runat="server"/>&nbsp;&nbsp; 签名系统预设，不允许发送时修改
                    </div>--%>

                    <div class="time_pass line">
                        <span class="tit_text">定时发送：</span><input type="text" id="txt_SendTime" runat="server" onclick="WdatePicker({ dateFmt: 'yyyy-MM-dd HH:mm:ss' });" />&nbsp;&nbsp; 不选择时间，默认立即发送
                    </div>


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
