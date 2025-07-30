declare var DotNet: any;

export namespace DOMInterop {
    export function getBoundingClientRect(
        element: HTMLElement
    ): DOMRect {
        return element.getBoundingClientRect();
    }

    export function registerResizeListener(
        element: HTMLElement,
        elementId: string
    ): void {
        var observer = new ResizeObserver(function () {
            DotNet.invokeMethodAsync(
                'LiveChartsCore.SkiaSharpView.Blazor',
                'InvokeResize',
                elementId,
                element.getBoundingClientRect());
        });
        observer.observe(element);
    }

    export function setPosition(
        element: HTMLElement,
        x: number,
        y: number,
        relativeTo: HTMLElement | undefined
    ) {
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

    const activeTickers = new Map();

    function createTicker(dotNetRef: any) {
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

    export function startFrameTicker(dotNetRef: any): void {
        if (activeTickers.has(dotNetRef)) return;
        createTicker(dotNetRef);
    }

    export function stopFrameTicker(dotNetRef: any): void {
        activeTickers.delete(dotNetRef);
    }
}
