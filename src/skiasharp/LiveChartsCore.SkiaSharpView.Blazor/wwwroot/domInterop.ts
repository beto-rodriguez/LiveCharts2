declare var DotNet: any;

export namespace DOMInterop {
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
