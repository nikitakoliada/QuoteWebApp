function postFav(parameterID, controllerUrl) {
    $.ajax({
        type: "POST",
        dataType: "json",
        url: String(controllerUrl),
        data: { id: parameterID },
        success: function (data) {
            if (data.success) {
                alert(data.message);
            }
        }
    });
}