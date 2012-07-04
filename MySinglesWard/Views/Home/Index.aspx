<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    MySinglesWard.com | Home
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="wrapper" style="margin-top: -100px">
        <div class="slider-wrapper theme-default">
            <div class="ribbon">
            </div>
            <div style="display:none" id="slider">
                <img name="slider" src="../../Content/slider/images/mysinglesward.jpg" />
                <img name="slider" src="../../Content/slider/images/makingmoretimeforministering.jpg" />
                <img name="slider" src="../../Content/slider/images/admin_tool.jpg" />
                <img name="slider" src="../../Content/slider/images/safe_and_secure.jpg" />
            </div>
        </div>
    </div>
    <br />
	
    <br />
	<br />
    <br /><a class="frontPage homePageLink1" href='<%: Url.Action("Register","Account") %>'></a>
					<a class="frontPage homePageLink2" href='<%: Url.Action("Index","Info") %>'></a>
    <script type="text/javascript">
    	$(window).load(function () {
    		$('#slider').nivoSlider();
    		$('#slider').show();
    	});
    </script>    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../../Content/slider/nivo-slider.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/slider/jquery.nivo.slider.pack.js" type="text/javascript"></script>
    <link href="../../Content/slider/themes/default/default.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
</asp:Content>
