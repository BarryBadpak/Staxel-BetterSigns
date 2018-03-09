const SignInfo = {
    hide: () => {
        $('#SignInfo_Container').fadeOut(250, function () {
            stxl.call('onHideSignInfo', '');
        });
    }
};

$(document).ready(function () {
    $('SignInfo_Close').click(function () {
        SignInfo.hide();
        stxl.call('playUISound', 'staxel.sounds.ui.Menu.close');
    });
});

stxl.showSignInfo = () => {
    $('#SignInfo_Container').fadeIn(250, function () {
        stxl.call('onShowSignInfo', '');
    });
};
stxl.hideSignInfo = SignInfo.hide;