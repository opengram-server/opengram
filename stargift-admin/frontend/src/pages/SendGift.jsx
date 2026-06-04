import { useState, useEffect } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { Send, Gift, User, MessageSquare, EyeOff, CheckCircle2, AlertCircle, Search } from 'lucide-react'
import toast from 'react-hot-toast'

export default function SendGift() {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const [gifts, setGifts] = useState([])
  const [loading, setLoading] = useState(true)
  const [sending, setSending] = useState(false)
  const [searchQuery, setSearchQuery] = useState('')
  const [searchResults, setSearchResults] = useState([])
  const [searching, setSearching] = useState(false)
  
  const [formData, setFormData] = useState({
    giftId: '',
    userId: searchParams.get('userId') || '',
    fromUserId: '',
    message: '',
    nameHidden: false,
    count: 1
  })

  useEffect(() => {
    fetchGifts()
    
    // If userId is in URL params, show a toast
    const userIdFromUrl = searchParams.get('userId')
    if (userIdFromUrl) {
      toast.success(`User ID ${userIdFromUrl} pre-filled from URL`)
    }
  }, [])

  const searchUsers = async (query) => {
    if (!query || query.length < 2) {
      setSearchResults([])
      return
    }

    setSearching(true)
    try {
      // Try to search by phone or username
      const isPhone = /^\d+$/.test(query)
      const params = isPhone 
        ? `phone=${encodeURIComponent(query)}`
        : `username=${encodeURIComponent(query)}`
      
      const response = await fetch(`/api/users/search?${params}`)
      const data = await response.json()
      setSearchResults(data)
    } catch (error) {
      console.error('Search error:', error)
      setSearchResults([])
    } finally {
      setSearching(false)
    }
  }

  const handleSearchChange = (e) => {
    const value = e.target.value
    setSearchQuery(value)
    searchUsers(value)
  }

  const selectUser = (user) => {
    setFormData({ ...formData, userId: user.UserId.toString() })
    setSearchQuery('')
    setSearchResults([])
    toast.success(`Selected: ${user.FirstName} (ID: ${user.UserId})`)
  }

  const fetchGifts = async () => {
    try {
      const response = await fetch('/api/gifts')
      const data = await response.json()
      setGifts(data.filter(g => !g.SoldOut)) // Only available gifts
      setLoading(false)
    } catch (error) {
      toast.error('Failed to load gifts')
      setLoading(false)
    }
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    
    if (!formData.giftId || !formData.userId) {
      toast.error('Gift and User ID are required')
      return
    }

    setSending(true)

    try {
      const response = await fetch(`/api/gifts/${formData.giftId}/send-to-user`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          userId: parseInt(formData.userId),
          fromUserId: formData.fromUserId ? parseInt(formData.fromUserId) : 0,
          message: formData.message || null,
          nameHidden: formData.nameHidden,
          count: parseInt(formData.count) || 1
        })
      })

      const data = await response.json()

      if (!response.ok) {
        throw new Error(data.error || 'Failed to send gift')
      }

      // Show success with restart instructions
      toast.success(`🎁 Gift added to database!`)
      
      if (data.instructions) {
        setTimeout(() => {
          toast((t) => (
            <div className="space-y-2">
              <p className="font-semibold">⚠️ Important: Restart Query Server</p>
              <p className="text-sm">To see the gift in user profile, run:</p>
              <code className="block bg-gray-800 text-white px-2 py-1 rounded text-xs mt-1">
                {data.instructions.command}
              </code>
              <button
                onClick={() => {
                  navigator.clipboard.writeText(data.instructions.command)
                  toast.success('Command copied!')
                }}
                className="text-xs text-primary-600 hover:underline"
              >
                Copy command
              </button>
            </div>
          ), {
            duration: 8000,
            icon: '🔄',
          })
        }, 500)
      }
      
      // Reset form
      setFormData({
        giftId: '',
        userId: '',
        fromUserId: '',
        message: '',
        nameHidden: false
      })
    } catch (error) {
      toast.error(error.message)
    } finally {
      setSending(false)
    }
  }

  const selectedGift = gifts.find(g => g.GiftId === parseInt(formData.giftId))

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-white">Send Gift to User</h1>
          <p className="mt-1 text-sm text-[#8b98a5]">
            Manually send a gift to any user (Admin function)
          </p>
        </div>
        <Send className="w-8 h-8 text-primary-500" />
      </div>

      {/* Info Alert */}
      <div className="bg-blue-500/20 border border-blue-500/30 rounded-lg p-4 flex gap-3">
        <AlertCircle className="w-5 h-5 text-blue-400 flex-shrink-0 mt-0.5" />
        <div className="text-sm text-blue-300">
          <p className="font-medium text-blue-400">Admin Gift Delivery</p>
          <p className="mt-1">
            This will directly insert a gift into the user's received gifts collection. 
            The gift will be automatically saved to their profile.
          </p>
        </div>
      </div>

      {/* Form */}
      <form onSubmit={handleSubmit} className="card p-6 space-y-6">
        {/* Gift Selection */}
        <div>
          <label className="block text-sm font-medium text-white mb-2">
            <Gift className="w-4 h-4 inline mr-2" />
            Select Gift
          </label>
          <select
            value={formData.giftId}
            onChange={(e) => setFormData({ ...formData, giftId: e.target.value })}
            className="input focus:ring-2 focus:ring-primary-500 focus:border-transparent"
            required
            disabled={loading}
          >
            <option value="">Choose a gift...</option>
            {gifts.map((gift) => (
              <option key={gift.GiftId} value={gift.GiftId}>
                #{gift.GiftId} - {gift.Title || 'Unnamed'} ({gift.Stars} ⭐)
                {gift.Limited && ` - Limited (${gift.AvailabilityRemains}/${gift.AvailabilityTotal})`}
              </option>
            ))}
          </select>
        </div>

        {/* Selected Gift Preview */}
        {selectedGift && (
          <div className="bg-gradient-to-br from-[#2b5278] to-[#3d5a7a] rounded-lg p-4 border border-[#5288c1]/30">
            <div className="flex items-start justify-between">
              <div>
                <h3 className="font-semibold text-white">
                  {selectedGift.Title || `Gift #${selectedGift.GiftId}`}
                </h3>
                <div className="mt-2 space-y-1 text-sm">
                  <p className="text-white">💫 Price: <span className="font-medium">{selectedGift.Stars} Stars</span></p>
                  <p className="text-white">💰 Convert Value: <span className="font-medium">{selectedGift.ConvertStars} Stars</span></p>
                  {selectedGift.UpgradeStars && (
                    <p className="text-white">⬆️ Upgrade: <span className="font-medium">{selectedGift.UpgradeStars} Stars</span></p>
                  )}
                  {selectedGift.Limited && (
                    <p className="text-amber-700">🎯 Limited Edition: {selectedGift.AvailabilityRemains} remaining</p>
                  )}
                </div>
              </div>
            </div>
          </div>
        )}

        {/* User Search */}
        <div>
          <label className="block text-sm font-medium text-white mb-2">
            <Search className="w-4 h-4 inline mr-2" />
            Search User (Optional)
          </label>
          <div className="relative">
            <input
              type="text"
              value={searchQuery}
              onChange={handleSearchChange}
              placeholder="Search by phone (79123456789) or username"
              className="input focus:ring-2 focus:ring-primary-500 focus:border-transparent"
            />
            {searching && (
              <div className="absolute right-3 top-3">
                <div className="w-5 h-5 border-2 border-primary-500 border-t-transparent rounded-full animate-spin" />
              </div>
            )}
          </div>
          
          {/* Search Results */}
          {searchResults.length > 0 && (
            <div className="mt-2 card border border-[#2b5278] rounded-lg shadow-lg max-h-60 overflow-y-auto">
              {searchResults.map((user) => (
                <button
                  key={user._id}
                  type="button"
                  onClick={() => selectUser(user)}
                  className="w-full px-4 py-3 text-left hover:bg-[#0e1621] border-b last:border-b-0 transition-colors"
                >
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="font-medium text-white">
                        {user.FirstName} {user.LastName}
                      </p>
                      <p className="text-xs text-[#8b98a5]">
                        {user.UserName && `@${user.UserName} • `}
                        {user.PhoneNumber}
                      </p>
                    </div>
                    <span className="text-sm font-mono text-primary-600">
                      ID: {user.UserId}
                    </span>
                  </div>
                </button>
              ))}
            </div>
          )}
          
          {searchQuery && !searching && searchResults.length === 0 && (
            <p className="mt-2 text-sm text-amber-600">
              No users found. Try phone number or username.
            </p>
          )}
        </div>

        {/* User ID */}
        <div>
          <label className="block text-sm font-medium text-white mb-2">
            <User className="w-4 h-4 inline mr-2" />
            Recipient User ID *
          </label>
          <input
            type="number"
            value={formData.userId}
            onChange={(e) => setFormData({ ...formData, userId: e.target.value })}
            placeholder="e.g. 2010001 (or search above)"
            className="input focus:ring-2 focus:ring-primary-500 focus:border-transparent"
            required
          />
          <p className="mt-1 text-xs text-[#8b98a5]">
            The Telegram user ID who will receive this gift
          </p>
        </div>

        {/* From User ID */}
        <div>
          <label className="block text-sm font-medium text-white mb-2">
            <User className="w-4 h-4 inline mr-2" />
            From User ID (Optional)
          </label>
          <input
            type="number"
            value={formData.fromUserId}
            onChange={(e) => setFormData({ ...formData, fromUserId: e.target.value })}
            placeholder="Leave empty for system gift (0)"
            className="input focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          />
          <p className="mt-1 text-xs text-[#8b98a5]">
            Who sent this gift (0 = System/Admin)
          </p>
        </div>

        {/* Count */}
        <div>
          <label className="block text-sm font-medium text-white mb-2">
            <Send className="w-4 h-4 inline mr-2" />
            Quantity
          </label>
          <input
            type="number"
            min="1"
            value={formData.count}
            onChange={(e) => setFormData({ ...formData, count: e.target.value })}
            placeholder="1"
            className="input focus:ring-2 focus:ring-primary-500 focus:border-transparent"
          />
          <p className="mt-1 text-xs text-[#8b98a5]">
            Number of gifts to send (will charge stars per gift)
          </p>
        </div>

        {/* Message */}
        <div>
          <label className="block text-sm font-medium text-white mb-2">
            <MessageSquare className="w-4 h-4 inline mr-2" />
            Gift Message (Optional)
          </label>
          <textarea
            value={formData.message}
            onChange={(e) => setFormData({ ...formData, message: e.target.value })}
            placeholder="Add a personal message with the gift..."
            rows={3}
            className="input focus:ring-2 focus:ring-primary-500 focus:border-transparent resize-none"
            maxLength={255}
          />
          <p className="mt-1 text-xs text-[#8b98a5]">
            {formData.message.length}/255 characters
          </p>
        </div>

        {/* Name Hidden */}
        <div className="flex items-center gap-3 p-4 bg-[#0e1621] rounded-lg">
          <input
            type="checkbox"
            id="nameHidden"
            checked={formData.nameHidden}
            onChange={(e) => setFormData({ ...formData, nameHidden: e.target.checked })}
            className="w-4 h-4 text-primary-600 border-[#2b5278] rounded focus:ring-primary-500"
          />
          <label htmlFor="nameHidden" className="flex items-center gap-2 text-sm text-white cursor-pointer">
            <EyeOff className="w-4 h-4" />
            <span>Send Anonymously (hide sender name)</span>
          </label>
        </div>

        {/* Submit */}
        <div className="flex gap-3 pt-4 border-t">
          <button
            type="button"
            onClick={() => navigate('/gifts')}
            className="flex-1 px-6 py-3 border border-[#2b5278] text-white rounded-lg hover:bg-[#0e1621] font-medium transition-colors"
            disabled={sending}
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={sending || !formData.giftId || !formData.userId}
            className="flex-1 px-6 py-3 bg-gradient-to-r from-[#2b5278]0 to-primary-600 text-white rounded-lg hover:from-primary-600 hover:to-primary-700 font-medium transition-all disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
          >
            {sending ? (
              <>
                <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
                Sending...
              </>
            ) : (
              <>
                <Send className="w-4 h-4" />
                Send Gift
              </>
            )}
          </button>
        </div>
      </form>

      {/* Important Notice */}
      <div className="bg-amber-50 border-2 border-amber-300 rounded-xl p-6">
        <div className="flex gap-3">
          <AlertCircle className="w-6 h-6 text-amber-600 flex-shrink-0" />
          <div>
            <h3 className="font-semibold text-amber-900 mb-2">⚠️ Important: Restart Required</h3>
            <p className="text-sm text-amber-800 mb-3">
              Gifts are inserted directly into the database. To make them visible in user profiles, 
              you must <strong>restart the Query Server</strong>:
            </p>
            <code className="block bg-amber-900 text-amber-100 px-3 py-2 rounded text-sm font-mono">
              docker restart messenger-query-server-1
            </code>
            <p className="text-xs text-amber-700 mt-2">
              This is because MyTelegram uses Event Sourcing and caches ReadModel in memory.
            </p>
          </div>
        </div>
      </div>

      {/* Tips */}
      <div className="card p-6">
        <h2 className="text-lg font-semibold text-white mb-4">💡 Tips & Tricks</h2>
        <ul className="space-y-2 text-sm text-[#8b98a5]">
          <li className="flex items-start gap-2">
            <CheckCircle2 className="w-4 h-4 text-green-600 flex-shrink-0 mt-0.5" />
            <span><strong>Search users</strong> - Type phone number or @username to quickly find user ID</span>
          </li>
          <li className="flex items-start gap-2">
            <CheckCircle2 className="w-4 h-4 text-green-600 flex-shrink-0 mt-0.5" />
            <span><strong>Auto-saved</strong> - Gifts sent via admin are automatically saved to user profile</span>
          </li>
          <li className="flex items-start gap-2">
            <CheckCircle2 className="w-4 h-4 text-green-600 flex-shrink-0 mt-0.5" />
            <span><strong>Limited gifts</strong> - Availability counter will automatically decrease</span>
          </li>
          <li className="flex items-start gap-2">
            <CheckCircle2 className="w-4 h-4 text-green-600 flex-shrink-0 mt-0.5" />
            <span><strong>System gifts</strong> - Leave "From User ID" empty to send as System (ID: 0)</span>
          </li>
          <li className="flex items-start gap-2">
            <CheckCircle2 className="w-4 h-4 text-green-600 flex-shrink-0 mt-0.5" />
            <span><strong>Find your ID</strong> - Search for yourself by phone to get your User ID</span>
          </li>
        </ul>
      </div>
    </div>
  )
}











