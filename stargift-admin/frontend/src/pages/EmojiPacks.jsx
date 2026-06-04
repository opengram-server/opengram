import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Package, Plus, Search, ExternalLink, Trash2, Edit, FileArchive, Database } from 'lucide-react';
import { toast } from 'react-hot-toast';

export default function EmojiPacks() {
  const [packs, setPacks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  useEffect(() => {
    fetchPacks();
  }, [currentPage, searchTerm]);

  const fetchPacks = async () => {
    try {
      setLoading(true);
      const response = await fetch(
        `/api/emojipacks?page=${currentPage}&limit=20&search=${searchTerm}&emojis=true`
      );
      const data = await response.json();
      setPacks(data.packs || []);
      setTotalPages(data.pagination?.totalPages || 1);
    } catch (error) {
      console.error('Error fetching emoji packs:', error);
      toast.error('Failed to load emoji packs');
      setPacks([]);
    } finally {
      setLoading(false);
    }
  };

  const deletePack = async (packId, packName) => {
    if (!confirm(`Delete emoji pack "${packName}"? This will delete all emojis in the pack.`)) {
      return;
    }

    try {
      const response = await fetch(`/api/emojipacks/${packId}`, {
        method: 'DELETE'
      });

      if (response.ok) {
        toast.success('Pack deleted successfully');
        fetchPacks();
      } else {
        toast.error('Failed to delete pack');
      }
    } catch (error) {
      console.error('Error deleting pack:', error);
      toast.error('Error deleting pack');
    }
  };

  return (
    <div className="space-y-6 animate-fade-in">
      {/* Header */}
      <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
        <div>
          <h1 className="text-3xl font-heading font-bold text-fg flex items-center gap-3">
            <Package className="w-8 h-8 text-purple" />
            Custom Emoji Packs
          </h1>
          <p className="text-fg-muted mt-1">Manage custom emoji sticker sets</p>
        </div>
        <div className="flex flex-wrap gap-3">
          <Link
            to="/emojipacks/backup"
            className="btn btn-secondary flex items-center gap-2"
          >
            <Database className="w-5 h-5" />
            Backup/Restore
          </Link>
          <Link
            to="/emojipacks/bulk-upload"
            className="btn bg-success/10 text-success hover:bg-success/20 border-success/20 flex items-center gap-2"
          >
            <FileArchive className="w-5 h-5" />
            Bulk Upload ZIP
          </Link>
          <Link
            to="/emojipacks/create"
            className="btn btn-primary flex items-center gap-2"
          >
            <Plus className="w-5 h-5" />
            Create Pack
          </Link>
        </div>
      </div>

      {/* Search */}
      <div className="card p-4">
        <div className="relative max-w-md">
          <Search className="absolute left-3 top-3 w-5 h-5 text-fg-muted" />
          <input
            type="text"
            placeholder="Search by title or short name..."
            value={searchTerm}
            onChange={(e) => {
              setSearchTerm(e.target.value);
              setCurrentPage(1);
            }}
            className="input pl-10 w-full"
          />
        </div>
      </div>

      {/* Packs Grid */}
      {loading ? (
        <div className="flex justify-center items-center py-20">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-purple"></div>
        </div>
      ) : packs.length === 0 ? (
        <div className="text-center py-20 card">
          <Package className="w-16 h-16 text-fg-muted mx-auto mb-4" />
          <p className="text-fg-muted text-lg">No emoji packs found</p>
          <Link
            to="/emojipacks/create"
            className="mt-6 btn btn-primary inline-flex"
          >
            <Plus className="w-5 h-5 mr-2" />
            Create First Pack
          </Link>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
          {packs.map((pack) => (
            <div
              key={pack.StickerSetId || pack.stickerset_id}
              className="card p-6 hover:border-purple/50 transition-colors group"
            >
              <div className="flex justify-between items-start mb-4">
                <div>
                  <h3 className="text-xl font-bold text-fg group-hover:text-purple transition-colors">
                    {pack.Title || pack.title}
                  </h3>
                  <p className="text-sm text-fg-muted">@{pack.ShortName || pack.short_name}</p>
                </div>
                <div className="flex gap-2">
                  <Link
                    to={`/emojipacks/${pack.StickerSetId || pack.stickerset_id}/edit`}
                    className="p-2 text-fg-muted hover:text-blue hover:bg-blue/10 rounded-lg transition-colors"
                  >
                    <Edit className="w-5 h-5" />
                  </Link>
                  <button
                    onClick={() => deletePack(pack.StickerSetId || pack.stickerset_id, pack.Title || pack.title)}
                    className="p-2 text-fg-muted hover:text-red hover:bg-red/10 rounded-lg transition-colors"
                  >
                    <Trash2 className="w-5 h-5" />
                  </button>
                </div>
              </div>

              <div className="space-y-3 mb-6 bg-muted/30 p-4 rounded-lg">
                <div className="flex items-center justify-between text-sm">
                  <span className="text-fg-muted">Emojis</span>
                  <span className="font-semibold text-fg">{pack.emoji_count || pack.Count || 0}</span>
                </div>
                <div className="flex items-center justify-between text-sm">
                  <span className="text-fg-muted">Text Color</span>
                  <span className={(pack.TextColor ?? pack.text_color) ? 'text-success' : 'text-fg-muted'}>
                    {(pack.TextColor ?? pack.text_color) ? 'Yes' : 'No'}
                  </span>
                </div>
                <div className="flex items-center justify-between text-sm">
                  <span className="text-fg-muted">Status</span>
                  <span className={`px-2 py-0.5 rounded text-xs font-medium ${(pack.Archived ?? pack.archived)
                    ? 'bg-red/10 text-red'
                    : 'bg-success/10 text-success'
                    }`}>
                    {(pack.Archived ?? pack.archived) ? 'Archived' : 'Active'}
                  </span>
                </div>
              </div>

              <div className="flex gap-3">
                <Link
                  to={`/emojipacks/${pack.StickerSetId || pack.stickerset_id}`}
                  className="flex-1 btn btn-secondary text-center justify-center"
                >
                  Manage
                </Link>
                <a
                  href={`https://t.me/addemoji/${pack.ShortName || pack.short_name}`}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="btn btn-ghost px-3 text-fg-muted hover:text-blue"
                  title="Open in Telegram"
                >
                  <ExternalLink className="w-5 h-5" />
                </a>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex justify-center gap-2 pt-6">
          <button
            onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
            disabled={currentPage === 1}
            className="btn btn-secondary disabled:opacity-50"
          >
            Previous
          </button>
          <span className="btn btn-ghost cursor-default">
            Page {currentPage} of {totalPages}
          </span>
          <button
            onClick={() => setCurrentPage((p) => Math.min(totalPages, p + 1))}
            disabled={currentPage === totalPages}
            className="btn btn-secondary disabled:opacity-50"
          >
            Next
          </button>
        </div>
      )}
    </div>
  );
}









