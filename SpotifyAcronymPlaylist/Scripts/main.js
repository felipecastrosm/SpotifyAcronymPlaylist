function HandleAjaxResponse(responseObject) {
	$("#messageHeader").text(responseObject.message);

	if (responseObject.status == "s") {
		$("#messageHeader").css("background-color", "limegreen");
	}
	else if (responseObject.status == "e") {
		$("#messageHeader").css("background-color", "red");
	}

	$("#messageHeader").show();
}

function SaveStarted() {
	$("#savePlaylistBtn").attr("disabled", "disabled");
	$("#savePlaylistBtn").attr("oldVal", $("#savePlaylistBtn").val());
	$("#savePlaylistBtn").val("Saving...");
}

function SaveCompleted() {
	$("#savePlaylistBtn").val($("#savePlaylistBtn").attr("oldVal"));
	$("#savePlaylistBtn").removeAttr("oldVal");
	$("#savePlaylistBtn").removeAttr("disabled");
}