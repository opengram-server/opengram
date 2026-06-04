import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { ArrowLeft, Package, AlertCircle } from 'lucide-react';
import { toast } from 'react-hot-toast';

export default function CreateStickerPack() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [form, setForm] = useState({
    title: '',
    short_name: '',
    masks: false
  });

  const validateShortName = (name) => {
    // Only lowercase letters, numbers, and underscores
    return /^[a-z0-9_]+$/.test(name);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    // Validation
    if (!form.title.trim()) {
      toast.error('Title is required');
      return;
    }

    if (!form.short_name.trim()) {
      toast.error('Short name is required');
      return;
    }

    if (!validateShortName(form.short_name)) {
      toast.error('Short name must contain only lowercase letters, numbers, and underscores');
      return;
    }

    setLoading(true);

    try {
      const response = await fetch('/api/stickerpacks', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(form)
      });

      if (response.ok) {
        const data = await response.json();
        toast.success('Sticker pack created successfully!');
        navigate(`/stickerpacks/${data.pack.StickerSetId}`);
      } else {
        const errorData = await response.json();
        toast.error(errorData.error || 'Failed to create pack');
      }
    } catch (err) {
      console.error('Error creating pack:', err);
      toast.error('Failed to create pack. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-2xl mx-auto space-y-6 animate-fade-in">
      {/* Header */}
      <div>
        <Link
          to="/stickerpacks"
          className="inline-flex items-center text-fg-muted hover:text-purple mb-4 transition-colors"
        >
          <ArrowLeft className="w-5 h-5 mr-2" />
          Back to Sticker Packs
        </Link>
        <h1 className="text-3xl font-heading font-bold text-fg flex items-center gap-3">
          <Package className="w-8 h-8 text-purple" />
          Create Sticker Pack
        </h1>
        <p className="text-fg-muted mt-1">Create a new sticker set for regular stickers (512x512 px)</p>
      </div>

      {/* Form */}
      <form onSubmit={handleSubmit} className="card p-6 space-y-6">
        {/* Title */}
        <div>
          <label className="block text-sm font-medium text-fg mb-2">
            Pack Title <span className="text-red">*</span>
          </label>
          <input
            type="text"
            value={form.title}
            onChange={(e) => setForm({ ...form, title: e.target.value })}
            className="input w-full focus:ring-2 focus:ring-purple/50"
            placeholder="My Awesome Stickers"
            required
          />
          <p className="text-sm text-fg-muted mt-1">Display name of your sticker pack</p>
        </div>

        {/* Short Name */}
        <div>
          <label className="block text-sm font-medium text-fg mb-2">
            Short Name <span className="text-red">*</span> (URL identifier)
          </label>
          <div className="flex items-center gap-2">
            <span className="text-fg-muted">t.me/addstickers/</span>
            <input
              type="text"
              value={form.short_name}
              onChange={(e) => setForm({ ...form, short_name: e.target.value.toLowerCase() })}
              className="input flex-1 focus:ring-2 focus:ring-purple/50"
              placeholder="my_stickers_123"
              pattern="[a-z0-9_]+"
              required
            />
          </div>
          <p className="text-sm text-fg-muted mt-1">
            Only lowercase letters (a-z), numbers (0-9), and underscores (_). Must be unique.
          </p>
        </div>

        {/* Masks */}
        <div>
          <label className="flex items-center gap-2 cursor-pointer group">
            <input
              type="checkbox"
              checked={form.masks}
              onChange={(e) => setForm({ ...form, masks: e.target.checked })}
              className="mt-1 w-4 h-4 text-purple rounded border-input focus:ring-purple/50 bg-muted"
            />
            <span className="text-sm font-medium text-fg group-hover:text-purple transition-colors">Mask Stickers</span>
          </label>
          <p className="text-sm text-fg-muted mt-1 ml-6">
            Check if this pack contains mask stickers (stickers for face masks)
          </p>
        </div>

        {/* Info Box */}
        <div className="bg-blue/5 border border-blue/20 rounded-lg p-4">
          <h3 className="font-medium text-blue mb-2">📋 Sticker Requirements</h3>
          <ul className="text-sm text-blue/80 space-y-1">
            <li>• <strong>Static:</strong> WebP or PNG (512x512 px)</li>
            <li>• <strong>Animated:</strong> TGS (Lottie, 512x512, max 3 sec, 60 FPS)</li>
            <li>• <strong>Video:</strong> WebM (VP9, 512x512, max 3 sec, no audio)</li>
            <li>• Maximum file size: 512 KB</li>
          </ul>
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
            to="/stickerpacks"
            className="btn btn-secondary font-semibold"
          >
            Cancel
          </Link>
        </div>
      </form>
    </div>
  );
}










