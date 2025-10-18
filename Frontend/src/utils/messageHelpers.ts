import type { Message, ToolResponse, LeaveMessageParams } from '../types'
import { Role } from '../types'

export const createErrorMessage = (): Message => ({
  Role: Role.Assistant,
  Content: 'Sorry, I encountered an error. Please try again.',
})

export const createUserMessage = (content: string): Message => ({
  Role: Role.User,
  Content: content,
})

export const createAssistantMessage = (content: string): Message => ({
  Role: Role.Assistant,
  Content: content,
})

export const createToolResponseMessage = (
  toolCallId: string,
  response: ToolResponse
): Message => ({
  Role: Role.Tool,
  Content: JSON.stringify(response),
  ToolCallId: toolCallId,
})

/**
 * Safely parses a tool response from a message content string.
 * Returns null if parsing fails or content is invalid.
 */
export function parseToolResponse(content: string | undefined): ToolResponse | null {
  if (!content) return null
  
  try {
    const parsed = JSON.parse(content) as ToolResponse
    if (parsed && typeof parsed.status === 'string') {
      return parsed
    }
    return null
  } catch (e) {
    console.error('Failed to parse tool response:', e)
    return null
  }
}

/**
 * Safely parses tool call arguments into LeaveMessageParams.
 * Returns null if parsing fails or the JSON is incomplete.
 */
export function parseLeaveMessageParams(args: string | undefined): LeaveMessageParams | null {
  if (!args) return null
  
  try {
    const trimmed = args.trim()
    // Only parse if we have valid JSON (starts with { and ends with })
    if (trimmed.startsWith('{') && trimmed.endsWith('}')) {
      return JSON.parse(trimmed) as LeaveMessageParams
    }
    return null
  } catch (e) {
    console.error('Failed to parse tool call arguments:', e)
    return null
  }
}

/**
 * Finds a tool response message for a given tool call ID.
 */
export function findToolResponseForCall(
  messages: Message[],
  toolCallId: string | undefined
): ToolResponse | null {
  if (!toolCallId) return null
  
  const responseMsg = messages.find(
    (msg) => msg.Role === Role.Tool && msg.ToolCallId === toolCallId
  )
  
  return responseMsg ? parseToolResponse(responseMsg.Content) : null
}
