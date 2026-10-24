module.exports = {
  content: [
    "./public/**/*.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  darkMode: "class",
  theme: {
    extend: {
      width: {
        sidebar: "264px",
        search: "400px",
      },
      rotate: {
        137: "137deg",
      },
      colors: {
        canvas: "#F4F7FB",
        ink: {
          900: "#0B1B2B",
          700: "#334155",
          500: "#64748B",
        },
        midnight: {
          950: "#07111F",
          900: "#0A1524",
          850: "#0D1B2E",
          800: "#12233A",
        },
        primary: {
          50: "#ecfdf8",
          100: "#d1faf0",
          200: "#a7f3e1",
          300: "#6ee7ce",
          400: "#34d3b4",
          500: "#10b99a",
          600: "#059682",
          700: "#04786a",
          800: "#065f55",
          900: "#064e47",
          950: "#022c27",
        },
        accent: {
          50: "#f0f9ff",
          100: "#e0f2fe",
          200: "#bae6fd",
          300: "#7dd3fc",
          400: "#38bdf8",
          500: "#0ea5e9",
        },
        surface: {
          DEFAULT: "#f8fafc",
          dark: "#0f172a",
        },
      },
      fontFamily: {
        sans: ["DM Sans", "system-ui", "-apple-system", "Segoe UI", "sans-serif"],
        display: ["Sora", "system-ui", "-apple-system", "Segoe UI", "sans-serif"],
      },
      boxShadow: {
        "soft": "0 1px 2px rgba(15, 23, 42, 0.04), 0 20px 45px -28px rgba(15, 23, 42, 0.25)",
        "soft-lg": "0 1px 2px rgba(15, 23, 42, 0.05), 0 28px 60px -30px rgba(15, 23, 42, 0.3)",
        "card-dark": "0 1px 0 rgba(255, 255, 255, 0.04) inset, 0 30px 60px -40px rgba(0, 0, 0, 0.9)",
        "glow": "0 24px 70px -24px rgba(13, 148, 136, 0.55)",
        "glow-sm": "0 12px 34px -16px rgba(13, 148, 136, 0.55)",
      },
      animation: {
        "fade-in": "fadeIn 0.35s ease-out",
        "slide-up": "slideUp 0.4s ease-out",
        "pulse-soft": "pulseSoft 4s ease-in-out infinite",
      },
      keyframes: {
        fadeIn: {
          "0%": { opacity: "0" },
          "100%": { opacity: "1" },
        },
        slideUp: {
          "0%": { opacity: "0", transform: "translateY(12px)" },
          "100%": { opacity: "1", transform: "translateY(0)" },
        },
        pulseSoft: {
          "0%, 100%": { opacity: "0.55", transform: "scale(1)" },
          "50%": { opacity: "0.9", transform: "scale(1.04)" },
        },
      },
      backgroundColor: {
        packed: "#f8fafc",
      },
    },
  },
  plugins: [],
};
