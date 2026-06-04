import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import toast from 'react-hot-toast';
import { Bell, ArrowLeft, Save, AlertCircle, FileText, CheckCircle2 } from 'lucide-react';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:3001';

export default function CreateServiceNotification() {
  const navigate = useNavigate();
  const { id } = useParams();
  const isEdit = !!id;

  const [formData, setFormData] = useState({
    type: '',
    title: '',
    message: '',
    mediaUrl: '',
    mediaType: '',
    isPopup: true,
    isActive: true
  });
  const [loading, setLoading] = useState(false);
  const [loadingTemplate, setLoadingTemplate] = useState(isEdit);

  useEffect(() => {
    if (isEdit) {
      loadTemplate();
    }
  }, [id]);

  const loadTemplate = async () => {
    try {
      setLoadingTemplate(true);
      const response = await fetch(`${API_URL}/api/service-notifications/${id}`);
      const data = await response.json();

      if (data.success) {
        setFormData({
          type: data.template.Type || '',
          title: data.template.Title || '',
          message: data.template.Message || '',
          mediaUrl: data.template.MediaUrl || '',
          mediaType: data.template.MediaType || '',
          isPopup: data.template.IsPopup !== undefined ? data.template.IsPopup : true,
          isActive: data.template.IsActive !== undefined ? data.template.IsActive : true
        });
      } else {
        toast.error('Failed to load template');
        navigate('/service-notifications');
      }
    } catch (error) {
      console.error('Error loading template:', error);
      toast.error('Error loading template');
      navigate('/service-notifications');
    } finally {
      setLoadingTemplate(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!formData.type || !formData.title || !formData.message) {
      toast.error('Please fill in all required fields');
      return;
    }

    try {
      setLoading(true);
      const url = isEdit
        ? `${API_URL}/api/service-notifications/${id}`
        : `${API_URL}/api/service-notifications`;

      const method = isEdit ? 'PUT' : 'POST';

      const response = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(formData)
      });

      const data = await response.json();

      if (data.success) {
        toast.success(isEdit ? 'Template updated successfully' : 'Template created successfully');
        navigate('/service-notifications');
      } else {
        toast.error(data.error || 'Failed to save template');
      }
    } catch (error) {
      console.error('Error saving template:', error);
      toast.error('Error saving template');
    } finally {
      setLoading(false);
    }
  };

  if (loadingTemplate) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-purple"></div>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto space-y-6 animate-fade-in">
      {/* Header */}
      <div className="flex items-center gap-4">
        <button
          onClick={() => navigate('/service-notifications')}
          className="p-2 -ml-2 text-fg-muted hover:text-fg rounded-full transition-colors"
        >
          <ArrowLeft className="w-6 h-6" />
        </button>
        <div>
          <h1 className="text-3xl font-heading font-bold text-fg flex items-center gap-3">
            <Bell className="w-8 h-8 text-purple" />
            {isEdit ? 'Edit' : 'Create'} Notification Template
          </h1>
          <p className="mt-1 text-fg-muted">
            {isEdit ? 'Update' : 'Create a new'} system notification template
          </p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Form */}
        <div className="lg:col-span-2">
          <form onSubmit={handleSubmit} className="bg-card border border-border rounded-xl shadow-lg p-6 space-y-6">
            {/* Type */}
            <div>
              <label className="block text-sm font-medium text-fg mb-1.5">
                Type <span className="text-red">*</span>
              </label>
              <input
                type="text"
                value={formData.type}
                onChange={(e) => setFormData({ ...formData, type: e.target.value })}
                className="input w-full"
                placeholder="e.g., ads, premium_promo, system_update"
                required
              />
              <p className="mt-1 text-xs text-fg-muted">
                Used for deduplication. Same type won't show twice within 15 minutes.
              </p>
            </div>

            {/* Title */}
            <div>
              <label className="block text-sm font-medium text-fg mb-1.5">
                Title <span className="text-red">*</span>
              </label>
              <input
                type="text"
                value={formData.title}
                onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                className="input w-full"
                placeholder="e.g., Premium Discount!"
                required
              />
            </div>

            {/* Message */}
            <div>
              <label className="block text-sm font-medium text-fg mb-1.5">
                Message <span className="text-red">*</span>
              </label>
              <textarea
                value={formData.message}
                onChange={(e) => setFormData({ ...formData, message: e.target.value })}
                rows={4}
                className="input w-full resize-none"
                placeholder="e.g., 🎉 Get Premium with 50% discount! Limited time offer."
                required
              />
              <p className="mt-1 text-xs text-fg-muted">
                Supports emoji and text formatting
              </p>
            </div>

            {/* Media URL (Optional) */}
            <div>
              <label className="block text-sm font-medium text-fg mb-1.5">
                Media URL (Optional)
              </label>
              <input
                type="url"
                value={formData.mediaUrl}
                onChange={(e) => setFormData({ ...formData, mediaUrl: e.target.value })}
                className="input w-full"
                placeholder="https://example.com/image.jpg"
              />
            </div>

            {/* Media Type */}
            {formData.mediaUrl && (
              <div className="animate-fade-in">
                <label className="block text-sm font-medium text-fg mb-1.5">
                  Media Type
                </label>
                <select
                  value={formData.mediaType}
                  onChange={(e) => setFormData({ ...formData, mediaType: e.target.value })}
                  className="input w-full"
                >
                  <option value="">Select type</option>
                  <option value="photo">Photo</option>
                  <option value="video">Video</option>
                </select>
              </div>
            )}

            {/* Display Type */}
            <div className="bg-muted/50 p-4 rounded-lg border border-border">
              <label className="block text-sm font-medium text-fg mb-3">
                Display Type
              </label>
              <div className="space-y-3">
                <label className={`flex items-start gap-3 cursor-pointer p-3 rounded-lg border transition-all ${formData.isPopup ? 'bg-purple/10 border-purple text-fg' : 'border-border text-fg-muted hover:bg-muted'}`}>
                  <div className="relative flex items-center mt-0.5">
                    <input
                      type="radio"
                      checked={formData.isPopup}
                      onChange={() => setFormData({ ...formData, isPopup: true })}
                      className="sr-only"
                    />
                    {formData.isPopup ? <CheckCircle2 className="w-5 h-5 text-purple" /> : <div className="w-5 h-5 rounded-full border border-fg-muted" />}
                  </div>
                  <div className="flex-1">
                    <span className="flex items-center gap-2 font-medium mb-0.5">
                      <AlertCircle className="w-4 h-4" /> Popup
                    </span>
                    <span className="text-sm opacity-80 block">
                      Show as alert/popup (user must dismiss)
                    </span>
                  </div>
                </label>

                <label className={`flex items-start gap-3 cursor-pointer p-3 rounded-lg border transition-all ${!formData.isPopup ? 'bg-purple/10 border-purple text-fg' : 'border-border text-fg-muted hover:bg-muted'}`}>
                  <div className="relative flex items-center mt-0.5">
                    <input
                      type="radio"
                      checked={!formData.isPopup}
                      onChange={() => setFormData({ ...formData, isPopup: false })}
                      className="sr-only"
                    />
                    {!formData.isPopup ? <CheckCircle2 className="w-5 h-5 text-purple" /> : <div className="w-5 h-5 rounded-full border border-fg-muted" />}
                  </div>
                  <div className="flex-1">
                    <span className="flex items-center gap-2 font-medium mb-0.5">
                      <FileText className="w-4 h-4" /> Message
                    </span>
                    <span className="text-sm opacity-80 block">
                      Save as message from "Telegram" (user 777000)
                    </span>
                  </div>
                </label>
              </div>
            </div>

            {/* Active Status */}
            <div>
              <label className="flex items-center gap-3 p-3 bg-muted/30 rounded-lg cursor-pointer hover:bg-muted/50 transition-colors">
                <div className="relative flex items-center">
                  <input
                    type="checkbox"
                    className="peer sr-only"
                    checked={formData.isActive}
                    onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                  />
                  <div className="w-11 h-6 bg-muted rounded-full peer peer-focus:ring-2 peer-focus:ring-purple/20 dark:peer-focus:ring-purple/40 peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-0.5 after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-purple"></div>
                </div>
                <span className="text-sm font-medium text-fg">
                  Active (can be sent to users)
                </span>
              </label>
            </div>

            {/* Buttons */}
            <div className="flex justify-end gap-3 pt-4 border-t border-border">
              <button
                type="button"
                onClick={() => navigate('/service-notifications')}
                className="btn btn-secondary"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={loading}
                className="btn btn-primary min-w-[140px]"
              >
                {loading ? (
                  <div className="flex items-center gap-2">
                    <div className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin"></div>
                    <span>Saving...</span>
                  </div>
                ) : (
                  <div className="flex items-center gap-2">
                    <Save className="w-4 h-4" />
                    <span>{isEdit ? 'Update Template' : 'Create Template'}</span>
                  </div>
                )}
              </button>
            </div>
          </form>
        </div>

        {/* Examples */}
        <div className="bg-card border border-border rounded-xl p-6 h-fit sticky top-6">
          <h3 className="text-lg font-bold font-heading text-fg mb-4 flex items-center gap-2">
            <span className="text-2xl">💡</span> Examples
          </h3>
          <div className="space-y-4">
            <div className="p-4 bg-muted/30 rounded-lg border border-border">
              <strong className="text-fg block mb-2">Premium Ad</strong>
              <div className="text-sm text-fg-muted space-y-1">
                <div className="flex flex-col gap-1">
                  <span className="text-xs uppercase tracking-wide opacity-70">Type</span>
                  <code className="bg-muted px-1.5 py-0.5 rounded text-purple font-mono text-xs w-fit">ads</code>
                </div>
                <div className="flex flex-col gap-1 mt-2">
                  <span className="text-xs uppercase tracking-wide opacity-70">Message</span>
                  <p className="italic">"🎉 Get Premium with 50% discount! Limited time offer."</p>
                </div>
              </div>
            </div>

            <div className="p-4 bg-muted/30 rounded-lg border border-border">
              <strong className="text-fg block mb-2">System Announcement</strong>
              <div className="text-sm text-fg-muted space-y-1">
                <div className="flex flex-col gap-1">
                  <span className="text-xs uppercase tracking-wide opacity-70">Type</span>
                  <code className="bg-muted px-1.5 py-0.5 rounded text-purple font-mono text-xs w-fit">system_update</code>
                </div>
                <div className="flex flex-col gap-1 mt-2">
                  <span className="text-xs uppercase tracking-wide opacity-70">Message</span>
                  <p className="italic">"⚠️ Server maintenance in 10 minutes. Service will be unavailable for 5 minutes."</p>
                </div>
              </div>
            </div>

            <div className="p-4 bg-muted/30 rounded-lg border border-border">
              <strong className="text-fg block mb-2">New Feature</strong>
              <div className="text-sm text-fg-muted space-y-1">
                <div className="flex flex-col gap-1">
                  <span className="text-xs uppercase tracking-wide opacity-70">Type</span>
                  <code className="bg-muted px-1.5 py-0.5 rounded text-purple font-mono text-xs w-fit">feature_announcement</code>
                </div>
                <div className="flex flex-col gap-1 mt-2">
                  <span className="text-xs uppercase tracking-wide opacity-70">Message</span>
                  <p className="italic">"🆕 New stickers and emojis added! Check them out now."</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
