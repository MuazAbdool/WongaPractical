/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,jsx,ts,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: "#646cff",
        primaryHover: "#535bf2",
        dark: "#242424",
        lightDark: "#1a1a1a",
      },
    },
  },
  plugins: [],
}

