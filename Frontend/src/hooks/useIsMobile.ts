import { useMediaQuery, useTheme } from '@mui/material'

/**
 * Custom hook to detect if the current viewport is mobile-sized
 * @returns true if viewport width is below 'sm' breakpoint (600px by default)
 */
export function useIsMobile(): boolean {
  const theme = useTheme()
  return useMediaQuery(theme.breakpoints.down('sm'))
}
