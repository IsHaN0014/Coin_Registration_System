
    function openImageInWindow(imageSrc) {
        var newWindow = window.open('', '_blank', 'width=800,height=600');
    newWindow.document.write('<html><head><title>Image</title></head><body style="margin:0;padding:0;">');
        newWindow.document.write('<img src="' + imageSrc + '" style="width:100%;height:100%;object-fit:contain;" />');
        newWindow.document.write('</body></html>');
}


