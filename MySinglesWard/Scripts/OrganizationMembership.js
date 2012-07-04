//Script used to manage the members of a organization

$("[name=ManageMembership]").click(function () {
    //Send the petition
    $('#membershipList').load("/Organization/ManageMembership", function () {
        $('#loading').hide();
    });
    $('#dialog').dialog('open');
    return false;
});

$("#dialog").dialog({
    autoOpen: false,
    height: 550,
    width: 685,
    modal: false,
    buttons: {
        "Add Selected Members": function () {
            $(this).dialog("close");
            $.blockUI();
            var data = $('#AddMembershipList').serialize();
            $.ajax({
                type: 'POST',
                url: '/Organization/ManageMembership',
                async: false,
                data: ({ MemberIDList: data }),
                success: function () {
                    location.reload();
                },
                error: function (xhr, textStatus, errorThrown) {
                    if (!userAborted(xhr)) {
                        $tabs.tabs('select', 0);
                        alert("Please try again later");
                    }
                    $.unblockUI();
                }
            });

        },
        "Cancel": function () {
            $(this).dialog("close");
        }
    }
});