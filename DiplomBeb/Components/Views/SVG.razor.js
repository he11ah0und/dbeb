function parse(fileContent) {
    const parser = new DOMParser();

    // Remove XML declaration to avoid parsing issues
    fileContent = fileContent.replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", "").trim();

    // Try parsing with text/xml
    const svgDoc = parser.parseFromString(fileContent, "text/xml");

    // Check for parser errors
    const parserError = svgDoc.querySelector("parsererror");
    if (parserError) {
        console.error("SVG parsing failed:", parserError.textContent);
        return null;
    }

    // Ensure we got an SVG document
    if (svgDoc.documentElement.tagName.toLowerCase() !== "svg") {
        console.error("Expected <svg> root element, got:", svgDoc.documentElement.tagName);
        return null;
    }

    function parseElement(element) {
        const result = {
            tagName: element.tagName,
            attributes: {},
            children: []
        };

        // Get attributes
        for (let i = 0; i < element.attributes.length; i++) {
            const attr = element.attributes[i];
            result.attributes[attr.name] = attr.value;
        }

        // Get children
        for (let i = 0; i < element.childNodes.length; i++) {
            const child = element.childNodes[i];
            if (child.nodeType === 1) { // Only element nodes
                result.children.push(parseElement(child));
            }
        }

        return result;
    }

    return {
        Root: parseElement(svgDoc.documentElement)
    };
}