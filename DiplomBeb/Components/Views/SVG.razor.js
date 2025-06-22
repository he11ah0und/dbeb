function parse(fileContent) {
    const parser = new DOMParser();
    const svgDoc = parser.parseFromString(fileContent, "image/svg+xml");
    const svgElement = svgDoc.documentElement;

    // Parse root SVG attributes
    const attributes = {};
    for (let i = 0; i < svgElement.attributes.length; i++) {
        const attr = svgElement.attributes[i];
        attributes[attr.name] = attr.value;
    }

    // Parse all <path> elements and their attributes
    const paths = [];
    const pathElements = svgDoc.querySelectorAll("path");
    pathElements.forEach(path => {
        const pathAttrs = {};
        for (let i = 0; i < path.attributes.length; i++) {
            const attr = path.attributes[i];
            pathAttrs[attr.name] = attr.value;
        }
        paths.push({
            d: path.getAttribute("d") || "",
            attributes: pathAttrs
        });
    });

    return {
        attributes: attributes,
        paths: paths
    };
}