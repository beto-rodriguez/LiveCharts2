export var DOMInterop;
(function (DOMInterop) {
    function getBoundingClientRect(element) {
        return element.getBoundingClientRect();
    }
    DOMInterop.getBoundingClientRect = getBoundingClientRect;
    function registerResizeListener(element, elementId) {
        var observer = new ResizeObserver(function () {
            console.log("[resized invoked js] " + elementId);
            DotNet.invokeMethodAsync('LiveChartsCore.SkiaSharpView.Blazor', 'InvokeResize', elementId, element.getBoundingClientRect());
        });
        observer.observe(element);
    }
    DOMInterop.registerResizeListener = registerResizeListener;
})(DOMInterop || (DOMInterop = {}));
//# sourceMappingURL=domInterop.js.map