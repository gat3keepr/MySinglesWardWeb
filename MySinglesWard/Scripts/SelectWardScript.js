$(document).ready(function () {
    $('[id=ChosenStake]').attr('disabled', 'disabled');
    $('[id=ChosenWard]').attr('disabled', 'disabled');

    $('[id=ChosenArea]').change(function () {
        var Area = $('[id=ChosenArea]').val();
        if (Area == 0) {
            window.location = "/Home/NoWard";
        }

        $.ajax({
            url: "/Home/GetStakes",
            data: ({ location: Area }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (Stake) {
                $('[id=submit]').attr('disabled', 'disabled');
                $('[id=ChosenStake]').attr('disabled', '');
                $('[id=ChosenStake]').find('option').remove();

                //Reset Ward Dropdown
                $('[id=ChosenWard]').find('option').remove();
                $('[id=ChosenWard]').append('<option value="">Please Select Your Ward</option>');

                $stake = $('[id=ChosenStake]');

                $.each(Stake, function (i, stake) {
                    if (i == 0) { $stake.append('<option value="">Please Select Your Stake</option>'); }
                    $stake.append('<option value="' + stake + '">' + stake + ' Stake</option>');
                });
            },
            error: function (error) {
            }

        });
    });

    $('[id=ChosenStake]').change(function () {
        var location = $('[id=ChosenArea]').val();
        var Stake = $('[id=ChosenStake]').val();
        $.ajax({
            url: "/Home/GetWards",
            data: ({ location:location, stake: Stake }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (Ward) {
                $('[id=ChosenWard]').attr('disabled', '');
                $('[id=ChosenWard]').find('option').remove();
                $ward = $('[id=ChosenWard]');

                $.each(Ward, function (i, ward) {
                    if (i == 0) { $ward.append('<option value="">Please Select Your Ward</option>'); }
                    $ward.append('<option value="' + ward.Value + '">' + ward.Text + ' Ward</option>');
                });

                $('#submit').attr('disabled', '');

            },
            error: function (error) {
                alert("An error has occurred: " + error);
            }

        });
    });
});