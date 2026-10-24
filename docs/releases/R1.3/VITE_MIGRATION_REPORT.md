# Vite Migration Report

## Result

The Vue 3 application now builds and runs with Vite 7.3.7 and `@vitejs/plugin-vue` 6.0.1. Vue CLI, its webpack plugins and the old HTML template/configuration were removed.

Changes include:

- Root `index.html` and `vite.config.mjs`.
- Preserved `@` alias, Vue Router, Vuex, API interceptors and lazy route imports.
- `/api` HTTP and `/chat-hub` WebSocket proxies target `VITE_API_TARGET` (default `http://127.0.0.1:5231`).
- Browser-exposed values now use `VITE_API_BASE_URL` and `VITE_SIGNALR_HUB_URL`; defaults are same-origin proxy paths. No secret was added to client configuration.
- The partner-image webpack context was replaced with eager `import.meta.glob` asset discovery.
- Manual vendor chunks separate Vue, PrimeVue and chart libraries.

## Verification

```powershell
cd telemedicine-spa
npm ci
npm run build
npm run dev -- --host 127.0.0.1 --port 8080 --strictPort
```

The final production build transformed 413 modules in 26.60 seconds. Largest emitted JavaScript chunks were charts 579.94 kB (159.55 kB gzip), Vue 163.85 kB (63.49 kB gzip), and the application entry 132.93 kB (45.34 kB gzip). No Vite chunk-size warning was emitted. This replaces the Release 1.2 webpack entry/vendor output of roughly 1.02 MiB with route and vendor chunking.
