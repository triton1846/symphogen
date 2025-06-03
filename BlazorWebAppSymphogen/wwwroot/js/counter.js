export function addButtonListeners(element, dotNetHelper, startMethodName, stopMethodName) {
    if (element) {
        element.addEventListener('mousedown', () => dotNetHelper.invokeMethodAsync(startMethodName));
        element.addEventListener('mouseup', () => dotNetHelper.invokeMethodAsync(stopMethodName));
        element.addEventListener('mouseleave', () => dotNetHelper.invokeMethodAsync(stopMethodName));

        // For touch devices
        element.addEventListener('touchstart', () => dotNetHelper.invokeMethodAsync(startMethodName));
        element.addEventListener('touchend', () => dotNetHelper.invokeMethodAsync(stopMethodName));
        element.addEventListener('touchcancel', () => dotNetHelper.invokeMethodAsync(stopMethodName));
    }
}