import { Box } from '@mui/material'
import { useState, useEffect, useRef, type ReactNode } from 'react'

interface FadeOutWrapperProps {
  children: ReactNode
  shouldFadeOut: boolean
  duration?: number // in milliseconds
  onFadeComplete?: () => void
  marginBottom?: number // MUI spacing units (will be multiplied by 8px)
}

function FadeOutWrapper({ 
  children, 
  shouldFadeOut, 
  duration = 600,
  onFadeComplete,
  marginBottom = 0
}: FadeOutWrapperProps) {
  const [isVisible, setIsVisible] = useState(true)
  const [shouldRender, setShouldRender] = useState(true)
  const containerRef = useRef<HTMLDivElement>(null)
  const [height, setHeight] = useState<number | 'auto'>('auto')
  const [scale, setScale] = useState(1)
  const [currentMarginBottom, setCurrentMarginBottom] = useState(marginBottom)

  useEffect(() => {
    if (shouldFadeOut) {
      // Capture current height before starting animation
      if (containerRef.current) {
        const currentHeight = containerRef.current.scrollHeight
        setHeight(currentHeight)
        
        // Force a reflow to ensure height is set before transitioning to 0
        requestAnimationFrame(() => {
          setHeight(0)
          setScale(0)
          setIsVisible(false)
          setCurrentMarginBottom(0)
        })
      }
      
      // Remove from DOM after animation completes
      const timer = setTimeout(() => {
        setShouldRender(false)
        onFadeComplete?.()
      }, duration)

      return () => clearTimeout(timer)
    } else {
      // Fade back in if needed
      setShouldRender(true)
      setIsVisible(true)
      setHeight('auto')
      setScale(1)
      setCurrentMarginBottom(marginBottom)
    }
  }, [shouldFadeOut, duration, onFadeComplete, marginBottom])

  if (!shouldRender) {
    return null
  }

  return (
    <Box
      ref={containerRef}
      sx={{
        opacity: isVisible ? 1 : 0,
        height: height,
        mb: currentMarginBottom,
        transition: `all ${duration}ms ease-in-out`,
        transform: `scale(${scale})`,
        transformOrigin: 'center top',
      }}
    >
      {children}
    </Box>
  )
}

export default FadeOutWrapper
