<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<link href="../../Content/admin/css/info.css" rel="stylesheet" type="text/css" />
<div class="infoFeatures">
	<h1>
		Getting Started...
	</h1>
	<p>
		Here are the steps to get started using MySinglesWard.com:</p>
	<script type="text/javascript">
		$(function () {
			$("#accordion").accordion({
				autoHeight: false
			});
		});
	</script>
	<div id="accordion" class="infoList">
		<h3>
			<a href="#">Members:</a></h3>
		<div class="infoList">
			<ol>
				<li>Click Register on the login menu.</li>
				<li>Choose Member User and fill out registration information.</li>
				<li>Fill out personal information on the member survey.</li>
				<li>Upload picture to be put on the ward list.</li>
				<li>Set Notification preferences to recieve e-mails and texts.</li>
				<li>Join groups to stay up-to-date on ward and stake events.</li>
			</ol>
			<p>
			</p>
		</div>
		<h3>
			<a href="#">Bishopric Members:</a></h3>
		<div class="infoList2">
			<ol>
				<li>Get Bishopric Code from MySinglesWard.</li>
				<li>Click Register on the login menu.</li>
				<li>Choose Bishopric User and fill out registration information with Bishopric Code.</li>
				<li>Create or Join a ward. You will have to either set or enter the ward password.</li>
				<li>Fill out personal information that will appear on the ward list.</li>
				<li>Upload picture to be put on the ward list.</li>
				<li>Add residences to the ward. Members taking the survey will get to choose from the
					residences.</li>
				<li>Set the authentications for your ward members to give them access to calling sheets.</li>
				<li>Create ward groups and assign group leaders.</li>
			</ol>
		</div>
		<h3>
			<a href="#">Stake Members:</a></h3>
		<div class="infoList">
			<ol>
				<li>Get Stake Code from MySinglesWard.</li>
				<li>Click Register on the login menu.</li>
				<li>Choose Stake User and fill out registration information with Stake Code.</li>
				<li>Fill out personal information that will appear on the stake list.</li>
				<li>Upload picture to be put on the stake list.</li>
				<li>Create stake groups and assign group leaders.</li>
			</ol>
		</div>
	</div>
</div>
