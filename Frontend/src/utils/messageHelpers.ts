import type { Message, ToolResponse, LeaveMessageParams, ToolCall } from '../types'
import { Role } from '../types'

export const createErrorMessage = (): Message => ({
  role: Role.Assistant,
  content: 'Sorry, I encountered an error. Please try again.',
})

export const createUserMessage = (content: string): Message => ({
  role: Role.User,
  content,
})

export const createAssistantMessage = (content: string): Message => ({
  role: Role.Assistant,
  content,
})

export const createToolResponseMessage = (
  toolCallId: string,
  response: ToolResponse
): Message => ({
  role: Role.Tool,
  content: JSON.stringify(response),
  toolCallId,
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
 * Finds a tool call by name in a message.
 */
export function findToolCall(toolCalls: ToolCall[] | undefined, toolName: string): ToolCall | undefined {
  return toolCalls?.find((tc) => tc.function?.name === toolName)
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
    (msg) => msg.role === Role.Tool && msg.toolCallId === toolCallId
  )
  
  return responseMsg ? parseToolResponse(responseMsg.content) : null
}
