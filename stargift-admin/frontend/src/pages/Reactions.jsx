import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Smile, Plus, Trash2, Edit, Loader, Upload } from 'lucide-react';
import Lottie from 'lottie-react';

export default function Reactions() {
  const [reactions, setReactions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [deleting, setDeleting] = useState(null);

  useEffect(() => {
    fetchReactions();
  }, []);

  const fetchReactions = async () => {
    try {
      setLoading(true);
      const response = await fetch('/api/reactions');
      const data = await response.json();
      setReactions(data.reactions || []);
    } catch (error) {
      console.error('Error fetching reactions:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (reactionId) => {
    if (!confirm('Are you sure you want to delete this reaction?')) return;

    try {
      setDeleting(reactionId);
      const response = await fetch(`/api/reactions/${reactionId}`, {
        method: 'DELETE'
      });

      if (response.ok) {
        setReactions(reactions.filter(r => r.id !== reactionId));
      } else {
        alert('Failed to delete reaction');
      }
    } catch (error) {
      console.error('Error deleting reaction:', error);
      alert('Error deleting reaction');
    } finally {
      setDeleting(null);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Loader className="w-8 h-8 animate-spin text-purple-600" />
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto">
      {/* Header */}
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-3">
          <div className="bg-gradient-to-br from-purple-500 to-pink-600 p-3 rounded-lg">
            <Smile className="w-6 h-6 text-white" />
          </div>
          <div>
            <h1 className="text-2xl font-bold">Message Reactions</h1>
            <p className="text-[#8b98a5]">Manage available reactions for messages</p>
          </div>
        </div>
        <div className="flex gap-3">
          <Link to="/reactions/bulk-upload" className="btn btn-secondary flex items-center gap-2">
            <Upload className="w-5 h-5" />
            Bulk Upload ZIP
          </Link>
          <Link to="/reactions/create" className="btn btn-primary flex items-center gap-2">
            <Plus className="w-5 h-5" />
            Create Reaction
          </Link>
        </div>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
        <div className="card">
          <div className="text-sm text-[#8b98a5]">Total Reactions</div>
          <div className="text-2xl font-bold">{reactions.length}</div>
        </div>
        <div className="card">
          <div className="text-sm text-[#8b98a5]">Free Reactions</div>
          <div className="text-2xl font-bold text-green-600">
            {reactions.filter(r => !r.premium).length}
          </div>
        </div>
        <div className="card">
          <div className="text-sm text-[#8b98a5]">Premium Reactions</div>
          <div className="text-2xl font-bold text-purple-600">
            {reactions.filter(r => r.premium).length}
          </div>
        </div>
        <div className="card">
          <div className="text-sm text-[#8b98a5]">Inactive</div>
          <div className="text-2xl font-bold text-[#8b98a5]">
            {reactions.filter(r => r.inactive).length}
          </div>
        </div>
      </div>

      {/* Reactions Grid */}
      {reactions.length === 0 ? (
        <div className="card text-center py-12">
          <Smile className="w-16 h-16 mx-auto text-gray-300 mb-4" />
          <h3 className="text-xl font-semibold text-white mb-2">No reactions yet</h3>
          <p className="text-[#8b98a5] mb-6">Create your first reaction or bulk upload from ZIP</p>
          <div className="flex gap-3 justify-center">
            <Link to="/reactions/bulk-upload" className="btn btn-secondary">
              Bulk Upload
            </Link>
            <Link to="/reactions/create" className="btn btn-primary">
              Create Reaction
            </Link>
          </div>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
          {reactions.map((reaction) => (
            <div key={reaction.id} className="card hover:shadow-lg transition-shadow">
              <div className="flex items-start justify-between mb-3">
                <div className="flex-1">
                  <div className="text-lg font-semibold">{reaction.emoji}</div>
                  <div className="text-sm text-[#8b98a5]">{reaction.title}</div>
                </div>
                <div className="flex gap-2">
                  <Link
                    to={`/reactions/edit/${reaction.id}`}
                    className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
                  >
                    <Edit className="w-4 h-4 text-[#8b98a5]" />
                  </Link>
                  <button
                    onClick={() => handleDelete(reaction.id)}
                    disabled={deleting === reaction.id}
                    className="p-2 hover:bg-red-500/300/20 rounded-lg transition-colors disabled:opacity-50"
                  >
                    {deleting === reaction.id ? (
                      <Loader className="w-4 h-4 text-red-400 animate-spin" />
                    ) : (
                      <Trash2 className="w-4 h-4 text-red-400" />
                    )}
                  </button>
                </div>
              </div>

              {/* Preview */}
              {reaction.selectAnimation && (
                <div className="bg-gradient-to-br from-purple-50 to-pink-50 rounded-lg p-4 mb-3 flex items-center justify-center">
                  <Lottie
                    animationData={reaction.selectAnimation}
                    loop={true}
                    style={{ width: 80, height: 80 }}
                  />
                </div>
              )}

              {/* Badges */}
              <div className="flex flex-wrap gap-2">
                {reaction.premium && (
                  <span className="px-2 py-1 bg-purple-100 text-purple-400 text-xs font-medium rounded">
                    Premium
                  </span>
                )}
                {reaction.inactive && (
                  <span className="px-2 py-1 bg-gray-100 text-white text-xs font-medium rounded">
                    Inactive
                  </span>
                )}
                {reaction.hasAllAnimations && (
                  <span className="px-2 py-1 bg-green-100 text-green-400 text-xs font-medium rounded">
                    Complete
                  </span>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}









