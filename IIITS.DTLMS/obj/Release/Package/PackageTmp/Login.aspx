<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="IIITS.DTLMS.Login" %>

<html>


<head runat="server">
    <title></title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta content="width=device-width, initial-scale=1.0" name="viewport" />
    <meta http-equiv="Content-type" content="text/html; charset=utf-8">
    <meta content="" name="description" />
    <meta content="" name="author" />
    <!-- BEGIN GLOBAL MANDATORY STYLES -->
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.min.css" />   
    <link href="//netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap-glyphicons.css" rel="stylesheet">  
    <link href="js/bootsrap1.css" rel="stylesheet" />
     <link href="Styles/LoginPage/uniform.default.css" rel="stylesheet" type="text/css" />
    <!-- END GLOBAL MANDATORY STYLES -->
    <!-- BEGIN PAGE LEVEL STYLES -->
    <link href="Styles/LoginPage/login-soft.css" rel="stylesheet" type="text/css" />
    <!-- END PAGE LEVEL SCRIPTS -->
    <!-- BEGIN THEME STYLES -->
    <link href="Styles/LoginPage/components-rounded.css" id="style_components" rel="stylesheet" type="text/css" />
    <link href="Styles/LoginPage/plugins.css" rel="stylesheet" type="text/css" />
    <%-- <link href="Styles/LoginPage/layout.css" rel="stylesheet" type="text/css" />--%>
    <link href="Styles/LoginPage/default.css" rel="stylesheet" type="text/css" id="style_color" />
    <%--<link href="Styles/LoginPage/custom.css" rel="stylesheet" type="text/css"/>--%>
    <!-- END THEME STYLES -->
    <link rel="shortcut icon" href="img/Bescom.jpg" />
    <script src="js/jquery1.js"></script>
    <script src="js/bootstrap1.js"></script>
    <script src="js/particle.js"></script> 
    <!-- END CORE PLUGINS -->
    <!-- BEGIN PAGE LEVEL PLUGINS -->
    <script src="Styles/LoginPage/jquery.validate.min.js" type="text/javascript"></script>
    <!-- END PAGE LEVEL PLUGINS -->
    <!-- BEGIN PAGE LEVEL SCRIPTS -->
    <script src="Styles/LoginPage/metronic.js" type="text/javascript"></script>
    <script src="Styles/LoginPage/layout.js" type="text/javascript"></script>
    <script src="Styles/LoginPage/demo.js" type="text/javascript"></script>
    <script src="Styles/LoginPage/login-soft.js" type="text/javascript"></script>
    <script type="text/javascript">
        history.pushState(null, null, location.href);
        window.onpopstate = function () {
            history.go(1);
        };
    </script>
    <!-- END PAGE LEVEL SCRIPTS -->
    <script type="text/javascript">
        jQuery(document).ready(function () {
            Metronic.init(); // init metronic core components
            Layout.init(); // init current layout
            Login.init();
            Demo.init();
            // init background slide images
            //    $.backstretch([
            //    "Styles/LoginPage/DTC.jpg",
            //    "Styles/LoginPage/DT-4.jpg",
            //    "Styles/LoginPage/DT-6.jpg",
            //    "Styles/LoginPage/T1.jpg"
            //    ], {
            //        fade: 1000,
            //        duration: 8000
            //    }
            //);
        });

    </script>
        <script type="text/javascript">
        jQuery(document).ready(function () {

            $('input').on('input', function () {
                $(this).val($(this).val().replace(/[^a-z0-9!@_+=*&%.()]/gi, ''));
            });           
            // init background slide images
            //    $.backstretch([
            //    "Styles/LoginPage/DTC.jpg",
            //    "Styles/LoginPage/DT-4.jpg",
            //    "Styles/LoginPage/DT-6.jpg",
            //    "Styles/LoginPage/T1.jpg"
            //    ], {
            //        fade: 1000,
            //        duration: 8000
            //    }
            //);
        });

    </script>

    <script type="text/javascript"> var message = ''; 
