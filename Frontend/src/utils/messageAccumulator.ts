import type { Message } from '../types'

/**
 * Creates a message accumulator for streaming responses from the backend.
 * Accumulates Content, Refusal, Reasoning, and tool call information (ToolName, ToolArguments).
 */
export function createMessageAccumulator() {
  const message: Message = {
    Role: 'assistant',
    Content: '',
    Refusal: '',
    Reasoning: '',
    ToolName: undefined,
    ToolArguments: undefined,
    ToolCallId: undefined,
  }

  /**
   * Accumulates a delta chunk into the message
   */
  function accumulate(delta: Message): void {
    // Set Role if provided (usually comes once at the start)
    if (delta.Role) {
      message.Role = delta.Role
    }

    // Set Id if provided
    if (delta.Id) {
      message.Id = delta.Id
    }

    // Set Status if provided
    if (delta.Status) {
      message.Status = delta.Status
    }

    // Append Content (comes incrementally)
    if (delta.Content) {
      message.Content = (message.Content || '') + delta.Content
    }

    // Append Refusal if present
    if (delta.Refusal) {
      message.Refusal = (message.Refusal || '') + delta.Refusal
    }

    // Append Reasoning if present
    if (delta.Reasoning) {
      message.Reasoning = (message.Reasoning || '') + delta.Reasoning
    }

    // Handle tool call information
    if (delta.ToolCallId) {
      message.ToolCallId = delta.ToolCallId
    }

    if (delta.ToolName) {
      message.ToolName = delta.ToolName
    }

    if (delta.ToolArguments) {
      message.ToolArguments = (message.ToolArguments || '') + delta.ToolArguments
    }
  }

  /**
   * Checks if the accumulated message has any content
   */
  function hasContent(): boolean {
    return Boolean(
      (message.Content && message.Content.length > 0) ||
      (message.Reasoning && message.Reasoning.length > 0) ||
      (message.ToolName && message.ToolArguments)
    )
  }

  /**
   * Gets the current accumulated message state
   */
  function getState(): Message {
    return { ...message }
  }

  return {
    accumulate,
    hasContent,
    getState
  }
}