import { Brightness7, Brightness4 } from '@mui/icons-material'
import { useTheme } from '@mui/material/styles'
import FloatingActionButton from '../common/FloatingActionButton'

interface ThemeToggleProps {
  onToggle: () => void
}

function ThemeToggle({ onToggle }: ThemeToggleProps) {
  const theme = useTheme()
  const isDark = theme.palette.mode === 'dark'

  return (
    <FloatingActionButton
      tooltip={isDark ? 'Switch to light mode' : 'Switch to dark mode'}
      onClick={onToggle}
      ariaLabel="toggle theme"
      fabPosition="secondary"
    >
      {isDark ? <Brightness7 /> : <Brightness4 />}
    </FloatingActionButton>
  )
}

export default ThemeToggle
