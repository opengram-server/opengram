import { useEffect } from 'react'
import { Link } from 'react-router-dom'
import { Star, Edit, Trash2, Filter, Plus } from 'lucide-react'
import useGiftsStore from '../store/useGiftsStore'
import StickerPreview from '../components/StickerPreview'

export default function GiftsList() {
  const { gifts, loading, filters, setFilters, fetchGifts, deleteGift, toggleSoldOut } = useGiftsStore()

  useEffect(() => {
    fetchGifts()
  }, [filters])

  const handleDelete = async (giftId) => {
    if (window.confirm('Are you sure you want to delete this gift?')) {
      await deleteGift(giftId)
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-black font-heading text-fg">All Gifts</h1>
          <p className="text-fg-muted font-medium mt-1">{gifts.length} gifts in catalog</p>
        </div>
        <Link to="/gifts/create" className="btn btn-primary gap-2 shadow-[0_0_15px_var(--accent-glow)]">
          <Plus className="w-5 h-5" />
          Create Gift
        </Link>
      </div>

      {/* Filters */}
      <div className="card">
        <div className="flex items-center gap-4">
          <Filter className="w-5 h-5 text-fg-muted" />
          <select
            className="input w-auto bg-bg-app border-border focus:border-accent"
            value={filters.soldOut ?? ''}
            onChange={(e) => setFilters({ soldOut: e.target.value === '' ? undefined : e.target.value === 'true' })}
          >
            <option value="">All Status</option>
            <option value="false">Available</option>
            <option value="true">Sold Out</option>
          </select>
          <select
            className="input w-auto bg-bg-app border-border focus:border-accent"
            value={filters.limited ?? ''}
            onChange={(e) => setFilters({ limited: e.target.value === '' ? undefined : e.target.value === 'true' })}
          >
            <option value="">All Types</option>
            <option value="false">Regular</option>
            <option value="true">Limited</option>
          </select>
        </div>
      </div>

      {/* Gifts Grid */}
      {loading ? (
        <div className="flex justify-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-accent"></div>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {gifts.map((gift) => (
            <div key={gift.GiftId} className="card group hover:border-accent hover:shadow-[0_0_20px_var(--accent-glow)] transition-all duration-300">
              {/* Sticker Preview */}
              <div className="mb-6 flex justify-center p-4 bg-bg-app rounded-xl border border-border group-hover:border-accent/30 transition-colors">
                <StickerPreview
                  documentId={typeof gift.Sticker === 'number' ? gift.Sticker : null}
                  stickerBase64={typeof gift.Sticker !== 'number' ? gift.Sticker : null}
                  size={120}
                />
              </div>

              <div className="flex justify-between items-start mb-4">
                <div>
                  <h3 className="text-xl font-bold font-heading text-fg flex items-center gap-2">
                    #{gift.GiftId}
                  </h3>
                  <p className="text-fg-muted font-medium text-sm mt-1">{gift.Title || 'Untitled'}</p>
                </div>
                <div className="flex flex-col items-end gap-2">
                  {gift.Limited && (
                    <span className="px-2 py-1 bg-purple-500/10 text-purple-400 border border-purple-500/20 text-[10px] font-bold uppercase tracking-wider rounded">
                      Limited
                    </span>
                  )}
                  {gift.Birthday && (
                    <span className="px-2 py-1 bg-pink-500/10 text-pink-400 border border-pink-500/20 text-[10px] font-bold uppercase tracking-wider rounded">
                      Birthday
                    </span>
                  )}
                </div>
              </div>

              <div className="space-y-3 mb-6 p-4 bg-bg-app rounded-lg border border-border">
                <div className="flex justify-between items-center">
                  <span className="text-fg-muted text-sm font-medium uppercase tracking-wide">Price</span>
                  <span className="flex items-center gap-1 font-bold text-yellow-500">
                    <Star className="w-4 h-4 fill-current" />{gift.Stars}
                  </span>
                </div>
                <div className="w-full h-[1px] bg-border" />
                <div className="flex justify-between items-center">
                  <span className="text-fg-muted text-sm font-medium uppercase tracking-wide">Convert</span>
                  <span className="font-bold text-fg">{gift.ConvertStars}</span>
                </div>
                {gift.Limited && (
                  <>
                    <div className="w-full h-[1px] bg-border" />
                    <div className="flex justify-between items-center">
                      <span className="text-fg-muted text-sm font-medium uppercase tracking-wide">Supply</span>
                      <span className="font-bold text-fg">
                        <span className="text-accent">{gift.AvailabilityRemains}</span>
                        <span className="text-fg-muted mx-1">/</span>
                        {gift.AvailabilityTotal}
                      </span>
                    </div>
                  </>
                )}
              </div>

              <div className="flex gap-3">
                <Link to={`/gifts/edit/${gift.GiftId}`} className="btn btn-secondary flex-1 group-hover:bg-bg-app group-hover:text-fg">
                  <Edit className="w-4 h-4" />
                  Edit
                </Link>
                <button
                  onClick={() => handleDelete(gift.GiftId)}
                  className="p-2.5 rounded-lg border border-border text-fg-muted hover:text-red-500 hover:border-red-500/50 hover:bg-red-500/10 transition-colors"
                >
                  <Trash2 className="w-5 h-5" />
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}








