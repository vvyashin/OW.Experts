var password;
$("#check").mousedown(function() {
    password = $("#hidden-pass").val();
    $("#shown-pass").val(password);
    $("#hidden-pass").hide();
    $("#shown-pass").show();
});

$("#check").mouseup(function() {
    password = $("#shown-pass").val();
    $("#hidden-pass").val(password);
    $("#shown-pass").hide();
    $("#hidden-pass").show();
});