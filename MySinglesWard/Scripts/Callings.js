var sorting = false;
var CALLED = 2;

$(document).ready(function () {


    //Collapse/Expand controls
    $('[name=orgTop]').click(function () {
        if (sorting == false) {
            $(this).children('[name=plus]').toggle();
            $(this).children('[name=minus]').toggle();
            $(this).parent().siblings('[name=orgBottom]').toggle();
        }

    });

    $('[name=collapseAll]').click(function () {
        collapseAll();
    });

    $('[name=expandAll]').click(function () {
        expandAll();
    });

    $('[name=AddCalling]').click(function () {
        var orgID = $(this).attr('orgID');

        AddCalling(orgID);
    });

    $("[name=update]").click(function () {
        updateCalling($(this).attr("callingid"), this);
    });

    //Remove Organization
    $("[name=RemoveOrganization]").click(function () {
        if (confirm("Are you sure you want to remove this organization and all callings associated with it?") == true) {
            var orgID = $(this).attr("orgid");

            $.ajax({
                url: '/Calling/RemoveOrganization',
                data: { orgID: orgID },
                success: function (data) {
                    $('#' + orgID).fadeOut();
                },
                error: function () {
                    alert("Please try again later");
                }
            });
        }
    });

    //Remove Calling
    $("[name=remove]").click(function () {
        var callingID = $(this).attr("callingid");
        removeCalling(callingID, this);
    });

    //Release Member
    $("[name=release]").click(function () {
        var callingID = $(this).attr("callingid");
        releaseMember(callingID, this);
    });

    $("[name=LeaderCallingID]").change(function () {
        var orgID = $(this).attr("orgLeader");
        var callingLeaderID = $(this).val();

        $.ajax({
            url: '/Calling/UpdateCallingLeader',
            data: { orgID: orgID, callingLeaderID: callingLeaderID },
            success: function (data) {
                if (data == "True") {
                    success(orgID);
                }
                else {
                    alert("That calling is already assigned as a co-leader calling.");
                }
            },
            error: function () {
                alert("Please try again later");
            }
        });
    });

    $("[name=ReportID]").change(function () {
        var orgID = $(this).attr("reportID");
        var ReportID = $(this).val();

        $.ajax({
            url: '/Calling/UpdateReportID',
            data: { orgID: orgID, ReportID: ReportID },
            success: function (data) {
                success(orgID);
            },
            error: function () {
                alert("Please try again later");
            }
        });
    });

    //DIALOG BOXES
    $("#descriptionEdit").dialog({
        autoOpen: false,
        height: 375,
        width: 800,
        modal: false,
        buttons: {
            "Save": function () {
                var callingID = $('#descriptionCallingID').val();
                $("[description=" + callingID + "]").val($('#CallingDescriptionText').val());
                var description = $('#CallingDescriptionText').val();

                $.ajax({
                    url: '/Calling/UpdateCallingDescription',
                    type: "POST",
                    data: { id: callingID, description: description },
                    success: function (data) {

                    },
                    error: function () {
                        alert("Check your internet connection and please try again later.");
                    }
                });

                $(this).dialog("close");
            },
            "Close": function () {
                var callingID = $('#descriptionCallingID').val();
                $("[description=" + callingID + "]").val($('#CallingDescriptionText').val());
                $(this).dialog("close");
            }
        }
    });

    $("[name=description]").click(function () {
        var callingID = $(this).attr("callingid");
        var orgID = $(this).attr("callingorgid");

        editDescription(orgID, callingID);
    });

    $("[name=coleaderDialog]").dialog({
        autoOpen: false,
        height: 365,
        width: 800,
        modal: false,
        buttons: {
            "Add Co-Leader": function () {
                var orgID = $(this).attr("orgid");
                var callingLeaderID = $(this).find("[name=newCoLeader" + orgID + "]").val();
                if (callingLeaderID == null) {
                    return false;
                }

                $.ajax({
                    url: '/Calling/AddCoLeader',
                    type: "POST",
                    data: { id: orgID, callingLeaderID: callingLeaderID },
                    success: function (data) {
                        if (data == "True") {
                            var text = $("#newCoLeader" + orgID + " option[value=" + callingLeaderID + "]").text();
                            $("#" + orgID + "table").append("<tr id=\"" + callingLeaderID + "coleader\"><td>" + text +
                            "</td><td><span name=\"removeCoLeaderCalling\" title=\"Remove Co-Leader Calling\" callingid=\"" + callingLeaderID + "\" " +
                        "class=\"icon-2 info-tooltip\" onClick=\"removeCoLeaderCalling(" + callingLeaderID + ")\"></span></td></tr>");
                        }
                        else {
                            alert("That calling is already a leadership calling.");
                        }
                    },
                    error: function () {
                        alert("Check your internet connection and please try again later.");
                    }
                });
            },
            "Close": function () {
                $(this).dialog("close");
            }
        }
    });

    $("#AddOrganization").dialog({
        autoOpen: false,
        height: 245,
        width: 800,
        modal: false,
        buttons: {
            "Add Organization": function () {
                if (confirm("You have saved all changes made to the callings?") == true) {
                    $.blockUI();
                    $(this).dialog("close");
                    var orgName = $("#NewOrgName").val();
                    var orgPreset = $("#orgPreset").val();

                    $.ajax({
                        url: '/Calling/AddOrganization',
                        type: "POST",
                        data: { orgName: orgName, orgPreset: orgPreset },
                        success: function (data) {
                            window.location = "/Calling";
                        },
                        error: function () {
                            $.unblockUI();
                            alert("Check your internet connection and please try again later.");
                        }
                    });
                }
            },
            "Close": function () {
                $(this).dialog("close");
            }
        }
    });

    $("[name=addOrg]").click(function () {
        $('#AddOrganization').dialog('open');
    });

    $("[name=AddCoLeaderCalling]").click(function () {
        var orgID = $(this).attr('orgid');
        $('#' + orgID + 'dialog').dialog('open');
    });

    $("[name=removeCoLeaderCalling]").click(function () {
        var callingLeaderID = $(this).attr('callingid');
        removeCoLeaderCalling(callingLeaderID);
    });

});

