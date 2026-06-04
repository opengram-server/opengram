import { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { ArrowLeft, Upload, Trash2, Copy, ExternalLink, BarChart3, FileImage, FileVideo, Film } from 'lucide-react';
import { toast } from 'react-hot-toast';

export default function ManageStickerPack() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [pack, setPack] = useState(null);
  const [stickers, setStickers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [uploading, setUploading] = useState(false);
  const [stats, setStats] = useState(null);

  // Redirect if no ID
  useEffect(() => {
    if (!id || id === 'undefined' || id === 'null') {
      navigate('/stickerpacks', { replace: true });
      return;
    }
  }, [id, navigate]);

  const [uploadForm, setUploadForm] = useState({
    file: null,
    emoji: '😀',
    creator_id: '2010001' // Default user ID
  });

  useEffect(() => {
    if (id && id !== 'undefined' && id !== 'null') {
      fetchPackData();
      fetchStats();
    }
  }, [id]);

  const fetchPackData = async () => {
    if (!id) return;

    try {
      setLoading(true);
      const response = await fetch(`/api/stickerpacks/${id}`);
      if (!response.ok) {
        throw new Error('Failed to fetch pack');
      }
      const data = await response.json();
      setPack(data.pack);
      setStickers(data.stickers || []);
    } catch (error) {
      console.error('Error fetching pack:', error);
      toast.error('Failed to load sticker pack');
    } finally {
      setLoading(false);
    }
  };

  const fetchStats = async () => {
    if (!id) return;

    try {
      const response = await fetch(`/api/stickerpacks/${id}/stats`);
      if (response.ok) {
        const data = await response.json();
        setStats(data);
      }
    } catch (error) {
      console.error('Error fetching stats:', error);
    }
  };

  const handleFileChange = (e) => {
    setUploadForm({ ...uploadForm, file: e.target.files[0] });
  };

  const handleUpload = async (e) => {
    e.preventDefault();

    if (!uploadForm.file || !uploadForm.emoji) {
      toast.error('Please select a file and enter an emoji');
      return;
    }

    // Validate file type
    const ext = uploadForm.file.name.split('.').pop().toLowerCase();
    const allowedExtensions = ['tgs', 'webm', 'webp', 'png'];

    if (!allowedExtensions.includes(ext)) {
      toast.error('Invalid file type. Allowed: .tgs, .webm, .webp, .png');
      return;
    }

    setUploading(true);

    const formData = new FormData();
    formData.append('file', uploadForm.file);
    formData.append('emoji', uploadForm.emoji);
    formData.append('creator_id', uploadForm.creator_id);

    try {
      const response = await fetch(`/api/stickerpacks/${id}/stickers`, {
        method: 'POST',
        body: formData
      });

      if (response.ok) {
        toast.success('Sticker uploaded successfully!');
        setUploadForm({ file: null, emoji: '😀', creator_id: uploadForm.creator_id });
        document.getElementById('file-input').value = '';
        fetchPackData();
        fetchStats();
      } else {
        const error = await response.json();
        toast.error(`Upload failed: ${error.error}`);
      }
    } catch (error) {
      console.error('Error uploading:', error);
      toast.error('Upload failed');
    } finally {
      setUploading(false);
    }
  };

  const deleteSticker = async (stickerId) => {
    if (!confirm('Delete this sticker?')) return;

    try {
      const response = await fetch(`/api/stickerpacks/${id}/stickers/${stickerId}`, {
        method: 'DELETE'
      });

      if (response.ok) {
        toast.success('Sticker deleted');
        fetchPackData();
        fetchStats();
      } else {
        toast.error('Failed to delete sticker');
      }
    } catch (error) {
      console.error('Error deleting sticker:', error);
      toast.error('Failed to delete sticker');
    }
  };

  const copyPackLink = () => {
    const shortName = pack.ShortName;
    const link = `https://t.me/addstickers/${shortName}`;
    navigator.clipboard.writeText(link);
    toast.success('Pack link copied to clipboard!');
  };

  const getStickerIcon = (mimeType) => {
    if (mimeType === 'application/x-tgsticker') {
      return <Film className="w-5 h-5 text-purple" />;
    } else if (mimeType === 'video/webm') {
      return <FileVideo className="w-5 h-5 text-blue" />;
    } else {
      return <FileImage className="w-5 h-5 text-success" />;
    }
  };

  const getStickerTypeLabel = (mimeType) => {
    switch (mimeType) {
      case 'application/x-tgsticker':
        return 'Animated (TGS)';
      case 'video/webm':
        return 'Video (WebM)';
      case 'image/webp':
        return 'Static (WebP)';
      case 'image/png':
        return 'Static (PNG)';
      default:
        return 'Unknown';
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center py-20">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-purple"></div>
      </div>
    );
  }

  if (!pack) {
    return (
      <div className="p-6 text-center">
        <p className="text-red-400">Pack not found</p>
        <Link to="/stickerpacks" className="text-purple hover:underline mt-4 inline-block">
          Back to Sticker Packs
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-6 animate-fade-in">
      {/* Header */}
      <div>
        <Link
          to="/stickerpacks"
          className="inline-flex items-center text-fg-muted hover:text-purple mb-4 transition-colors"
        >
          <ArrowLeft className="w-5 h-5 mr-2" />
          Back to Sticker Packs
        </Link>
        <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
          <div>
            <h1 className="text-3xl font-heading font-bold text-fg">{pack.Title}</h1>
            <p className="text-fg-muted mt-1">@{pack.ShortName}</p>
          </div>
          <button
            onClick={copyPackLink}
            className="btn btn-secondary flex items-center gap-2"
          >
            <Copy className="w-5 h-5" />
            Copy Link
          </button>
        </div>
      </div>

      {/* Stats */}
      {stats && (
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="card p-4">
            <div className="text-sm text-fg-muted">Total Stickers</div>
            <div className="text-2xl font-bold text-purple">{stats.total_stickers}</div>
          </div>
          <div className="card p-4">
            <div className="text-sm text-fg-muted">Animated (TGS)</div>
            <div className="text-2xl font-bold text-blue">{stats.sticker_types?.tgs || 0}</div>
          </div>
          <div className="card p-4">
            <div className="text-sm text-fg-muted">Video (WebM)</div>
            <div className="text-2xl font-bold text-yellow">{stats.sticker_types?.webm || 0}</div>
          </div>
          <div className="card p-4">
            <div className="text-sm text-fg-muted">Static (WebP/PNG)</div>
            <div className="text-2xl font-bold text-success">
              {(stats.sticker_types?.webp || 0) + (stats.sticker_types?.png || 0)}
            </div>
          </div>
        </div>
      )}

      {/* Upload Form */}
      <div className="card p-6">
        <h2 className="text-xl font-bold text-fg mb-6 flex items-center gap-2">
          <Upload className="w-6 h-6 text-purple" />
          Upload Sticker
        </h2>
        <form onSubmit={handleUpload} className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div>
              <label className="block text-sm font-medium text-fg mb-2">
                Sticker File <span className="text-red">*</span>
              </label>
              <input
                id="file-input"
                type="file"
                accept=".tgs,.webm,.webp,.png"
                onChange={handleFileChange}
                className="input w-full p-2"
                required
              />
              <p className="text-xs text-fg-muted mt-1">
                Formats: TGS (animated), WebM (video), WebP/PNG (static)
              </p>
            </div>

            <div>
              <label className="block text-sm font-medium text-fg mb-2">
                Associated Emoji <span className="text-red">*</span>
              </label>
              <input
                type="text"
                value={uploadForm.emoji}
                onChange={(e) => setUploadForm({ ...uploadForm, emoji: e.target.value })}
                className="input w-full focus:ring-2 focus:ring-purple/50"
                placeholder="😀"
                maxLength="2"
                required
              />
              <p className="text-xs text-fg-muted mt-1">
                Emoji that represents this sticker
              </p>
            </div>

            <div>
              <label className="block text-sm font-medium text-fg mb-2">
                Creator User ID <span className="text-red">*</span>
              </label>
              <input
                type="number"
                value={uploadForm.creator_id}
                onChange={(e) => setUploadForm({ ...uploadForm, creator_id: e.target.value })}
                className="input w-full focus:ring-2 focus:ring-purple/50"
                placeholder="2010001"
                required
              />
              <p className="text-xs text-fg-muted mt-1">
                User ID who created the sticker
              </p>
            </div>
          </div>

          <div className="bg-blue/5 border border-blue/20 rounded-lg p-4 text-sm">
            <strong className="text-blue">Requirements:</strong>
            <ul className="mt-2 space-y-1 text-blue/80">
              <li>• Static (WebP/PNG): 512x512 pixels</li>
              <li>• Animated (TGS): 512x512 px, max 3 sec, 60 FPS</li>
              <li>• Video (WebM): VP9 codec, 512x512 px, max 3 sec, no audio</li>
              <li>• Max file size: 512 KB</li>
            </ul>
          </div>

          <button
            type="submit"
            disabled={uploading}
            className="btn btn-primary flex items-center justify-center gap-2 w-full md:w-auto px-8"
          >
            <Upload className="w-5 h-5" />
            {uploading ? 'Uploading...' : 'Upload Sticker'}
          </button>
        </form>
      </div>

      {/* Stickers List */}
      <div className="card p-6">
        <h2 className="text-xl font-bold text-fg mb-6">Stickers ({stickers.length})</h2>

        {stickers.length === 0 ? (
          <div className="text-center py-12 text-fg-muted">
            <Upload className="w-12 h-12 mx-auto mb-2 opacity-50" />
            <p>No stickers yet. Upload your first sticker above!</p>
          </div>
        ) : (
          <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
            {stickers.map((sticker) => (
              <div
                key={sticker.DocumentId}
                className="card border-border hover:border-purple transition-colors relative group p-3"
              >
                <div className="aspect-square bg-muted/30 rounded-lg mb-2 flex items-center justify-center">
                  {getStickerIcon(sticker.MimeType)}
                </div>

                <div className="text-xs text-fg-muted mb-1 text-center">
                  {getStickerTypeLabel(sticker.MimeType)}
                </div>

                <div className="text-xs text-fg-muted mb-2 text-center">
                  {Math.round(sticker.Size / 1024)} KB
                </div>

                <div className="text-xs text-fg-muted mb-2 font-mono truncate text-center">
                  ID: {sticker.DocumentId}
                </div>

                <button
                  onClick={() => deleteSticker(sticker.DocumentId)}
                  className="w-full px-2 py-1 bg-red/10 text-red rounded hover:bg-red/20 text-xs flex items-center justify-center gap-1 transition-colors opacity-0 group-hover:opacity-100"
                >
                  <Trash2 className="w-3 h-3" />
                  Delete
                </button>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Pack Link */}
      <div className="card p-6">
        <h2 className="text-xl font-bold text-fg mb-4">Share Pack</h2>
        <div className="flex items-center gap-4">
          <input
            type="text"
            value={`https://t.me/addstickers/${pack.ShortName}`}
            readOnly
            className="input flex-1 bg-muted font-mono text-sm"
          />
          <button
            onClick={copyPackLink}
            className="btn btn-secondary flex items-center gap-2"
          >
            <Copy className="w-5 h-5" />
            Copy
          </button>
          <a
            href={`https://t.me/addstickers/${pack.ShortName}`}
            target="_blank"
            rel="noopener noreferrer"
            className="btn btn-primary"
          >
            <ExternalLink className="w-5 h-5" />
          </a>
        </div>
      </div>
    </div>
  );
}










