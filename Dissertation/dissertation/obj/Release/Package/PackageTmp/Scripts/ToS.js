//Running the JS file
$(function () {
    GenerateSlider("slider-range", ".slider-time", ".slider-time2");
    GenerateSlider("slider-range2", ".slider-time3", ".slider-time4");
    GenerateSlider("slider-range3", ".slider-time5", ".slider-time6");
    GenerateSlider("slider-range4", ".slider-time7", ".slider-time8");
    GenerateSlider("slider-range5", ".slider-time9", ".slider-time10");
    GenerateSlider("slider-range6", ".slider-time11", ".slider-time12");
    GenerateSlider("slider-range7", ".slider-time13", ".slider-time14");



    $("#update-tos").unbind().on("click", function () {
        var monday = document.getElementById("slider-range").noUiSlider.get();
        var tuesday = document.getElementById("slider-range2").noUiSlider.get();
        var wednesday = document.getElementById("slider-range3").noUiSlider.get();
        var thursday = document.getElementById("slider-range4").noUiSlider.get();
        var friday = document.getElementById("slider-range5").noUiSlider.get();
        var saturday = document.getElementById("slider-range6").noUiSlider.get();
        var sunday = document.getElementById("slider-range7").noUiSlider.get();

        var tosData = [
            sunday,
            monday,
            tuesday,
            wednesday,
            thursday,
            friday,
            saturday
        ];
        var ClientName = $("#Client").val();
        $.ajax({
            url: url.RetrieveClientTosSettings,
            method: "POST",
            data: { tosData: tosData, ClientName: ClientName },
            async: false,
            success: function (data) {
                //Display updated successful
            }
        });
    });
});


function GenerateSlider(elementId, sliderTime, sliderTime2) {

    var element = document.getElementById(elementId);
    noUiSlider.create(element, {
        start: [1, 1440],
        connect: true,
        step: 5,
        range: {
            'min': [0],
            'max': [1439]
        }
    });
    element.noUiSlider.on('slide', function () { SetTime(sliderTime, sliderTime2); });

    function SetTime(sliderTime, sliderTime2) {
        var ui = element.noUiSlider.get();
        var hours1 = Math.floor(ui[0] / 60);
        var minutes1 = ui[0] - (hours1 * 60);
        if (hours1.length == 1) hours1 = '0' + hours1;
        if (minutes1.length == 1) minutes1 = '0' + minutes1;
        if (minutes1 == 0) minutes1 = '00';
        if (hours1 >= 12) {
            if (hours1 == 12) {
                hours1 = hours1;
                minutes1 = minutes1 + " PM";
            } else {
                hours1 = hours1 - 12;
                minutes1 = minutes1 + " PM";
            }
        } else {
            hours1 = hours1;
            minutes1 = minutes1 + " AM";
        }
        if (hours1 == 0) {
            hours1 = 12;
            minutes1 = minutes1;
        }

        $(sliderTime).html(hours1 + ':' + minutes1);

        var hours2 = Math.floor(ui[1] / 60);
        var minutes2 = ui[1] - (hours2 * 60);

        if (hours2.length == 1) hours2 = '0' + hours2;
        if (minutes2.length == 1) minutes2 = '0' + minutes2;
        if (minutes2 == 0) minutes2 = '00';
        if (hours2 >= 12) {
            if (hours2 == 12) {
                hours2 = hours2;
                minutes2 = minutes2 + " PM";
            } else if (hours2 == 24) {
                hours2 = 11;
                minutes2 = "59 PM";
            } else {
                hours2 = hours2 - 12;
                minutes2 = minutes2 + " PM";
            }
        } else {
            hours2 = hours2;
            minutes2 = minutes2 + " AM";
        }

        $(sliderTime2).html(hours2 + ':' + minutes2);
    }
}