$('[name=sortable]').sortable();

$('[name=sortable]').bind("sortupdate", function (event, ui) {
    var result = $(this).sortable('toArray');
    var orgID = $(this).attr('orgid');

    $.ajax({
        url: '/Calling/SortCallings',
        dataType: 'json',
        type: 'POST',
        traditional: true,
        data: { orgID: orgID, result: result }
    });

});

$('#sortableOrg').sortable();
$('#sortableOrg').sortable('disable');
$('[name=sortOrg]').click(function () {
    if (sorting == false) {
        sorting = true;
        collapseAll();
        //Show Loading Icon and change Text
        $('#sorting').show();
        $('#sortingTitle').attr('style', 'color:#36B7F4');
        $('#sortingTitle').html('Sorting Organizations');

        $('#sortableOrg').sortable('enable');
        $('#sortableOrg').disableSelection();
    }
    else {
        sorting = false;
        expandAll();
        $('#sorting').hide();
        $('#sortingTitle').attr('style', '');
        $('#sortingTitle').html('Sort Organizations');

        var result = $('#sortableOrg').sortable('toArray');

        $.ajax({
            url: '/Calling/SortOrganizations',
            dataType: 'json',
            type: 'POST',
            traditional: true,
            data: { result: result }
        });

        $('#sortableOrg').sortable('disable');
    }

    
});

function success(orgID) {
        $('[orgID=' + orgID + 'bg]').animate({ backgroundColor: "#36B7F4" }, 400);
        $('[orgID=' + orgID + 'bg]').animate({ backgroundColor: "white" }, 300, "linear", function () {
            $('[orgID=' + orgID + 'bg]').css('background-color', '');
        });
    }

function expandAll() {
    $('[name=orgBottom]').show();
    $('[name=plus]').hide();
    $('[name=minus]').show();
}

function collapseAll() {
    $('[name=orgBottom]').hide();
    $('[name=plus]').show();
    $('[name=minus]').hide();
}

function removeCoLeaderCalling(callingLeaderID) {
    $.ajax({
        url: '/Calling/RemoveCoLeader',
        type: "POST",
        data: { callingLeaderID: callingLeaderID },
        success: function (data) {
            $('#' + callingLeaderID + 'coleader').fadeOut();
        },
        error: function () {
            alert("Check your internet connection and please try again later.");
        }
    });
}

