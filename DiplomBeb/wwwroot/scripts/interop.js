function saveImageWithDialog(filename, bytes) {
    const blob = new Blob([bytes], { type: "image/png" });

    saveWithDownload(blob, filename);
    // Check if File System Access API is available
    /*if ('showSaveFilePicker' in window) {
        saveWithNativeDialog(blob, filename);
    } else {
        
    }*/
}

// 🖼️ Option A: Native Save Dialog (Chromium-based browsers)
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

// 📥 Option B: Fallback to standard download
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