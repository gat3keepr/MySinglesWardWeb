<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MSW.Models.dbo.MemberSurvey>" %>
<% var DropDowns = new MSW.Models.DropDowns(); %>
<% DropDowns.generateLists(); %>
<%     using (Html.BeginForm("PersonalInformation", "Home", FormMethod.Post, new { @id = "PersonalInformation" }))
	   {%>
<%: Html.HiddenFor(x => x.memberID)%>
<div id="tabs">
	<ul>
		<li><a href="#tabs-1">Personal Information</a></li>
		<li><a href="#tabs-2">Church Information</a></li>
		<li><a href="#tabs-3">Other Information</a></li>
	</ul>
	<div id="tabs-1">
		<%: Html.ValidationSummary(true, "Some information was enter incorrectly. Please fix errors and try again.", new { @class = "error" })%>
		<div class="editor-label">
			<strong>First name</strong> to appear on Ward List <span style="color: Maroon">(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.TextBoxFor(model => model.prefName)%>
			<span class="surveyError" id="PrefNameX">Fill in name for ward list</span>
		</div>
		<div class="editor-label">
			Gender <span style="color: Maroon">(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.RadioButtonFor(model => model.gender, true, new { @gender = "male" })%>
			Male &nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.gender, false, new { @gender = "female" })%>
			Female <span class="surveyError" id="GenderX">Select Gender</span>
		</div>
		<div class="editor-label">
			Select Address:
			<%: Html.DropDownList("apartmentDD", ViewData["ApartmentList"] as IEnumerable<SelectListItem>, "Select an Apartment")%>
			<span style="color: Maroon">(Required)</span>&nbsp;&nbsp;&nbsp; <span class="surveyError"
				id="ApartmentX">Select a residence</span>
		</div>
		<div class="editor-field">
			<strong>or</strong> Type Current Address
			<br />
			<br />
			<%: Html.TextAreaFor(model => model.residence, new { @class = "text-area" })%>
			<br />
		</div>
		<div class="editor-label">
			Is it ok to show your Email on the ward list? <span style="color: Maroon">(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.RadioButtonFor(model => model.publishEmail, true, new { @PublishEmail = "true" })%>
			Yes &nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.publishEmail, false, new { @PublishEmail = "false" })%>
			No <span class="surveyError" id="PublishEmailX">Indicate your email preference</span>
		</div>
		<div class="editor-label">
			Birthday <span style="color: Maroon">(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.TextBoxFor(model => model.birthday, new { @id = "datepicker" })%>
			<span class="surveyError" id="BirthdayX">Select Your Birthday</span>
		</div>
		<div class="editor-label">
			Cell-Phone Number <span style="color: Maroon">(Required)</span>
			<p class="subtext">
				(XXX) XXX-XXXX</p>
		</div>
		<div class="editor-field">
			<%: Html.TextBoxFor(model => model.cellPhone)%>
			<span class="surveyError" id="CellPhoneX">Enter a valid phone number</span>
		</div>
		<div class="editor-label">
			Is it ok to show your cell-phone number on the ward list? <span style="color: Maroon">
				(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.RadioButtonFor(model => model.publishCell, true, new { @PublishCell = "true" })%>Yes&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.publishCell, false, new { @PublishCell = "false" })%>No
			<span class="surveyError" id="PublishCellX">Indicate your phone preference</span>
		</div>
		<div class="editor-label">
			Permanent (Home) Address <span style="color: Maroon">(Required)</span> <span class="surveyError"
				id="HomeAddressX">Enter an Address</span>
		</div>
		<div class="editor-field">
			<%: Html.TextAreaFor(model => model.homeAddress, new { @class = "text-area" })%>
		</div>
		<div class="editor-label">
			Home-Phone Number
			<p class="subtext">
				(XXX) XXX-XXXX</p>
		</div>
		<div class="editor-field">
			<%: Html.TextBoxFor(model => model.homePhone)%>
			<%: Html.ValidationMessageFor(model => model.homePhone)%>
		</div>
		<div class="editor-label">
			Emergency Contact Name <span style="color: Maroon">(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.TextBoxFor(model => model.emergContact)%>
			<span class="surveyError" id="EmergContactX">Enter an emergency contact</span>
		</div>
		<div class="editor-label">
			Emergency Contact Phone <span style="color: Maroon">(Required)</span>
			<p class="subtext">
				(XXX) XXX-XXXX</p>
		</div>
		<div class="editor-field">
			<%: Html.TextBoxFor(model => model.emergPhone)%>
			<span class="surveyError" id="EmergPhoneX">Enter a phone number</span>
		</div>
		<div class="editor-label">
			Home Ward & Stake
		</div>
		<div class="editor-field">
			<%: Html.TextBoxFor(model => model.homeWardStake)%>
			<%: Html.ValidationMessageFor(model => model.homeWardStake)%>
		</div>
		<div class="editor-label">
			Home Bishop
		</div>
		<div class="editor-field">
			<%: Html.TextBoxFor(model => model.homeBishop)%>
			<%: Html.ValidationMessageFor(model => model.homeBishop)%>
		</div>
		<span id="PriesthoodField">
			<div class="editor-label">
				Priesthood Office <span style="color: Maroon">(Required)</span>
			</div>
			<div class="editor-field">
				<%: Html.DropDownListFor(model => model.priesthood, DropDowns.getPriesthoodList(), "Select Office or Select N/A")%>
				<span class="surveyError" id="PriesthoodX">Select an option</span>
			</div>
		</span>
		<p>
		</p>
		<div class="editor-label">
			<button id="secondLink" class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
				<span class="ui-button-text">Save & Continue</span>
			</button>
		</div>
	</div>
	<div id="tabs-2">
		<% }
	   using (Html.BeginForm("ChurchInformation", "Home", FormMethod.Post, new { @id = "ChurchInformation" }))
	   {%>
		<%: Html.HiddenFor(x => x.memberID)%>
		<div class="editor-label">
			<strong>Use semi-colons to separate information. (i.e. Old Ward, Bishop Smith, 555-5555;
				Old Ward...)</strong>
		</div>
		<div class="editor-label">
			Previous Ward, Bishop's Name and Phone in last 12 months
		</div>
		<div class="editor-field">
			<%: Html.TextAreaFor(model => model.prevBishops, new { @class = "text-area" })%>
			<%: Html.ValidationMessageFor(model => model.prevBishops)%>
		</div>
		<div class="editor-label">
			Have you served a mission? <span style="color: Maroon">(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.RadioButtonFor(model => model.mission, true, new { @Mission = "true" })%>
			Yes &nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.mission, false, new { @Mission = "false" })%>
			No <span class="surveyError" id="MissionX">Indicate your mission status</span>
		</div>
		<div id="YesMission">
			<div class="editor-label">
				Where did you serve?
			</div>
			<div class="editor-field">
				<%: Html.TextBoxFor(model => model.missionLocation)%>
				<%: Html.ValidationMessageFor(model => model.missionLocation)%>
			</div>
		</div>
		<div id="NoMission">
			<div class="editor-label">
				Do you plan to serve a mission?
			</div>
			<div class="editor-field">
				<%: Html.RadioButtonFor(model => model.planMission, 1)%>
				Yes&nbsp;&nbsp;&nbsp;
				<%: Html.RadioButtonFor(model => model.planMission, 0)%>
				No&nbsp;&nbsp;&nbsp;
				<%: Html.RadioButtonFor(model => model.planMission, -1)%>
				Maybe&nbsp;&nbsp;&nbsp;
				<%: Html.ValidationMessageFor(model => model.planMission)%>
			</div>
			<div class="editor-label">
				If so, when do you plan to serve a mission?
			</div>
			<div class="editor-field">
				<%: Html.TextBoxFor(model => model.planMissionTime)%>
				<%: Html.ValidationMessageFor(model => model.planMissionTime)%>
			</div>
		</div>
		<div class="editor-label">
			Do you have your Patriarchal Blessing? <span style="color: Maroon">(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.RadioButton("PatriarchalBlessing", true, Model == null ? false : Model._PatriarchalBlessing, new { @PatriarchalBlessing = "true" })%>
			Yes&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButton("PatriarchalBlessing", false, Model == null ? false : !Model._PatriarchalBlessing, new { @PatriarchalBlessing = "false" })%>
			No&nbsp;&nbsp;&nbsp; <span class="surveyError" id="PatriarchalBlessingX">Indicate your
				patriarchal blessing status</span>
		</div>
		<div class="editor-label">
			Are you Endowed? <span style="color: Maroon">(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.RadioButton("Endowed", true, Model == null ? false : Model._Endowed, new { @Endowed = "true" })%>
			Yes&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButton("Endowed", false, Model == null ? false : !Model._Endowed, new { @Endowed = "false" })%>
			No&nbsp;&nbsp;&nbsp; <span class="surveyError" id="EndowedX">Indicate your endowment
				status</span>
		</div>
		<div class="editor-label">
			Do you have a current temple recommend? <span style="color: Maroon">(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.RadioButton("TempleRecommend", true, Model == null ? false : Model._TempleRecommend, new { @TempleRecommend = "true" })%>
			Yes&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButton("TempleRecommend", false, Model == null ? false : !Model._TempleRecommend, new { @TempleRecommend = "false" })%>
			No&nbsp;&nbsp;&nbsp; <span class="surveyError" id="TempleRecommendX">Indicate your temple
				recommend status</span>
		</div>
		<div class="editor-label">
			Temple Recommend Expiration Date
		</div>
		<div class="editor-field">
			<%: Html.TextBoxFor(model => model.templeExpDate, new { @id = "datepicker1" })%>
			<%: Html.ValidationMessageFor(model => model.templeExpDate, "Enter a Valid Date", new { @class = "error" })%>
		</div>
		<div class="editor-label">
			Are you set apart as a Temple Worker? <span style="color: Maroon">(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.RadioButton("TempleWorker", true, Model == null ? false : Model._TempleWorker, new { @TempleWorker = "true" })%>
			Yes&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButton("TempleWorker", false, Model == null ? false : !Model._TempleWorker, new { @TempleWorker = "false" })%>
			No&nbsp;&nbsp;&nbsp; <span class="surveyError" id="TempleWorkerX">Indicate your temple
				worker status</span>
		</div>
		<div class="editor-label">
			Callings Held in the last 2 years? <span style="color: Maroon">(Required)</span>
			<span class="surveyError" id="PastCallingsX">Enter your past callings</span>
			<br />
			<span class="subtext">Please use commas to separate callings. (i.e. Activities, Elders
				Quorum, FHE)</span>
		</div>
		<div class="editor-field">
			<%: Html.TextAreaFor(model => model.pastCallings, new { @class = "text-area" })%>
		</div>
		<p>
		</p>
		<div class="editor-label">
			<button id="back1" class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
				<span class="ui-button-text">Back</span></button>
			<button id="thirdLink" class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
				<span class="ui-button-text">Save & Continue</span>
			</button>
		</div>
	</div>
	<div id="tabs-3">
		<% }
		using (Html.BeginForm("FinishSurvey", "Home", FormMethod.Post, new { @id = "OtherInformation" }))
		{%>
		<%: Html.HiddenFor(x => x.memberID) %>
		<div class="editor-label">
			How long do you plan to stay in the ward? <span style="color: Maroon">(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.DropDownListFor(model => model.timeInWard, DropDowns.getTimeWardList(), "Select Best Guess") %>
			<span class="surveyError" id="TimeInWardX">Indicate your estimated time in the ward</span>
		</div>
		<div class="editor-label">
			Are you attending school? <span style="color: Maroon">(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.RadioButtonFor(model => model.enrolledSchool, true, new { @School = "true" })%>
			Yes&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.enrolledSchool, false, new { @School = "false" })%>
			No&nbsp;&nbsp;&nbsp; <span class="surveyError" id="EnrolledSchoolX">Indicate your school
				status</span>
		</div>
		<div id="School" style="display: none">
			<div class="editor-label">
				Where are you attending school?
			</div>
			<div class="editor-field">
				<%: Html.TextBoxFor(model => model.school) %>
				<%: Html.ValidationMessageFor(model => model.school) %>
			</div>
			<div class="editor-label">
				What is your course of study?
			</div>
			<div class="editor-field">
				<%: Html.TextBoxFor(model => model.major) %>
				<%: Html.ValidationMessageFor(model => model.major) %>
			</div>
		</div>
		<div class="editor-label">
			Are you enrolled in a religion class or institute? <span style="color: Maroon">(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.RadioButtonFor(model => model.religionClass, "Religion Class", new { @ReligionClass = "1" })%>
			Religion&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.religionClass, "Institute", new { @ReligionClass = "2" })%>
			Institute&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.religionClass, "No", new { @ReligionClass = "3" })%>
			No&nbsp;&nbsp;&nbsp; <span class="surveyError" id="ReligionClassX">Indicate your Religion
				class status</span>
		</div>
		<div class="editor-label">
			Are you employed? <span style="color: Maroon">(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.DropDownListFor(model => model.employed, DropDowns.getEmployedList(), "Select Employment") %>
			<span class="surveyError" id="EmployedX">Indicate your employment status</span>
		</div>
		<div id="OccupationX" style="display: none">
			<div class="editor-label">
				Where do you work?
			</div>
			<div class="editor-field">
				<%: Html.TextBoxFor(model => model.occupation) %>
				<%: Html.ValidationMessageFor(model => model.occupation) %>
			</div>
		</div>
		<div class="editor-label">
			Rate your Music Skill <span style="color: Maroon">(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.RadioButtonFor(model => model.musicSkill, 0, new { @MusicSkill = "0" })%>
			0&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.musicSkill, 1, new { @MusicSkill = "1" })%>
			1&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.musicSkill, 2, new { @MusicSkill = "2" })%>
			2&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.musicSkill, 3, new { @MusicSkill = "3" })%>
			3&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.musicSkill, 4, new { @MusicSkill = "4" })%>
			4&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.musicSkill, 5, new { @MusicSkill = "5" })%>
			5&nbsp;&nbsp;&nbsp; <span class="surveyError" id="MusicSkillX">Indicate your music skill</span>
		</div>
		<div class="editor-label">
			Musical Abilities Information
			<p class="subtext">
				Instruments you play, singing, etc.</p>
		</div>
		<div class="editor-field">
			<%: Html.TextBoxFor(model => model.musicTalent) %>
			<%: Html.ValidationMessageFor(model => model.musicTalent) %>
		</div>
		<div class="editor-label">
			Rate your Teaching Skills <span style="color: Maroon">(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.RadioButtonFor(model => model.teachSkill, 0, new { @TeachSkill = "0" })%>
			0&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.teachSkill, 1, new { @TeachSkill = "1" })%>
			1&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.teachSkill, 2, new { @TeachSkill = "2" })%>
			2&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.teachSkill, 3, new { @TeachSkill = "3" })%>
			3&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.teachSkill, 4, new { @TeachSkill = "4" })%>
			4&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.teachSkill, 5, new { @TeachSkill = "5" })%>
			5&nbsp;&nbsp;&nbsp; <span class="surveyError" id="TeachSkillX">Indicate your teaching
				skill</span>
		</div>
		<div class="editor-label">
			Rate your Desire to Teach <span style="color: Maroon">(Required)</span>
		</div>
		<div class="editor-field">
			<%: Html.RadioButtonFor(model => model.teachDesire, 0, new { @TeachDesire = "0" })%>
			0&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.teachDesire, 1, new { @TeachDesire = "1" })%>
			1&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.teachDesire, 2, new { @TeachDesire = "2" })%>
			2&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.teachDesire, 3, new { @TeachDesire = "3" })%>
			3&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.teachDesire, 4, new { @TeachDesire = "4" })%>
			4&nbsp;&nbsp;&nbsp;
			<%: Html.RadioButtonFor(model => model.teachDesire, 5, new { @TeachDesire = "5" })%>
			5&nbsp;&nbsp;&nbsp; <span class="surveyError" id="TeachDesireX">Indicate your desire
				to teach</span>
		</div>
		<div class="editor-label">
			Where would you like to serve in the ward?
		</div>
		<div class="editor-field">
			<%: Html.TextBoxFor(model => model.callingPref) %>
			<%: Html.ValidationMessageFor(model => model.callingPref) %>
		</div>
		<div class="editor-label">
			<strong>Please limit responses to 100 characters and use commas to separate items on
				a list. (i.e. Activities, Elders Quorum, FHE)</strong>
		</div>
		<div class="editor-label">
			Suggestions for Ward Activities
		</div>
		<div class="editor-field">
			<%: Html.TextAreaFor(model => model.activities, new { @class = "text-area" })%>
			<%: Html.ValidationMessageFor(model => model.activities) %>
		</div>
		<div class="editor-label">
			Interests, Hobbies, Special Abilities/Skills
		</div>
		<div class="editor-field">
			<%: Html.TextAreaFor(model => model.interests, new { @class = "text-area" })%>
			<%: Html.ValidationMessageFor(model => model.interests) %>
		</div>
		<div class="editor-label">
			How would you describe yourself?
		</div>
		<div class="editor-field">
			<%: Html.TextAreaFor(model => model.description, new { @class = "text-area" })%>
			<%: Html.ValidationMessageFor(model => model.description) %>
		</div>
		<br />
		<p>
			<button id="back2" class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
				<span class="ui-button-text">Back</span></button>
			<button name="finalLink" value="save" class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
				<span class="ui-button-text">Save</span></button>
			<input name="finalLink" type="submit" value="Finish Survey" class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all" />
		</p>
	</div>
