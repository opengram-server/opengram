import { useState, useEffect } from 'react'
import { Search, Coins, Sparkles, TrendingUp, User, Calendar, Hash } from 'lucide-react'
import toast from 'react-hot-toast'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:3001/api'

export default function IssueStars() {
    const [userId, setUserId] = useState('')
    const [amount, setAmount] = useState('')
    const [reason, setReason] = useState('')
    const [loading, setLoading] = useState(false)
    const [userInfo, setUserInfo] = useState(null)
    const [recentTransactions, setRecentTransactions] = useState([])

    useEffect(() => {
        loadRecentTransactions()
    }, [])

    const loadRecentTransactions = async () => {
        try {
            const response = await fetch(`${API_URL}/stars/recent?limit=10`)
            const data = await response.json()
            if (data.success) {
                setRecentTransactions(data.data)
            }
        } catch (error) {
            console.error('Failed to load recent transactions:', error)
        }
    }

    const searchUser = async () => {
        if (!userId) return

        try {
            const response = await fetch(`${API_URL}/stars/user/${userId}`)
            const data = await response.json()
            if (data.success) {
                setUserInfo(data.data)
                toast.success(`User found: ${data.data.user.firstName || 'User ' + userId}`)
            } else {
                toast.error(data.error || 'User not found')
                setUserInfo(null)
            }
        } catch (error) {
            toast.error('User not found')
            setUserInfo(null)
        }
    }

    const issueStars = async (e) => {
        e.preventDefault()

        if (!userId || !amount || amount <= 0) {
            toast.error('Please enter valid User ID and amount')
            return
        }

        setLoading(true)
        try {
            const response = await fetch(`${API_URL}/stars/issue`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    userId: parseInt(userId),
                    amount: parseInt(amount),
                    reason: reason || 'Admin issued stars'
                })
            })
            const data = await response.json()

            if (data.success) {
                toast.success(`✨ Successfully issued ${amount} stars!`)
                setAmount('')
                setReason('')
                loadRecentTransactions()
                searchUser() // Refresh user info
            } else {
                toast.error(data.error || 'Failed to issue stars')
            }
        } catch (error) {
            toast.error('Failed to issue stars')
        } finally {
            setLoading(false)
        }
    }

    return (
        <div className="space-y-6 animate-fade-in">
            {/* Page Header */}
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-3xl font-heading font-bold text-fg flex items-center gap-3">
                        <div className="p-2.5 bg-gradient-to-br from-yellow to-orange-500 rounded-xl shadow-lg shadow-orange-500/20">
                            <Coins className="w-8 h-8 text-white" />
                        </div>
                        Issue Telegram Stars
                    </h1>
                    <p className="text-fg-muted mt-2">Grant stars to users directly from admin panel</p>
                </div>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                {/* Main Form */}
                <div className="lg:col-span-2 space-y-6">
                    {/* User Search Card */}
                    <div className="card p-6 border-border">
                        <h2 className="text-xl font-heading font-semibold text-fg mb-4 flex items-center gap-2">
                            <Search className="w-5 h-5 text-purple" />
                            Find User
                        </h2>

                        <div className="flex gap-3">
                            <div className="flex-1 relative">
                                <Hash className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-fg-muted" />
                                <input
                                    type="number"
                                    value={userId}
                                    onChange={(e) => setUserId(e.target.value)}
                                    onKeyPress={(e) => e.key === 'Enter' && searchUser()}
                                    placeholder="Enter User ID"
                                    className="input w-full pl-12"
                                />
                            </div>
                            <button
                                onClick={searchUser}
                                className="btn btn-primary flex items-center gap-2 px-6"
                            >
                                <Search className="w-5 h-5" />
                                Search
                            </button>
                        </div>

                        {/* User Info Display */}
                        {userInfo && (
                            <div className="mt-4 p-4 bg-muted/40 border border-border rounded-xl animate-scale-in">
                                <div className="flex items-center gap-4">
                                    <div className="w-12 h-12 bg-gradient-to-br from-purple to-blue rounded-full flex items-center justify-center shadow-lg shadow-purple/20">
                                        <User className="w-6 h-6 text-white" />
                                    </div>
                                    <div className="flex-1">
                                        <h3 className="text-fg font-semibold text-lg">
                                            {userInfo.user.firstName} {userInfo.user.lastName || ''}
                                        </h3>
                                        <p className="text-fg-muted text-sm font-mono">
                                            @{userInfo.user.userName || `user${userInfo.user.userId}`} · ID: {userInfo.user.userId}
                                        </p>
                                    </div>
                                    <div className="text-right">
                                        <p className="text-fg-muted text-xs uppercase tracking-wider mb-1">Current Balance</p>
                                        <p className="text-2xl font-bold text-yellow flex items-center justify-end gap-1.5">
                                            <Coins className="w-6 h-6 drop-shadow-sm" />
                                            {userInfo.balance}
                                        </p>
                                    </div>
                                </div>
                            </div>
                        )}
                    </div>

                    {/* Issue Stars Form */}
                    <form onSubmit={issueStars} className="card p-6 border-border relative overflow-hidden">
                        {/* Background decoration */}
                        <div className="absolute top-0 right-0 p-32 bg-yellow/5 rounded-full blur-3xl -mr-16 -mt-16 pointer-events-none"></div>

                        <h2 className="text-xl font-heading font-semibold text-fg mb-6 flex items-center gap-2 relative z-10">
                            <Sparkles className="w-5 h-5 text-yellow" />
                            Issue Stars
                        </h2>

                        <div className="space-y-6 relative z-10">
                            <div>
                                <label className="block text-fg font-medium text-sm mb-2">
                                    Amount of Stars
                                </label>
                                <div className="relative">
                                    <Coins className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-yellow" />
                                    <input
                                        type="number"
                                        value={amount}
                                        onChange={(e) => setAmount(e.target.value)}
                                        placeholder="Enter amount"
                                        min="1"
                                        required
                                        className="input w-full pl-12 text-lg font-medium"
                                    />
                                </div>

                                {/* Quick Amount Buttons */}
                                <div className="flex flex-wrap gap-2 mt-3">
                                    {[50, 100, 500, 1000, 5000].map((amt) => (
                                        <button
                                            key={amt}
                                            type="button"
                                            onClick={() => setAmount(amt.toString())}
                                            className="px-3 py-1.5 bg-muted border border-border hover:border-yellow hover:text-yellow rounded-lg text-sm text-fg-muted transition-all hover:-translate-y-0.5"
                                        >
                                            +{amt}
                                        </button>
                                    ))}
                                </div>
                            </div>

                            <div>
                                <label className="block text-fg font-medium text-sm mb-2">
                                    Reason (Optional)
                                </label>
                                <textarea
                                    value={reason}
                                    onChange={(e) => setReason(e.target.value)}
                                    placeholder="e.g. Promotional bonus, Bug bounty reward, etc."
                                    rows="3"
                                    className="input w-full resize-none"
                                />
                            </div>

                            <button
                                type="submit"
                                disabled={loading || !userId || !amount}
                                className="btn w-full bg-gradient-to-r from-yellow to-orange-500 hover:from-yellow/90 hover:to-orange-500/90 text-black font-bold text-lg py-4 shadow-xl shadow-orange-500/20 hover:shadow-orange-500/30 transition-all transform hover:scale-[1.02] disabled:opacity-50 disabled:scale-100 disabled:cursor-not-allowed flex items-center justify-center gap-2"
                            >
                                {loading ? (
                                    <>
                                        <div className="w-5 h-5 border-2 border-black/30 border-t-black rounded-full animate-spin" />
                                        Processing...
                                    </>
                                ) : (
                                    <>
                                        <Sparkles className="w-5 h-5" />
                                        Issue {amount || '0'} Stars
                                    </>
                                )}
                            </button>
                        </div>
                    </form>
                </div>

                {/* Recent Transactions Sidebar */}
                <div className="lg:col-span-1">
                    <div className="card p-6 border-border sticky top-6 max-h-[calc(100vh-2rem)] flex flex-col">
                        <h2 className="text-xl font-heading font-semibold text-fg mb-4 flex items-center gap-2 shrink-0">
                            <TrendingUp className="w-5 h-5 text-success" />
                            Recent Transactions
                        </h2>

                        <div className="space-y-3 overflow-y-auto pr-2 custom-scrollbar flex-1">
                            {recentTransactions.length === 0 ? (
                                <div className="text-center py-10 bg-muted/20 rounded-xl border border-dashed border-border">
                                    <div className="w-10 h-10 bg-muted rounded-full flex items-center justify-center mx-auto mb-3">
                                        <TrendingUp className="w-5 h-5 text-fg-muted" />
                                    </div>
                                    <p className="text-fg-muted font-medium">No transactions yet</p>
                                </div>
                            ) : (
                                recentTransactions.map((tx, idx) => (
                                    <div
                                        key={idx}
                                        className="p-3 bg-muted/20 border border-border rounded-xl hover:border-purple/50 hover:bg-muted/40 transition-colors group"
                                    >
                                        <div className="flex items-center justify-between mb-2">
                                            <span className="text-fg font-medium flex items-center gap-1.5 text-sm">
                                                <User className="w-3.5 h-3.5 text-fg-muted" />
                                                User {tx.userId}
                                            </span>
                                            <span className={`font-bold text-sm ${tx.Amount > 0 ? 'text-success' : 'text-red'}`}>
                                                {tx.Amount > 0 ? '+' : ''}{tx.Amount} ⭐
                                            </span>
                                        </div>
                                        <p className="text-fg-muted text-xs line-clamp-1 mb-1.5 group-hover:text-fg transition-colors">{tx.Reason}</p>
                                        <div className="flex items-center gap-1.5 text-fg-muted opacity-70 text-[10px]">
                                            <Calendar className="w-3 h-3" />
                                            {new Date(tx.Date).toLocaleString()}
                                        </div>
                                    </div>
                                ))
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}
