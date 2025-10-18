import type { Message, ToolCall } from '../types'

/**
 * Creates a message accumulator for streaming responses.
 * Returns an object with the accumulated message and an accumulate function.
 */
export function createMessageAccumulator() {
  const message: Message = {
    role: 'assistant',
    content: '',
    toolCalls: []
  }

  /**
   * Accumulates a delta chunk into the message
   */
  function accumulate(delta: Message): void {
    // Set role if provided (usually comes once at the start)
    if (delta.role) {
      message.role = delta.role
    }

    // Append content (comes incrementally)
    if (delta.content) {
      message.content += delta.content
    }

    // Append refusal if present
    if (delta.refusal) {
      message.refusal = (message.refusal || '') + delta.refusal
    }

    // Accumulate tool calls - add to the last (current) tool call
    if (delta.toolCalls && delta.toolCalls.length > 0) {
      const toolCallDelta = delta.toolCalls[0] // Always use first delta (streaming sends one at a time)
      
      // Get or create the last tool call
      if (!message.toolCalls || message.toolCalls.length === 0) {
        message.toolCalls = [{
          id: '',
          type: 'function',
          function: {
            name: '',
            arguments: ''
          }
        }]
      }
      
      const toolCall = message.toolCalls[message.toolCalls.length - 1]
      
      // Set id (usually comes once in first chunk)
      if (toolCallDelta.id !== undefined) {
        toolCall.id = toolCallDelta.id
      }
      
      // Set type (usually comes once in first chunk)
      if (toolCallDelta.type !== undefined) {
        toolCall.type = toolCallDelta.type
      }
      
      // Accumulate function fields
      if (toolCallDelta.function) {
        // Set function name (usually comes once in first chunk)
        if (toolCallDelta.function.name !== undefined) {
          toolCall.function.name = toolCallDelta.function.name
        }
        
        // Append function arguments (comes incrementally)
        if (toolCallDelta.function.arguments !== undefined) {
          toolCall.function.arguments = (toolCall.function.arguments || '') + toolCallDelta.function.arguments
        }
      }
    }
  }

  /**
   * Checks if the accumulated message has any content
   */
  function hasContent(): boolean {
    return Boolean(message.content && message.content.length > 0) || Boolean(message.toolCalls && message.toolCalls.length > 0)
  }

  /**
   * Gets the complete tool calls (with valid JSON arguments)
   */
  function getCompleteToolCalls(): ToolCall[] {
    if (!message.toolCalls || message.toolCalls.length === 0) {
      return []
    }

    return message.toolCalls.filter(tc => {
      if (!tc.id || !tc.function.name) return false
      
      // Check if arguments is complete JSON
      const args = tc.function.arguments?.trim()
      if (!args) return false
      
      try {
        JSON.parse(args)
        return true
      } catch {
        return false
      }
    })
  }

  /**
   * Gets the current accumulated message state
   */
  function getState() {
    return {
      ...message,
      toolCalls: getCompleteToolCalls()
    }
  }

  return {
    accumulate,
    hasContent,
    getState
  }
}