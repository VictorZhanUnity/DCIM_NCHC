mergeInto(LibraryManager.library, {
    DownloadFileFromBytes: function (ptr, length, fileNamePtr, mimePtr) {

        const bytes = new Uint8Array(HEAPU8.buffer, ptr, length);
        const fileName = UTF8ToString(fileNamePtr);
        const mimeType = UTF8ToString(mimePtr);

        const blob = new Blob([bytes], { type: mimeType });
        const url = URL.createObjectURL(blob);

        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);

        URL.revokeObjectURL(url);
    }
});