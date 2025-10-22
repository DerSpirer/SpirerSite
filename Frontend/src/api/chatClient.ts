import axios, { type AxiosInstance } from 'axios'
import { API } from '../constants'
import type { ChatRequest, Message } from '../types'

export class ChatApiClient {
  private axiosInstance: AxiosInstance

  constructor(baseUrl: string = API.BASE_URL) {
    this.axiosInstance = axios.create({
      baseURL: baseUrl,
      headers: {
        'Content-Type': 'application/json',
      },
    })
  }

  /**
   * Generates a response from the chat API using Server-Sent Events
   * @param input - The user's input message
   * @param previousResponseId - Optional ID from the previous response for conversation context
   * @param onChunk - Callback function called for each chunk received
   * @returns Promise that resolves when streaming is complete
   */
  async generateResponseStream(
    input: string,
    previousResponseId: string | undefined,
    onChunk: (chunk: Message) => void
  ): Promise<void> {
    const request: ChatRequest = {
      Input: input,
      PreviousResponseId: previousResponseId,
    }
    let lastProcessedIndex = 0
    let buffer = '' // Buffer for incomplete lines

    try {
      await this.axiosInstance.post(
        API.ENDPOINTS.CHAT_STREAM,
        request,
        {
          responseType: 'text',
          onDownloadProgress: (progressEvent) => {
            // Get the cumulative response text
            const responseText = progressEvent.event.target.responseText || ''
            
            // Process only the new part since last update
            const newText = responseText.slice(lastProcessedIndex)
            lastProcessedIndex = responseText.length

            // Add new text to buffer
            const text = buffer + newText
            const lines = text.split('\n')

            // The last element might be an incomplete line, so buffer it
            buffer = lines.pop() || ''

            // Process complete lines
            for (const line of lines) {
              if (line.startsWith('data: ')) {
                const data = line.slice(6) // Remove 'data: ' prefix
                if (data === '[DONE]' || data === '[ERROR]') {
                  continue
                }
                if (data) {
                  try {
                    const message = JSON.parse(data) as Message
                    // Only send non-empty chunks
                    if (Object.keys(message).length > 0) {
                      onChunk(message)
                    }
                  } catch (e) {
                    console.error('Failed to parse SSE data as JSON:', data, e)
                  }
                }
              }
            }
          },
        }
      )

      // Process any remaining buffered line after stream completes
      if (buffer.trim()) {
        if (buffer.startsWith('data: ')) {
          const data = buffer.slice(6)
          if (data && data !== '[DONE]' && data !== '[ERROR]') {
            try {
              const message = JSON.parse(data) as Message
              // Only send non-empty chunks
              if (Object.keys(message).length > 0) {
                onChunk(message)
              }
            } catch (e) {
              console.error('Failed to parse SSE data as JSON:', data, e)
            }
          }
        }
      }
    } catch (error) {
      console.error('Error generating response:', error)
      throw error
    }
  }

  /**
   * Leaves a message for Tom (contact form submission)
   * @param params - Message parameters
   * @returns Promise that resolves when message is sent
   */
  async leaveMessage(params: {
    fromName: string
    fromEmail: string
    subject: string
    body: string
  }): Promise<void> {
    try {
      const response = await this.axiosInstance.post(API.ENDPOINTS.LEAVE_MESSAGE, params)
      
      if (response.data.status !== 'success') {
        throw new Error(response.data.message || 'Failed to send message')
      }
    } catch (error) {
      console.error('Error leaving message:', error)
      throw error
    }
  }
}

export const chatApi = new ChatApiClient()
