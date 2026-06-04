import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Star, ArrowLeft, Plus, Trash2, ChevronUp, ChevronDown, GripVertical } from 'lucide-react';

export default function FeaturedStickerPacks() {
  const [featuredPacks, setFeaturedPacks] = useState([]);
  const [allPacks, setAllPacks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showAddModal, setShowAddModal] = useState(false);

  useEffect(() => {
    fetchFeaturedPacks();
    fetchAllPacks();
  }, []);

  const fetchFeaturedPacks = async () => {
    try {
      setLoading(true);
      const response = await fetch('/api/stickerpacks/featured');
      const data = await response.json();
      setFeaturedPacks(data.packs || []);
    } catch (error) {
      console.error('Error fetching featured packs:', error);
    } finally {
      setLoading(false);
    }
  };

  const fetchAllPacks = async () => {
    try {
      const response = await fetch('/api/stickerpacks?limit=100');
      const data = await response.json();
      setAllPacks(data.packs || []);
    } catch (error) {
      console.error('Error fetching all packs:', error);
    }
  };

  const toggleFeatured = async (stickerSetId, isFeatured) => {
    try {
      const response = await fetch(`/api/stickerpacks/${stickerSetId}/featured`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          isFeatured: !isFeatured,
          featuredOrder: isFeatured ? 0 : (featuredPacks.length + 1)
        })
      });

      if (response.ok) {
        fetchFeaturedPacks();
        fetchAllPacks();
      } else {
        alert('Failed to update featured status');
      }
    } catch (error) {
      console.error('Error toggling featured:', error);
      alert('Error updating featured status');
    }
  };

  const moveUp = async (pack, index) => {
    if (index === 0) return;

    const newOrder = [...featuredPacks];
    [newOrder[index], newOrder[index - 1]] = [newOrder[index - 1], newOrder[index]];

    await reorderPacks(newOrder);
  };

  const moveDown = async (pack, index) => {
    if (index === featuredPacks.length - 1) return;

    const newOrder = [...featuredPacks];
    [newOrder[index], newOrder[index + 1]] = [newOrder[index + 1], newOrder[index]];

    await reorderPacks(newOrder);
  };

  const reorderPacks = async (newOrder) => {
    try {
      const updates = newOrder.map((pack, idx) => ({
        stickerSetId: pack.StickerSetId,
        featuredOrder: idx + 1
      }));

      const response = await fetch('/api/stickerpacks/featured/reorder', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ packs: updates })
      });

      if (response.ok) {
        fetchFeaturedPacks();
      } else {
        alert('Failed to reorder packs');
      }
    } catch (error) {
      console.error('Error reordering:', error);
      alert('Error reordering packs');
    }
  };

  const availablePacks = allPacks.filter(
    pack => !featuredPacks.some(fp => fp.StickerSetId === pack.StickerSetId)
  );

  return (
    <div className="p-6 max-w-6xl mx-auto">
      {/* Header */}
      <div className="mb-6">
        <Link
          to="/stickerpacks"
          className="inline-flex items-center text-blue-400 hover:text-blue-300 mb-4"
        >
          <ArrowLeft className="w-5 h-5 mr-2" />
          Back to Sticker Packs
        </Link>
        <div className="flex justify-between items-start">
          <div>
            <h1 className="text-3xl font-bold text-white flex items-center gap-2">
              <Star className="w-8 h-8 text-yellow-500" />
              Featured Sticker Packs
            </h1>
            <p className="text-[#8b98a5] mt-1">
              Manage featured regular sticker packs shown to users
            </p>
          </div>
          <button
            onClick={() => setShowAddModal(true)}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 flex items-center gap-2"
          >
            <Plus className="w-5 h-5" />
            Add Featured Pack
          </button>
        </div>
      </div>

      {/* Info Box */}
      <div className="bg-blue-500/10 border border-blue-500/30 rounded-lg p-4 mb-6">
        <h3 className="font-semibold text-blue-400 mb-2">ℹ️ About Featured Sticker Packs</h3>
        <ul className="text-sm text-[#8b98a5] space-y-1">
          <li>• Featured packs are shown in the sticker panel for all users</li>
          <li>• Only <strong>regular stickers</strong> (not custom emoji) can be featured</li>
          <li>• Order matters - use ↑↓ buttons to reorder</li>
          <li>• Client calls <code>messages.getFeaturedStickers</code> to get this list</li>
        </ul>
      </div>

      {/* Featured Packs List */}
      <div className="card rounded-lg p-6">
        <h2 className="text-xl font-bold text-white mb-4">
          Featured Packs ({featuredPacks.length})
        </h2>

        {loading ? (
          <div className="text-center py-8">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
            <p className="mt-2 text-[#8b98a5]">Loading...</p>
          </div>
        ) : featuredPacks.length === 0 ? (
          <div className="text-center py-8 text-[#8b98a5]">
            <Star className="w-12 h-12 mx-auto mb-2 opacity-50" />
            <p>No featured packs yet</p>
            <button
              onClick={() => setShowAddModal(true)}
              className="mt-4 text-blue-400 hover:text-blue-300"
            >
              Add your first featured pack
            </button>
          </div>
        ) : (
          <div className="space-y-2">
            {featuredPacks.map((pack, index) => (
              <div
                key={pack.StickerSetId}
                className="flex items-center gap-3 p-4 bg-[#0e1621] rounded-lg border border-[#2b5278] hover:border-blue-500/50 transition-colors"
              >
                <GripVertical className="w-5 h-5 text-[#8b98a5]" />
                
                <div className="flex-1">
                  <h3 className="font-semibold text-white">{pack.Title}</h3>
                  <p className="text-sm text-[#8b98a5]">
                    @{pack.ShortName} • {pack.Count} stickers • Order: {pack.FeaturedOrder}
                  </p>
                </div>

                <div className="flex items-center gap-2">
                  <button
                    onClick={() => moveUp(pack, index)}
                    disabled={index === 0}
                    className="p-2 text-[#8b98a5] hover:text-white disabled:opacity-30 disabled:cursor-not-allowed"
                    title="Move up"
                  >
                    <ChevronUp className="w-5 h-5" />
                  </button>
                  <button
                    onClick={() => moveDown(pack, index)}
                    disabled={index === featuredPacks.length - 1}
                    className="p-2 text-[#8b98a5] hover:text-white disabled:opacity-30 disabled:cursor-not-allowed"
                    title="Move down"
                  >
                    <ChevronDown className="w-5 h-5" />
                  </button>
                  <button
                    onClick={() => toggleFeatured(pack.StickerSetId, true)}
                    className="p-2 text-red-400 hover:text-red-300"
                    title="Remove from featured"
                  >
                    <Trash2 className="w-5 h-5" />
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Add Modal */}
      {showAddModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-[#17212b] rounded-lg p-6 max-w-2xl w-full mx-4 max-h-[80vh] overflow-y-auto">
            <h2 className="text-2xl font-bold text-white mb-4">Add Featured Pack</h2>
            
            {availablePacks.length === 0 ? (
              <div className="text-center py-8 text-[#8b98a5]">
                <p>All packs are already featured</p>
              </div>
            ) : (
              <div className="space-y-2">
                {availablePacks.map(pack => (
                  <div
                    key={pack.StickerSetId}
                    className="flex items-center justify-between p-4 bg-[#0e1621] rounded-lg border border-[#2b5278] hover:border-blue-500/50"
                  >
                    <div>
                      <h3 className="font-semibold text-white">{pack.Title}</h3>
                      <p className="text-sm text-[#8b98a5]">
                        @{pack.ShortName} • {pack.Count} stickers
                      </p>
                    </div>
                    <button
                      onClick={() => {
                        toggleFeatured(pack.StickerSetId, false);
                        setShowAddModal(false);
                      }}
                      className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
                    >
                      Add
                    </button>
                  </div>
                ))}
              </div>
            )}

            <div className="mt-6 flex justify-end">
              <button
                onClick={() => setShowAddModal(false)}
                className="px-4 py-2 bg-[#2b5278] text-white rounded-lg hover:bg-[#3d5a7a]"
              >
                Close
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
