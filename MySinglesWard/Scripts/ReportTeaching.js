//Change current Teaching Report
$('[name=visitSelection]').change(function () {
	//Get current selected month
	var prevID = $(this).siblings('[name=visitSelected]').val();
	var newID = $(this).val();

	//Toggle row being showed
	$('#' + prevID + 'visit').hide();
	$('#' + newID + 'visit').fadeIn();

	$(this).siblings('[name=visitSelected]').val(newID);
});

//Submit Teaching Report
$('[name=button]').click(function () {
	var visitID = $(this).attr('visitid');
	var wasVisited = $('[wasVisited=' + visitID + ']').prop("checked");
	var needsAttention = $('[needsAttention=' + visitID + ']').prop("checked");

	$.ajax({
		url: "/Organization/ReportVisit",
		type: "POST",
		data: ({ visitID: visitID, wasVisited: wasVisited, needsAttention: needsAttention }),
		success: function (memberID) {
			success(memberID, visitID);			
		}
	});
});

function success(memberID, visitID) {
	$('#' + memberID + 'X').animate({ backgroundColor: "#36B7F4" }, 400);
	$('#' + memberID + 'X').animate({ backgroundColor: "white" }, 300, "linear", function () {
		$('#' + memberID + 'X').css('background-color', '');

		//show the isReported green check mark
		$('[visitreportedid=' + visitID + ']').fadeIn();
	}); 
}