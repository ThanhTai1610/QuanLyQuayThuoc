(function () {
    function loadPartial(containerId, partialPath) {
        var container = document.getElementById(containerId);
        if (!container) return;

        var request = new XMLHttpRequest();
        request.open('GET', partialPath, false);
        request.send(null);

        if (request.status === 200 || request.status === 0) {
            container.outerHTML = request.responseText;
        } else {
            container.innerHTML = '';
            console.error('Khong the tai POS partial:', partialPath, request.status);
        }
    }

    loadPartial('pos-search-header-container', 'partials/pos/search-header.html');
    loadPartial('pos-cart-panel-container', 'partials/pos/cart-panel.html');
    loadPartial('pos-payment-panel-container', 'partials/pos/payment-panel.html');
    loadPartial('pos-modals-container', 'partials/pos/modals.html');
})();
