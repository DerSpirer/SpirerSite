import { Box } from '@mui/material'
import { useState, useRef, useEffect, useCallback, type RefObject } from 'react'
import { CHAT_WINDOW } from './constants'
import ChatInput from './components/chat/ChatInput'
import MessageList from './components/chat/MessageList'
import { useIsMobile } from './hooks/useIsMobile'
import { useChatContext } from './contexts/ChatContext'

interface ChatWindowProps {
  inputText: string
  onInputTextChange: (text: string) => void
  inputContainerRef: RefObject<HTMLDivElement | null>
}

function ChatWindow({
  inputText,
  onInputTextChange,
  inputContainerRef,
}: ChatWindowProps) {
  const { messages, isLoading, hasChatStarted, sendMessage, handleAcceptLeaveMessage, handleRejectLeaveMessage } = useChatContext()
  const [inputAreaHeight, setInputAreaHeight] = useState<string>(CHAT_WINDOW.INPUT_HEIGHT)
  const inputRef = useRef<HTMLTextAreaElement>(null)

  // Measure the input area height dynamically
  useEffect(() => {
    if (inputContainerRef.current) {
      const height = inputContainerRef.current.offsetHeight
      setInputAreaHeight(`${height}px`)
    }
  }, [inputContainerRef])

  const handleSend = useCallback(async () => {
    if (inputText.trim() === '' || isLoading) return
    const userMessageText = inputText
    onInputTextChange('')
    await sendMessage(userMessageText)
  }, [inputText, isLoading, sendMessage, onInputTextChange])

  const handleKeyDown = useCallback((e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault()
      handleSend()
    }
  }, [handleSend])

  const isMobile = useIsMobile()

  return (
    <Box
      component="main"
      role="region"
      aria-label="Chat interface"
      sx={{
        width: '100%',
        maxWidth: isMobile && hasChatStarted ? '100%' : CHAT_WINDOW.MAX_WIDTH,
        height: hasChatStarted ? '100%' : inputAreaHeight,
        maxHeight: hasChatStarted ? '100%' : inputAreaHeight,
        backgroundColor: (theme) => theme.palette.background.default,
        borderRadius: isMobile && hasChatStarted ? 0 : CHAT_WINDOW.BORDER_RADIUS,
        border: (theme) => isMobile && hasChatStarted ? 'none' : `1px solid ${theme.customColors.borders.light}`,
        boxShadow: (theme) =>
          isMobile && hasChatStarted ? 'none' : theme.customColors.shadows.chatWindow,
        display: 'flex',
        flexDirection: 'column',
        justifyContent: hasChatStarted ? 'flex-end' : 'center',
        position: 'relative',
        overflow: 'hidden',
        transition: 'all 0.4s cubic-bezier(0.4, 0, 0.2, 1)',
        flexShrink: 1,
      }}
    >
      {/* Messages Container */}
      <MessageList
        messages={messages}
        hasChatStarted={hasChatStarted}
        isLoading={isLoading}
        onLeaveMessage={handleAcceptLeaveMessage}
        onRejectMessage={handleRejectLeaveMessage}
      />

      <ChatInput
        inputText={inputText}
        isLoading={isLoading}
        inputRef={inputRef}
        containerRef={inputContainerRef}
        onInputChange={onInputTextChange}
        onSend={handleSend}
        onKeyDown={handleKeyDown}
      />
    </Box>
  )
}

export default ChatWindow
