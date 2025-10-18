import React from 'react'
import { Typography, Box } from '@mui/material'
import { listStyles, listItemStyles, paragraphStyles } from './markdownStyles'

interface MarkdownElement {
  type: 'ol' | 'ul' | 'p'
  content?: string
  items?: string[]
}

// Regex patterns
const ORDERED_LIST_REGEX = /^\d+\.\s+/  // Matches "1. ", "2. ", etc. at start of line
const UNORDERED_LIST_REGEX = /^-\s+/   // Matches "- " at start of line
const BOLD_REGEX = /\*\*([^*]+)\*\*/g   // Matches **text** and captures the text inside

/**
 * Simple markdown parser that supports:
 * - **bold text**
 * - Ordered lists (1. item)
 * - Unordered lists (- item)
 */
export function parseMarkdown(text: string): MarkdownElement[] {
  const lines = text.split('\n')
  const elements: MarkdownElement[] = []
  let i = 0

  while (i < lines.length) {
    const line = lines[i]

    // Check for ordered list
    if (ORDERED_LIST_REGEX.test(line)) {
      const items: string[] = []
      while (i < lines.length && ORDERED_LIST_REGEX.test(lines[i])) {
        items.push(lines[i].replace(ORDERED_LIST_REGEX, ''))
        i++
      }
      elements.push({ type: 'ol', items })
      continue
    }

    // Check for unordered list
    if (UNORDERED_LIST_REGEX.test(line)) {
      const items: string[] = []
      while (i < lines.length && UNORDERED_LIST_REGEX.test(lines[i])) {
        items.push(lines[i].replace(UNORDERED_LIST_REGEX, ''))
        i++
      }
      elements.push({ type: 'ul', items })
      continue
    }

    // Regular paragraph (may contain bold)
    if (line.trim()) {
      elements.push({ type: 'p', content: line })
    }

    i++
  }

  return elements
}

/**
 * Parse inline markdown (bold) within text
 */
function parseInline(text: string): React.ReactNode[] {
  const parts: React.ReactNode[] = []
  let lastIndex = 0
  let match
  let key = 0

  while ((match = BOLD_REGEX.exec(text)) !== null) {
    // Add text before the bold
    if (match.index > lastIndex) {
      parts.push(text.substring(lastIndex, match.index))
    }
    // Add bold text as MUI Typography
    parts.push(
      <Typography key={key++} component="span" fontWeight="bold">
        {match[1]}
      </Typography>
    )
    lastIndex = BOLD_REGEX.lastIndex
  }

  // Add remaining text
  if (lastIndex < text.length) {
    parts.push(text.substring(lastIndex))
  }

  return parts.length > 0 ? parts : [text]
}

/**
 * Render parsed markdown elements to React components using MUI
 */
export function renderMarkdown(elements: MarkdownElement[]): React.ReactNode {
  return elements.map((element, index) => {
    switch (element.type) {
      case 'ol':
        return (
          <Box key={index} component="ol" sx={listStyles}>
            {element.items?.map((item, itemIndex) => (
              <Typography key={itemIndex} component="li" sx={listItemStyles}>
                {parseInline(item)}
              </Typography>
            ))}
          </Box>
        )
      case 'ul':
        return (
          <Box key={index} component="ul" sx={listStyles}>
            {element.items?.map((item, itemIndex) => (
              <Typography key={itemIndex} component="li" sx={listItemStyles}>
                {parseInline(item)}
              </Typography>
            ))}
          </Box>
        )
      case 'p':
        return (
          <Typography key={index} component="p" sx={paragraphStyles}>
            {parseInline(element.content || '')}
          </Typography>
        )
      default:
        return null
    }
  })
}

/**
 * Main function to convert markdown text to React elements
 */
export function markdownToReact(text: string): React.ReactNode {
  const elements = parseMarkdown(text)
  return renderMarkdown(elements)
}
