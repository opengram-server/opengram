import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Package, ArrowLeft } from 'lucide-react';
import { Link } from 'react-router-dom';
import { toast } from 'react-hot-toast';

export default function CreateEmojiPack() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    title: '',
    short_name: '',
    text_color: false,
    channel_emoji_status: false,
    creator_id: ''
  });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      const response = await fetch('/api/emojipacks', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(formData)
      });

      if (response.ok) {
        const data = await response.json();
        toast.success('Emoji pack created successfully!');
        navigate(`/emojipacks/${data.pack.StickerSetId}`);
      } else {
        const error = await response.json();
        toast.error(`Error: ${error.error}`);
      }
    } catch (error) {
      console.error('Error creating pack:', error);
      toast.error('Failed to create pack');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  return (
    <div className="max-w-3xl mx-auto space-y-6 animate-fade-in">
      {/* Header */}
      <div>
        <Link
          to="/emojipacks"
          className="inline-flex items-center text-fg-muted hover:text-purple mb-4 transition-colors"
        >
          <ArrowLeft className="w-5 h-5 mr-2" />
          Back to Packs
        </Link>
        <h1 className="text-3xl font-heading font-bold text-fg flex items-center gap-3">
          <Package className="w-8 h-8 text-purple" />
          Create Emoji Pack
        </h1>
        <p className="text-fg-muted mt-1">Create a new custom emoji sticker set</p>
      </div>

      {/* Form */}
      <form onSubmit={handleSubmit} className="card p-6 space-y-6">
        {/* Title */}
        <div>
          <label className="block text-sm font-medium text-fg mb-2">
            Title <span className="text-red">*</span>
          </label>
          <input
            type="text"
            name="title"
            value={formData.title}
            onChange={handleChange}
            required
            placeholder="My Awesome Emojis"
            className="input w-full focus:ring-2 focus:ring-purple/50"
          />
          <p className="mt-1 text-sm text-fg-muted">
            Display name for the emoji pack
          </p>
        </div>

        {/* Short Name */}
        <div>
          <label className="block text-sm font-medium text-fg mb-2">
            Short Name <span className="text-red">*</span>
          </label>
          <div className="flex items-center gap-2">
            <span className="text-fg-muted">t.me/addemoji/</span>
            <input
              type="text"
              name="short_name"
              value={formData.short_name}
              onChange={handleChange}
              required
              pattern="[a-zA-Z0-9_]+"
              placeholder="my_emojis"
              className="input flex-1 focus:ring-2 focus:ring-purple/50"
            />
          </div>
          <p className="mt-1 text-sm text-fg-muted">
            Unique identifier (letters, numbers, and underscores only)
          </p>
        </div>

        {/* Creator ID */}
        <div>
          <label className="block text-sm font-medium text-fg mb-2">
            Creator User ID
          </label>
          <input
            type="number"
            name="creator_id"
            value={formData.creator_id}
            onChange={handleChange}
            placeholder="2000001"
            className="input w-full focus:ring-2 focus:ring-purple/50"
          />
          <p className="mt-1 text-sm text-fg-muted">
            Optional: MyTelegram user ID of the pack creator
          </p>
        </div>

        {/* Options */}
        <div className="space-y-4 pt-2 border-t border-border">
          <h3 className="text-lg font-semibold text-fg">Options</h3>

          <label className="flex items-start gap-3 cursor-pointer group">
            <input
              type="checkbox"
              name="text_color"
              checked={formData.text_color}
              onChange={handleChange}
              className="mt-1 w-4 h-4 text-purple rounded border-input focus:ring-purple/50 bg-muted"
            />
            <div>
              <div className="font-medium text-fg group-hover:text-purple transition-colors">Text Color Support</div>
              <div className="text-sm text-fg-muted">
                Emojis will change color based on context (text color, status, etc.)
              </div>
            </div>
          </label>

          <label className="flex items-start gap-3 cursor-pointer group">
            <input
              type="checkbox"
              name="channel_emoji_status"
              checked={formData.channel_emoji_status}
              onChange={handleChange}
              className="mt-1 w-4 h-4 text-purple rounded border-input focus:ring-purple/50 bg-muted"
            />
            <div>
              <div className="font-medium text-fg group-hover:text-purple transition-colors">Channel Emoji Status</div>
              <div className="text-sm text-fg-muted">
                Allow using emojis as channel status icons
              </div>
            </div>
          </label>
        </div>

        {/* Info Box */}
        <div className="bg-purple/5 border border-purple/20 rounded-lg p-4">
          <h4 className="font-semibold text-purple mb-2">📝 Next Steps</h4>
          <ol className="list-decimal list-inside text-sm text-purple/80 space-y-1">
            <li>Create the pack with a unique short name</li>
            <li>Upload TGS/WEBM files for each emoji</li>
            <li>Assign fallback emoji (alt) for each uploaded file</li>
            <li>Share the pack link with users</li>
          </ol>
        </div>

        {/* Submit */}
        <div className="flex gap-4 pt-4">
          <button
            type="submit"
            disabled={loading}
            className="flex-1 btn btn-primary font-semibold text-lg"
          >
            {loading ? 'Creating...' : 'Create Pack'}
          </button>
          <Link
            to="/emojipacks"
            className="btn btn-secondary font-semibold"
          >
            Cancel
          </Link>
        </div>
      </form>
    </div>
  );
}










