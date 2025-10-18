import { createTheme } from '@mui/material/styles'
import type { PaletteMode } from '@mui/material/styles'

declare module '@mui/material/styles' {
  interface Theme {
    glow: {
      color: string
      rgb: string
    }
    customColors: {
      blobs: {
        fuchsia: string
        pink: string
        purple1: string
        purple2: string
        rose: string
        magenta: string
        slate: string
      }
      overlays: {
        paper: string
        paperDark: string
        white05: string
        white10: string
        white12: string
        white25: string
        white30: string
        white85: string
      }
      borders: {
        light: string
        glow: string
      }
      components: {
        suggestionChip: string
        suggestionChipHover: string
        agentMessage: string
      }
      shadows: {
        standard: string
        chatWindow: string
        suggestionChip: string
        suggestionChipHover: string
      }
    }
  }
  interface ThemeOptions {
    glow?: {
      color?: string
      rgb?: string
    }
    customColors?: {
      blobs?: {
        fuchsia?: string
        pink?: string
        purple1?: string
        purple2?: string
        rose?: string
        magenta?: string
        slate?: string
      }
      overlays?: {
        paper?: string
        paperDark?: string
        white05?: string
        white10?: string
        white12?: string
        white25?: string
        white30?: string
        white85?: string
      }
      borders?: {
        light?: string
        glow?: string
      }
      components?: {
        suggestionChip?: string
        suggestionChipHover?: string
        agentMessage?: string
      }
      shadows?: {
        standard?: string
        chatWindow?: string
        suggestionChip?: string
        suggestionChipHover?: string
      }
    }
  }
}

export const createAppTheme = (mode: PaletteMode) => {
  const isDark = mode === 'dark'
  
  return createTheme({
    palette: {
      mode,
      primary: {
        main: isDark ? '#6366f1' : '#6366f1',
      },
      secondary: {
        main: isDark ? '#ec4899' : '#ec4899',
      },
      success: {
        main: isDark ? '#10b981' : '#10b981',
      },
      error: {
        main: isDark ? '#ef4444' : '#ef4444',
      },
      warning: {
        main: isDark ? '#f59e0b' : '#f59e0b',
      },
      background: {
        default: isDark ? '#0a0a0f' : '#fafbfc',
        paper: isDark ? '#1a1a2e' : '#ffffff',
      },
      text: {
        primary: isDark ? '#f0f0f3' : '#1e293b',
        secondary: isDark ? 'rgba(240, 240, 243, 0.7)' : 'rgba(51, 65, 85, 0.7)',
      },
    },
    typography: {
      fontFamily: [
        'Roboto',
        '-apple-system',
        'BlinkMacSystemFont',
        '"Segoe UI"',
        'Arial',
        'sans-serif',
      ].join(','),
    },
    glow: {
      color: isDark ? '#c084fc' : '#a78bfa',
      rgb: isDark ? '192, 132, 252' : '167, 139, 250',
    },
    customColors: {
      blobs: isDark ? {
        fuchsia: '#110a1c',
        pink: '#130a16',
        purple1: '#0e0a18',
        purple2: '#100c1d',
        rose: '#130a14',
        magenta: '#120a1a',
        slate: '#0a0a12',
      } : {
        fuchsia: '#f8f4ff',
        pink: '#fcf2ff',
        purple1: '#f4f1ff',
        purple2: '#ebe8fe',
        rose: '#fff3f4',
        magenta: '#fdeff5',
        slate: '#f4f6fc',
      },
      overlays: isDark ? {
        paper: 'rgba(26, 26, 46, 0.85)',
        paperDark: 'rgba(26, 26, 46, 0.95)',
        white05: 'rgba(255, 255, 255, 0.05)',
        white10: 'rgba(255, 255, 255, 0.1)',
        white12: 'rgba(255, 255, 255, 0.12)',
        white25: 'rgba(255, 255, 255, 0.25)',
        white30: 'rgba(255, 255, 255, 0.3)',
        white85: 'rgba(255, 255, 255, 0.85)',
      } : {
        paper: 'rgba(255, 255, 255, 0.9)',
        paperDark: 'rgba(255, 255, 255, 0.95)',
        white05: 'rgba(99, 102, 241, 0.02)',
        white10: 'rgba(99, 102, 241, 0.04)',
        white12: 'rgba(99, 102, 241, 0.05)',
        white25: 'rgba(99, 102, 241, 0.08)',
        white30: 'rgba(99, 102, 241, 0.1)',
        white85: 'rgba(30, 41, 59, 0.9)',
      },
      borders: {
        light: isDark ? 'rgba(192, 132, 252, 0.2)' : 'rgba(167, 139, 250, 0.25)',
        glow: `rgba(${isDark ? '192, 132, 252' : '167, 139, 250'}, ${isDark ? '0.3' : '0.4'})`,
      },
      components: isDark ? {
        suggestionChip: 'rgba(26, 26, 46, 0.9)',
        suggestionChipHover: 'rgba(99, 102, 241, 0.2)',
        agentMessage: 'rgba(192, 132, 252, 0.08)',
      } : {
        suggestionChip: 'rgba(255, 255, 255, 0.95)',
        suggestionChipHover: 'rgba(167, 139, 250, 0.12)',
        agentMessage: 'rgba(167, 139, 250, 0.08)',
      },
      shadows: {
        standard: isDark 
          ? '0 4px 24px rgba(0, 0, 0, 0.6), 0 8px 48px rgba(99, 102, 241, 0.15)'
          : '0 4px 20px rgba(167, 139, 250, 0.15), 0 2px 8px rgba(99, 102, 241, 0.08)',
        chatWindow: isDark
          ? '0 12px 48px rgba(0, 0, 0, 0.7), 0 0 32px rgba(192, 132, 252, 0.15), 0 4px 16px rgba(99, 102, 241, 0.2)'
          : '0 10px 40px rgba(167, 139, 250, 0.15), 0 4px 20px rgba(99, 102, 241, 0.1), 0 2px 8px rgba(167, 139, 250, 0.08)',
        suggestionChip: isDark
          ? '0 4px 24px rgba(0, 0, 0, 0.6), 0 0 24px rgba(192, 132, 252, 0.12)'
          : '0 4px 16px rgba(167, 139, 250, 0.12), 0 2px 8px rgba(99, 102, 241, 0.08)',
        suggestionChipHover: isDark
          ? '0 0 48px rgba(192, 132, 252, 0.3), 0 0 80px rgba(99, 102, 241, 0.2), 0 8px 32px rgba(0, 0, 0, 0.5)'
          : '0 6px 30px rgba(167, 139, 250, 0.25), 0 4px 16px rgba(99, 102, 241, 0.15), 0 0 20px rgba(167, 139, 250, 0.2)',
      },
    },
  })
}

const theme = createAppTheme('dark')
export default theme
