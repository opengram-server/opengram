import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Sticker, Plus, Search, ExternalLink, Trash2, Edit, FileArchive, Star } from 'lucide-react';
import { toast } from 'react-hot-toast';

export default function StickerPacks() {
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
        `/api/stickerpacks?page=${currentPage}&limit=20&search=${searchTerm}`
      );
      const data = await response.json();
      console.log('Sticker packs response:', data);
      setPacks(data.packs || []);
      setTotalPages(data.pagination?.totalPages || 1);
    } catch (error) {
      console.error('Error fetching sticker packs:', error);
      toast.error('Failed to load sticker packs');
      setPacks([]);
    } finally {
      setLoading(false);
    }
  };

  const deletePack = async (packId, packName) => {
    if (!confirm(`Delete sticker pack "${packName}"? This will delete all stickers in the pack.`)) {
      return;
    }

    try {
      const response = await fetch(`/api/stickerpacks/${packId}`, {
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
            <Sticker className="w-8 h-8 text-blue" />
            Sticker Packs
          </h1>
          <p className="text-fg-muted mt-1">Manage regular sticker sets (not custom emoji)</p>
        </div>
        <div className="flex flex-wrap gap-3">
          <Link
            to="/stickerpacks/featured"
            className="btn btn-secondary text-yellow hover:text-yellow/80 hover:bg-yellow/10 border-yellow/20 flex items-center gap-2"
          >
            <Star className="w-5 h-5" />
            Featured
          </Link>
          <Link
            to="/stickerpacks/bulk-upload"
            className="btn bg-success/10 text-success hover:bg-success/20 border-success/20 flex items-center gap-2"
          >
            <FileArchive className="w-5 h-5" />
            Bulk ZIP
          </Link>
          <Link
            to="/stickerpacks/bulk-upload-stickers"
            className="btn bg-purple/10 text-purple hover:bg-purple/20 border-purple/20 flex items-center gap-2"
          >
            <FileArchive className="w-5 h-5" />
            Bulk TGS
          </Link>
          <Link
            to="/stickerpacks/create"
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
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue"></div>
        </div>
      ) : packs.length === 0 ? (
        <div className="text-center py-20 card">
          <Sticker className="w-16 h-16 text-fg-muted mx-auto mb-4" />
          <p className="text-fg-muted text-lg">No sticker packs found</p>
          <Link
            to="/stickerpacks/create"
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
              key={pack.StickerSetId}
              className="card p-6 hover:border-blue/50 transition-colors group"
            >
              <div className="flex justify-between items-start mb-4">
                <div>
                  <h3 className="text-xl font-bold text-fg group-hover:text-blue transition-colors">
                    {pack.Title}
                  </h3>
                  <p className="text-sm text-fg-muted">@{pack.ShortName}</p>
                </div>
                <div className="flex gap-2">
                  <Link
                    to={`/stickerpacks/${pack.StickerSetId}/edit`}
                    className="p-2 text-fg-muted hover:text-blue hover:bg-blue/10 rounded-lg transition-colors"
                  >
                    <Edit className="w-5 h-5" />
                  </Link>
                  <button
                    onClick={() => deletePack(pack.StickerSetId, pack.Title)}
                    className="p-2 text-fg-muted hover:text-red hover:bg-red/10 rounded-lg transition-colors"
                  >
                    <Trash2 className="w-5 h-5" />
                  </button>
                </div>
              </div>

              <div className="space-y-3 mb-6 bg-muted/30 p-4 rounded-lg">
                <div className="flex items-center justify-between text-sm">
                  <span className="text-fg-muted">Stickers</span>
                  <span className="font-semibold text-fg">{pack.Count || 0}</span>
                </div>
                <div className="flex items-center justify-between text-sm">
                  <span className="text-fg-muted">Type</span>
                  <span className="text-fg">
                    {pack.Masks ? 'Mask Stickers' : 'Regular Stickers'}
                  </span>
                </div>
                <div className="flex items-center justify-between text-sm">
                  <span className="text-fg-muted">Status</span>
                  <span className={`px-2 py-0.5 rounded text-xs font-medium ${pack.Archived
                    ? 'bg-red/10 text-red'
                    : 'bg-success/10 text-success'
                    }`}>
                    {pack.Archived ? 'Archived' : 'Active'}
                  </span>
                </div>
              </div>

              <div className="flex gap-3">
                <Link
                  to={`/stickerpacks/${pack.StickerSetId}`}
                  className="flex-1 btn btn-secondary text-center justify-center"
                >
                  Manage
                </Link>
                <a
                  href={`https://t.me/addstickers/${pack.ShortName}`}
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









