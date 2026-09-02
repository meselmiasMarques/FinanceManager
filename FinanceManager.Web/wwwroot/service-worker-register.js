// Registro do service worker com aviso de nova versão (RF-P01).
(function () {
    if (!('serviceWorker' in navigator)) return;

    var refreshing = false;
    navigator.serviceWorker.addEventListener('controllerchange', function () {
        if (refreshing) return;
        refreshing = true;
        window.location.reload();
    });

    navigator.serviceWorker.register('service-worker.js', { updateViaCache: 'none' }).then(function (reg) {
        function showToast(worker) {
            var toast = document.getElementById('pwa-update-toast');
            var btn = document.getElementById('pwa-update-btn');
            if (!toast || !btn || !worker) return;
            toast.hidden = false;
            btn.onclick = function () {
                btn.disabled = true;
                worker.postMessage('SKIP_WAITING');
            };
        }

        // Já há uma versão nova aguardando (aba reaberta).
        if (reg.waiting && navigator.serviceWorker.controller) {
            showToast(reg.waiting);
        }

        reg.addEventListener('updatefound', function () {
            var installing = reg.installing;
            if (!installing) return;
            installing.addEventListener('statechange', function () {
                if (installing.state === 'installed' && navigator.serviceWorker.controller) {
                    showToast(reg.waiting || installing);
                }
            });
        });
    }).catch(function () { /* registro falhou — app segue funcionando online */ });
})();
