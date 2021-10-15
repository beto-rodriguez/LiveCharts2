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
}
