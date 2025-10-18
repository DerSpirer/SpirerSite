import { Box, Chip, useTheme } from '@mui/material'
import { type RefObject, useState, useEffect } from 'react'
import { useChatContext } from '../../contexts/ChatContext'
import { useIsMobile } from '../../hooks/useIsMobile'

const suggestionTexts = [
  "Tell me about your projects",
  "What technologies do you use?",
  "Share your experience",
  "What's your background?",
]

interface SuggestionChipsProps {
  inputText: string
  inputContainerRef: RefObject<HTMLDivElement | null>
  onChipClick: (text: string) => void
}

function SuggestionChips({ inputText, inputContainerRef, onChipClick }: SuggestionChipsProps) {
  const { hasChatStarted } = useChatContext()
  const theme = useTheme()
  const isMobile = useIsMobile()
  const isVisible = !hasChatStarted || inputText.trim().length === 0
  const [inputRect, setInputRect] = useState<DOMRect | null>(null)

  // Track the input container position
  useEffect(() => {
    const updatePosition = () => {
      if (inputContainerRef.current) {
        setInputRect(inputContainerRef.current.getBoundingClientRect())
      }
    }

    updatePosition()

    // Use ResizeObserver to track when the input container moves or resizes
    const resizeObserver = new ResizeObserver(updatePosition)
    if (inputContainerRef.current) {
      resizeObserver.observe(inputContainerRef.current)
    }

    // Also track window events
    window.addEventListener('resize', updatePosition)
    window.addEventListener('scroll', updatePosition)

    // Update position on animation frames during chat start transition
    let animationFrameId: number
    const animatePosition = () => {
      updatePosition()
      animationFrameId = requestAnimationFrame(animatePosition)
    }
    
    // Run animation updates for a few seconds to catch the transition
    animatePosition()
    const timeoutId = setTimeout(() => {
      cancelAnimationFrame(animationFrameId)
    }, 3000)

    return () => {
      resizeObserver.disconnect()
      window.removeEventListener('resize', updatePosition)
      window.removeEventListener('scroll', updatePosition)
      cancelAnimationFrame(animationFrameId)
      clearTimeout(timeoutId)
    }
  }, [inputContainerRef, hasChatStarted])

  // Render as a horizontal scrollable/flex container above the input
  const offsetPx = hasChatStarted ? 0 : 12
  if (!inputRect) return null

  return (
    <Box
      sx={{
        position: 'fixed',
        left: isMobile ? 0 : `${inputRect.left}px`,
        top: `${inputRect.top}px`,
        transform: `translateY(calc(-100% - ${offsetPx}px))`,
        width: isMobile ? '100vw' : `${inputRect.width}px`,
        overflowX: 'auto',
        display: 'flex',
        alignItems: 'center',
        justifyContent: isMobile ? 'flex-start' : 'center',
        flexWrap: isMobile ? 'nowrap' : 'wrap',
        gap: 1,
        px: isMobile ? 2 : 0,
        opacity: isVisible ? 1 : 0,
        pointerEvents: isVisible ? 'auto' : 'none',
        transition: 'opacity 0.3s, transform 0.4s cubic-bezier(0.4, 0, 0.2, 1)',
        zIndex: 1000,
      }}
    >
      {suggestionTexts.map((text) => (
        <Chip
          key={text}
          label={text}
          onClick={() => onChipClick(text)}
          sx={{
            backgroundColor: theme.customColors.components.suggestionChip,
            border: `1px solid ${theme.customColors.borders.light}`,
            fontSize: '0.85rem',
            flexShrink: 0,
            boxShadow: theme.palette.mode === 'dark' ? theme.customColors.shadows.suggestionChip : 'none',
            transition: 'background-color 0.2s ease, border-color 0.2s ease, opacity 0.2s ease',
            '&:hover': {
              backgroundColor: theme.customColors.components.suggestionChipHover,
              border: `1px solid ${theme.customColors.borders.glow}`,
              opacity: 0.9,
            },
          }}
        />
      ))}
    </Box>
  )
}

export default SuggestionChips
