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
    const activeTickers = new Map();
    function createTicker(dotNetRef) {
        function tick() {
            if (!activeTickers.has(dotNetRef))
                return; // If unsubscribed, stop loop
            dotNetRef
                .invokeMethodAsync("OnFrameTick")
                .catch(() => activeTickers.delete(dotNetRef)); // Auto-clean if component is disposed
            requestAnimationFrame(tick);
        }
        requestAnimationFrame(tick);
        activeTickers.set(dotNetRef, tick);
    }
    function startFrameTicker(dotNetRef) {
        if (activeTickers.has(dotNetRef))
            return;
        createTicker(dotNetRef);
    }
    DOMInterop.startFrameTicker = startFrameTicker;
    function stopFrameTicker(dotNetRef) {
        activeTickers.delete(dotNetRef);
    }
    DOMInterop.stopFrameTicker = stopFrameTicker;
})(DOMInterop || (DOMInterop = {}));
//# sourceMappingURL=domInterop.js.map