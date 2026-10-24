# Frontend bundle analysis

Before route splitting, the initial application entrypoint was 1.21 MiB and `app.js` was 193.47 KiB. `chunk-vendors` was 757.21 KiB; PrimeVue/icons, charts, tables, scrolling packages and shared Vue dependencies are its main contributors.

All major public, auth, admin, patient, physician, booking, appointment and chat routes now use dynamic imports. After rebuilding, `app.js` is 22.70 KiB and the entrypoint is 1020 KiB. Feature chunks range from 9.94 KiB to 44.76 KiB. Administration and physician screens are no longer loaded on the patient entry route.

The remaining warning is dominated by the 757.21 KiB shared vendor chunk and large existing image assets (265–558 KiB). Removing required UI/chart dependencies or redesigning imagery is disproportionate for R1.1, so this is non-blocking but should be budgeted later.
