export var DOMInterop;
(function (DOMInterop) {
    function getBoundingClientRect(element) {
        return element.getBoundingClientRect();
    }
    DOMInterop.getBoundingClientRect = getBoundingClientRect;
    function registerResizeListener(element, elementId) {
        console.log('registerResizeListener:');
        console.log(element);
        var observer = new ResizeObserver(function () {
            DotNet.invokeMethodAsync('LiveChartsCore.SkiaSharpView.Blazor', 'InvokeResize', elementId, element.getBoundingClientRect());
        });
        observer.observe(element);
    }
    DOMInterop.registerResizeListener = registerResizeListener;
})(DOMInterop || (DOMInterop = {}));
//# sourceMappingURL=domInterop.js.map