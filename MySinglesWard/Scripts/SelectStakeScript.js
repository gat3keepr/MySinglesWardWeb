$(document).ready(function () {
    $('[id=ChosenStake]').attr('disabled', 'disabled');

    $('[id=ChosenArea]').change(function () {
        var Area = $('[id=ChosenArea]').val();
        if (Area == 0)
            window.location = "/Stake/NoStake";

        $.ajax({
            url: "/Stake/GetStakes",
            data: ({ location: Area }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (Stake) {
                $('[id=submit]').attr('disabled', '');
                $('[id=ChosenStake]').attr('disabled', '');
                $('[id=ChosenStake]').find('option').remove();

                $stake = $('[id=ChosenStake]');

                $.each(Stake, function (i, stake) {
                    if (i == 0) { $stake.append('<option value="">Please Select Your Stake</option>'); }
                    $stake.append('<option value="' + stake.Value + '">' + stake.Text + ' Stake</option>');
                });
            },
            error: function (error) {
            }

        });
    });
});