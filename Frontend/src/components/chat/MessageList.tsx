import { Box } from '@mui/material'
import type { Message, LeaveMessageParams } from '../../types'
import MessageBubble from './MessageBubble'
import { useIsMobile } from '../../hooks/useIsMobile'

interface MessageListProps {
  messages: Message[]
  hasChatStarted: boolean
  isLoading: boolean
  onLeaveMessage: (toolCallId: string, params: LeaveMessageParams) => Promise<void>
  onRejectMessage: (toolCallId: string) => void
}

function MessageList({ messages, hasChatStarted, isLoading, onLeaveMessage, onRejectMessage }: MessageListProps) {
  const isMobile = useIsMobile()
  return (
    <Box
      role="log"
      aria-label="Chat messages"
      aria-live="polite"
      sx={{
        flex: 1,
        minHeight: 0,
        overflowY: 'auto',
        display: hasChatStarted ? 'flex' : 'none',
        flexDirection: 'column',
        gap: 2,
        padding: hasChatStarted ? 3 : 0,
        paddingBottom: isMobile ? 3 + 4 : 3 + 12,
        opacity: hasChatStarted ? 1 : 0,
        transition: 'opacity 0.4s cubic-bezier(0.4, 0, 0.2, 1), padding 0.4s cubic-bezier(0.4, 0, 0.2, 1)',
        // Firefox scrollbar
        scrollbarWidth: 'thin',
        scrollbarColor: (theme) => `${theme.customColors.borders.light} ${theme.customColors.overlays.white05}`,
        // Webkit scrollbar (Chrome, Safari, Edge)
        '&::-webkit-scrollbar': {
          width: '8px',
        },
        '&::-webkit-scrollbar-track': {
          background: (theme) => theme.customColors.overlays.white05,
          borderRadius: '4px',
        },
        '&::-webkit-scrollbar-thumb': {
          background: (theme) => theme.customColors.borders.light,
          borderRadius: '4px',
          '&:hover': {
            background: (theme) => theme.customColors.overlays.white25,
          },
        },
      }}
    >
      {messages.map((message, index) => (
        <MessageBubble 
          key={index} 
          message={message}
          allMessages={messages}
          messageIndex={index}
          onLeaveMessage={onLeaveMessage}
          onRejectMessage={onRejectMessage}
          isStreaming={isLoading}
        />
      ))}
    </Box>
  )
}

export default MessageList
