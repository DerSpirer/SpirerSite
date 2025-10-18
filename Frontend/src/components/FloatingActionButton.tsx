import { IconButton, Tooltip } from '@mui/material'
import type { SxProps, Theme } from '@mui/material'
import type { ReactNode } from 'react'
import { useIsMobile } from '../hooks/useIsMobile'
import { FAB } from '../constants'

export type FabPosition = 'primary' | 'secondary'

interface FloatingActionButtonProps {
  children: ReactNode
  tooltip: string
  onClick: (event: React.MouseEvent<HTMLElement>) => void
  ariaLabel: string
  position?: FabPosition
  ariaControls?: string
  ariaHaspopup?: boolean
  ariaExpanded?: boolean
  sx?: SxProps<Theme>
}

/**
 * Shared floating action button component with consistent styling and positioning
 */
function FloatingActionButton({ 
  children,
  tooltip,
  onClick,
  ariaLabel,
  position = 'primary',
  ariaControls,
  ariaHaspopup,
  ariaExpanded,
  sx = {},
}: FloatingActionButtonProps) {
  const isMobile = useIsMobile()
  
  // Calculate position based on primary/secondary FAB
  const getPositioning = () => {
    if (isMobile) {
      return {
        left: position === 'primary' ? FAB.MARGIN : FAB.MARGIN + FAB.SIZE + FAB.SPACING,
        right: 'auto',
      }
    }
    return {
      right: position === 'primary' ? FAB.MARGIN : FAB.MARGIN + FAB.SIZE + FAB.SPACING,
      left: 'auto',
    }
  }

  return (
    <Tooltip title={tooltip} placement={'right'}>
      <IconButton
        onClick={onClick}
        aria-label={ariaLabel}
        aria-controls={ariaControls}
        aria-haspopup={ariaHaspopup}
        aria-expanded={ariaExpanded}
        sx={{
          position: 'fixed',
          top: FAB.MARGIN,
          ...getPositioning(),
          zIndex: 1000,
          backgroundColor: (theme) => theme.customColors.overlays.paper,
          border: (theme) => `1px solid ${theme.customColors.borders.light}`,
          boxShadow: (theme) => theme.palette.mode === 'dark' 
            ? '0 4px 16px rgba(0, 0, 0, 0.5), 0 2px 8px rgba(0, 0, 0, 0.3)'
            : '0 4px 20px rgba(0, 0, 0, 0.25), 0 2px 10px rgba(0, 0, 0, 0.15)',
          transition: 'all 0.3s ease',
          '&:hover': {
            boxShadow: (theme) => theme.palette.mode === 'dark'
              ? '0 6px 24px rgba(0, 0, 0, 0.6), 0 4px 12px rgba(0, 0, 0, 0.4)'
              : '0 6px 30px rgba(0, 0, 0, 0.30), 0 4px 16px rgba(0, 0, 0, 0.20)',
            transform: 'scale(1.05)',
          },
          ...sx,
        }}
      >
        {children}
      </IconButton>
    </Tooltip>
  )
}

export default FloatingActionButton
