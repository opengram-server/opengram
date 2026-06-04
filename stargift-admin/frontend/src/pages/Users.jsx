import { useState, useEffect } from 'react'
import { Users as UsersIcon, Search, Copy, Check, Bot, UserX, ChevronLeft, ChevronRight, Hash } from 'lucide-react'
import toast from 'react-hot-toast'

export default function Users() {
  const [users, setUsers] = useState([])
  const [channels, setChannels] = useState([])
  const [loading, setLoading] = useState(true)
  const [searchQuery, setSearchQuery] = useState('')
  const [copiedId, setCopiedId] = useState(null)
  const [showChannels, setShowChannels] = useState(false)
  const [pagination, setPagination] = useState({
    page: 1,
    limit: 50,
    total: 0,
    totalPages: 0
  })

  useEffect(() => {
    if (showChannels) {
      fetchChannels()
    } else {
      fetchUsers()
    }
  }, [pagination.page, searchQuery, showChannels])

  const fetchUsers = async () => {
    setLoading(true)
    try {
      const params = new URLSearchParams({
        page: pagination.page,
        limit: pagination.limit,
        search: searchQuery
      })

      const response = await fetch(`/api/users?${params}`)
      const data = await response.json()

      setUsers(data.users)
      setPagination(prev => ({
        ...prev,
        total: data.pagination.total,
        totalPages: data.pagination.totalPages
      }))
    } catch (error) {
      toast.error('Failed to load users')
    } finally {
      setLoading(false)
    }
  }

  const fetchChannels = async () => {
    setLoading(true)
    try {
      const response = await fetch('/api/channels')
      const data = await response.json()
      setChannels(data.channels || [])
      setPagination(prev => ({
        ...prev,
        total: data.channels?.length || 0,
        totalPages: 1
      }))
    } catch (error) {
      toast.error('Failed to load channels')
    } finally {
      setLoading(false)
    }
  }

  const copyToClipboard = (userId) => {
    navigator.clipboard.writeText(userId.toString())
    setCopiedId(userId)
    toast.success('User ID copied!')
    setTimeout(() => setCopiedId(null), 2000)
  }

  const handleSearchChange = (e) => {
    setSearchQuery(e.target.value)
    setPagination(prev => ({ ...prev, page: 1 }))
  }

  const nextPage = () => {
    if (pagination.page < pagination.totalPages) {
      setPagination(prev => ({ ...prev, page: prev.page + 1 }))
    }
  }

  const prevPage = () => {
    if (pagination.page > 1) {
      setPagination(prev => ({ ...prev, page: prev.page - 1 }))
    }
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-black font-heading text-fg">
            {showChannels ? 'All Channels' : 'All Users'}
          </h1>
          <p className="mt-1 text-fg-muted font-medium">
            {pagination.total} {showChannels ? 'channels' : 'users'} in MyTelegram
          </p>
        </div>
        <div className="flex items-center gap-3">
          <button
            onClick={() => {
              setShowChannels(!showChannels)
              setPagination(prev => ({ ...prev, page: 1 }))
              setSearchQuery('')
            }}
            className="btn btn-secondary flex items-center gap-2"
          >
            {showChannels ? <UsersIcon className="w-5 h-5" /> : <Hash className="w-5 h-5" />}
            {showChannels ? 'Show Users' : 'Show Channels'}
          </button>

          <div className="p-2 bg-bg-app rounded-lg border border-border">
            {showChannels ? <Hash className="w-8 h-8 text-accent" /> : <UsersIcon className="w-8 h-8 text-accent" />}
          </div>
        </div>
      </div>

      {/* Search */}
      <div className="card p-4">
        <div className="relative">
          <Search className="absolute left-3 top-3 w-5 h-5 text-fg-muted" />
          <input
            type="text"
            value={searchQuery}
            onChange={handleSearchChange}
            placeholder="Search by name, username, or phone..."
            className="input pl-10 bg-bg-app border-border focus:border-accent"
          />
        </div>
      </div>

      {/* Users Table */}
      <div className="card overflow-hidden p-0 border border-border">
        {loading ? (
          <div className="flex items-center justify-center h-64">
            <div className="w-8 h-8 border-4 border-accent border-t-transparent rounded-full animate-spin" />
          </div>
        ) : users.length === 0 ? (
          <div className="flex flex-col items-center justify-center h-64 text-fg-muted">
            <UserX className="w-12 h-12 mb-2" />
            <p>No users found</p>
          </div>
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="bg-bg-panel border-b border-border">
                  <tr>
                    <th className="px-6 py-4 text-left text-xs font-bold text-fg-muted uppercase tracking-wider">
                      {showChannels ? 'Channel ID' : 'User ID'}
                    </th>
                    <th className="px-6 py-4 text-left text-xs font-bold text-fg-muted uppercase tracking-wider">
                      {showChannels ? 'Title' : 'Name'}
                    </th>
                    <th className="px-6 py-4 text-left text-xs font-bold text-fg-muted uppercase tracking-wider">
                      Username
                    </th>
                    {!showChannels && (
                      <th className="px-6 py-4 text-left text-xs font-bold text-fg-muted uppercase tracking-wider">
                        Phone
                      </th>
                    )}
                    <th className="px-6 py-4 text-left text-xs font-bold text-fg-muted uppercase tracking-wider">
                      {showChannels ? 'Members' : 'Type'}
                    </th>
                    <th className="px-6 py-4 text-right text-xs font-bold text-fg-muted uppercase tracking-wider">
                      Actions
                    </th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-border bg-bg-app">
                  {showChannels ? channels.map((channel) => (
                    <tr key={channel._id} className="hover:bg-bg-panel/50 transition-colors">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="flex items-center gap-2">
                          <span className="font-mono text-sm font-medium text-fg">
                            {channel.ChannelId}
                          </span>
                          <button
                            onClick={() => copyToClipboard(channel.ChannelId)}
                            className="p-1 hover:bg-bg-panel rounded transition-colors group"
                            title="Copy Channel ID"
                          >
                            {copiedId === channel.ChannelId ? (
                              <Check className="w-3.5 h-3.5 text-success" />
                            ) : (
                              <Copy className="w-3.5 h-3.5 text-fg-muted group-hover:text-fg" />
                            )}
                          </button>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm font-bold text-fg">
                          {channel.Title || <span className="text-fg-muted">Untitled</span>}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm font-medium text-accent">
                          {channel.UserName ? `@${channel.UserName}` : <span className="text-fg-muted font-normal">—</span>}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm font-medium text-fg-muted">
                          <span className="text-fg font-bold">{channel.MembersCount || 0}</span> members
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-right">
                        <a
                          href={`/sponsored-messages?channelId=${channel.ChannelId}`}
                          className="text-accent hover:text-accent-hover text-sm font-bold transition-colors"
                        >
                          Create Ad →
                        </a>
                      </td>
                    </tr>
                  )) : users.map((user) => (
                    <tr key={user._id} className="hover:bg-bg-panel/50 transition-colors">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="flex items-center gap-2">
                          <span className="font-mono text-sm font-medium text-fg">
                            {user.UserId}
                          </span>
                          <button
                            onClick={() => copyToClipboard(user.UserId)}
                            className="p-1 hover:bg-bg-panel rounded transition-colors group"
                            title="Copy User ID"
                          >
                            {copiedId === user.UserId ? (
                              <Check className="w-3.5 h-3.5 text-success" />
                            ) : (
                              <Copy className="w-3.5 h-3.5 text-fg-muted group-hover:text-fg" />
                            )}
                          </button>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm font-bold text-fg">
                          {user.FirstName} {user.LastName}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm font-medium text-accent">
                          {user.UserName ? `@${user.UserName}` : <span className="text-fg-muted font-normal">—</span>}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm font-mono text-fg-muted">
                          {user.PhoneNumber || <span className="text-fg-muted">—</span>}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="flex items-center gap-2">
                          {user.Bot && (
                            <span className="inline-flex items-center gap-1 px-2 py-0.5 bg-blue-500/10 text-blue-400 border border-blue-500/20 rounded text-[10px] font-bold uppercase tracking-wider">
                              <Bot className="w-3 h-3" />
                              Bot
                            </span>
                          )}
                          {user.IsDeleted && (
                            <span className="inline-flex items-center gap-1 px-2 py-0.5 bg-red-500/10 text-red-500 border border-red-500/20 rounded text-[10px] font-bold uppercase tracking-wider">
                              <UserX className="w-3 h-3" />
                              Deleted
                            </span>
                          )}
                          {!user.Bot && !user.IsDeleted && (
                            <span className="text-xs font-medium text-fg-muted px-2 py-0.5 rounded bg-bg-panel border border-border">User</span>
                          )}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-right">
                        <a
                          href={`/gifts/send?userId=${user.UserId}`}
                          className="text-accent hover:text-accent-hover text-sm font-bold transition-colors"
                        >
                          Send Gift →
                        </a>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {/* Pagination */}
            <div className="bg-bg-panel px-6 py-4 border-t border-border flex items-center justify-between">
              <div className="text-sm text-fg-muted">
                Showing <span className="font-bold text-fg">{(pagination.page - 1) * pagination.limit + 1}</span> to{' '}
                <span className="font-bold text-fg">
                  {Math.min(pagination.page * pagination.limit, pagination.total)}
                </span>{' '}
                of <span className="font-bold text-fg">{pagination.total}</span> users
              </div>

              <div className="flex items-center gap-2">
                <button
                  onClick={prevPage}
                  disabled={pagination.page === 1}
                  className="px-4 py-2 border border-border rounded-lg text-sm font-medium text-fg hover:bg-bg-app hover:border-accent/50 disabled:opacity-50 disabled:cursor-not-allowed transition-all flex items-center gap-2"
                >
                  <ChevronLeft className="w-4 h-4" />
                  Previous
                </button>

                <span className="px-4 py-2 text-sm text-fg">
                  Page <span className="font-bold text-accent">{pagination.page}</span> of{' '}
                  <span className="font-medium">{pagination.totalPages}</span>
                </span>

                <button
                  onClick={nextPage}
                  disabled={pagination.page === pagination.totalPages}
                  className="px-4 py-2 border border-border rounded-lg text-sm font-medium text-fg hover:bg-bg-app hover:border-accent/50 disabled:opacity-50 disabled:cursor-not-allowed transition-all flex items-center gap-2"
                >
                  Next
                  <ChevronRight className="w-4 h-4" />
                </button>
              </div>
            </div>
          </>
        )}
      </div>

      {/* Info */}
      <div className="bg-accent/10 border border-accent/20 rounded-lg p-5 flex gap-4 animate-fade-in shadow-[0_0_15px_var(--accent-glow-subtle)]">
        <div className="p-2 bg-accent/20 rounded-lg h-fit">
          <span className="text-lg">💡</span>
        </div>
        <div className="text-sm text-fg">
          <p className="font-bold text-accent mb-2">Quick Actions</p>
          <ul className="space-y-2 text-fg-muted font-medium">
            <li className="flex items-center gap-2">
              <span className="w-1.5 h-1.5 rounded-full bg-accent"></span>
              Click <Copy className="w-3.5 h-3.5 inline text-fg" /> to copy User ID
            </li>
            <li className="flex items-center gap-2">
              <span className="w-1.5 h-1.5 rounded-full bg-accent"></span>
              Click "Send Gift →" to send a gift directly to this user
            </li>
            <li className="flex items-center gap-2">
              <span className="w-1.5 h-1.5 rounded-full bg-accent"></span>
              Use search to quickly find users by name, username, or phone
            </li>
          </ul>
        </div>
      </div>
    </div>
  )
}









