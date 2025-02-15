$(document).ready(function () {
    const container = $('.container');

    $('.register-btn').on('click', function (e) {
        e.preventDefault();
        container.addClass('active');
    });

    $('.login-btn').on('click', function (e) {
        e.preventDefault();
        container.removeClass('active');
    });

});
