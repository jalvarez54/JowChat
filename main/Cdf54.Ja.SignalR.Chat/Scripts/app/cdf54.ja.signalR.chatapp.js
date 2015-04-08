/* Add extension */
function readURL(input) {
	if (input.files && input.files[0]) {
		var reader = new FileReader();
		reader.onload = function (e) {
			$('#blah').attr('src', e.target.result);
		}

		reader.readAsDataURL(input.files[0]);
	}
}
$("#filePhoto").change(function () {
	readURL(this);
});
/* \Add extension */
