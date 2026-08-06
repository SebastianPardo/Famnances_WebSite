// sw.js
// Service worker mínimo — su único propósito es cumplir el requisito de Chrome
// de tener un manejador de "fetch" registrado para poder mostrar el botón de
// instalación (evento beforeinstallprompt). No cachea nada todavía.
//
// Si más adelante quieres soporte offline real, aquí es donde se agregaría
// una estrategia de caché (cache-first, network-first, etc.) — por ahora
// simplemente deja pasar cada petición tal cual, sin modificarla.

self.addEventListener('install', () => {
    self.skipWaiting();
});

self.addEventListener('activate', (event) => {
    event.waitUntil(self.clients.claim());
});

self.addEventListener('fetch', (event) => {
    event.respondWith(fetch(event.request));
});
