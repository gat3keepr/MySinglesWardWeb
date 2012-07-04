//Script used for Home/Visiting Teaching Managment

//Collapse/Expand controls
$('[name=districtTop]').click(function () {
    $(this).children('[name=plus]').toggle();
    $(this).children('[name=minus]').toggle();
    $(this).parent().siblings('[name=districtBottom]').toggle();
});

//Districts
$("[name=addDistrict]").click(function () {
    //Send the petition
    $('#AddDistrictDialog').dialog('open');
    return false;
});

$("#AddDistrictDialog").dialog({
    autoOpen: false,
    height: 165,
    width: 400,
    modal: false,
    buttons: {
        "Create New District": function () {
            if ($('[name=NewDistrictName]').val() != "") {
                $('#NewDistrict').submit();
            }
            else {
                alert("Please enter a District name.");
            }
        },
        "Cancel": function () {
            $(this).dialog("close");
        }
    }
});

$("[name=RemoveDistrict]").click(function () {
    if (confirm("Are you sure you want to delete this district and all information that belongs to it?") == true) {
        var DistrictID = $(this).attr('districtid');
        //Send the petition
        $.post('/Organization/RemoveDistrict?DistrictID=' + DistrictID, function (data) {
            $('#' + DistrictID + 'District').fadeOut();
        });
    }

    return false;
});

//Add a new Companionship
$("[name=AddCompanionship]").click(function () {
    var DistrictID = $(this).attr('districtid');

    $.post('/Organization/AddCompanionship?DistrictID=' + DistrictID, function (compID) {
        //Add new companionship Table
        var comp = $('#clonableComp').clone();
        $(comp).attr('id', '');
        $(comp).attr('style', '');
        $(comp).children().attr('companionshipid', compID);

        $(comp).find('[name=RemoveCompanionship]').attr('onclick', 'RemoveCompanionship(' + compID + ')');
        $(comp).find('ul').each(function (index) {
            if (index == 0) {
                $(this).attr('companionshipid', compID + "teachers");
            }
            else {
                $(this).attr('companionshipid', compID + "teachees");
            }
        });
        $(comp).find('[name=AddTeacher]').attr('onclick', 'addTeacher(' + compID + ')');
        $(comp).find('[name=AddTeachee]').attr('onclick', 'addTeachee(' + compID + ')');

        //Determine where to add the companionship
        var table = $('#' + DistrictID + 'table');
        var count = parseInt($(table).find('.Companionship').size()) % 2;

        //count will equal two if there is an even amount of companionships
        //A new row will need to be created. else the companionship needs to be appended to a new <td>

        //Create new row
        if (count == 0) {
            $(table).append($("<tr name='compRow'>").append($(comp)));
        }
        else { //Append to table
            $('#' + DistrictID + 'table').find("[name=compRow]:last").append($(comp));
        }
    });

    return false;
});

//Remove Companionship
function RemoveCompanionship(CompanionshipID) {
    $.post('/Organization/RemoveCompanionship?CompanionshipID=' + CompanionshipID, function (compID) {
        $('div [companionshipid=' + CompanionshipID + ']').hide();
    });
}

//Add Teacher to Companionship
$("[name=AddTeacher]").click(function () {
    var companionshipID = $(this).attr('companionshipid');
    //Send the petition
    addTeacher(companionshipID);
    return false;
});

function addTeacher(companionshipID) {
    //Send the petition
    $('#teacherList').load("/Organization/AddTeacherList?CompanionshipID=" + companionshipID, function () {
        $('[name=loading]').hide();
        $('#teacherList').show();
    });
    $('#AddTeacherDialog').dialog('open');
    return false;
}

$("#AddTeacherDialog").dialog({
    autoOpen: false,
    height: 550,
    width: 685,
    modal: true,
    buttons: {
        "Cancel": function () {
            $(this).dialog("close");
            $('[name=loading]').show();
            $('#teacherList').hide();
        }
    }
});

function RemoveTeacher(memberID, companionshipID) {
    $.ajax({
        url: '/Organization/RemoveTeacher',
        traditional: true,
        data: { memberID: memberID, CompanionshipID: companionshipID },
        success: function (data) {
            $('[teacherid=' + memberID + ']').hide();
        },
        error: function (error) {
            alert("Please try again later.");
        }
    });
}

//Teachees
$("[name=AddTeachee]").click(function () {
    var companionshipID = $(this).attr('companionshipid');
    //Send the petition
    addTeachee(companionshipID);
    return false;
});

function addTeachee(companionshipID) {
    //Send the petition
    $('#teacheeList').load("/Organization/AddTeacheeList?CompanionshipID=" + companionshipID, function () {
        $('[name=loading]').hide();
        $('#teacheeList').show();
    });
    $('#AddTeacheeDialog').dialog('open');
    return false;
}

$("#AddTeacheeDialog").dialog({
    autoOpen: false,
    height: 550,
    width: 685,
    modal: true,
    buttons: {
        "Cancel": function () {
            $(this).dialog("close");
            $('[name=loading]').show();
            $('#teacheeList').hide();
        }
    }
});


function RemoveTeachee(memberID, companionshipID) {
    $.ajax({
        url: '/Organization/RemoveTeachee',
        traditional: true,
        data: { memberID: memberID, CompanionshipID: companionshipID },
        success: function (data) {
            $('[teacheeid=' + memberID + ']').hide();
        },
        error: function (error) {
            alert("Please try again later.");
        }
    });
}

//Organizations to Teach
$("[name=addOrg]").click(function () {

    $('#orgToTeachList').load("/Organization/OrgToTeachList", function () {
        $('[name=loading]').hide();
        $('#orgToTeachList').show();
    });
    $('#OrgToTeachDialog').dialog('open');
    return false;
});

$("#OrgToTeachDialog").dialog({
    autoOpen: false,
    height: 450,
    width: 850,
    modal: true,
    buttons: {
        "Close": function () {
            $(this).dialog("close");
            $('[name=loading]').show();
            $('#orgToTeachList').hide();
        }
    }
});

//Change district leader callingID
$('[control=districtLeaderBox]').change(function () {
    var districtID = $(this).attr("districtid");
    var newLeaderID = $(this).val();
    var selectList = $(this);
    $.ajax({
        url: '/Organization/ChangeDistrictLeader',
        traditional: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: { newLeaderCallingID: newLeaderID, districtID: districtID },
        async: false,
        success: function (data) {
            if (data.error != "") {
                if (data.error == "alreadyDistrictLeader") {
                    alert("That calling is already assigned to be a district leader");
                    $(selectList).val(data.callingID);
                }
            }
            else {
                success(districtID);
            }

        },
        error: function (error) {
            alert("Please try again later.");
        }
    });

});

function success(districtID) {
    $('[districtid=' + districtID + 'bg]').animate({ backgroundColor: "#36B7F4" }, 400);
    $('[districtid=' + districtID + 'bg]').animate({ backgroundColor: "white" }, 300, "linear", function () {
        $('[districtid=' + districtID + 'bg]').css('background-color', '');
    });
}

//Set districtLeader boxes to null if there is no value
$(document).ready(function () {
    $('[leader=]').val("");
});

