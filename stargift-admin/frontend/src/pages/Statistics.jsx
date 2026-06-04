import { useEffect, useState } from 'react'
import { TrendingUp, Gift, Star, Users } from 'lucide-react'
import { LineChart, Line, BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts'
import { statsApi } from '../lib/api'

const COLORS = ['#f97316', '#3b82f6', '#10b981', '#f59e0b', '#8b5cf6', '#ec4899']

export default function Statistics() {
  const [stats, setStats] = useState(null)
  const [sentStats, setSentStats] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    loadStats()
  }, [])

  const loadStats = async () => {
    try {
      const [overview, sent] = await Promise.all([
        statsApi.getOverview(),
        statsApi.getSentStats()
      ])
      setStats(overview.data)
      setSentStats(sent.data)
    } catch (error) {
      console.error('Failed to load stats:', error)
    } finally {
      setLoading(false)
    }
  }

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    )
  }

  const giftDistribution = [
    { name: 'Available', value: stats.availableGifts, color: COLORS[2] },
    { name: 'Sold Out', value: stats.soldOutGifts, color: COLORS[0] },
  ]

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-black font-heading text-fg">Statistics</h1>
        <p className="text-fg-muted font-medium mt-1">Analytics and insights</p>
      </div>

      {/* Quick Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        {[
          { label: 'Total Gifts', value: stats.totalGifts, icon: Gift, color: 'text-blue-500' },
          { label: 'Sent', value: stats.totalSentGifts, icon: TrendingUp, color: 'text-success' },
          { label: 'Stars Earned', value: stats.totalStarsEarned?.toLocaleString(), icon: Star, color: 'text-yellow-500' },
          { label: 'Limited', value: stats.limitedGifts, icon: Gift, color: 'text-purple-500' },
        ].map((stat, i) => (
          <div key={i} className="card group">
            <div className="flex justify-between items-start">
              <div>
                <p className="text-sm font-bold text-fg-muted uppercase tracking-wider">{stat.label}</p>
                <p className="text-2xl font-black font-heading text-fg mt-1 group-hover:text-accent transition-colors">{stat.value}</p>
              </div>
              <div className="p-2 bg-bg-app rounded-lg border border-border group-hover:border-accent group-hover:shadow-[0_0_10px_var(--accent-glow)] transition-all">
                <stat.icon className={`w-6 h-6 ${stat.color} group-hover:text-accent transition-colors`} />
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Charts */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Gift Distribution */}
        <div className="card">
          <h3 className="text-lg font-bold font-heading text-fg mb-4">Gift Status Distribution</h3>
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={giftDistribution}
                cx="50%"
                cy="50%"
                labelLine={false}
                label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                outerRadius={80}
                fill="#8884d8"
                dataKey="value"
              >
                {giftDistribution.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={entry.color} stroke="var(--bg-panel)" strokeWidth={2} />
                ))}
              </Pie>
              <Tooltip
                contentStyle={{ backgroundColor: 'var(--bg-panel)', borderColor: 'var(--border)', color: 'var(--fg)' }}
                itemStyle={{ color: 'var(--fg)' }}
              />
            </PieChart>
          </ResponsiveContainer>
        </div>

        {/* Conversion Rate */}
        {sentStats?.conversion && (
          <div className="card">
            <h3 className="text-lg font-bold font-heading text-fg mb-4">Gift Actions</h3>
            <div className="space-y-6">
              <div>
                <div className="flex justify-between text-sm mb-2">
                  <span className="text-fg-muted font-medium">Saved</span>
                  <span className="font-bold text-fg">{sentStats.conversion[0]?.saved || 0}</span>
                </div>
                <div className="w-full bg-bg-app rounded-full h-2 border border-border overflow-hidden">
                  <div
                    className="bg-success h-full rounded-full shadow-[0_0_10px_var(--success)]"
                    style={{ width: `${(sentStats.conversion[0]?.saved / sentStats.conversion[0]?.total * 100) || 0}%` }}
                  ></div>
                </div>
              </div>
              <div>
                <div className="flex justify-between text-sm mb-2">
                  <span className="text-fg-muted font-medium">Converted</span>
                  <span className="font-bold text-fg">{sentStats.conversion[0]?.converted || 0}</span>
                </div>
                <div className="w-full bg-bg-app rounded-full h-2 border border-border overflow-hidden">
                  <div
                    className="bg-accent h-full rounded-full shadow-[0_0_10px_var(--accent)]"
                    style={{ width: `${(sentStats.conversion[0]?.converted / sentStats.conversion[0]?.total * 100) || 0}%` }}
                  ></div>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Daily Stats */}
      {sentStats?.byDay && sentStats.byDay.length > 0 && (
        <div className="card">
          <h3 className="text-lg font-bold font-heading text-fg mb-4">Gifts Sent Over Time</h3>
          <ResponsiveContainer width="100%" height={300}>
            <LineChart data={sentStats.byDay.reverse()}>
              <CartesianGrid strokeDasharray="3 3" stroke="var(--border)" />
              <XAxis dataKey="_id" stroke="var(--fg-muted)" />
              <YAxis stroke="var(--fg-muted)" />
              <Tooltip
                contentStyle={{ backgroundColor: 'var(--bg-panel)', borderColor: 'var(--border)', color: 'var(--fg)' }}
                labelStyle={{ color: 'var(--accent)' }}
              />
              <Line type="monotone" dataKey="count" stroke="var(--accent)" strokeWidth={3} dot={{ fill: 'var(--bg-app)', strokeWidth: 2 }} activeDot={{ r: 6, fill: 'var(--accent)' }} name="Gifts Sent" />
            </LineChart>
          </ResponsiveContainer>
        </div>
      )}

      {/* Top Gifts */}
      {sentStats?.byGift && sentStats.byGift.length > 0 && (
        <div className="card">
          <h3 className="text-lg font-bold font-heading text-fg mb-4">Most Sent Gifts</h3>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={sentStats.byGift.slice(0, 10)}>
              <CartesianGrid strokeDasharray="3 3" stroke="var(--border)" />
              <XAxis dataKey="_id" stroke="var(--fg-muted)" />
              <YAxis stroke="var(--fg-muted)" />
              <Tooltip
                contentStyle={{ backgroundColor: 'var(--bg-panel)', borderColor: 'var(--border)', color: 'var(--fg)' }}
                cursor={{ fill: 'var(--bg-app)' }}
              />
              <Bar dataKey="count" fill="var(--accent)" radius={[4, 4, 0, 0]} name="Times Sent" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      )}
    </div>
  )
}








