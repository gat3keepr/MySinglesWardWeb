$(function () {
	//Datepicker
	$('#datepicker').datepicker({
		inline: true,
		changeMonth: true,
		changeYear: true,
		yearRange: '1975:2000'
	});
	$('#datepicker1').datepicker({
		inline: true,
		changeMonth: true,
		changeYear: true,
		yearRange: '2009:2015'
	});

	// Tabs
	var $tabs = $('#tabs').tabs({ disabled: [1, 2] }); // first tab selected

	$('#secondLink').click(function () { // bind click event to link
		//Hide Errors
		_hideErrors(1);
		//Verify Page 2 form
		var error = "";

		if ($('#prefName').val() == "") {
			error += "-Please enter your First Name\n";
			$('#PrefNameX').show();
		}
		if ($('[gender=male]:checked').val() == null && $('[gender=female]:checked').val() == null) {
			error += "-Please select your gender\n";
			$('#GenderX').show();
		}
		if ($('#residence').val() == "") {
			error += "-Please enter your Current Address\n";
			$('#ApartmentX').show();
		}
		if ($('[publishEmail=true]:checked').val() == null && $('[publishEmail=false]:checked').val() == null) {
			error += "-Please select your email preference\n";
			$('#PublishEmailX').show();
		}
		var birthday = $('#datepicker').val();
		if ($('#datepicker').val() == "" || !birthday.match(/^\d{1,2}\/\d{1,2}\/\d{4}$/)) {
			error += "-Please enter a valid Birthday\n";
			$('#BirthdayX').show();
		}
		var cellphone = $('#cellPhone').val();
		if ($('#cellPhone').val() == "" || !cellphone.match(/^\(?(\d{3})\)?[- ]?(\d{3})[- ]?(\d{4})$/)) {
			error += "-Please enter a valid Cell Phone Number\n";
			$('#CellPhoneX').show();
		}
		if ($('[publishCell=true]:checked').val() == null && $('[publishCell=false]:checked').val() == null) {
			error += "-Please select your cell phone preference\n";
			$('#PublishCellX').show();
		}
		if ($('#homeAddress').val() == "") {
			error += "-Please enter your Home Address\n";
			$('#HomeAddressX').show();
		}
		if ($('#emergContact').val() == "") {
			error += "-Please enter your Emergency Contact\n";
			$('#EmergContactX').show();
		}
		if ($('#emergPhone').val() == "") {
			error += "-Please enter your Emergency Contact's Phone Number\n";
			$('#EmergPhoneX').show();
		}
		if ($('#priesthood').val() == "") {
			error += "-Please enter your Priesthood Office\n";
			$('#PriesthoodX').show();
		}

		if (error != "") {
			alert("Please fill out all the required fields on this page before moving on");
		}
		else {
			$.blockUI();
			var data = $('#PersonalInformation').serialize();
			$.ajax({
				type: 'POST',
				url: '/Home/PersonalInformation',
				async: false,
				data: ({ memberSurvey: data }),
				success: function () {
					// switch to second tab   
					$.unblockUI();         
					$tabs.tabs("enable", 1);
					$tabs.tabs('select', 1);
					$tabs.tabs("disable", 2);
					$tabs.tabs("disable", 0);
					window.scrollTo(0, 0);
					
				},
				error: function (xhr, textStatus, errorThrown) {
					if (!userAborted(xhr)) {
						$tabs.tabs('select', 0);
						alert("Please try again later");
					}
					$.unblockUI();
				}
			});
		}
		return false;
	});

	$('#thirdLink').click(function () { // bind click event to link
		//Hide Errors
		_hideErrors(2);
		//Verify Page 2 form
		var error = "";

		if ($('[mission=true]:checked').val() == null && $('[mission=false]:checked').val() == null) {
			error += "-Please select your mission status\n";
			$('#MissionX').show();
		}
		if ($('[patriarchalBlessing=true]:checked').val() == null && $('[patriarchalBlessing=false]:checked').val() == null) {
			error += "-Please select your Patriarchal Blessing status\n";
			$('#PatriarchalBlessingX').show();
		}
		if ($('[endowed=true]:checked').val() == null && $('[endowed=false]:checked').val() == null) {
			error += "-error\n";
			$('#EndowedX').show();
		}
		if ($('[templeRecommend=true]:checked').val() == null && $('[templeRecommend=false]:checked').val() == null) {
			error += "-errorn";
			$('#TempleRecommendX').show();
		}
		if ($('[templeWorker=true]:checked').val() == null && $('[templeWorker=false]:checked').val() == null) {
			error += "-error\n";
			$('#TempleWorkerX').show();
		}
		if ($('#pastCallings').val() == "") {
			error += "-error\n";
			$('#PastCallingsX').show();
		}

		if (error != "") {
			alert("Please fill out all the required fields on this page before moving on");
		}
		else {
			var data = $('#ChurchInformation').serialize();
			$.blockUI();
			$.ajax({
				type: 'POST',
				url: '/Home/ChurchInformation',
				async: false,
				data: ({ memberSurvey: data }),
				success: function () {
					// switch to third tab
					$.unblockUI();
					$tabs.tabs("enable", 2);
					$tabs.tabs('select', 2);
					$tabs.tabs("disable", 1);
					$tabs.tabs("disable", 0);
					window.scrollTo(0, 0);
					
				},
				error: function (xhr, textStatus, errorThrown) {
					if (!userAborted(xhr)) {
						$tabs.tabs('select', 1);
						alert("Please try again later");
					}
					$.unblockUI();
				}
			});
		}
		return false;
	});

	$('[name=finalLink]').click(function () { // bind click event to link
		//Hide Errors
		_hideErrors(3);
		//Verify Page 3 form
		var error = "";

		if ($('#timeInWard').val() == "") {
			error += "-error\n";
			$('#TimeInWardX').show();
		}
		if ($('[school=true]:checked').val() == null && $('[school=false]:checked').val() == null) {
			error += "-Please select your Patriarchal Blessing status\n";
			$('#EnrolledSchoolX').show();
		}
		if ($('[religionClass=1]:checked').val() == null && $('[religionClass=2]:checked').val() == null && $('[religionClass=3]:checked').val() == null) {
			error += "-error\n";
			$('#ReligionClassX').show();
		}
		if ($('#employed').val() == "") {
			error += "-error\n";
			$('#EmployedX').show();
		}
		if ($('[musicSkill=0]:checked').val() == null && $('[musicSkill=1]:checked').val() == null && $('[musicSkill=2]:checked').val() == null && $('[musicSkill=3]:checked').val() == null && $('[musicSkill=4]:checked').val() == null && $('[musicSkill=5]:checked').val() == null) {
			error += "-errorn";
			$('#MusicSkillX').show();
		}
		if ($('[teachSkill=0]:checked').val() == null && $('[teachSkill=1]:checked').val() == null && $('[teachSkill=2]:checked').val() == null && $('[teachSkill=3]:checked').val() == null && $('[teachSkill=4]:checked').val() == null && $('[teachSkill=5]:checked').val() == null) {
			error += "-error\n";
			$('#TeachSkillX').show();
		}
		if ($('[teachDesire=0]:checked').val() == null && $('[teachDesire=1]:checked').val() == null && $('[teachDesire=2]:checked').val() == null && $('[teachDesire=3]:checked').val() == null && $('[teachDesire=4]:checked').val() == null && $('[teachDesire=5]:checked').val() == null) {
			error += "-error\n";
			$('#TeachDesireX').show();
		}

		if (error != "") {
			alert("Please fill out all the required fields on this page before moving on");
		}
		else {
			if ($(this).val() == "save") {
				var data = $('#OtherInformation').serialize();
				$.blockUI();
				$.ajax({
					type: 'POST',
					url: '/Home/OtherInformation',
					async: false,
					data: ({ memberSurvey: data }),
					success: function(){
						$.unblockUI();
					},
					error: function (xhr, textStatus, errorThrown) {
						if (!userAborted(xhr)) {
							alert("Please try again later");
						}
						$.unblockUI();
					}
				});
			}
			else {
				$('#OtherInformation').submit();
			}
		}
		return false;
	});

	//Back Controls
	$('#back1').click(function () {
		// switch to third tab
		$tabs.tabs("enable", 0);
		$tabs.tabs('select', 0);
		$tabs.tabs("disable", 1);
		$tabs.tabs("disable", 2);
		window.scrollTo(0, 0);
		return false;
	});

	$('#back2').click(function () {
		// switch to third tab
		$tabs.tabs("enable", 1);
		$tabs.tabs('select', 1);
		$tabs.tabs("disable", 0);
		$tabs.tabs("disable", 2);
		window.scrollTo(0, 0);
		return false;
	});
});

