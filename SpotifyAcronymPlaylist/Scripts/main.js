function HandleAjaxResponse(responseObject) {
	$("#messageHeader").text(responseObject.message);

	if (responseObject.status == "s") {
		$("#messageHeader").backgroundColor = "green";
	}
	else if (responseObject.status == "e") {
		$("#messageHeader").backgroundColor = "red";
	}

	$("#messageHeader").show();
}