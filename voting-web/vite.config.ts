import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";

export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
  ],
  build: {
    rollupOptions: {
      output: {
        manualChunks(id) {
          if (id.includes("node_modules")) {
            if (
              id.includes("react/") ||
              id.includes("react-dom/") ||
              id.includes("react-router-dom/")
            ) {
              return "vendor-react";
            }
            if (id.includes("ethers")) {
              return "vendor-ethers";
            }
            if (id.includes("recharts") || id.includes("d3")) {
              return "vendor-recharts";
            }
          }
        }
      }
    }
  },
  server: {
    proxy: {
      "/api": {
        target: "https://localhost:7297",
        changeOrigin: true,
        secure : false
      },
    },
  },
});
