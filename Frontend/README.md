# Tom Spirer Portfolio

A modern, interactive portfolio website featuring an AI-powered chat interface to learn more about Tom Spirer's work and experience.

## Features

- 💬 **Interactive Chat Interface** - Engage with an AI chatbot to explore professional experience, projects, and skills
- 🎨 **Dark/Light Theme** - Toggle between dark and light modes with smooth transitions
- 📱 **Responsive Design** - Optimized for desktop and mobile viewing experiences
- ✨ **Real-time Streaming** - AI responses stream in real-time using Server-Sent Events
- 🎯 **Quick Suggestions** - Pre-configured prompt bubbles for common questions
- 📝 **Markdown Support** - Rich text formatting with syntax highlighting for code
- 🔗 **GitHub Integration** - Quick access to GitHub repositories

## Tech Stack

- **Frontend Framework**: React 19 with TypeScript
- **Build Tool**: Vite (Rolldown)
- **UI Library**: Material-UI (MUI) v7
- **Styling**: Emotion (CSS-in-JS)
- **HTTP Client**: Axios
- **Markdown**: react-markdown with remark-gfm and rehype-highlight
- **Backend API**: Azure-hosted chat service

## Project Structure

```
src/
├── api/              # API client and chat integration
├── components/       # Reusable UI components
│   ├── ChatInput.tsx
│   ├── GithubMenu.tsx
│   ├── MessageBubble.tsx
│   └── ThemeToggle.tsx
├── config/           # Configuration files
├── hooks/            # Custom React hooks
├── styles/           # Styling utilities
├── utils/            # Helper functions
├── App.tsx           # Main application component
├── ChatWindow.tsx    # Chat interface logic
└── theme.ts          # MUI theme configuration
```

## Getting Started

### Prerequisites

- Node.js (v18 or higher recommended)
- npm or yarn

### Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/SpirerSite.git
cd SpirerSite/Frontend
```

2. Install dependencies:
```bash
npm install
```

3. (Optional) Configure environment variables:
```bash
# Create a .env file
VITE_API_BASE_URL=your_api_url_here
```

### Development

Start the development server:
```bash
npm run dev
```

The application will be available at `http://localhost:5173`

### Building for Production

Build the application:
```bash
npm run build
```

Preview the production build:
```bash
npm run preview
```

## Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run lint` - Run ESLint
- `npm run preview` - Preview production build

## Environment Variables

- `VITE_API_BASE_URL` - Base URL for the chat API (defaults to Azure endpoint)

## Architecture Highlights

### Chat System
The chat interface uses a streaming API architecture:
- Messages are sent to an Azure-hosted backend
- Responses stream back in real-time using Server-Sent Events (SSE)
- UI updates progressively as chunks arrive

### Theme System
- Custom Material-UI theme with dark/light mode support
- Color palette designed for optimal readability
- Smooth transitions between theme modes
- Custom colors for components and overlays

### State Management
- React hooks for local state management
- Custom hooks for message ID generation
- Efficient re-rendering with useCallback and useMemo

## Browser Support

- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- Mobile browsers (iOS Safari, Chrome Mobile)

## License

Private - All rights reserved