function AddCalling(orgID) {
    $.ajax({
        url: '/Calling/AddCalling',
        data: { orgID: orgID },
        success: function (callingID) {
            var newCalling = $('#newCalling').clone();
            $(newCalling).attr('id', callingID);
            $(newCalling).find("[name=title]").attr('title', callingID);
            $(newCalling).find("[name=Description]").attr('description', callingID);
            $(newCalling).find("[name=MemberID]").attr('assignment', callingID);
            $(newCalling).find("[name=MemberID]").val('');
            $(newCalling).find("[name=CallingStatus]").attr('status', callingID);
            $(newCalling).find("[name=description]").attr('callingid', callingID);
            $(newCalling).find("[name=description]").attr('callingorgid', orgID);
            $(newCalling).find("[name=description]").attr('onClick', "editDescription(" + orgID + "," + callingID + ")");
            $(newCalling).find("[name=ITStake]").attr('its', callingID);
            $(newCalling).find("[name=ITStake]").attr('onClick', "clickITS(this)");
            $(newCalling).find(".ui-checkbox").attr('onClick', "clickITS(this)");
            $(newCalling).find(".ui-checkbox").attr('onMouseOver', "hoverITS(this)");
            $(newCalling).find(".ui-checkbox").attr('onMouseout', "hoverITS(this)");
            $(newCalling).find("[name=update]").attr('onClick', "updateCalling(\"" + callingID + "\",this)");
            $(newCalling).find("[name=update]").attr('callingid', callingID);
            $(newCalling).find("[name=update]").attr('callingorgid', orgID);
            $(newCalling).find("[name=release]").attr('onClick', "releaseMember(\"" + callingID + "\",this)");
            $(newCalling).find("[name=release]").attr('callingid', callingID);
            $(newCalling).find("[name=remove]").attr('onClick', "removeCalling(\"" + callingID + "\",this)");
            $(newCalling).find("[name=remove]").attr('callingid', callingID);
            $(newCalling).find("[name=remove]").attr('callingorgid', orgID);
            $('#' + orgID + 'ul').append(newCalling);
            $("[orgLeader=" + orgID + "]").append("<option value=\"" + callingID + "\"></option>");
            $("#newCoLeader" + orgID).append("<option value=\"" + callingID + "\"></option>");

            $(newCalling).fadeIn();

        },
        error: function (error) {
            alert("An error has occurred: " + error);
        }
    });
}

function updateCalling(callingID, element) {
    var orgID = $(element).attr("callingorgid");
    var title = $("[title=" + callingID + "]").val();
    var assignment = $("[assignment=" + callingID + "]").val();
    var status = $("[status=" + callingID + "]").val();
    var its = $("[its=" + callingID + "]").attr("checked");
    var description = $("[description=" + callingID + "]").val();

    $.ajax({
        url: '/Calling/UpdateCalling',
        type: "POST",
        data: { id: callingID, title: title, assignment: assignment, status: status, its: its, description: description },
        success: function (data) {
            $("[orgLeader=" + orgID + "]").children("[value=" + callingID + "]").html(title);
            $("#newCoLeader" + orgID).children("[value=" + callingID + "]").html(title);

            if (status < CALLED) {
                $("[assignment=" + callingID + "]").attr("disabled", '');
            }
               else {
				if(assignment != "")
					$("[assignment=" + callingID + "]").attr("disabled", "disabled");
            }
            success(orgID);
        },
        error: function () {
            alert("Please try again later");
        }
    });
}

function removeCalling(callingID, element) {
    var orgID = $(element).attr("callingorgid");

    $.ajax({
        url: '/Calling/RemoveCalling',
        data: { callingID: callingID },
        success: function (data) {
            $('#' + callingID).fadeOut();
            $("[orgLeader=" + orgID + "]").children("[value=" + callingID + "]").remove();
            $("#newCoLeader" + orgID).children("[value=" + callingID + "]").remove();
        },
        error: function () {
            alert("Please try again later");
        }
    });
}

function releaseMember(callingID, element) {
    var orgID = $(element).attr("callingorgid");

    $.ajax({
        url: '/Calling/ReleaseMember',
        data: { callingID: callingID },
        success: function (data) {
            if (data == "True") {
                $("[assignment=" + callingID + "] option:first").attr('selected', 'selected');
                $("[status=" + callingID + "] option:first").attr('selected', 'selected');
                $("[assignment=" + callingID + "]").attr("disabled", "");
            }
            else {
                alert("Member has not been called to this calling.");
            }
        },
        error: function () {
            alert("Please try again later");
        }
    });
}

function editDescription(orgID, callingID) {
    $('#CallingDescriptionText').val($("[description=" + callingID + "]").val());
    $('#descriptionCallingID').val(callingID);
    $('#descriptionEdit').dialog('open');
}

function clickITS(element) {
    $(element).toggleClass("ui-checkbox-state-checked");
}

function hoverITS(element) {
    $(element).toggleClass("ui-checkbox-state-hover");
}
