// for selecting file
function readFileAsBytes(dotNetHelper) {
    const input = document.getElementById("upFile");

    input.onchange = async () => {
        const file = input.files[0];
        if (!file) return;

        const reader = new FileReader();

        reader.onload = () => {
            const arrayBuffer = reader.result;
            const uint8Array = new Uint8Array(arrayBuffer);
            
            dotNetHelper.invokeMethod('ReceiveFileBytes', Array.from(uint8Array));
        };

        reader.onerror = () => {
            alert("Error reading file.");
        };

        reader.readAsArrayBuffer(file);
    };

    input.click(); 
}

// for saving
function saveImageWithDialog(filename, bytes) {
    const blob = new Blob([bytes], { type: "image/png" });

    saveWithDownload(blob, filename);
    /*if ('showSaveFilePicker' in window) {
        saveWithNativeDialog(blob, filename);
    } else {
        
    }*/
}
async function saveWithNativeDialog(blob, filename) {
    try {
        const handle = await window.showSaveFilePicker({
            suggestedName: filename,
            types: [{
                description: 'PNG Files',
                accept: { 'image/png': ['.png'] },
            }],
        });

        const writable = await handle.createWritable();
        await writable.write(blob);
        await writable.close();
        alert("Image saved successfully!");
    } catch (err) {
        if (err.name !== "AbortError") {
            console.error("Error saving file:", err);
            alert("Failed to save file.");
        }
    }
}
function saveWithDownload(blob, filename) {
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
}