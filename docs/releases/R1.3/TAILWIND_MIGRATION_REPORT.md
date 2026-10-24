# Tailwind Migration Report

## Decision

Tailwind 4.3.3 was selected after testing the lower-risk Tailwind 3 path. The latest compatible v3 installation retained six high-severity transitive scanner advisories with no patched v3 resolution, while v4 removed that vulnerable chain and remained compatible with the existing Vue components.

## Changes

- Installed `tailwindcss` 4.3.3 and `@tailwindcss/postcss` 4.3.3.
- Updated PostCSS to use the Tailwind v4 plugin.
- The CSS entry uses `@import "tailwindcss"`, retains the existing JavaScript theme through `@config`, and scans Vue source through `@source`.
- Scoped Vue styles using `@apply` explicitly reference the shared theme stylesheet.
- Existing colors, fonts, breakpoints and component styles remain in `tailwind.config.js`.
- Replaced the obsolete scrollbar integration with `vue3-perfect-scrollbar` 2.x and its supported stylesheet/export.

The production build and seven browser workflows verified representative patient and physician screens. Automated coverage exercises desktop Chromium; a separate visual-baseline comparison at multiple narrow viewport widths was not available, so pixel-level responsive parity remains a manual release-check item.
