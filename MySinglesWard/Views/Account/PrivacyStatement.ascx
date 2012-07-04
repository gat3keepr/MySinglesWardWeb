<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div id="privacyStatement" title="MySinglesWard Privacy Statement">
	<ul class="privacyStatement">
		<li><i>MySinglesWard.com is for church use only. </i>
			<p>
				This website is designed to help your bishopric get to know you faster and improve
				your singles ward experience.
			</p>
			<p>
				The Member Survey is patterned after a “New Member Survey” that many YSA wards have
				new members fill out. Sensitive information will never be given to anyone other
				than your bishopric. Your contact information, phone number and email, can be removed
				from the ward list by selecting to not publish that information to the ward list
				during the survey.
			</p>
			<p>
				Only members approved by the bishopric will have access to the ward list which only
				includes your name, phone number, email, address, and photo. This ensures that only
				people in your ward will have access to your contact information that is part of
				the ward list. Your Elders Quorum/Relief Society President will only see that part
				of the information from the survey that is relevant to their stewardship (no sensitive
				information), as will other key ward leaders.</p>
			<p>
				Two passwords are required to become a bishopric or stake member. One password is
				given controlled by MySinglesWard and the other is controlled by your bishop.</p>
		</li>
		<li><i>Your information is secure.</i>
			<p>
				MySinglesWard has been professionally analyzed for security threats, with physical
				and software security measures implemented to keep your data secure. The use of
				GoDaddy.com ensures MySinglesWard data is transferred over the Internet securely.
				All data is encrypted as it travels between us to you.
			</p>
			<p>
				Once we receive your information, it is encrypted again and stored by MySinglesWard.
				You control your information and can update your information at any time. The only
				people that will be able to view your information is you and your ward and stake
				leadership. No one can hack the system to see your data, since the data is stored
				in encrypted form. Not even the people developing MySinglesWard can view your information.</p>
			<p>
				Additionally, we guarantee none of your personal information will be sold, distributed,
				or given to any third parties besides your church leaders and your fellow ward members.</p>
		</li>
		<li><i>Your Bishop has reviewed this software and believes it will help the ward better
			minister to you and your fellow ward members. </i>
			<p>
				Your ward will operate more efficiently and your ward leadership will be able to
				help you on a more personal level when you use MySinglesWard.</p>
		</li>
		<i><small>Not an official part of the Church of Jesus Christ of Latter-day Saints</small></i>
		
	</ul>
</div>
<script type="text/javascript">
	$("[name=privacy]").click(function () {
		$('#privacyStatement').dialog('open');
	});

	$("#privacyStatement").dialog({
		autoOpen: false,
		height: 699,
		width: 755,
		modal: false,
		buttons: {
			"Close": function () {
				$(this).dialog("close");
			}
		}
	});
</script>
