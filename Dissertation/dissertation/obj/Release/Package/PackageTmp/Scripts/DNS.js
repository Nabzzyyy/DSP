//Runs JS
$(function () {
    $("#update-dns").unbind().on("click", function () {
        var ClientName = $("#Client").val;
        var DnsSetting = int($("#Dns").val);

        $.ajax({
            url: url.UpdateDns,
            method: "POST",
            data: { ClientName: ClientName, DnsSetting: DnsSetting },
            async: false,
            success: function (data) {
                //Display updated successful
            }
        });
    });
});

