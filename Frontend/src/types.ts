export const Role = {
  System: 'system',
  User: 'user',
  Assistant: 'assistant',
  Tool: 'tool',
  Developer: 'developer',
} as const

export type RoleType = (typeof Role)[keyof typeof Role]

/**
 * Status enum matching Backend.Api.Models.Agent.Status
 */
export const Status = {
  Completed: 'completed',
  Failed: 'failed',
  InProgress: 'in_progress',
  Cancelled: 'cancelled',
  Queued: 'queued',
  Incomplete: 'incomplete',
} as const

export type StatusType = (typeof Status)[keyof typeof Status]

export interface ToolCall {
  id?: string
  type?: string
  function: {
    name?: string
    arguments?: string
  }
}

export interface LeaveMessageParams {
  fromName: string
  fromEmail: string
  subject: string
  body: string
}

export const ToolName = {
  LeaveMessage: 'leave_message',
} as const

export type ToolNameType = (typeof ToolName)[keyof typeof ToolName]

export const ToolResponseStatus = {
  Sent: 'sent',
  Failed: 'failed',
  Cancelled: 'cancelled',
} as const

export type ToolResponseStatusType = (typeof ToolResponseStatus)[keyof typeof ToolResponseStatus]

export interface ToolResponse {
  status: string
  parameters?: LeaveMessageParams
}

/**
 * Chat request model matching Backend.Api.Models.Agent.ChatRequest
 */
export interface ChatRequest {
  /** The input string to send to the agent */
  Input: string
  /** Optional previous response ID for context */
  PreviousResponseId?: string
}

/**
 * Message interface matching Backend.Api.Models.Agent.ChatResponse
 * 
 * All fields are optional to support streaming API where messages are built
 * incrementally. Messages are tracked by their position in the array, and React uses
 * array indices as keys when rendering.
 */
export interface Message {
  /** OpenAI Response ID */
  Id?: string
  Status?: StatusType
  Role?: string
  Content?: string
  Refusal?: string
  Reasoning?: string
  ToolCallId?: string
  ToolName?: string
  ToolArguments?: string
}
