import { Outlet, NavLink, useLocation } from 'react-router-dom'
import { LayoutDashboard, Gift, Plus, Send, Users, BarChart3, Sparkles, Package, Sticker, Shield, Smile, Star, UserCog, Snowflake, Megaphone, ChevronRight, Menu, X, Bell, Coins, AlertCircle, Search, LogOut } from 'lucide-react'
import { useState, useEffect } from 'react'

export default function Layout() {
  const [sidebarOpen, setSidebarOpen] = useState(window.innerWidth >= 1024)
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false)
  const location = useLocation()

  // Close mobile menu on route change
  useEffect(() => {
    setMobileMenuOpen(false)
  }, [location.pathname])

  const menuSections = [
    {
      title: 'Overview',
      items: [
        { to: '/dashboard', icon: LayoutDashboard, label: 'Dashboard', badge: null }
      ]
    },
    {
      title: 'Star Gifts',
      items: [
        { to: '/gifts', icon: Gift, label: 'All Gifts', badge: null },
        { to: '/gifts/create', icon: Plus, label: 'Create Gift', badge: null },
        { to: '/gifts/send', icon: Send, label: 'Send to User', badge: null },
        { to: '/gifts/attributes', icon: Sparkles, label: 'Attributes', badge: 'New' },
        { to: '/stars/issue', icon: Coins, label: 'Issue Stars', badge: 'New' }
      ]
    },
    {
      title: 'Content',
      items: [
        { to: '/emojipacks', icon: Package, label: 'Emoji Packs', badge: null },
        { to: '/emojipacks/featured', icon: Star, label: 'Featured Packs', badge: 'New' },
        { to: '/stickerpacks', icon: Sticker, label: 'Sticker Packs', badge: null },
        { to: '/reactions', icon: Smile, label: 'Reactions', badge: null },
        { to: '/reactions/bulk-upload', icon: Package, label: 'Bulk Upload Reactions', badge: null }
      ]
    },
    {
      title: 'Advertising',
      items: [
        { to: '/sponsored-messages', icon: Megaphone, label: 'Sponsored Ads', badge: 'Hot' },
        { to: '/service-notifications', icon: Bell, label: 'Notifications', badge: 'New' }
      ]
    },
    {
      title: 'Users',
      items: [
        { to: '/users', icon: Users, label: 'All Users', badge: null },
        { to: '/user-management', icon: UserCog, label: 'Management', badge: null },
        { to: '/verification', icon: Shield, label: 'Verification', badge: null },
        { to: '/frozen-accounts', icon: AlertCircle, label: 'Frozen Accounts', badge: 'New' },
        { to: '/frozen-icon-settings', icon: Snowflake, label: 'Frozen Icon', badge: null },
        { to: '/frozen-settings', icon: Snowflake, label: 'Frozen Settings', badge: 'New' }
      ]
    },
    {
      title: 'Analytics',
      items: [
        { to: '/statistics', icon: BarChart3, label: 'Statistics', badge: null }
      ]
    }
  ]

  // Breadcrumb logic
  const currentSection = menuSections.find(s => s.items.some(i => i.to === location.pathname))?.title || 'Dashboard'
  const currentPage = menuSections.flatMap(s => s.items).find(i => i.to === location.pathname)?.label || 'Overview'

  return (
    <div className="flex h-screen w-screen overflow-hidden bg-bg-app text-fg font-sans selection:bg-accent selection:text-black">
      {/* Mobile Menu Overlay */}
      {mobileMenuOpen && (
        <div
          className="fixed inset-0 bg-black/60 backdrop-blur-sm z-40 lg:hidden"
          onClick={() => setMobileMenuOpen(false)}
        />
      )}

      {/* Sidebar */}
      <aside className={`
        fixed lg:static inset-y-0 left-0 z-50
        w-[280px] flex-shrink-0 bg-bg-side border-r border-border
        transform transition-transform duration-300 ease-in-out
        ${mobileMenuOpen ? 'translate-x-0' : '-translate-x-full lg:translate-x-0'}
        flex flex-col
      `}>
        {/* Logo */}
        <div className="h-[70px] flex items-center px-6 border-b border-border">
          <NavLink to="/" className="flex items-center gap-3 font-heading font-black text-xl tracking-tight hover:opacity-80 transition-opacity">
            <div className="bg-accent/10 p-2 rounded-lg border border-accent/20 shadow-[0_0_10px_rgba(0,242,255,0.1)]">
              <Sparkles className="w-5 h-5 text-accent" />
            </div>
            <span>
              MY<span className="text-accent">TELEGRAM</span>
            </span>
          </NavLink>
        </div>

        {/* Search (Visual Placeholder) */}
        <div className="p-5 pb-0">
          <div className="relative group">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-fg-muted group-focus-within:text-accent transition-colors" />
            <input
              type="text"
              placeholder="Search..."
              className="w-full bg-bg-panel border border-border rounded-lg pl-9 pr-4 py-2.5 text-sm text-fg placeholder-fg-muted focus:border-accent focus:shadow-[0_0_15px_var(--accent-glow)] outline-none transition-all"
            />
          </div>
        </div>

        {/* Navigation */}
        <nav className="flex-1 overflow-y-auto py-5 px-3 space-y-6 scrollbar-thin scrollbar-thumb-bg-panel">
          {menuSections.map((section, idx) => (
            <div key={idx}>
              <h3 className="px-4 mb-2 text-xs font-bold text-fg-muted uppercase tracking-wider">
                {section.title}
              </h3>
              <div className="space-y-1">
                {section.items.map(({ to, icon: Icon, label, badge }) => (
                  <NavLink
                    key={to}
                    to={to}
                    className={({ isActive }) =>
                      `flex items-center gap-3 px-4 py-2.5 rounded-lg text-sm font-medium transition-all duration-200 border border-transparent
                      ${isActive
                        ? 'bg-accent/10 text-accent border-accent/20 shadow-[0_0_10px_rgba(0,242,255,0.1)]'
                        : 'text-fg-muted hover:text-fg hover:bg-bg-panel hover:border-border'
                      }`
                    }
                  >
                    <Icon className="w-4 h-4" />
                    <span className="flex-1">{label}</span>
                    {badge && (
                      <span className={`px-1.5 py-0.5 rounded text-[10px] font-bold uppercase tracking-wide border
                        ${badge === 'Hot'
                          ? 'bg-red-500/10 text-red-500 border-red-500/20'
                          : 'bg-accent/10 text-accent border-accent/20'
                        }`}>
                        {badge}
                      </span>
                    )}
                  </NavLink>
                ))}
              </div>
            </div>
          ))}
        </nav>
      </aside>

      {/* Main Content Wrapper */}
      <div className="flex-1 flex flex-col min-w-0 bg-bg-app relative">
        {/* Background Ambient Glow */}
        <div className="absolute top-0 left-0 w-full h-[500px] bg-accent/5 rounded-full blur-[120px] pointer-events-none -translate-y-1/2 opacity-50" />

        {/* Top Header */}
        <header className="h-[70px] flex items-center justify-between px-6 lg:px-8 border-b border-border bg-bg-app/80 backdrop-blur-md sticky top-0 z-30">
          {/* Left: Breadcrumbs / Mobile Menu Toggle */}
          <div className="flex items-center gap-4">
            <button
              onClick={() => setMobileMenuOpen(true)}
              className="lg:hidden p-2 text-fg hover:bg-bg-panel rounded-lg transition-colors"
            >
              <Menu className="w-5 h-5" />
            </button>

            <nav className="hidden sm:flex items-center text-sm font-medium text-fg-muted">
              <span className="hover:text-fg transition-colors cursor-pointer">Admin</span>
              <ChevronRight className="w-4 h-4 mx-2 opacity-50" />
              <span className="hover:text-fg transition-colors cursor-pointer">{currentSection}</span>
              <ChevronRight className="w-4 h-4 mx-2 opacity-50" />
              <span className="text-accent font-semibold">{currentPage}</span>
            </nav>
          </div>

          {/* Right: Actions */}
          <div className="flex items-center gap-4">
            {/* Notifications */}
            <button className="relative p-2 text-fg-muted hover:text-accent hover:bg-bg-panel rounded-lg transition-all group">
              <Bell className="w-5 h-5" />
              <span className="absolute top-2 right-2 w-2 h-2 bg-accent rounded-full border-2 border-bg-app shadow-[0_0_10px_var(--accent)]" />
            </button>
          </div>
        </header>

        {/* Content Area */}
        <main className="flex-1 overflow-y-auto p-4 lg:p-8 relative z-0 scrollbar-thin scrollbar-thumb-bg-panel">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
