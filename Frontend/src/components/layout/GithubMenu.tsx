import { useState } from 'react'
import { Menu, MenuItem, ListItemIcon, ListItemText } from '@mui/material'
import GitHubIcon from '@mui/icons-material/GitHub'
import CodeIcon from '@mui/icons-material/Code'
import StorageIcon from '@mui/icons-material/Storage'
import FloatingActionButton from '../common/FloatingActionButton'

function GithubMenu() {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null)
  const open = Boolean(anchorEl)

  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget)
  }

  const handleClose = () => {
    setAnchorEl(null)
  }

  const handleNavigate = (url: string) => {
    window.open(url, '_blank', 'noopener,noreferrer')
    handleClose()
  }

  return (
    <>
      <FloatingActionButton
        tooltip="View source code"
        onClick={handleClick}
        ariaLabel="github repositories"
        ariaControls={open ? 'github-menu' : undefined}
        ariaHaspopup={true}
        ariaExpanded={open}
        fabPosition="primary"
      >
        <GitHubIcon />
      </FloatingActionButton>
      <Menu
        id="github-menu"
        anchorEl={anchorEl}
        open={open}
        onClose={handleClose}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'left',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'left',
        }}
        slotProps={{
          paper: {
            sx: {
              mt: 1,
              backgroundColor: (theme) => theme.customColors.overlays.paperDark,
              border: (theme) => `1px solid ${theme.customColors.borders.light}`,
              boxShadow: (theme) => theme.customColors.shadows.chatWindow,
            },
          },
        }}
      >
        <MenuItem 
          onClick={() => handleNavigate('https://github.com/DerSpirer/tom-spirer-site')}
          sx={{
            transition: 'all 0.2s ease',
            '&:hover': {
              backgroundColor: (theme) => theme.customColors.overlays.white10,
            },
          }}
        >
          <ListItemIcon>
            <CodeIcon fontSize="small" />
          </ListItemIcon>
          <ListItemText>Frontend Repository</ListItemText>
        </MenuItem>
        <MenuItem 
          onClick={() => handleNavigate('https://github.com/DerSpirer/TomSpirerSiteBackend')}
          sx={{
            transition: 'all 0.2s ease',
            '&:hover': {
              backgroundColor: (theme) => theme.customColors.overlays.white10,
            },
          }}
        >
          <ListItemIcon>
            <StorageIcon fontSize="small" />
          </ListItemIcon>
          <ListItemText>Backend Repository</ListItemText>
        </MenuItem>
      </Menu>
    </>
  )
}

export default GithubMenu