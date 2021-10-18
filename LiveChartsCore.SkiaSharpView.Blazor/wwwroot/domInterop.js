export var DOMInterop;
(function (DOMInterop) {
    function getBoundingClientRect(element) {
        return element.getBoundingClientRect();
    }
    DOMInterop.getBoundingClientRect = getBoundingClientRect;
    function registerResizeListener(element, elementId) {
        var observer = new ResizeObserver(function () {
            DotNet.invokeMethodAsync('LiveChartsCore.SkiaSharpView.Blazor', 'InvokeResize', elementId, element.getBoundingClientRect());
        });
        observer.observe(element);
    }
    DOMInterop.registerResizeListener = registerResizeListener;
    function setPosition(element, x, y, relativeTo) {
        var rx = 0;
        var ry = 0;
        if (relativeTo) {
            var bounds = relativeTo.getBoundingClientRect();
            rx = bounds.left;
            ry = bounds.top;
        }
        element.style.top = (y + ry) + 'px';
        element.style.left = (x + rx) + 'px';
    }
    DOMInterop.setPosition = setPosition;
})(DOMInterop || (DOMInterop = {}));
//# sourceMappingURL=domInterop.js.map