/*------------------------------------------------------------*
*                                                             *
*            Used to select the different menus, should       *
*            be called on each page                           *
*                                                             *
*-------------------------------------------------------------*/

function MenuSelect(topMenu, lowerMenu) {
    $("#" + topMenu).addClass('current');
    $("#" + topMenu).find('div').addClass('show');
    if (lowerMenu != null) {
        $("#" + lowerMenu).addClass('sub_show');
    }
}