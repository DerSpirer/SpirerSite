import type { SxProps, Theme } from '@mui/material'

/**
 * Returns container styles for message bubbles based on sender role.
 */
export const getMessageBubbleStyles = (sender: 'user' | 'agent' | 'tool'): SxProps<Theme> => ({
  maxWidth: '70%',
  padding: 2,
  borderRadius: '12px',
  backgroundColor: sender === 'user' 
    ? 'primary.main'
    : sender === 'tool'
    ? (theme) => theme.customColors.overlays.white10
    : (theme) => theme.customColors.components.agentMessage,
  color: 'text.primary',
  boxShadow: (theme) => theme.customColors.shadows.standard,
  border: (theme) => sender === 'agent' 
    ? `1px solid ${theme.customColors.borders.light}`
    : 'none',
})

/**
 * Styles for markdown list containers (ol/ul)
 */
export const listStyles: SxProps<Theme> = {
  margin: '0.5em 0',
  paddingLeft: '1.5em',
  '&:first-of-type': { marginTop: 0 },
  '&:last-child': { marginBottom: 0 },
}

/**
 * Styles for list items (li)
 */
export const listItemStyles: SxProps<Theme> = {
  marginBottom: '0.25em',
}

/**
 * Styles for paragraphs (p)
 */
export const paragraphStyles: SxProps<Theme> = {
  margin: 0,
  marginBottom: '0.5em',
  '&:last-child': { marginBottom: 0 },
}