<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.CallingReports.Report>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Reports
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="table-content">
        <div id="tabs">
            <ul>
                <li><a href="#Dashboard" >Dashboard</a></li>
                <li><a href="CallingOverview" title="CallingOverview"><span>Calling Overview</span></a></li>
                <li><a href="WithoutCallings" title="WithoutCallings"><span>Members w/o Callings</span></a></li>
                <li><a href="Recommended" title="Recommended">Recommended</a></li>
                <li><a href="Approved" title="Approved">Approved</a></li>
                <li><a href="Called" title="Called">Called</a></li>
                <li><a href="Sustained" title="Sustained">Sustained</a></li>                
                <li><a href="Releases" title="Releases">Releases Pending</a></li>
                <li><a href="NoSurvey" title="NoSurvey">Haven't Taken Survey</a></li>
				<li><a href="SpeakingAssignments" title="SpeakingAssignments">Speaking Assignments</a></li>
            </ul>
            <div id="Dashboard">
            <% Html.RenderPartial("Dashboard", Model); %>
            </div>
            <div id="CallingOverview">
                <center name="loading">
                    <img src="../../Content/images/loading.gif" style="width: 100px" /></center>
            </div>
            <div id="WithoutCallings">
                <center name="loading">
                    <img src="../../Content/images/loading.gif" style="width: 100px" /></center>
            </div>
            <div id="Recommended">
                <center name="loading">
                    <img src="../../Content/images/loading.gif" style="width: 100px" /></center>
            </div>
            <div id="Approved">
                <center name="loading">
                    <img src="../../Content/images/loading.gif" style="width: 100px" /></center>
            </div>
            <div id="Called">
                <center name="loading">
                    <img src="../../Content/images/loading.gif" style="width: 100px" /></center>
            </div>
            <div id="Sustained">
                <center name="loading">
                    <img src="../../Content/images/loading.gif" style="width: 100px" /></center>
            </div>            
            <div id="Releases">
                <center name="loading">
                    <img src="../../Content/images/loading.gif" style="width: 100px" /></center>
            </div>
            <div id="NoSurvey">
                <center name="loading">
                    <img src="../../Content/images/loading.gif" style="width: 100px" /></center>
            </div>
			<div id="SpeakingAssignments">
                <center name="loading">
                    <img src="../../Content/images/loading.gif" style="width: 100px" /></center>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        // Tabs
        $('#tabs').tabs({
            select: function (event, ui) {
                $('[name=loading]').show();

            },
            show: function (event, ui) {
                $('[name=loading]').show();

            },
            load: function (event, ui) { $('[name=loading]').hide(); },
            ajaxOptions: {
                error: function (xhr, status, index, anchor) {
                    if (xhr.status === 0 || xhr.readyState === 0) {
                        $('[name=loading]').show(); 
                        return;
                    }

                    $(anchor.hash).html(
						"Couldn't load this report. Please try refreashing this page.");
                },
                cache: false
            },
            spinner: 'Retrieving report...'

        });
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        Reports</h1>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
    <script type="text/javascript">
        MenuSelect("Calling", "Reports");
    </script>
</asp:Content>
