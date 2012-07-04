$(document).ready(function () {
    // a workaround for a flaw in the demo system (http://dev.jqueryui.com/ticket/4375), ignore!
    $("#dialog:ui-dialog").dialog("destroy");

    $("#dialog").dialog({
        autoOpen: true,
        height: 257,
        width: 609,
        modal: true,
        resizable: false,
        draggable: false
    });

    $('a.ui-dialog-titlebar-close').remove();

    $("#memberSection").hide();
    $("#bishopricSection").hide();
    $("#stakeSection").hide();

    $("#member").click(function () {
        $("#bishopricSection").hide();
        $("#stakeSection").hide();
        $("#dialog").dialog("close");
        destroyRecaptchaWidget()

        $("#memberSection").fadeIn();
        showRecaptcha('dynamic_recaptcha_member', 'MemberSubmit', 'white');
        $("[user=member]").focus();
    });

    $("#bishopric").click(function () {
        $("#stakeSection").hide();
        $("#memberSection").hide();
        $("#dialog").dialog("close");
        destroyRecaptchaWidget();

        $("#bishopricSection").fadeIn();
        showRecaptcha('dynamic_recaptcha_bishopric', 'BishopricSubmit', 'white');
        $("[user=bishopric]").focus();
    });

    $("#stake").click(function () {
        $("#memberSection").hide();
        $("#bishopricSection").hide();
        $("#dialog").dialog("close");
        destroyRecaptchaWidget()

        $("#stakeSection").fadeIn();

        showRecaptcha('dynamic_recaptcha_stake', 'StakeSubmit', 'white');
        $("[user=stake]").focus();
    });


});

         function showRecaptcha(element, submitButton, themeName) {
             Recaptcha.create("6Lc3idESAAAAAFkSjwtSHxcf58MQOSApJLdKSAHt", element, {
                theme: themeName,
                tabindex: 0
            });
        }

        function destroyRecaptchaWidget() {
            Recaptcha.destroy();
        }