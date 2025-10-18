import { createContext, useContext } from 'react'
import type { ReactNode } from 'react'
import { useStreamingChat } from '../hooks/useStreamingChat'
import type { Message, LeaveMessageParams } from '../types'

interface ChatContextType {
  messages: Message[]
  isLoading: boolean
  hasChatStarted: boolean
  sendMessage: (text: string) => Promise<void>
  handleAcceptLeaveMessage: (toolCallId: string, params: LeaveMessageParams) => Promise<void>
  handleRejectLeaveMessage: (toolCallId: string) => void
}

const ChatContext = createContext<ChatContextType | undefined>(undefined)

export function ChatProvider({ children }: { children: ReactNode }) {
  const chatState = useStreamingChat()
  
  return (
    <ChatContext.Provider value={chatState}>
      {children}
    </ChatContext.Provider>
  )
}

export function useChatContext() {
  const context = useContext(ChatContext)
  if (context === undefined) {
    throw new Error('useChatContext must be used within a ChatProvider')
  }
  return context
}
