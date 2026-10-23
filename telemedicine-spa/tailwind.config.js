module.exports = {
  content: [
    "./public/**/*.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  darkMode: "class",
  theme: {
    extend: {
      width: {
        sidebar: "280px",
        search: "400px",
      },
      rotate: {
        137: "137deg",
      },
      colors: {
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
        sans: ["DM Sans", "system-ui", "sans-serif"],
        display: ["Sora", "system-ui", "sans-serif"],
      },
      boxShadow: {
        "soft": "0 2px 15px -3px rgba(0, 0, 0, 0.07), 0 10px 20px -2px rgba(0, 0, 0, 0.04)",
        "soft-lg": "0 10px 40px -10px rgba(0, 0, 0, 0.1)",
        "glow": "0 0 40px -10px rgba(16, 185, 154, 0.4)",
      },
      animation: {
        "fade-in": "fadeIn 0.4s ease-out",
        "slide-up": "slideUp 0.4s ease-out",
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
      },
      backgroundColor: {
        packed: "#f8fafc",
      },
    },
  },
  plugins: [],
};
