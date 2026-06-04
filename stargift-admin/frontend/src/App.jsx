import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { Toaster } from 'react-hot-toast'
import Layout from './components/Layout'
import Dashboard from './pages/Dashboard'
import GiftsList from './pages/GiftsList'
import CreateGift from './pages/CreateGift'
import EditGift from './pages/EditGift'
import SendGift from './pages/SendGift'
import Attributes from './pages/Attributes'
import Users from './pages/Users'
import UserManagement from './pages/UserManagement'
import FrozenIconSettings from './pages/FrozenIconSettings'
import Statistics from './pages/Statistics'
import EmojiPacks from './pages/EmojiPacks'
import CreateEmojiPack from './pages/CreateEmojiPack'
import ManageEmojiPack from './pages/ManageEmojiPack'
import BulkUploadEmojis from './pages/BulkUploadEmojis'
import StickerPacks from './pages/StickerPacks'
import CreateStickerPack from './pages/CreateStickerPack'
import ManageStickerPack from './pages/ManageStickerPack'
import BulkUploadStickers from './pages/BulkUploadStickers'
import BulkUploadPacks from './pages/BulkUploadPacks'
import BulkUploadStickerPacks from './pages/BulkUploadStickerPacks'
import FeaturedStickerPacks from './pages/FeaturedStickerPacks'
import Verification from './pages/Verification'
import Reactions from './pages/Reactions'
import CreateReaction from './pages/CreateReaction'
import BulkUploadReactions from './pages/BulkUploadReactions'
import FeaturedEmojiPacks from './FeaturedEmojiPacks'
import EmojiPacksBackup from './pages/EmojiPacksBackup'
import SponsoredMessages from './pages/SponsoredMessages'
import ServiceNotifications from './pages/ServiceNotifications'
import CreateServiceNotification from './pages/CreateServiceNotification'
import IssueStars from './pages/IssueStars'
import FrozenAccounts from './pages/FrozenAccounts'
import FrozenSettings from './pages/FrozenSettings'

function App() {
  return (
    <BrowserRouter>
      <Toaster
        position="top-right"
        toastOptions={{
          duration: 3000,
          style: {
            background: '#0f0f0f',
            color: '#fff',
            border: '1px solid rgba(255,255,255,0.1)',
          },
          success: {
            duration: 3000,
            iconTheme: {
              primary: '#00ff88',
              secondary: '#000',
            },
          },
          error: {
            duration: 4000,
            iconTheme: {
              primary: '#ef4444',
              secondary: '#fff',
            },
          },
        }}
      />

      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<Navigate to="/dashboard" replace />} />
          <Route path="dashboard" element={<Dashboard />} />
          <Route path="gifts" element={<GiftsList />} />
          <Route path="gifts/create" element={<CreateGift />} />
          <Route path="gifts/edit/:giftId" element={<EditGift />} />
          <Route path="gifts/send" element={<SendGift />} />
          <Route path="gifts/attributes" element={<Attributes />} />
          <Route path="users" element={<Users />} />
          <Route path="user-management" element={<UserManagement />} />
          <Route path="frozen-icon-settings" element={<FrozenIconSettings />} />
          <Route path="statistics" element={<Statistics />} />
          <Route path="emojipacks" element={<EmojiPacks />} />
          <Route path="emojipacks/create" element={<CreateEmojiPack />} />
          <Route path="emojipacks/bulk-upload" element={<BulkUploadEmojis />} />
          <Route path="emojipacks/featured" element={<FeaturedEmojiPacks />} />
          <Route path="emojipacks/backup" element={<EmojiPacksBackup />} />
          <Route path="emojipacks/:id" element={<ManageEmojiPack />} />
          <Route path="emojipacks/:id/edit" element={<ManageEmojiPack />} />
          <Route path="emojipacks/:packId/bulk-upload" element={<BulkUploadEmojis />} />
          <Route path="stickerpacks" element={<StickerPacks />} />
          <Route path="stickerpacks/create" element={<CreateStickerPack />} />
          <Route path="stickerpacks/bulk-upload" element={<BulkUploadStickers />} />
          <Route path="stickerpacks/bulk-upload-nested" element={<BulkUploadPacks />} />
          <Route path="stickerpacks/bulk-upload-stickers" element={<BulkUploadStickerPacks />} />
          <Route path="stickerpacks/featured" element={<FeaturedStickerPacks />} />
          <Route path="stickerpacks/:id" element={<ManageStickerPack />} />
          <Route path="stickerpacks/:id/edit" element={<ManageStickerPack />} />
          <Route path="stickerpacks/:packId/bulk-upload" element={<BulkUploadStickers />} />
          <Route path="verification" element={<Verification />} />
          <Route path="reactions" element={<Reactions />} />
          <Route path="reactions/create" element={<CreateReaction />} />
          <Route path="reactions/bulk-upload" element={<BulkUploadReactions />} />
          <Route path="sponsored-messages" element={<SponsoredMessages />} />
          <Route path="service-notifications" element={<ServiceNotifications />} />
          <Route path="service-notifications/create" element={<CreateServiceNotification />} />
          <Route path="service-notifications/edit/:id" element={<CreateServiceNotification />} />
          <Route path="stars/issue" element={<IssueStars />} />
          <Route path="frozen-accounts" element={<FrozenAccounts />} />
          <Route path="frozen-settings" element={<FrozenSettings />} />
        </Route>
      </Routes>
    </BrowserRouter>
  )
}

export default App
