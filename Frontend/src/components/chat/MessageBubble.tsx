import { Box, Skeleton, Button, Typography, TextField, CircularProgress } from '@mui/material'
import SendIcon from '@mui/icons-material/Send'
import CloseIcon from '@mui/icons-material/Close'
import EditIcon from '@mui/icons-material/Edit'
import type { Message, LeaveMessageParams, ToolResponse } from '../../types'
import { Role, ToolName, ToolResponseStatus } from '../../types'
import { getMessageBubbleStyles } from '../../markdown/markdownStyles'
import { useMemo, memo, useState } from 'react'
import { markdownToReact } from '../../markdown/markdownRenderer'
import {
  parseLeaveMessageParams,
  findToolCall,
  findToolResponseForCall,
  parseToolResponse,
} from '../../utils/messageHelpers'

// ===== CONSTANTS =====

const MESSAGE_CONTAINER_STYLES = {
  maxWidth: '70%',
  padding: 2,
  borderRadius: '12px',
  color: 'text.primary',
  boxShadow: (theme: any) => theme.customColors.shadows.standard,
} as const

const FORM_FIELDS = [
  { name: 'fromName', label: 'Name', type: undefined, multiline: false, rows: undefined },
  { name: 'fromEmail', label: 'Email', type: 'email', multiline: false, rows: undefined },
  { name: 'subject', label: 'Subject', type: undefined, multiline: false, rows: undefined },
  { name: 'body', label: 'Message', type: undefined, multiline: true, rows: 4 },
] as const

const TOOL_STATUS_CONFIG = {
  [ToolResponseStatus.Sent]: {
    message: '✓ Message sent successfully!',
    bgColor: 'success.main',
  },
  [ToolResponseStatus.Failed]: {
    message: '✗ Failed to leave message. Please try again.',
    bgColor: 'error.main',
  },
  [ToolResponseStatus.Cancelled]: {
    message: 'Message cancelled.',
    bgColor: 'warning.main',
  },
} as const

// ===== HELPER FUNCTIONS =====

function getRoleType(role: string | undefined): 'user' | 'agent' | 'tool' {
  if (role === Role.User) return 'user'
  if (role === Role.Tool) return 'tool'
  return 'agent'
}

// ===== SUB-COMPONENTS =====

interface ToolResponseBubbleProps {
  toolResponse: ToolResponse
}

function ToolResponseBubble({ toolResponse }: ToolResponseBubbleProps) {
  const config = TOOL_STATUS_CONFIG[toolResponse.status as keyof typeof TOOL_STATUS_CONFIG]
  const statusMessage = config?.message || toolResponse.status
  const bgColor = config?.bgColor || ((theme: any) => theme.customColors.overlays.white10)

  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: 'flex-start',
        width: '100%',
      }}
    >
      <Box
        sx={{
          ...MESSAGE_CONTAINER_STYLES,
          backgroundColor: bgColor,
        }}
      >
        {markdownToReact(statusMessage)}
      </Box>
    </Box>
  )
}

interface TypingIndicatorProps {}

function TypingIndicator({}: TypingIndicatorProps) {
  return (
    <Skeleton
      variant="rounded"
      animation="wave"
      sx={{
        maxWidth: '70%',
        width: '50%',
        height: 48,
        borderRadius: '12px',
        bgcolor: (theme) => theme.customColors.overlays.white10,
      }}
    />
  )
}

interface MessagePreviewProps {
  toolCallId: string
  params: LeaveMessageParams
  onLeave?: (toolCallId: string, params: LeaveMessageParams) => void
  onReject?: (toolCallId: string) => void
  readOnly?: boolean
  sentParams?: LeaveMessageParams
}

