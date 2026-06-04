import { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { ArrowLeft, Upload, Trash2, Download, ExternalLink, Copy, BarChart3 } from 'lucide-react';
import { toast } from 'react-hot-toast';

export default function ManageEmojiPack() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [pack, setPack] = useState(null);
  const [emojis, setEmojis] = useState([]);
  const [loading, setLoading] = useState(true);
  const [uploading, setUploading] = useState(false);
  const [stats, setStats] = useState(null);

  // Redirect if no ID
  useEffect(() => {
    if (!id || id === 'undefined' || id === 'null') {
      navigate('/emojipacks', { replace: true });
      return;
    }
  }, [id, navigate]);

  const [uploadForm, setUploadForm] = useState({
    file: null,
    alt: '',
    is_free: true,
    has_text_color: false
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
      const response = await fetch(`/api/emojipacks/${id}`);
      if (!response.ok) {
        throw new Error('Failed to fetch pack');
      }
      const data = await response.json();
      setPack(data.pack);
      setEmojis(data.emojis || []);
    } catch (error) {
      console.error('Error fetching pack:', error);
      toast.error('Failed to load emoji pack');
    } finally {
      setLoading(false);
    }
  };

  const fetchStats = async () => {
    if (!id) return;

    try {
      const response = await fetch(`/api/emojipacks/${id}/stats`);
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

    if (!uploadForm.file || !uploadForm.alt) {
      toast.error('Please select a file and enter a fallback emoji');
      return;
    }

    setUploading(true);

    const formData = new FormData();
    formData.append('file', uploadForm.file);
    formData.append('alt', uploadForm.alt);
    formData.append('is_free', uploadForm.is_free);
    formData.append('has_text_color', uploadForm.has_text_color);

    try {
      const response = await fetch(`/api/emojipacks/${id}/emojis`, {
        method: 'POST',
        body: formData
      });

      if (response.ok) {
        toast.success('Emoji uploaded successfully!');
        setUploadForm({ file: null, alt: '', is_free: true, has_text_color: false });
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

  const deleteEmoji = async (emojiId) => {
    if (!confirm('Delete this emoji?')) return;

    try {
      const response = await fetch(`/api/emojipacks/${id}/emojis/${emojiId}`, {
        method: 'DELETE'
      });

      if (response.ok) {
        toast.success('Emoji deleted');
        fetchPackData();
        fetchStats();
      } else {
        toast.error('Failed to delete emoji');
      }
    } catch (error) {
      console.error('Error deleting emoji:', error);
      toast.error('Failed to delete emoji');
    }
  };

  const copyPackLink = () => {
    const shortName = pack.ShortName || pack.short_name;
    const link = `https://t.me/addemoji/${shortName}`;
    navigator.clipboard.writeText(link);
    toast.success('Pack link copied to clipboard!');
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
        <Link to="/emojipacks" className="text-purple hover:underline mt-4 inline-block">
          Back to Packs
        </Link>
      </div>
    );
  }

  const packTitle = pack.Title || pack.title;
  const packShortName = pack.ShortName || pack.short_name;

  return (
    <div className="space-y-6 animate-fade-in">
      {/* Header */}
      <div>
        <Link
          to="/emojipacks"
          className="inline-flex items-center text-fg-muted hover:text-purple mb-4 transition-colors"
        >
          <ArrowLeft className="w-5 h-5 mr-2" />
          Back to Packs
        </Link>
        <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
          <div>
            <h1 className="text-3xl font-heading font-bold text-fg">{packTitle}</h1>
            <p className="text-fg-muted">@{packShortName}</p>
          </div>
          <div className="flex gap-2">
            <button
              onClick={copyPackLink}
              className="btn btn-secondary flex items-center gap-2"
            >
              <Copy className="w-4 h-4" />
              Copy Link
            </button>
            <a
              href={`https://t.me/addemoji/${packShortName}`}
              target="_blank"
              rel="noopener noreferrer"
              className="btn btn-primary flex items-center gap-2"
            >
              <ExternalLink className="w-4 h-4" />
              Open in Telegram
            </a>
          </div>
        </div>
      </div>

      {/* Stats */}
      {stats && (
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="card p-4">
            <div className="text-sm text-fg-muted">Total Emojis</div>
            <div className="text-2xl font-bold text-purple">{stats.total_emojis}</div>
          </div>
          <div className="card p-4">
            <div className="text-sm text-fg-muted">Free Emojis</div>
            <div className="text-2xl font-bold text-success">{stats.free_emojis}</div>
          </div>
          <div className="card p-4">
            <div className="text-sm text-fg-muted">Premium Emojis</div>
            <div className="text-2xl font-bold text-yellow">{stats.premium_emojis}</div>
          </div>
          <div className="card p-4">
            <div className="text-sm text-fg-muted">Total Usage</div>
            <div className="text-2xl font-bold text-blue">{stats.total_usage}</div>
          </div>
        </div>
      )}

      {/* Upload Form */}
      <div className="card p-6">
        <h2 className="text-xl font-bold text-fg mb-6 flex items-center gap-2">
          <Upload className="w-6 h-6 text-purple" />
          Upload New Emoji
        </h2>

        <form onSubmit={handleUpload} className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {/* File Upload */}
            <div>
              <label className="block text-sm font-medium text-fg mb-2">
                TGS/WEBM File <span className="text-red">*</span>
              </label>
              <input
                id="file-input"
                type="file"
                accept=".tgs,.json,.webm"
                onChange={handleFileChange}
                className="input w-full p-2"
              />
              <p className="mt-1 text-xs text-fg-muted">Max 64 KB, 512×512px, 60 FPS, ≤3 sec</p>
            </div>

            {/* Alt Emoji */}
            <div>
              <label className="block text-sm font-medium text-fg mb-2">
                Fallback Emoji <span className="text-red">*</span>
              </label>
              <input
                type="text"
                value={uploadForm.alt}
                onChange={(e) => setUploadForm({ ...uploadForm, alt: e.target.value })}
                placeholder="🔥"
                maxLength="10"
                className="input w-full"
              />
              <p className="mt-1 text-xs text-fg-muted">UTF-8 emoji for fallback</p>
            </div>
          </div>

          {/* Options */}
          <div className="flex flex-wrap gap-6 pt-2">
            <label className="flex items-center gap-2 cursor-pointer group">
              <input
                type="checkbox"
                checked={uploadForm.is_free}
                onChange={(e) => setUploadForm({ ...uploadForm, is_free: e.target.checked })}
                className="w-4 h-4 text-purple rounded border-input focus:ring-purple/50 bg-muted"
              />
              <span className="text-sm text-fg group-hover:text-purple transition-colors">Free (non-Premium users)</span>
            </label>

            <label className="flex items-center gap-2 cursor-pointer group">
              <input
                type="checkbox"
                checked={uploadForm.has_text_color}
                onChange={(e) => setUploadForm({ ...uploadForm, has_text_color: e.target.checked })}
                className="w-4 h-4 text-purple rounded border-input focus:ring-purple/50 bg-muted"
              />
              <span className="text-sm text-fg group-hover:text-purple transition-colors">Text Color Support</span>
            </label>
          </div>

          <button
            type="submit"
            disabled={uploading}
            className="btn btn-primary flex items-center justify-center gap-2 w-full md:w-auto px-8"
          >
            <Upload className="w-5 h-5" />
            {uploading ? 'Uploading...' : 'Upload Emoji'}
          </button>
        </form>
      </div>

      {/* Emojis List */}
      <div className="card p-6">
        <h2 className="text-xl font-bold text-fg mb-6">
          Emojis ({emojis.length})
        </h2>

        {emojis.length === 0 ? (
          <p className="text-fg-muted text-center py-8">
            No emojis yet. Upload your first emoji above!
          </p>
        ) : (
          <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
            {emojis.map((emoji) => (
              <div
                key={emoji.document_id}
                className="card border-border hover:border-purple transition-colors relative group p-4"
              >
                <div className="text-center">
                  <div className="text-5xl mb-2">{emoji.attributes.alt}</div>
                  <div className="text-xs text-fg-muted truncate w-full">ID: {emoji.document_id}</div>
                  <div className="text-xs text-fg-muted">{(emoji.size / 1024).toFixed(1)} KB</div>

                  {/* Badges */}
                  <div className="mt-3 flex flex-wrap gap-1 justify-center">
                    {emoji.attributes.free && (
                      <span className="px-2 py-0.5 bg-success/10 text-success text-[10px] rounded border border-success/20">Free</span>
                    )}
                    {!emoji.attributes.free && (
                      <span className="px-2 py-0.5 bg-yellow/10 text-yellow text-[10px] rounded border border-yellow/20">Premium</span>
                    )}
                    {emoji.attributes.text_color && (
                      <span className="px-2 py-0.5 bg-purple/10 text-purple text-[10px] rounded border border-purple/20">Color</span>
                    )}
                  </div>

                  {/* Actions */}
                  <button
                    onClick={() => deleteEmoji(emoji.document_id)}
                    className="absolute top-2 right-2 p-1.5 text-red-400 hover:text-red hover:bg-red/10 rounded-lg opacity-0 group-hover:opacity-100 transition-all"
                    title="Delete"
                  >
                    <Trash2 className="w-4 h-4" />
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}










