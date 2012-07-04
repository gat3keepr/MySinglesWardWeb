<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Features
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
	<h1>
		Features</h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<table style="margin-left: 20px">
		<tr valign="top">
			<td>
				<center>
					<img src="../../Content/images/loading.gif" style="display: none; width: 30px;" id="loading" /></center>
				<div style="width: 830px">
					<div id="current" name="content">
					</div>
				</div>
				<!-- end #content -->
				<div style="clear: both;">
					&nbsp;</div>
			</td>
			<td>
				<!--  start Information & Features -->
				<div id="related-activities">
					<!--  start related-act-top -->
					<div id="related-act-top">
						<img src="../../Content/admin/images/forms/header_related_act.gif" width="271" height="43"
							alt="" />
					</div>
					<!-- end related-act-top -->
					<!--  start related-act-bottom -->
					<div id="related-act-bottom">
						<!--  start related-act-inner -->
						<div id="related-act-inner">
							<div class="left">
								<span name="newStake_link">
									<img src="../../Content/admin/images/forms/icon_edit.gif" width="21" height="21"
										alt="" /></span></div>
							<div class="right">
								<span name="newStake_link">
									<h5>
										Stake & Bishopric Information</h5>
									<ul class="greyarrow">
										<li><a href="#gettingStarted" type="link">Getting Started...</a> </li>
										<li><a href="#password" type="link">Request Ward or Stake Password</a></li>
										<li><a href="#faq" type="link">FAQ</a></li>
									</ul>
								</span>
							</div>
							<div class="clear">
							</div>
							<div class="lines-dotted-short">
							</div>
							<div class="left">
								<span name="newStake_link">
									<img src="../../Content/admin/images/forms/icon_edit.gif" width="21" height="21"
										alt="" /></span></div>
							<div class="right">
								<span name="newStake_link">
									<h5>
										Member Features</h5>
									<ul class="greyarrow">
										<li><a href="#wardList" type="link">Online Ward List</a> </li>
										<li><a href="#survey" type="link">Online Ward Survey</a></li>
										<li><a href="#uploadPicture" type="link">Upload Picture</a></li>
										<li><a href="#groups" type="link">Join Ward Groups</a></li>
										<li><a href="#notifications" type="link">Send & Recieve Txt Reminders</a></li>
										<li><a href="#callingInfo" type="link">Get Calling Documents</a></li>
									</ul>
								</span>
							</div>
							<div class="clear">
							</div>
							<div class="lines-dotted-short">
							</div>
							<div class="left">
								<span name="newStake_link">
									<img src="../../Content/admin/images/forms/icon_edit.gif" width="21" height="21"
										alt="" /></span></div>
							<div class="right">
								<span name="newStake_link">
									<h5>
										Leadership Features</h5>
									<ul class="greyarrow">
										<li><a href="#reports" type="link">PDF & Excel Reports</a></li>
										<li><a href="#wardListCreation" type="link">Automated Ward List Creation</a></li>
										<li><a href="#searchable" type="link">Searchable Online Member Data</a></li>
										<li><a href="#groups" type="link">Create Ward Groups</a></li>
										<li><a href="#notifications" type="link">Send Txt & Email Notifications</a></li>
										<li><a href="#callingInfo" type="link">Provide Members Calling Info</a></li>
									</ul>
								</span>
							</div>
							<div class="clear">
							</div>
							<div class="lines-dotted-short">
							</div>
							<div class="left">
								<span name="newStake_link">
									<img src="../../Content/admin/images/forms/icon_edit.gif" width="21" height="21"
										alt="" /></span></div>
							<div class="right">
								<span name="newStake_link">
									<h5>
										Calling Management</h5>
									<ul class="greyarrow">
										<li><a href="#callings" type="link">Organizations & Callings</a></li>
										<li><a href="#callingreports" type="link">Calling Reports</a></li>
										<li><a href="#stewardship" type="link">Stewardship Information</a></li>
									</ul>
								</span>
							</div>
							<div class="clear">
							</div>
							<div class="lines-dotted-short">
							</div>
							<div class="left">
								<span name="newStake_link">
									<img src="../../Content/admin/images/forms/icon_edit.gif" width="21" height="21"
										alt="" /></span></div>
							<div class="right">
								<span name="newStake_link">
									<h5>
										PDFs & Reports</h5>
									<ul class="greyarrow">
										<li><a href="#listPDF" type="link">Ward List</a> </li>
										<li><a href="#callingsheet" type="link">Calling Sheets</a></li>
										<li><a href="#memberSheet" type="link">Member Sheets</a></li>
										<li><a href="#membershipReport" type="link">Membership Reports</a></li>
										<li><a href="#stakeReports" type="link">Stake Reports</a></li>
									</ul>
								</span>
							</div>
							<div class="clear">
							</div>
						</div>
						<!-- end related-act-inner -->
						<div class="clear">
						</div>
					</div>
					<!-- end related-act-bottom -->
				</div>
				<!-- end Information & Features -->
			</td>
		</tr>
		<tr>
			<td>
				<img src="../../Content/admin/images/shared/blank.gif" width="695" height="1" alt="blank" />
			</td>
			<td>
			</td>
		</tr>
	</table>
	<script type="text/javascript">

		//On load page, init the timer which check if the there are anchor changes each 300 ms  
		$().ready(function () {
			setInterval("checkAnchor()", 100);
		});
		var currentAnchor = null;
		//Function which chek if there are anchor changes, if there are, sends the ajax petition  
		function checkAnchor() {
			//Check if it has changes  
			if (currentAnchor != document.location.hash) {
				$('#current').hide();
				$('#loading').show();
				currentAnchor = document.location.hash;
				//if there is not anchor, the loads the default section  
				if (!currentAnchor)
					query = "welcome";
				else {
					//Creates the  string callback. This converts the url URL/#main&id=2 in URL/?section=main&id=2  
					var splits = currentAnchor.substring(1).split('&');
					//Get the section  
					var section = splits[0];
					delete splits[0];
					//Create the params string
					var query = section;
				}
				//Send the petition
				$('#current').load("/info/" + query, function () {
					$('#loading').hide();
					$('#current').show();
				});
			}
		}  
    
	</script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
