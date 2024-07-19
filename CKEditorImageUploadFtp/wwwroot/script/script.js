$(document).ready(function () {
    var editor = CKEDITOR.instances['description'];
    if (editor) {
        editor.destroy(true);
    }
    CKEDITOR.replace('description', {
        enterMode: CKEDITOR.ENTER_BR,
        filebrowserUploadUrl: '/Home/UploadCKEDITOR',
        filebrowserBrowseUrl: '/Home/FileBrowserCKEDITOR',
    });
});