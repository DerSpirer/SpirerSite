export const CHAT_WINDOW = {
  MAX_WIDTH: '520px',
  HEIGHT: '650px',
  BORDER_RADIUS: '24px',
  INPUT_HEIGHT: '90px',
} as const;

export const FAB = {
  MARGIN: 16,
  SIZE: 40,
  SPACING: 16, // spacing between FABs
} as const

export const API = {
  BASE_URL: import.meta.env.DEV ? import.meta.env.VITE_API_BASE_URL_LOCALHOST : import.meta.env.VITE_API_BASE_URL_PROD,
  ENDPOINTS: {
    GENERATE_RESPONSE: '/api/Chat/GenerateResponse',
    LEAVE_MESSAGE: '/api/chat/LeaveMessage',
  },
} as const;
