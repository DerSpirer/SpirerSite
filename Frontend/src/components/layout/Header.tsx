import { Box, Typography } from '@mui/material'
import { useChatContext } from '../../contexts/ChatContext'
import { useIsMobile } from '../../hooks/useIsMobile'
import FadeOutWrapper from '../common/FadeOutWrapper'

function Header() {
  const { hasChatStarted } = useChatContext()
  const isMobile = useIsMobile()

  return (
    <FadeOutWrapper 
      shouldFadeOut={isMobile && hasChatStarted} 
      duration={600}
      marginBottom={isMobile ? 6 : (!hasChatStarted ? 12 : 0)}
    >
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          gap: hasChatStarted ? 0.5 : 1.5,
          transition: 'all 0.4s cubic-bezier(0.4, 0, 0.2, 1)',
        }}
      >
        <Typography
          variant="h2"
          sx={{
            fontSize: hasChatStarted
              ? { xs: '1.75rem', sm: '2rem', md: '2.25rem' }
              : { xs: '2.5rem', sm: '3rem', md: '3.5rem' },
            fontWeight: 400,
            letterSpacing: '-0.02em',
            color: (theme) => theme.palette.text.primary,
            textAlign: 'center',
            animation: !hasChatStarted ? 'fadeInTitle 0.6s ease 0.2s forwards' : 'none',
            transition: 'font-size 0.4s cubic-bezier(0.4, 0, 0.2, 1)',
            '@keyframes fadeInTitle': {
              from: {
                opacity: 0,
                transform: 'translateY(-10px)',
              },
              to: {
                opacity: 1,
                transform: 'translateY(0)',
              },
            },
          }}
        >
          Tom Spirer
        </Typography>
        <Typography
          variant="h6"
          sx={{
            fontSize: hasChatStarted
              ? { xs: '0.85rem', sm: '0.9rem', md: '1rem' }
              : { xs: '1rem', sm: '1.1rem', md: '1.25rem' },
            fontWeight: 300,
            letterSpacing: '0.01em',
            color: (theme) => theme.palette.text.secondary,
            textAlign: 'center',
            animation: !hasChatStarted ? 'fadeInSubtitle 0.6s ease 0.4s forwards' : 'none',
            transition: 'font-size 0.4s cubic-bezier(0.4, 0, 0.2, 1)',
            '@keyframes fadeInSubtitle': {
              from: {
                opacity: 0,
                transform: 'translateY(-10px)',
              },
              to: {
                opacity: 1,
                transform: 'translateY(0)',
              },
            },
          }}
        >
          Software Engineer
        </Typography>
      </Box>
    </FadeOutWrapper>
  )
}

export default Header