function MessagePreview({ 
  toolCallId, 
  params, 
  onLeave, 
  onReject, 
  readOnly = false, 
  sentParams 
}: MessagePreviewProps) {
  const [isEditing, setIsEditing] = useState(false)
  const [isSending, setIsSending] = useState(false)
  const [currentParams, setCurrentParams] = useState<LeaveMessageParams>(params)

  const handleLeave = async () => {
    if (!onLeave) return
    setIsSending(true)
    try {
      await onLeave(toolCallId, currentParams)
    } finally {
      setIsSending(false)
    }
  }

  const handleCancelEdit = () => {
    setIsEditing(false)
  }

  const handleReject = () => {
    if (!onReject) return
    onReject(toolCallId)
  }

  const displayParams = sentParams || currentParams

  if (isEditing && !readOnly) {
    return (
      <Box
        sx={{
          ...MESSAGE_CONTAINER_STYLES,
          backgroundColor: (theme) => theme.customColors.components.agentMessage,
          border: (theme) => `1px solid ${theme.customColors.borders.light}`,
        }}
      >
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          {FORM_FIELDS.map((field) => (
            <TextField
              key={field.name}
              label={field.label}
              type={field.type}
              value={currentParams[field.name as keyof LeaveMessageParams]}
              onChange={(e) =>
                setCurrentParams({ ...currentParams, [field.name]: e.target.value })
              }
              fullWidth
              multiline={field.multiline}
              rows={field.rows}
              size="small"
            />
          ))}

          <Box sx={{ display: 'flex', gap: 1, justifyContent: 'flex-end', mt: 1 }}>
            <Button
              variant="outlined"
              onClick={handleCancelEdit}
              disabled={isSending}
              size="small"
            >
              Cancel
            </Button>
            <Button
              variant="contained"
              startIcon={isSending ? <CircularProgress size={16} /> : <SendIcon />}
              onClick={handleLeave}
              disabled={isSending}
              size="small"
            >
              {isSending ? 'Sending...' : 'Send'}
            </Button>
          </Box>
        </Box>
      </Box>
    )
  }

  return (
    <Box
      sx={{
        ...MESSAGE_CONTAINER_STYLES,
        backgroundColor: (theme) => theme.customColors.components.agentMessage,
        border: (theme) => `1px solid ${theme.customColors.borders.light}`,
      }}
    >
      <Box sx={{ mb: readOnly ? 0 : 2 }}>
        <Typography variant="body2" sx={{ color: 'text.secondary', mb: 0.5 }}>
          <strong>From:</strong> {displayParams.fromName}
        </Typography>
        <Typography variant="body2" sx={{ color: 'text.secondary', mb: 0.5 }}>
          <strong>Email:</strong> {displayParams.fromEmail}
        </Typography>
        <Typography variant="body2" sx={{ color: 'text.secondary', mb: 0.5 }}>
          <strong>Subject:</strong> {displayParams.subject}
        </Typography>
        <Typography variant="body2" sx={{ color: 'text.secondary', mt: 1, whiteSpace: 'pre-wrap' }}>
          <strong>Message:</strong>
        </Typography>
        <Typography variant="body2" sx={{ color: 'text.primary', mt: 0.5, whiteSpace: 'pre-wrap' }}>
          {displayParams.body}
        </Typography>
      </Box>

      {!readOnly && (
        <Box sx={{ display: 'flex', gap: 1, justifyContent: 'flex-end', mt: 2 }}>
          <Button
            variant="outlined"
            startIcon={<CloseIcon />}
            onClick={handleReject}
            disabled={isSending}
            color="error"
            size="small"
          >
            Reject
          </Button>
          <Button
            variant="outlined"
            startIcon={<EditIcon />}
            onClick={() => setIsEditing(true)}
            disabled={isSending}
            size="small"
          >
            Edit
          </Button>
          <Button
            variant="contained"
            startIcon={isSending ? <CircularProgress size={16} /> : <SendIcon />}
            onClick={handleLeave}
            disabled={isSending}
            size="small"
          >
            {isSending ? 'Sending...' : 'Send'}
          </Button>
        </Box>
      )}
    </Box>
  )
}

// ===== MAIN COMPONENT =====

interface MessageBubbleProps {
  message: Message
  allMessages: Message[]
  messageIndex: number
  onLeaveMessage?: (toolCallId: string, params: LeaveMessageParams) => Promise<void>
  onRejectMessage?: (toolCallId: string) => void
  isStreaming?: boolean
}

function MessageBubble({ 
  message, 
  allMessages, 
  messageIndex, 
  onLeaveMessage, 
  onRejectMessage, 
  isStreaming 
}: MessageBubbleProps) {
  // Memoize markdown styles
  const markdownStyles = useMemo(
    () => getMessageBubbleStyles(getRoleType(message.role)),
    [message.role]
  )

  // Handle tool response messages
  if (message.role === Role.Tool) {
    const toolResponse = parseToolResponse(message.content)
    if (!toolResponse) return null
    return <ToolResponseBubble toolResponse={toolResponse} />
  }

  // Show typing animation for empty assistant messages being streamed
  const showTypingAnimation = useMemo(() => {
    return (
      message.role === Role.Assistant &&
      !message.content &&
      isStreaming &&
      messageIndex === allMessages.length - 1
    )
  }, [message.role, message.content, isStreaming, messageIndex, allMessages.length])

  // Find leave_message tool call if present
  const leaveMessageToolCall = useMemo(
    () => findToolCall(message.toolCalls, ToolName.LeaveMessage),
    [message.toolCalls]
  )

  // Find tool response for this tool call
  const toolResponse = useMemo(
    () => findToolResponseForCall(allMessages, leaveMessageToolCall?.id),
    [leaveMessageToolCall?.id, allMessages]
  )

  // Parse tool call parameters
  const messageParams = useMemo(
    () => parseLeaveMessageParams(leaveMessageToolCall?.function?.arguments),
    [leaveMessageToolCall?.function?.arguments]
  )

  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: message.role === Role.User ? 'flex-end' : 'flex-start',
        width: '100%',
      }}
    >
      {showTypingAnimation ? (
        <TypingIndicator />
      ) : messageParams && leaveMessageToolCall?.id ? (
        <Box sx={{ width: '100%', maxWidth: '500px' }}>
          <MessagePreview
            toolCallId={leaveMessageToolCall.id}
            params={messageParams}
            onLeave={toolResponse ? undefined : onLeaveMessage}
            onReject={toolResponse ? undefined : onRejectMessage}
            readOnly={!!toolResponse}
            sentParams={toolResponse?.parameters}
          />
        </Box>
      ) : (
        <Box sx={markdownStyles}>
          {markdownToReact(message.content || '')}
        </Box>
      )}
    </Box>
  )
}

export default memo(MessageBubble)