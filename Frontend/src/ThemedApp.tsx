import { useEffect, useState, useMemo } from 'react'
import { ThemeProvider } from '@mui/material/styles'
import type { Theme, PaletteMode } from '@mui/material/styles'
import CssBaseline from '@mui/material/CssBaseline'
import App from './App'
import { createAppTheme } from './theme'
import ThemeToggle from './components/layout/ThemeToggle'
import GithubMenu from './components/layout/GithubMenu'
import { ChatProvider } from './contexts/ChatContext'

const THEME_STORAGE_KEY = 'app-theme-mode'

const getInitialThemeMode = (): PaletteMode => {
  const stored = localStorage.getItem(THEME_STORAGE_KEY)
  if (stored === 'light' || stored === 'dark') {
    return stored
  }
  return 'light'
}

const injectThemeColors = (theme: Theme) => {
  const root = document.documentElement
  root.style.setProperty('--color-slate', theme.customColors.blobs.slate)
  root.style.setProperty('--color-fuchsia', theme.customColors.blobs.fuchsia)
  root.style.setProperty('--color-pink', theme.customColors.blobs.pink)
  root.style.setProperty('--color-purple1', theme.customColors.blobs.purple1)
  root.style.setProperty('--color-purple2', theme.customColors.blobs.purple2)
  root.style.setProperty('--color-rose', theme.customColors.blobs.rose)
  root.style.setProperty('--color-magenta', theme.customColors.blobs.magenta)
}

function ThemedApp() {
  const [mode, setMode] = useState<PaletteMode>(getInitialThemeMode)
  const theme = useMemo(() => createAppTheme(mode), [mode])

  useEffect(() => {
    injectThemeColors(theme)
  }, [theme])

  useEffect(() => {
    const isDark = mode === 'dark'
    const existingLink = document.getElementById('highlight-theme')
    
    if (existingLink) {
      existingLink.remove()
    }

    const link = document.createElement('link')
    link.id = 'highlight-theme'
    link.rel = 'stylesheet'
    link.href = isDark
      ? 'https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/styles/github-dark.min.css'
      : 'https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/styles/github.min.css'
    document.head.appendChild(link)

    return () => {
      link.remove()
    }
  }, [mode])

  const toggleTheme = () => {
    setMode((prevMode) => {
      const newMode = prevMode === 'light' ? 'dark' : 'light'
      localStorage.setItem(THEME_STORAGE_KEY, newMode)
      return newMode
    })
  }

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <ChatProvider>
        <GithubMenu />
        <ThemeToggle onToggle={toggleTheme} />
        <App />
      </ChatProvider>
    </ThemeProvider>
  )
}

export default ThemedApp
