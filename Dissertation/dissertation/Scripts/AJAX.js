var url = new urls();
 /// <summary>
    /// Update tables to add violation into the table.
 /// </summary>
 /// <returns>rows of violations</returns>
function UpdateTable() {

    var id = $("#Client").val();
    console.log(id);
    if (id != null || id != 'Select option...') {
        $.ajax({
            url: url.RetrieveClientInformation,
            method: "POST",
            data: { id: id },
            async: false,
            success: function (data) {
                //Remove previous data from table
                RemoveFromTable();
                $("#client_table").append("<tr><th> Client Name</th><th>OS</th><th>Current IP</th><th>Violation</th><th>Screenshot</th><th>Time of Violation</th></tr>")
                console.log(data);
                var jsonData = JSON.parse(data);

                for (var i = 0; i < jsonData.length; i++) {
                    return AddRow(jsonData[i].Name, jsonData[i].OS, jsonData[i].CurrentIP, jsonData[i].Keyword, jsonData[i].screenshot, jsonData[i].AlertTime)
                }
            }
        });
    }
}

 /// <summary>
    /// gets client name and stores variable when download button is clicked.
 /// </summary>
 /// <returns></returns>
function DownloadSoftware() {
    var clientName = $("#client_name").val();
    if (clientName != null && clientName != '') {
        window.location.href = url.Software + "?clientName=" + clientName;
        console.log(clientName.innerText || console.textContent); 
    }
}


 /// <summary>
    /// Converts base64encoded screenshot to be displayable.
 /// </summary>
 /// <returns></returns>
function GetSource(base64Value) {

    document.getElementById("Screenshot").src = "data:image/png;base64," + String(base64Value)
}


 /// <summary>
    /// Alert struct allowing necessary data to be passed into the Alert API.
 /// </summary>
 /// <returns></returns>
function AddRow(clientName, os, currentIP, keyword, screenshot, alerttime) {
    // Append client Information to the table
    $("#client_table").append("<tr>" +
        "<td>" + clientName + "</td>" +
        "<td>" + os + "</td>" +
        "<td>" + currentIP + "</td>" +
        "<td>" + keyword + "</td>" +
        "<td>" + GetSource(screenshot) + "</td>" + 
        "<td>" + alerttime + "</td>" +
        "</tr>");
}
 /// <summary>
    /// clears table
 /// </summary>
 /// <returns></returns>
function RemoveFromTable() {
    $("#client_table").empty();
}