function clickIE() { if (event.button == 2) {  return false; } } 
function clickNS(e) { 
if (document.layers || (document.getElementById && !document.all)) { 
if (e.which == 2 || e.which == 3) {  return false; } 
} 
} 
if (document.layers) { document.captureEvents(Event.MOUSEDOWN); document.onmousedown = clickNS; } 
else if (document.all && !document.getElementById) { document.onmousedown = clickIE; } 
document.oncontextmenu = new Function('return false') </script>

    <style>
        body {
            background: #0264d6; /* Old browsers */
            background: -moz-radial-gradient(center, ellipse cover, #0264d6 1%, #1c2b5a 100%); /* FF3.6+ */
            background: -webkit-gradient(radial, center center, 0px, center center, 100%, color-stop(1%,#0264d6), color-stop(100%,#1c2b5a)); /* Chrome,Safari4+ */
            background: -webkit-radial-gradient(center, ellipse cover, #0264d6 1%,#1c2b5a 100%); /* Chrome10+,Safari5.1+ */
            background: -o-radial-gradient(center, ellipse cover, #0264d6 1%,#1c2b5a 100%); /* Opera 12+ */
            background: -ms-radial-gradient(center, ellipse cover, #0264d6 1%,#1c2b5a 100%); /* IE10+ */
            background: radial-gradient(ellipse at center, #0264d6 1%,#1c2b5a 100%); /* W3C */
            filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#0264d6', endColorstr='#1c2b5a',GradientType=1 ); /* IE6-9 fallback on horizontal gradient */
            height: calc(100vh);
            width: 100%;
            overflow: hidden!important;
        }

        .form-group {
            position: relative;
        }

            .form-group input[type="password"] {
                padding-right: 30px;
            }

            .form-group .glyphicon {
                right: 6px;
                position: absolute;
                top: 12px;
            }
    </style>
    <style>
        .ResetPwd-form {
            margin-top: -59px!important;
        }

        input#btnResetPwd {
            margin-top: -58px;
        }

        canvas {
            z-index: -1;
            top: 0;
            left: 0;
        }

        #particles-js {
            width: 100%;
            height: 100%;
            position: absolute;
            opacity: 0.3;
          
        }
        div#Form2 {
            border-right:1px solid #fff!important;
            padding: 9px 45px 9px 0px!important;
        }
        .login .content .Reset-password {
            margin-left: -37%!important;
            }
    </style>
    <script type="text/javascript">

        function ValidateMyForm() {
            if (document.getElementById('<%= txtUsername.ClientID %>').value.trim() == "") {
                alert('Enter Valid User Name')
                document.getElementById('<%= txtUsername.ClientID %>').focus()
            return false
        }
        if (document.getElementById('<%= txtPassword.ClientID %>').value.trim() == "") {
                alert('Enter Valid Password')
                document.getElementById('<%= txtPassword.ClientID %>').focus()
            return false
        }
    }

    function ValidateNumber() {
        if (document.getElementById('<%= txtEmail.ClientID %>').value.trim() == "") {
            alert('Please Enter Register Mail Id / Mobile number to get OTP')
            document.getElementById('<%= txtEmail.ClientID %>').focus()
            return false
        }
    }

    function ValidatePassword() {

        if (document.getElementById('<%= txtOTP.ClientID %>').value.trim() == "") {
            alert('Enter Valid OTP')
            document.getElementById('<%= txtOTP.ClientID %>').focus()
            return false
        }

        if (document.getElementById('<%= txtNewpwd.ClientID %>').value.trim() == "") {
            alert('Enter Valid Password')
            document.getElementById('<%= txtNewpwd.ClientID %>').focus()
            return false
        }
        if (document.getElementById('<%= txtCnfrmPwd.ClientID %>').value.trim() == "") {
            alert('Enter Valid Confirmation Password')
            document.getElementById('<%= txtCnfrmPwd.ClientID %>').focus()
            return false
        }

        if (document.getElementById('<%= txtNewpwd.ClientID %>').value.trim() != "") {
            var pass = document.getElementById('<%= txtNewpwd.ClientID %>').value
            //if (!pass.match(/^(?=.{8,})(?=.*[a-zA-Z])(?=.*[0-9])(?=.*[#!*()$%^&+-={}@@]).*$/)) {
            if (!pass.match(/^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,12}$/)) {
                alert("Password Length Should 8 Character and  contains at least 1 Capital Letter or 1 Small Letter,1 Digit, 1 Special Character")
                document.getElementById('<%=txtNewpwd.ClientID %>').focus()
                return false;
            }

        }

        if (document.getElementById('<%= txtCnfrmPwd.ClientID %>').value.trim() != "") {
            var pass = document.getElementById('<%= txtCnfrmPwd.ClientID %>').value
                     if (!pass.match(/^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,12}$/)) {
                         alert("Password Length Should 8 Character and  contains at least 1 Capital Letter or 1 Small Letter,1 Digit, 1 Special Character")
                         document.getElementById('<%=txtCnfrmPwd.ClientID %>').focus()
                return false;
            }

        }
    }

    </script>

</head>

<body class="login">
     
    <div id="particles-js">
        <canvas height="651" width="1366" style="width: 100%; height: 100%;"></canvas>
    </div>
  <div class="logo" style="color: antiquewhite; font-weight: bold">
            <h1 style="text-transform:capitalize">Distribution Transformer LifeCycle 
                <br />Management Software</h1>
            <%--<a href="index.html">
	<img src="../../assets/admin/layout4/img/logo-big.png" alt=""/>
	</a>--%>
        </div>
    <form id="form1" runat="server">
        
        <div class="menu-toggler sidebar-toggler">
        </div>
        <!-- END SIDEBAR TOGGLER BUTTON -->
        <!-- BEGIN LOGIN -->
        <div>
        <div class="content">
            <div class="row">
            <!-- BEGIN LOGIN FORM -->
                <div class="col-md-10">
            <div id="Form2" class="login-form" runat="server">
               <%-- <h3 class="form-title" style="font-weight: bold">Login to your account</h3>--%>
                <div class="alert alert-danger display-hide">
                    <button class="close" data-close="alert"></button>
                    <span>Enter any username and password. </span>
                </div>
                <div style=" margin-left:-40%;"class="form-group">
                    <!--ie8, ie9 does not support html5 placeholder, so we just show field title for that-->
                    <label class="control-label visible-ie8 visible-ie9">Login Type</label>
                    <div class="input-icon">
                        <i style="color:#000"class="fa fa-user"></i>

                        <asp:TextBox ID="txtUsername" autocomplete="off"  class="form-control placeholder-no-fix" placeholder="User Name" runat="server"></asp:TextBox>
                    </div>
                </div>
                <div style=" margin-left:-40%;"class="form-group">
                    <label class="control-label visible-ie8 visible-ie9">Section Office</label>
                    <div class="input-icon">

                        <i style="color:#000"class="fa fa-lock"></i>
                        <asp:TextBox ID="txtPassword"   class="form-control placeholder-no-fix" placeholder="Password" MaxLength="15" runat="server" TextMode="Password"></asp:TextBox>
                        <span class="glyphicon glyphicon-eye-open"></span>
                    </div>
                </div>

                <div class="form-actions form-group">

                    <asp:Button ID="cmdLogin" runat="server" Text="Login" OnClientClick="javascript:return ValidateMyForm()"
                        class="btn blue pull-right " Style="z-index: 9999999!important" OnClick="cmdLogin_Click" />



                    <%--<div class="forget-password" runat="server" id="dvForgtPwd">		
			<p>
				 <a href="javascript:;" id="forget-password" style="color:White" >
				Forgot your password ? </a>
				
			</p>
		  </div>--%>

                    <div class="Reset-password" runat="server" id="dvResetPwd">
                        <p>
                            <a href="javascript:;" id="Reset-password" style="color: White;font-size: 11px;z-index: 9999999!important">Forgot your password ?  </a>

                        </p>
                    </div>
                    <asp:Label ID="lblMsg" runat="server" ForeColor="Red"></asp:Label>
                </div>
            </div>
                </div>
                <div class="col-md-2">
            <div class="logooo">
	  <img style="width:114px;border-bottom-right-radius: 29px;border-top-left-radius:29px;margin-top:18px;box-shadow: 0px 3px 8px 6px #888888" src="img/bescomlogo.jpg"/>
       <div class="clearfix"></div>
      </div>
                 </div>
                </div>
            <!-- END LOGIN FORM -->

            <!-- BEGIN FORGOT PASSWORD FORM -->

            <%--<div class="forget-form" action="index.html" method="post">
		<h3>Forget Password ?</h3>
		<p>
			 Enter your e-mail ID / Mobile No. below to reset your password.
		</p>
		<div class="form-group">
			<div class="input-icon">
				<i class="fa fa-envelope"></i>
                 <asp:TextBox ID="txtEmail" class="form-control placeholder-no-fix" placeholder="Email"  runat="server"></asp:TextBox>
				
			</div>
             <asp:Label ID="lblFMsg" runat="server" ForeColor="Red" ></asp:Label>
		</div>
		<div class="form-actions">
			<button type="button" id="back-btn" class="btn">
			<i class="m-icon-swapleft"></i> Back </button>
            <asp:Button ID="cmdFSave" runat="server" Text="Submit" 
                class="btn blue pull-right " onclick="cmdFSave_Click" />
			
		</div>
	</div>--%>

            <!-- END FORGOT PASSWORD FORM -->
        </div>
        <div class="content form-group">
            <!-- BEGIN FORGOT PASSWORD BY OTP FORM -->
            <div id="ResetPwd" runat="server" class="ResetPwd-form" action="index.html" method="post">

                <h3>Forget Password ?</h3>
                <p>
                    Enter your e-mail ID / Mobile No. below to reset your password.
                </p>

                <div class="form-group">
                    <div class="input-icon">
                        <i class="fa fa-envelope"></i>
                        <asp:TextBox ID="txtEmail" class="form-control placeholder-no-fix" placeholder="Email/Mobile No" autocomplete="off" runat="server"></asp:TextBox>

                    </div>
                    <asp:Label ID="lblFMsg" runat="server" ForeColor="Red"></asp:Label>
                </div>
                <div class="form-actions">
                    <%--<button type="button" id="back-btn" class="btn">
			<i class="m-icon-swapleft"></i> Back </button>--%>
                    <div class="Reset-password" runat="server" id="Div2">
                        <asp:Button ID="cmdFSave" runat="server" Text="Get OTP"
                            class="btn blue pull-right " OnClick="cmdFSave_Click" OnClientClick="javascript:return ValidateNumber()" />
                    </div>
                    <%--  <i class="m-icon-swapright m-icon-white"></i>--%>
                </div>

                <%--<h3 class="form-title" style="font-weight:bold">Reset Password</h3>--%>
                <p>
                    Enter OTP
                </p>
                <div class="form-group">
                    <div class="input-icon">
                        <i class="fa fa-user"></i>
                        <asp:TextBox ID="txtOTP" class="form-control placeholder-no-fix" autocomplete="off" placeholder="Enter OTP" MaxLength="9" runat="server"></asp:TextBox>

                    </div>
                    <%--<asp:Label ID="Label1" runat="server" ForeColor="Red" ></asp:Label>--%>
                </div>

                <p>
                    Enter New Password
                </p>
                <div class="form-group">
                    <div class="input-icon">
                        <i class="fa fa-user"></i>
                        <asp:TextBox ID="txtNewpwd" class="form-control placeholder-no-fix" autocomplete="off" TextMode="Password" placeholder="New Password" MaxLength="15" runat="server"></asp:TextBox>

                    </div>
                </div>
                <p>
                    Enter Confirm New Password
                </p>
                <div class="form-group">
                    <div class="input-icon">
                        <i class="fa fa-envelope"></i>
                        <asp:TextBox ID="txtCnfrmPwd" class="form-control placeholder-no-fix" autocomplete="off" TextMode="Password" MaxLength="15" placeholder="Confirm New Password" runat="server"></asp:TextBox>

                    </div>
                </div>
                <div class="form-actions">
                    <%--<button type="button" id="Reset-back-btn" class="btn">
			<i class="m-icon-swapleft"></i> Back </button>--%>
                    <button type="button" id="back-btn" class="btn">
                        <i class="m-icon-swapleft"></i>Back
                    </button>
                    <div class="Reset-password" runat="server" id="Div1">
                        <asp:Button ID="btnResetPwd" runat="server" Text="Reset Password" OnClientClick="javascript:return ValidatePassword()"
                            class="btn blue pull-right " OnClick="btnResetPwd_Click" />
                    </div>
                    <%--  <i class="m-icon-swapright m-icon-white"></i>--%>
                </div>
            </div>
            <!-- END FORGOT PASSWORD BY OTP FORM -->
        </div>
             
        </div>
       
        <!-- END LOGIN -->
        <!-- BEGIN COPYRIGHT -->
        <div style="color:#fff;font-size:16px"class="copyright">
            2023 &copy; Idea Infinity IT Solutions (P) Ltd.
        </div>

    </form>
    <script type="text/javascript">
        $(document).ready(function () {
            function launchParticlesJS(a, e) { var i = document.querySelector("#" + a + " > canvas"); pJS = { canvas: { el: i, w: i.offsetWidth, h: i.offsetHeight }, particles: { color: "#fff", shape: "circle", opacity: 1, size: 2.5, size_random: true, nb: 200, line_linked: { enable_auto: true, distance: 100, color: "#fff", opacity: 1, width: 1, condensed_mode: { enable: true, rotateX: 65000, rotateY: 65000 } }, anim: { enable: true, speed: 1 }, array: [] }, interactivity: { enable: true, mouse: { distance: 100 }, detect_on: "canvas", mode: "grab", line_linked: { opacity: 1 }, events: { onclick: { enable: true, mode: "push", nb: 4 } } }, retina_detect: false, fn: { vendors: { interactivity: {} } } }; if (e) { if (e.particles) { var b = e.particles; if (b.color) { pJS.particles.color = b.color } if (b.shape) { pJS.particles.shape = b.shape } if (b.opacity) { pJS.particles.opacity = b.opacity } if (b.size) { pJS.particles.size = b.size } if (b.size_random == false) { pJS.particles.size_random = b.size_random } if (b.nb) { pJS.particles.nb = b.nb } if (b.line_linked) { var j = b.line_linked; if (j.enable_auto == false) { pJS.particles.line_linked.enable_auto = j.enable_auto } if (j.distance) { pJS.particles.line_linked.distance = j.distance } if (j.color) { pJS.particles.line_linked.color = j.color } if (j.opacity) { pJS.particles.line_linked.opacity = j.opacity } if (j.width) { pJS.particles.line_linked.width = j.width } if (j.condensed_mode) { var g = j.condensed_mode; if (g.enable == false) { pJS.particles.line_linked.condensed_mode.enable = g.enable } if (g.rotateX) { pJS.particles.line_linked.condensed_mode.rotateX = g.rotateX } if (g.rotateY) { pJS.particles.line_linked.condensed_mode.rotateY = g.rotateY } } } if (b.anim) { var k = b.anim; if (k.enable == false) { pJS.particles.anim.enable = k.enable } if (k.speed) { pJS.particles.anim.speed = k.speed } } } if (e.interactivity) { var c = e.interactivity; if (c.enable == false) { pJS.interactivity.enable = c.enable } if (c.mouse) { if (c.mouse.distance) { pJS.interactivity.mouse.distance = c.mouse.distance } } if (c.detect_on) { pJS.interactivity.detect_on = c.detect_on } if (c.mode) { pJS.interactivity.mode = c.mode } if (c.line_linked) { if (c.line_linked.opacity) { pJS.interactivity.line_linked.opacity = c.line_linked.opacity } } if (c.events) { var d = c.events; if (d.onclick) { var h = d.onclick; if (h.enable == false) { pJS.interactivity.events.onclick.enable = false } if (h.mode != "push") { pJS.interactivity.events.onclick.mode = h.mode } if (h.nb) { pJS.interactivity.events.onclick.nb = h.nb } } } } pJS.retina_detect = e.retina_detect } pJS.particles.color_rgb = hexToRgb(pJS.particles.color); pJS.particles.line_linked.color_rgb_line = hexToRgb(pJS.particles.line_linked.color); if (pJS.retina_detect && window.devicePixelRatio > 1) { pJS.retina = true; pJS.canvas.pxratio = window.devicePixelRatio; pJS.canvas.w = pJS.canvas.el.offsetWidth * pJS.canvas.pxratio; pJS.canvas.h = pJS.canvas.el.offsetHeight * pJS.canvas.pxratio; pJS.particles.anim.speed = pJS.particles.anim.speed * pJS.canvas.pxratio; pJS.particles.line_linked.distance = pJS.particles.line_linked.distance * pJS.canvas.pxratio; pJS.particles.line_linked.width = pJS.particles.line_linked.width * pJS.canvas.pxratio; pJS.interactivity.mouse.distance = pJS.interactivity.mouse.distance * pJS.canvas.pxratio } pJS.fn.canvasInit = function () { pJS.canvas.ctx = pJS.canvas.el.getContext("2d") }; pJS.fn.canvasSize = function () { pJS.canvas.el.width = pJS.canvas.w; pJS.canvas.el.height = pJS.canvas.h; window.onresize = function () { if (pJS) { pJS.canvas.w = pJS.canvas.el.offsetWidth; pJS.canvas.h = pJS.canvas.el.offsetHeight; if (pJS.retina) { pJS.canvas.w *= pJS.canvas.pxratio; pJS.canvas.h *= pJS.canvas.pxratio } pJS.canvas.el.width = pJS.canvas.w; pJS.canvas.el.height = pJS.canvas.h; pJS.fn.canvasPaint(); if (!pJS.particles.anim.enable) { pJS.fn.particlesRemove(); pJS.fn.canvasRemove(); f() } } } }; pJS.fn.canvasPaint = function () { pJS.canvas.ctx.fillRect(0, 0, pJS.canvas.w, pJS.canvas.h) }; pJS.fn.canvasRemove = function () { pJS.canvas.ctx.clearRect(0, 0, pJS.canvas.w, pJS.canvas.h) }; pJS.fn.particle = function (n, o, m) { this.x = m ? m.x : Math.random() * pJS.canvas.w; this.y = m ? m.y : Math.random() * pJS.canvas.h; this.radius = (pJS.particles.size_random ? Math.random() : 1) * pJS.particles.size; if (pJS.retina) { this.radius *= pJS.canvas.pxratio } this.color = n; this.opacity = o; this.vx = -0.5 + Math.random(); this.vy = -0.5 + Math.random(); this.draw = function () { pJS.canvas.ctx.fillStyle = "rgba(" + this.color.r + "," + this.color.g + "," + this.color.b + "," + this.opacity + ")"; pJS.canvas.ctx.beginPath(); switch (pJS.particles.shape) { case "circle": pJS.canvas.ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2, false); break; case "edge": pJS.canvas.ctx.rect(this.x, this.y, this.radius * 2, this.radius * 2); break; case "triangle": pJS.canvas.ctx.moveTo(this.x, this.y - this.radius); pJS.canvas.ctx.lineTo(this.x + this.radius, this.y + this.radius); pJS.canvas.ctx.lineTo(this.x - this.radius, this.y + this.radius); pJS.canvas.ctx.closePath(); break } pJS.canvas.ctx.fill() } }; pJS.fn.particlesCreate = function () { for (var m = 0; m < pJS.particles.nb; m++) { pJS.particles.array.push(new pJS.fn.particle(pJS.particles.color_rgb, pJS.particles.opacity)) } }; pJS.fn.particlesAnimate = function () { for (var n = 0; n < pJS.particles.array.length; n++) { var q = pJS.particles.array[n]; q.x += q.vx * (pJS.particles.anim.speed / 2); q.y += q.vy * (pJS.particles.anim.speed / 2); if (q.x - q.radius > pJS.canvas.w) { q.x = q.radius } else { if (q.x + q.radius < 0) { q.x = pJS.canvas.w + q.radius } } if (q.y - q.radius > pJS.canvas.h) { q.y = q.radius } else { if (q.y + q.radius < 0) { q.y = pJS.canvas.h + q.radius } } for (var m = n + 1; m < pJS.particles.array.length; m++) { var o = pJS.particles.array[m]; if (pJS.particles.line_linked.enable_auto) { pJS.fn.vendors.distanceParticles(q, o) } if (pJS.interactivity.enable) { switch (pJS.interactivity.mode) { case "grab": pJS.fn.vendors.interactivity.grabParticles(q, o); break } } } } }; pJS.fn.particlesDraw = function () { pJS.canvas.ctx.clearRect(0, 0, pJS.canvas.w, pJS.canvas.h); pJS.fn.particlesAnimate(); for (var m = 0; m < pJS.particles.array.length; m++) { var n = pJS.particles.array[m]; n.draw("rgba(" + n.color.r + "," + n.color.g + "," + n.color.b + "," + n.opacity + ")") } }; pJS.fn.particlesRemove = function () { pJS.particles.array = [] }; pJS.fn.vendors.distanceParticles = function (t, r) { var o = t.x - r.x, n = t.y - r.y, s = Math.sqrt(o * o + n * n); if (s <= pJS.particles.line_linked.distance) { var m = pJS.particles.line_linked.color_rgb_line; pJS.canvas.ctx.beginPath(); pJS.canvas.ctx.strokeStyle = "rgba(" + m.r + "," + m.g + "," + m.b + "," + (pJS.particles.line_linked.opacity - s / pJS.particles.line_linked.distance) + ")"; pJS.canvas.ctx.moveTo(t.x, t.y); pJS.canvas.ctx.lineTo(r.x, r.y); pJS.canvas.ctx.lineWidth = pJS.particles.line_linked.width; pJS.canvas.ctx.stroke(); pJS.canvas.ctx.closePath(); if (pJS.particles.line_linked.condensed_mode.enable) { var o = t.x - r.x, n = t.y - r.y, q = o / (pJS.particles.line_linked.condensed_mode.rotateX * 1000), p = n / (pJS.particles.line_linked.condensed_mode.rotateY * 1000); r.vx += q; r.vy += p } } }; pJS.fn.vendors.interactivity.listeners = function () { if (pJS.interactivity.detect_on == "window") { var m = window } else { var m = pJS.canvas.el } m.onmousemove = function (p) { if (m == window) { var o = p.clientX, n = p.clientY } else { var o = p.offsetX || p.clientX, n = p.offsetY || p.clientY } if (pJS) { pJS.interactivity.mouse.pos_x = o; pJS.interactivity.mouse.pos_y = n; if (pJS.retina) { pJS.interactivity.mouse.pos_x *= pJS.canvas.pxratio; pJS.interactivity.mouse.pos_y *= pJS.canvas.pxratio } pJS.interactivity.status = "mousemove" } }; m.onmouseleave = function (n) { if (pJS) { pJS.interactivity.mouse.pos_x = 0; pJS.interactivity.mouse.pos_y = 0; pJS.interactivity.status = "mouseleave" } }; if (pJS.interactivity.events.onclick.enable) { switch (pJS.interactivity.events.onclick.mode) { case "push": m.onclick = function (o) { if (pJS) { for (var n = 0; n < pJS.interactivity.events.onclick.nb; n++) { pJS.particles.array.push(new pJS.fn.particle(pJS.particles.color_rgb, pJS.particles.opacity, { x: pJS.interactivity.mouse.pos_x, y: pJS.interactivity.mouse.pos_y })) } } }; break; case "remove": m.onclick = function (n) { pJS.particles.array.splice(0, pJS.interactivity.events.onclick.nb) }; break } } }; pJS.fn.vendors.interactivity.grabParticles = function (r, q) { var u = r.x - q.x, s = r.y - q.y, p = Math.sqrt(u * u + s * s); var t = r.x - pJS.interactivity.mouse.pos_x, n = r.y - pJS.interactivity.mouse.pos_y, o = Math.sqrt(t * t + n * n); if (p <= pJS.particles.line_linked.distance && o <= pJS.interactivity.mouse.distance && pJS.interactivity.status == "mousemove") { var m = pJS.particles.line_linked.color_rgb_line; pJS.canvas.ctx.beginPath(); pJS.canvas.ctx.strokeStyle = "rgba(" + m.r + "," + m.g + "," + m.b + "," + (pJS.interactivity.line_linked.opacity - o / pJS.interactivity.mouse.distance) + ")"; pJS.canvas.ctx.moveTo(r.x, r.y); pJS.canvas.ctx.lineTo(pJS.interactivity.mouse.pos_x, pJS.interactivity.mouse.pos_y); pJS.canvas.ctx.lineWidth = pJS.particles.line_linked.width; pJS.canvas.ctx.stroke(); pJS.canvas.ctx.closePath() } }; pJS.fn.vendors.destroy = function () { cancelAnimationFrame(pJS.fn.requestAnimFrame); i.remove(); delete pJS }; function f() { pJS.fn.canvasInit(); pJS.fn.canvasSize(); pJS.fn.canvasPaint(); pJS.fn.particlesCreate(); pJS.fn.particlesDraw() } function l() { pJS.fn.particlesDraw(); pJS.fn.requestAnimFrame = requestAnimFrame(l) } f(); if (pJS.particles.anim.enable) { l() } if (pJS.interactivity.enable) { pJS.fn.vendors.interactivity.listeners() } } window.requestAnimFrame = (function () { return window.requestAnimationFrame || window.webkitRequestAnimationFrame || window.mozRequestAnimationFrame || window.oRequestAnimationFrame || window.msRequestAnimationFrame || function (a) { window.setTimeout(a, 1000 / 60) } })(); window.cancelRequestAnimFrame = (function () { return window.cancelAnimationFrame || window.webkitCancelRequestAnimationFrame || window.mozCancelRequestAnimationFrame || window.oCancelRequestAnimationFrame || window.msCancelRequestAnimationFrame || clearTimeout })(); function hexToRgb(c) { var b = /^#?([a-f\d])([a-f\d])([a-f\d])$/i; c = c.replace(b, function (e, h, f, d) { return h + h + f + f + d + d }); var a = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(c); return a ? { r: parseInt(a[1], 16), g: parseInt(a[2], 16), b: parseInt(a[3], 16) } : null } window.particlesJS = function (d, c) { if (typeof (d) != "string") { c = d; d = "particles-js" } if (!d) { d = "particles-js" } var b = document.createElement("canvas"); b.style.width = "100%"; b.style.height = "100%"; var a = document.getElementById(d).appendChild(b); if (a != null) { launchParticlesJS(d, c) } };

            /* particlesJS('dom-id', params);
            /* @dom-id : set the html tag id [string, optional, default value : particles-js]
            /* @params: set the params [object, optional, default values : check particles.js] */

            /* config dom id (optional) + config particles params */
            particlesJS('particles-js', {
                particles: {
                    color: '#fff',
                    shape: 'circle', // "circle", "edge" or "triangle"
                    opacity: 3,
                    size: 4,
                    size_random: true,
                    nb: 560,
                    line_linked: {
                        enable_auto: true,
                        distance: 100,
                        color: '#fff',
                        opacity: 1,
                        width: 1,
                        condensed_mode: {
                            enable: false,
                            rotateX: 600,
                            rotateY: 600
                        }
                    },
                    anim: {
                        enable: true,
                        speed: 1
                    }
                },
                interactivity: {
                    enable: true,
                    mouse: {
                        distance: 250
                    },
                    detect_on: 'canvas', // "canvas" or "window"
                    mode: 'grab',
                    line_linked: {
                        opacity: .5
                    },
                    events: {
                        onclick: {
                            enable: true,
                            mode: 'push', // "push" or "remove" (particles)
                            nb: 4
                        },
                        "onhover": {
                            "enable": true,
                            "mode": "grab",
                            "color": "white"
                        },
                    }
                },
                /* Retina Display Support */
                retina_detect: true
            });
        });
    </script>

</body>



<script type="text/javascript">
    $(".glyphicon-eye-open").on("click", function () {
        $(this).toggleClass("glyphicon-eye-close");
        var type = $("#txtPassword").attr("type");
        if (type == "text")
        { $("#txtPassword").prop('type', 'password'); }
        else
        { $("#txtPassword").prop('type', 'text'); }
    });</script>
</html>