function _hideErrors(pageNumber) {

	if (pageNumber == 1) {
		$('#PrefNameX').hide();
		$('#GenderX').hide();
		$('#ApartmentX').hide();
		$('#PublishEmailX').hide();
		$('#BirthdayX').hide();
		$('#CellPhoneX').hide();
		$('#PublishCellX').hide();
		$('#EmergContactX').hide();
		$('#EmergPhoneX').hide();
		$('#PriesthoodX').hide();
	}
	else if (pageNumber == 2) {
		$('#MissionX').hide();
		$('#PatriarchalBlessingX').hide();
		$('#EndowedX').hide();
		$('#TempleRecommendX').hide();
		$('#TempleWorkerX').hide();
		$('#PastCallingsX').hide();
	}
	else if (pageNumber == 3) {
		$('#TimeInWardX').hide();
		$('#EnrolledSchoolX').hide();
		$('#ReligionClassX').hide();
		$('#EmployedX').hide();
		$('#MusicSkillX').hide();
		$('#TeachSkillX').hide();
		$('#TeachDesireX').hide();

	}

}

/**
* Returns true if the user hit Esc or navigated away from the
* current page before an AJAX call was done. (The response
* headers will be null or empty, depending on the browser.)
*
* NOTE: this function is only meaningful when called from
* inside an AJAX "error" callback!
*
* The 'xhr' param is an XMLHttpRequest instance.
*/
function userAborted(xhr) {
	return !xhr.getAllResponseHeaders();
}
