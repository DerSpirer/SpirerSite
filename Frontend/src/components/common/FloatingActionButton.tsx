import { IconButton, Tooltip } from '@mui/material'
import type { ReactNode } from 'react'
import { FAB } from '../../constants'

export type FabPosition = 'primary' | 'secondary'

interface FloatingActionButtonProps {
  children: ReactNode
  tooltip: string
  onClick: (event: React.MouseEvent<HTMLElement>) => void
  fabPosition?: FabPosition
  ariaLabel: string
  ariaControls?: string
  ariaHaspopup?: boolean
  ariaExpanded?: boolean
}

/**
 * Shared floating action button component with consistent styling and positioning
 */
function FloatingActionButton({ 
  children,
  tooltip,
  onClick,
  ariaLabel,
  fabPosition,
  ariaControls,
  ariaHaspopup,
  ariaExpanded,
}: FloatingActionButtonProps) {
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
          left: fabPosition === 'primary' ? FAB.MARGIN : FAB.MARGIN + FAB.SIZE + FAB.SPACING,
          zIndex: 1000,
          backgroundColor: (theme) => theme.palette.mode === 'dark' 
            ? 'rgba(40, 40, 60, 0.9)'
            : theme.customColors.overlays.paper,
          border: (theme) => `1px solid ${theme.customColors.borders.light}`,
          boxShadow: (theme) => theme.palette.mode === 'dark' 
            ? '0 4px 24px rgba(0, 0, 0, 0.6), 0 0 32px rgba(192, 132, 252, 0.15)'
            : '0 4px 20px rgba(0, 0, 0, 0.25), 0 2px 10px rgba(0, 0, 0, 0.15)',
          transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
          '&:hover': {
            backgroundColor: (theme) => theme.palette.mode === 'dark' 
              ? 'rgba(50, 50, 75, 0.95)'
              : theme.customColors.overlays.paper,
            border: (theme) => `1px solid ${theme.customColors.borders.glow}`,
            boxShadow: (theme) => theme.palette.mode === 'dark'
              ? '0 6px 32px rgba(0, 0, 0, 0.7), 0 0 48px rgba(192, 132, 252, 0.3)'
              : '0 6px 30px rgba(0, 0, 0, 0.30), 0 4px 16px rgba(0, 0, 0, 0.20)',
            transform: 'scale(1.05)',
          },
        }}
      >
        {children}
      </IconButton>
    </Tooltip>
  )
}

export default FloatingActionButton