</div>
<% } %>
<script type="text/javascript">
	$('#YesMission').hide();
	$('#NoMission').hide();

	if ($('[mission=true]:checked').val() != null) {
		$('#YesMission').show();
	}
	if ($('[mission=false]:checked').val() != null) {
		$('#NoMission').show();
	}
	if ($('[gender=female]:checked').val() != null) {
		$('#PriesthoodField').hide();
	}

	$('[id=mission]').click(function () {
		var check = $(this).val();
		if (check == 'True') {
			$('#YesMission').fadeIn();
			$('#NoMission').hide();
		}
		else {
			$('#YesMission').hide();
			$('#NoMission').fadeIn();
		}
	});

	$('[id=enrolledSchool]').click(function () {
		var check = $(this).val();
		if (check == 'True') {
			$('#School').fadeIn();
		}
		else {
			$('#School').hide();
		}
	});

	if ($('[school=true]:checked').val() != null) {
		$('#School').show();
	}

	//Employments
	$('#employed').change(function () {
		if ($(this).val() == "Full-Time" || $(this).val() == "Part-Time") {
			$("#OccupationX").fadeIn();
		}
		else {
			$("#OccupationX").hide();
			$("#occupation").val("");
		}
	});

	if ($('#employed').val() == "Full-Time" || $('#employed').val() == "Part-Time") {
		$("#OccupationX").show();
	}

	//Apartments
	$('#apartmentDD').change(function () {
		$("#residence").val($(this).val());
	});

	$("[gender=female]").click(function () {
		$("#PriesthoodField").hide();
		$("#priesthood").val("N/A");
	});

	$("[gender=male]").click(function () {
		$("#PriesthoodField").show();
		$("#priesthood").val("");
	});
</script>
<script type="text/javascript">
	if ($.browser.msie) {
		alert("MySinglesWard.com sometimes has problems with Internet Explorer. If you have problems, please use a different browser.");
	}

</script>
