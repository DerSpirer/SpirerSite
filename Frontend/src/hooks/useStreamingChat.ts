import { useState, useCallback, useRef, useEffect } from 'react'
import { chatApi } from '../api/chatClient'
import type { Message, LeaveMessageParams } from '../types'
import { ToolResponseStatus } from '../types'
import { createUserMessage, createAssistantMessage, createErrorMessage, createToolResponseMessage } from '../utils/messageHelpers'
import { createMessageAccumulator } from '../utils/messageAccumulator'

/**
 * Custom React hook for managing streaming chat functionality with tool call support.
 * 
 * This hook handles:
 * - Sending user messages and receiving streaming assistant responses
 * - Managing message history and loading states
 * - Processing tool calls and tool responses
 * - Automatic continuation after tool responses
 * - Error handling for failed requests
 * - Tracking conversation context via previousResponseId
 */
export function useStreamingChat() {
  const [messages, setMessages] = useState<Message[]>([])
  const [isLoading, setIsLoading] = useState(false)
  const [hasChatStarted, setHasChatStarted] = useState(false)
  const [previousResponseId, setPreviousResponseId] = useState<string | undefined>(undefined)
  
  // Prevents duplicate API calls when the same tool response triggers multiple renders
  const lastProcessedMessageCount = useRef(0)

  /**
   * Generates an assistant response from the backend.
   * Handles streaming responses, tool calls, and error states.
   * Always updates the last message in the array (no IDs needed).
   * 
   * @param userInput - The user's input text to send to the backend
   */
  const generateResponse = useCallback(async (userInput: string) => {
    if (isLoading) return
    
    setIsLoading(true)
    
    const placeholderMessage = createAssistantMessage('')
    setMessages((currentMessages) => [...currentMessages, placeholderMessage])

    try {
      const accumulator = createMessageAccumulator()

      await chatApi.generateResponseStream(userInput, previousResponseId, (chunk) => {
        accumulator.accumulate(chunk)
        const state = accumulator.getState()
        
        // Update previousResponseId if we receive an Id in the chunk
        // This handles multiple responses in a single request (e.g., initial + KB tool call response)
        if (chunk.Id) {
          console.log('Updating response ID:', chunk.Id)
          setPreviousResponseId(chunk.Id)
        }
        
        setMessages((prev) => {
          const updated = [...prev]
          const lastIndex = updated.length - 1
          updated[lastIndex] = state
          return updated
        })
      })

      if (!accumulator.hasContent()) {
        setMessages((prev) => {
          const updated = [...prev]
          updated[updated.length - 1] = createErrorMessage()
          return updated
        })
      }
    } catch (error) {
      console.error('Error generating response:', error)
      setMessages((prev) => {
        const updated = [...prev]
        updated[updated.length - 1] = createErrorMessage()
        return updated
      })
    } finally {
      setIsLoading(false)
    }
  }, [isLoading, previousResponseId])

  /**
   * Sends a user message and triggers an assistant response.
   * This is the main entry point for user interactions.
   * 
   * @param userMessageText - The text content of the user's message
   */
  const sendMessage = useCallback(async (userMessageText: string) => {
    if (userMessageText.trim() === '' || isLoading) return

    if (!hasChatStarted) {
      setHasChatStarted(true)
    }

    const newUserMessage = createUserMessage(userMessageText)
    setMessages((prev) => [...prev, newUserMessage])

    await generateResponse(userMessageText)
  }, [isLoading, hasChatStarted, generateResponse])

  /**
   * Handles accepting the leave_message tool call (contact form submission).
   * 
   * @param toolCallId - The ID of the tool call being responded to
   * @param params - The contact form parameters (email, name, subject, body)
   */
  const handleAcceptLeaveMessage = useCallback(async (toolCallId: string, params: LeaveMessageParams) => {
    let status: string = ToolResponseStatus.Sent
    
    try {
      await chatApi.leaveMessage(params)
    } catch (error) {
      console.error('Error leaving message:', error)
      status = ToolResponseStatus.Failed
    }
    
    const toolResponse = createToolResponseMessage(toolCallId, { status, parameters: params })
    setMessages((prev) => [...prev, toolResponse])
  }, [])

  /**
   * Handles rejecting the leave_message tool call.
   * 
   * @param toolCallId - The ID of the tool call being rejected
   */
  const handleRejectLeaveMessage = useCallback((toolCallId: string) => {
    const toolResponse = createToolResponseMessage(toolCallId, { status: ToolResponseStatus.Cancelled })
    setMessages((prev) => [...prev, toolResponse])
  }, [])

  /**
   * Automatically trigger a follow-up assistant response when a tool response is added.
   * This creates a continuous conversation flow where the assistant can acknowledge
   * tool results and continue the interaction.
   * 
   * TODO: This needs to be reworked with the new backend API that uses previousResponseId
   * instead of sending the full message history.
   */
  useEffect(() => {
    const lastMessage = messages[messages.length - 1]
    
    if (
      lastMessage?.Role === 'tool' && 
      !isLoading && 
      messages.length > lastProcessedMessageCount.current
    ) {
      lastProcessedMessageCount.current = messages.length
      // TODO: Need to send the tool response to the backend properly
      // For now, this functionality is disabled
      console.warn('Tool response follow-up not yet implemented with new API')
    }
  }, [messages, isLoading])

  return {
    messages,
    isLoading,
    hasChatStarted,
    sendMessage,
    handleAcceptLeaveMessage,
    handleRejectLeaveMessage
  }
}
