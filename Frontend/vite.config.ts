import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  base: '/SpirerSite/',
  build: {
    rollupOptions: {
      output: {
        manualChunks: (id) => {
          // Split MUI into its own chunk
          if (id.includes('@mui/material') || id.includes('@mui/icons-material')) {
            return 'mui-core';
          }
          // Split emotion (MUI's styling engine) into its own chunk
          if (id.includes('@emotion/react') || id.includes('@emotion/styled')) {
            return 'mui-emotion';
          }
          // Split React into its own chunk
          if (id.includes('react') || id.includes('react-dom')) {
            return 'react-vendor';
          }
          // Split markdown-related dependencies
          if (id.includes('react-markdown') || id.includes('remark-gfm') || id.includes('rehype-highlight')) {
            return 'markdown';
          }
        },
      },
    },
    chunkSizeWarningLimit: 1000, // Increase limit to 1000kb for chunks we know are large
  },
})
