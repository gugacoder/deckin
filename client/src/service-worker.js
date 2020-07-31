importScripts(
  "/precache-manifest.30e1256d9a5c1b1d8e03fe56a6388b8a.js"
);

workbox.core.setCacheNameDetails({prefix: "paper-app"});

self.addEventListener('message', (event) => {
  if (event.data && event.data.type === 'SKIP_WAITING') {
    self.skipWaiting();
  }
});

self.__precacheManifest = [].concat(self.__precacheManifest || []);
workbox.precaching.precacheAndRoute(self.__precacheManifest, {});
