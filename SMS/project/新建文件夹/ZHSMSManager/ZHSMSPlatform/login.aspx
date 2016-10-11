<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="ZHCRM.login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>短信平台管理员登录</title>
    <link href="css/load1.css" type="text/css" rel="Stylesheet" />
    <script type="text/javascript" src="scripts/jquery/jquery-1.8.3.min.js"></script>
    <script type="text/javascript" src="scripts/jquery/jquery.validate.min.js"></script>
    <script type="text/javascript" src="scripts/jquery/messages_cn.js"></script>
    <script type="text/javascript" src="scripts/ui/js/ligerBuild.min.js"></script>
    <script type="text/javascript" src="Root/js/function.js"></script>
    <script type="text/javaScript">
        if (window != top)
            top.location.href = location.href;
    </script>
    <script type="text/javascript">
        //表单验证
        $(function () {
            //检测IE
            if ($.browser.msie && $.browser.version == "6.0") {
                window.location.href = 'ie6update.html';
            }
            $('#txtUserName').focus();
            //$("#form1").validate({
            //    errorPlacement: function (lable, element) {
            //        element.ligerTip({ content: lable.html(), appendIdTo: lable });
            //    },
            //    success: function (lable) {
            //        lable.ligerHideTip();
            //    }
            //});
        });
    </script>
    <script type="text/javascript">
        $(function () {
            $("#num").focus(function () {
                if ($(this).val() === "帐号") {
                    $(this).val("");
                    $(this).removeClass("input_txt");
                }
            });
            $("#num").blur(function () {
                if ($(this).val() === "帐号" || $(this).val() === "") {
                    $(this).val("帐号");
                    $(this).addClass("input_txt");
                }
            });

            $("#psw").focus(function () {
                if ($(this).val() === "密码") {
                    $(this).val("");
                    $(this).removeClass("input_txt");
                }
            });
            $("#psw").blur(function () {
                if ($(this).val() === "密码" || $(this).val() === "") {
                    $(this).val("密码");
                    $(this).addClass("input_txt");
                }
            });

            $("#code").focus(function () {
                if ($(this).val() === "验证码") {
                    $(this).val("");
                    $(this).removeClass("input_txt");
                }
            });
            $("#code").blur(function () {
                if ($(this).val() === "验证码" || $(this).val() === "") {
                    $(this).val("验证码");
                    $(this).addClass("input_txt");
                }
            });

            if ($(window).height() <= 768) {
                $(".top").css("height", "80px");
            }
        });
    </script>
</head>
<body class="loginbody">
    <form id="form1" runat="server">
        <div class="top"></div>
        <div class="middle">
            <div class="load_con">
                <div class="load_box">
                    <input type="text" runat="server" class="input_txt" id="num" value="帐号" />
                    <input type="password" class="input_txt" runat="server" id="psw" value="密码" />
                    <input type="text" class="input_txt" runat="server" id="code" value="验证码" maxlength="6" />
                    <div class="code_img">
                        <img src="../tools/verify_code.ashx" width="80" height="42" alt="点击切换验证码" title="点击切换验证码" style="margin-top: 0px; vertical-align: top; cursor: pointer;" onclick="ToggleCode(this, '../tools/verify_code.ashx');return false;" />
                    </div>
                    <div class="clear"></div>
                    <label class="rem">
                        <input type="checkbox" id="cbRememberId" runat="server" style="vertical-align: middle;" />记住密码</label>
                    <%--   <input type="submit" id="sub" runat="server" value="登录" />--%>
                    <div>
                        <asp:Button ID="btnSubmit" runat="server" Text="登 录" OnClick="btnSubmit_Click" CssClass="sub" />
                    </div>
                    <br />
                    <div>
                        <asp:Label ID="lblTip" runat="server" Text="请输入用户名及密码" Visible="False" CssClass="sub" BackColor="Red" />
                    </div>
                </div>
            </div>
        </div>
        <div class="bottom">copyright© 版权所有 · 中国 · 众呼</div>
    </form>
</body>
</html>
