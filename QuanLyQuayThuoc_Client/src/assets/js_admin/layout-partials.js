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
            console.error('Khong the tai partial:', partialPath, request.status);
        }
    }

    function markActiveSidebarItem() {
        var sidebar = document.getElementById('accordionSidebar');
        if (!sidebar) return;

        var currentPath = window.location.pathname.split('/').pop() || 'index.html';
        var links = sidebar.querySelectorAll('.nav-item .nav-link');
        for (var i = 0; i < links.length; i++) {
            var link = links[i];
            var item = link.closest('.nav-item');
            if (!item) continue;
            item.classList.remove('active');

            var href = link.getAttribute('href') || '';
            if (href === currentPath) {
                item.classList.add('active');
            }
        }
    }

    loadPartial('sidebar-container', 'partials/sidebar.html');
    loadPartial('topbar-container', 'partials/topbar.html');
    loadPartial('footer-container', 'partials/footer.html');
    markActiveSidebarItem();
})();
