import axios, { type AxiosInstance } from 'axios'
import { API } from '../constants'
import type { Message } from '../types'

export interface GenerateResponseRequest {
  messages: Message[]
}

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
   * @param messages - Array of messages in the conversation
   * @param onChunk - Callback function called for each chunk received
   * @returns Promise that resolves when streaming is complete
   */
  async generateResponseStream(
    messages: Message[],
    onChunk: (chunk: Message) => void
  ): Promise<void> {
    const request: GenerateResponseRequest = {
      messages,
    }
    let lastProcessedIndex = 0
    let buffer = '' // Buffer for incomplete lines

    try {
      await this.axiosInstance.post(
        API.ENDPOINTS.GENERATE_RESPONSE,
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
                if (data) {
                  try {
                    const chunk = JSON.parse(data) as Message
                    // Only send non-empty chunks
                    if (Object.keys(chunk).length > 0) {
                      onChunk(chunk)
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
          if (data) {
            try {
              const chunk = JSON.parse(data) as Message
              // Only send non-empty chunks
              if (Object.keys(chunk).length > 0) {
                onChunk(chunk)
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
    await this.axiosInstance.post(API.ENDPOINTS.LEAVE_MESSAGE, params)
  }
}

export const chatApi = new ChatApiClient()
