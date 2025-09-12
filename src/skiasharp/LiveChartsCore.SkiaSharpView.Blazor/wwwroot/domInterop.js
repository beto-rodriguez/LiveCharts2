export var DOMInterop;
(function (DOMInterop) {
    const activeTickers = new Map();
    function createTicker(dotNetRef) {
        function tick() {
            if (!activeTickers.has(dotNetRef))
                return;
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