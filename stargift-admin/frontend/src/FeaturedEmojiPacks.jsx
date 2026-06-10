import React, { useState, useEffect } from 'react';
import './FeaturedEmojiPacks.css';

const API_URL = import.meta.env.VITE_API_URL || '';

function FeaturedEmojiPacks() {
  const [featuredPacks, setFeaturedPacks] = useState([]);
  const [allPacks, setAllPacks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showAddModal, setShowAddModal] = useState(false);
  const [selectedPack, setSelectedPack] = useState(null);

  useEffect(() => {
    loadFeaturedPacks();
    loadAllPacks();
  }, []);

  const loadFeaturedPacks = async () => {
    try {
      const response = await fetch(`${API_URL}/api/emojipacks/featured`);
      const data = await response.json();
      if (data.success) {
        setFeaturedPacks(data.packs);
      }
    } catch (err) {
      setError('Failed to load featured packs');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const loadAllPacks = async () => {
    try {
      const response = await fetch(`${API_URL}/api/emojipacks?limit=1000&emojis=true`);
      const data = await response.json();
      if (data.success) {
        // Filter only emoji packs (Emojis === true)
        setAllPacks(data.packs.filter(p => p.Emojis === true));
      }
    } catch (err) {
      console.error('Failed to load all packs:', err);
    }
  };

  const addToFeatured = async (stickerSetId) => {
    try {
      const maxOrder = Math.max(...featuredPacks.map(p => p.FeaturedOrder || 0), 0);
      const response = await fetch(`${API_URL}/api/emojipacks/${stickerSetId}/featured`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ isFeatured: true, featuredOrder: maxOrder + 1 })
      });
      
      const data = await response.json();
      if (data.success) {
        await loadFeaturedPacks();
        setShowAddModal(false);
      } else {
        alert('Failed to add to featured: ' + data.error);
      }
    } catch (err) {
      alert('Error: ' + err.message);
    }
  };

  const removeFromFeatured = async (stickerSetId) => {
    if (!confirm('Remove this pack from featured?')) return;
    
    try {
      const response = await fetch(`${API_URL}/api/emojipacks/${stickerSetId}/featured`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ isFeatured: false })
      });
      
      const data = await response.json();
      if (data.success) {
        await loadFeaturedPacks();
      } else {
        alert('Failed to remove: ' + data.error);
      }
    } catch (err) {
      alert('Error: ' + err.message);
    }
  };

  const moveUp = async (pack, index) => {
    if (index === 0) return;
    
    const newPacks = [...featuredPacks];
    [newPacks[index], newPacks[index - 1]] = [newPacks[index - 1], newPacks[index]];
    
    await reorderPacks(newPacks);
  };

  const moveDown = async (pack, index) => {
    if (index === featuredPacks.length - 1) return;
    
    const newPacks = [...featuredPacks];
    [newPacks[index], newPacks[index + 1]] = [newPacks[index + 1], newPacks[index]];
    
    await reorderPacks(newPacks);
  };

  const reorderPacks = async (newPacks) => {
    const reorderedPacks = newPacks.map((pack, index) => ({
      stickerSetId: pack.StickerSetId,
      featuredOrder: index + 1
    }));
    
    try {
      const response = await fetch(`${API_URL}/api/emojipacks/featured/reorder`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ packs: reorderedPacks })
      });
      
      const data = await response.json();
      if (data.success) {
        await loadFeaturedPacks();
      }
    } catch (err) {
      alert('Error reordering: ' + err.message);
    }
  };

  if (loading) return <div className="loading">Loading...</div>;
  if (error) return <div className="error">{error}</div>;

  const availablePacks = allPacks.filter(
    p => !featuredPacks.some(fp => fp.StickerSetId === p.StickerSetId)
  );

  return (
    <div className="featured-packs-container">
      <div className="header">
        <h1>🌟 Featured Emoji Packs</h1>
        <button className="btn-add" onClick={() => setShowAddModal(true)}>
          + Add Featured Pack
        </button>
      </div>

      <div className="featured-list">
        {featuredPacks.length === 0 ? (
          <div className="empty-state">
            <p>No featured packs yet. Add some to get started!</p>
          </div>
        ) : (
          featuredPacks.map((pack, index) => (
            <div key={pack.StickerSetId} className="featured-pack-item">
              <div className="pack-order">#{index + 1}</div>
              <div className="pack-info">
                <h3>{pack.Title}</h3>
                <p className="pack-meta">
                  {pack.ShortName} • {pack.Count} emojis • ID: {pack.StickerSetId}
                </p>
              </div>
              <div className="pack-actions">
                <button 
                  onClick={() => moveUp(pack, index)}
                  disabled={index === 0}
                  className="btn-icon"
                  title="Move up"
                >
                  ↑
                </button>
                <button 
                  onClick={() => moveDown(pack, index)}
                  disabled={index === featuredPacks.length - 1}
                  className="btn-icon"
                  title="Move down"
                >
                  ↓
                </button>
                <button 
                  onClick={() => removeFromFeatured(pack.StickerSetId)}
                  className="btn-remove"
                  title="Remove from featured"
                >
                  ✕
                </button>
              </div>
            </div>
          ))
        )}
      </div>

      {showAddModal && (
        <div className="modal-overlay" onClick={() => setShowAddModal(false)}>
          <div className="modal-content" onClick={e => e.stopPropagation()}>
            <h2>Add Pack to Featured</h2>
            <div className="pack-list">
              {availablePacks.length === 0 ? (
                <p>All emoji packs are already featured!</p>
              ) : (
                availablePacks.map(pack => (
                  <div key={pack.StickerSetId} className="pack-list-item">
                    <div>
                      <h4>{pack.Title}</h4>
                      <p className="pack-meta">
                        {pack.ShortName} • {pack.Count} emojis
                      </p>
                    </div>
                    <button 
                      onClick={() => addToFeatured(pack.StickerSetId)}
                      className="btn-add-small"
                    >
                      Add
                    </button>
                  </div>
                ))
              )}
            </div>
            <button onClick={() => setShowAddModal(false)} className="btn-close">
              Close
            </button>
          </div>
        </div>
      )}
    </div>
  );
}

export default FeaturedEmojiPacks;
