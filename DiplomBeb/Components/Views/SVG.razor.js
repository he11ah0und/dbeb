function parse (fileContent) {
    const parser = new DOMParser();
    const svgDoc = parser.parseFromString(fileContent, "image/svg+xml");
    const root = svgDoc.documentElement;

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

    // Wrap the root element in a Root property
    return {
        Root: parseElement(root)
    };
}