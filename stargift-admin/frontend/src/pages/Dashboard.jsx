import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { Gift, TrendingUp, Star, ShoppingBag, Plus, AlertCircle } from 'lucide-react'
import { statsApi } from '../lib/api'
import useGiftsStore from '../store/useGiftsStore'

export default function Dashboard() {
  const [stats, setStats] = useState(null)
  const [loading, setLoading] = useState(true)
  const { gifts, fetchGifts } = useGiftsStore()

  useEffect(() => {
    loadData()
  }, [])

  const loadData = async () => {
    setLoading(true)
    try {
      const [statsRes] = await Promise.all([
        statsApi.getOverview(),
        fetchGifts()
      ])
      setStats(statsRes.data)
    } catch (error) {
      console.error('Failed to load dashboard data:', error)
    } finally {
      setLoading(false)
    }
  }

  const statCards = [
    {
      title: 'Total Gifts',
      value: stats?.totalGifts || 0,
      icon: Gift,
      color: 'bg-blue-500',
      trend: null,
    },
    {
      title: 'Available Gifts',
      value: stats?.availableGifts || 0,
      icon: ShoppingBag,
      color: 'bg-green-500/200',
      trend: '+' + stats?.availableGifts || 0,
    },
    {
      title: 'Sold Out',
      value: stats?.soldOutGifts || 0,
      icon: AlertCircle,
      color: 'bg-red-500/200',
      trend: null,
    },
    {
      title: 'Total Sent',
      value: stats?.totalSentGifts || 0,
      icon: TrendingUp,
      color: 'bg-purple-500',
      trend: null,
    },
    {
      title: 'Stars Earned',
      value: stats?.totalStarsEarned?.toLocaleString() || '0',
      icon: Star,
      color: 'bg-yellow-500/200',
      trend: null,
    },
    {
      title: 'Limited Edition',
      value: stats?.limitedGifts || 0,
      icon: Gift,
      color: 'bg-pink-500',
      trend: null,
    },
  ]

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-[#5288c1]"></div>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-black font-heading text-fg">Dashboard</h1>
          <p className="text-fg-muted font-medium mt-1">Overview of your Star Gifts catalog</p>
        </div>
        <Link to="/gifts/create" className="btn btn-primary gap-2">
          <Plus className="w-5 h-5" />
          Create New Gift
        </Link>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {statCards.map((stat, index) => (
          <div key={index} className="card group">
            <div className="flex items-start justify-between">
              <div className="flex-1">
                <p className="text-sm font-bold text-fg-muted uppercase tracking-wider">{stat.title}</p>
                <p className="text-3xl font-black font-heading text-fg mt-2 group-hover:text-accent transition-colors">{stat.value}</p>
                {stat.trend && (
                  <p className="text-sm text-success mt-1 font-medium">{stat.trend}</p>
                )}
              </div>
              <div className={`p-3 rounded-lg bg-bg-app border border-border group-hover:border-accent group-hover:shadow-[0_0_10px_var(--accent-glow)] transition-all`}>
                <stat.icon className="w-6 h-6 text-fg group-hover:text-accent transition-colors" />
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Recent Gifts */}
      <div className="card">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-bold font-heading text-fg">Recent Gifts</h2>
          <Link to="/gifts" className="text-accent hover:text-white text-sm font-bold tracking-wide uppercase transition-colors">
            View All →
          </Link>
        </div>

        {gifts.length === 0 ? (
          <div className="text-center py-12">
            <Gift className="w-12 h-12 text-fg-muted mx-auto mb-3" />
            <p className="text-fg-muted font-medium">No gifts created yet</p>
            <Link to="/gifts/create" className="mt-4 inline-flex items-center gap-2 btn btn-primary">
              <Plus className="w-4 h-4" />
              Create Your First Gift
            </Link>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
            {gifts.slice(0, 8).map((gift) => (
              <Link
                key={gift.GiftId}
                to={`/gifts/edit/${gift.GiftId}`}
                className="bg-bg-app border border-border rounded-lg p-4 hover:border-accent hover:shadow-[0_0_15px_var(--accent-glow)] transition-all group"
              >
                <div className="flex items-center justify-between mb-2">
                  <span className="text-lg font-bold text-fg group-hover:text-accent transition-colors">#{gift.GiftId}</span>
                  {gift.Limited && (
                    <span className="px-2 py-1 bg-purple-500/20 text-purple-400 text-xs rounded-full font-bold uppercase tracking-wider">
                      Limited
                    </span>
                  )}
                </div>
                <p className="text-sm text-fg-muted mb-2 truncate font-medium">{gift.Title || 'Untitled'}</p>
                <div className="flex items-center justify-between text-sm">
                  <span className="flex items-center gap-1 text-yellow-400 font-bold">
                    <Star className="w-3.5 h-3.5 fill-current" />
                    {gift.Stars}
                  </span>
                  {gift.SoldOut && (
                    <span className="text-red-500 font-bold uppercase tracking-wider text-xs">Sold Out</span>
                  )}
                </div>
              </Link>
            ))}
          </div>
        )}
      </div>

      {/* Most Popular */}
      {stats?.mostPopular && stats.mostPopular.length > 0 && (
        <div className="card">
          <h2 className="text-xl font-bold font-heading text-fg mb-4">Most Popular Gifts</h2>
          <div className="space-y-3">
            {stats.mostPopular.map((item, index) => (
              <div key={item._id} className="flex items-center justify-between p-3 bg-bg-app border border-border rounded-lg hover:border-accent/50 transition-colors">
                <div className="flex items-center gap-3">
                  <div className="bg-accent/10 text-accent border border-accent/20 w-8 h-8 rounded-full flex items-center justify-center font-bold text-sm shadow-[0_0_10px_rgba(0,242,255,0.1)]">
                    {index + 1}
                  </div>
                  <span className="font-medium text-fg">Gift #{item._id}</span>
                </div>
                <span className="text-fg-muted font-mono">{item.count} sent</span>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  )
}









