import { Box, TextField, IconButton, CircularProgress, useTheme } from '@mui/material'
import SendIcon from '@mui/icons-material/Send'
import type { RefObject } from 'react'

interface ChatInputProps {
  inputText: string
  isLoading: boolean
  inputRef: RefObject<HTMLTextAreaElement | null>
  containerRef?: RefObject<HTMLDivElement | null>
  onInputChange: (value: string) => void
  onSend: () => void
  onKeyDown: (e: React.KeyboardEvent) => void
}

function ChatInput({ 
  inputText, 
  isLoading, 
  inputRef,
  containerRef,
  onInputChange, 
  onSend, 
  onKeyDown
}: ChatInputProps) {
  const theme = useTheme()
  
  return (
    <Box
      ref={containerRef}
      sx={{
        display: 'flex',
        gap: 1,
        alignItems: 'flex-end',
        padding: 2,
        borderRadius: '12px',
        opacity: 1,
        flexShrink: 0,
      }}
    >
      <TextField
        fullWidth
        multiline
        maxRows={4}
        variant="outlined"
        placeholder="Ask me anything..."
        value={inputText}
        onChange={(e) => onInputChange(e.target.value)}
        onKeyDown={onKeyDown}
        disabled={isLoading}
        inputRef={inputRef}
        slotProps={{
          htmlInput: {
            'aria-label': 'Chat message input',
          },
        }}
        sx={{
          '& .MuiOutlinedInput-root': {
            borderRadius: '12px',
            backgroundColor: theme.customColors.overlays.white05,
            border: `1px solid ${theme.customColors.borders.light}`,
            boxShadow: theme.customColors.shadows.standard,
            transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
            '& fieldset': {
              border: 'none',
            },
            '&:hover': {
              border: `1px solid ${theme.customColors.borders.glow}`,
              '& fieldset': {
                border: 'none',
              },
            },
            '&.Mui-focused': {
              border: `1px solid ${theme.customColors.borders.glow}`,
              boxShadow: theme.palette.mode === 'dark' 
                ? `0 0 32px rgba(192, 132, 252, 0.25), ${theme.customColors.shadows.standard}`
                : theme.customColors.shadows.standard,
              '& fieldset': {
                border: 'none',
              },
            },
          '&.Mui-disabled': {
            '& input': {
              color: (theme) => theme.palette.text.primary,
              WebkitTextFillColor: (theme) => theme.palette.text.primary,
            },
          },
        },
        '& .MuiInputBase-input.Mui-disabled': {
          color: (theme) => theme.palette.text.primary,
          WebkitTextFillColor: (theme) => theme.palette.text.primary,
        },
        }}
      />
      <IconButton
        onClick={onSend}
        disabled={isLoading}
        aria-label={isLoading ? 'Sending message' : 'Send message'}
        sx={{
          backgroundColor: 'primary.main',
          boxShadow: theme.customColors.shadows.standard,
          transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
          '&:hover': {
            backgroundColor: 'primary.dark',
            boxShadow: theme.palette.mode === 'dark' 
              ? `0 0 24px rgba(99, 102, 241, 0.5), ${theme.customColors.shadows.standard}`
              : theme.customColors.shadows.standard,
            transform: 'scale(1.05)',
          },
          '&.Mui-disabled': {
            backgroundColor: theme.customColors.overlays.white12,
          },
          width: 48,
          height: 48,
        }}
      >
        {isLoading ? (
          <CircularProgress size={24} sx={{ color: 'white' }} />
        ) : (
          <SendIcon sx={{ color: 'white' }} />
        )}
      </IconButton>
    </Box>
  )
}

export default ChatInput
