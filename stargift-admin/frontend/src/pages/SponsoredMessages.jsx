import { useState, useEffect } from 'react';
import { Plus, Edit, Trash2, Power, BarChart3, Filter, X } from 'lucide-react';
import { toast } from 'react-hot-toast';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:3001';

export default function SponsoredMessages() {
  const [messages, setMessages] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingMessage, setEditingMessage] = useState(null);
  const [filterChannelId, setFilterChannelId] = useState('');
  const [filterActive, setFilterActive] = useState('');
  const [actionLoading, setActionLoading] = useState(false);

  const [formData, setFormData] = useState({
    channelId: '',
    title: '',
    message: '',
    url: '',
    buttonText: 'Learn More',
    photoUrl: '',
    sponsorInfo: '',
    additionalInfo: '',
    isActive: true,
    recommended: false,
    canReport: true,
    postsBetween: 10,
    expiresDate: null
  });

  useEffect(() => {
    fetchMessages();
  }, [filterChannelId, filterActive]);

  const fetchMessages = async () => {
    try {
      setLoading(true);
      const params = new URLSearchParams();
      if (filterChannelId) params.append('channelId', filterChannelId);
      if (filterActive !== '') params.append('isActive', filterActive);

      const response = await fetch(`${API_URL}/api/sponsored-messages?${params}`);
      const data = await response.json();
      setMessages(data.messages || []);
    } catch (error) {
      console.error('Error fetching sponsored messages:', error);
      toast.error('Failed to load sponsored messages');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setActionLoading(true);

    try {
      const url = editingMessage
        ? `${API_URL}/api/sponsored-messages/${editingMessage.Id}`
        : `${API_URL}/api/sponsored-messages`;

      const method = editingMessage ? 'PUT' : 'POST';

      const response = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          ...formData,
          channelId: parseInt(formData.channelId),
          postsBetween: parseInt(formData.postsBetween),
          expiresDate: formData.expiresDate ? Math.floor(new Date(formData.expiresDate).getTime() / 1000) : null
        })
      });

      const data = await response.json();

      if (data.success) {
        toast.success(editingMessage ? 'Sponsored message updated!' : 'Sponsored message created!');
        setShowModal(false);
        resetForm();
        fetchMessages();
      } else {
        toast.error('Error: ' + data.error);
      }
    } catch (error) {
      console.error('Error saving sponsored message:', error);
      toast.error('Failed to save sponsored message');
    } finally {
      setActionLoading(false);
    }
  };

  const handleEdit = (message) => {
    setEditingMessage(message);
    setFormData({
      channelId: message.ChannelId?.toString() || '',
      title: message.Title || '',
      message: message.Message || '',
      url: message.Url || '',
      buttonText: message.ButtonText || '',
      photoUrl: message.PhotoUrl || '',
      sponsorInfo: message.SponsorInfo || '',
      additionalInfo: message.AdditionalInfo || '',
      isActive: message.IsActive,
      recommended: message.Recommended,
      canReport: message.CanReport,
      postsBetween: message.PostsBetween || 10,
      expiresDate: message.ExpiresDate ? new Date(message.ExpiresDate * 1000).toISOString().split('T')[0] : ''
    });
    setShowModal(true);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this sponsored message?')) return;

    try {
      const response = await fetch(`${API_URL}/api/sponsored-messages/${id}`, {
        method: 'DELETE'
      });

      const data = await response.json();
      if (data.success) {
        toast.success('Sponsored message deleted!');
        fetchMessages();
      } else {
        toast.error('Error: ' + data.error);
      }
    } catch (error) {
      console.error('Error deleting sponsored message:', error);
      toast.error('Failed to delete sponsored message');
    }
  };

  const handleToggleActive = async (id) => {
    try {
      const response = await fetch(`${API_URL}/api/sponsored-messages/${id}/toggle`, {
        method: 'PATCH'
      });

      const data = await response.json();
      if (data.success) {
        fetchMessages();
        toast.success('Status updated successfully');
      } else {
        toast.error('Error: ' + data.error);
      }
    } catch (error) {
      console.error('Error toggling status:', error);
      toast.error('Failed to toggle status');
    }
  };

  const resetForm = () => {
    setEditingMessage(null);
    setFormData({
      channelId: '',
      title: '',
      message: '',
      url: '',
      buttonText: 'Learn More',
      photoUrl: '',
      sponsorInfo: '',
      additionalInfo: '',
      isActive: true,
      recommended: false,
      canReport: true,
      postsBetween: 10,
      expiresDate: null
    });
  };

  const formatDate = (timestamp) => {
    if (!timestamp) return 'Never';
    return new Date(timestamp * 1000).toLocaleDateString();
  };

  return (
    <div className="space-y-6 animate-fade-in">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-heading font-bold text-fg">Sponsored Messages</h1>
          <p className="text-fg-muted mt-1">{messages.length} campaigns</p>
        </div>
        <button
          onClick={() => { resetForm(); setShowModal(true); }}
          className="btn btn-primary flex items-center justify-center gap-2"
        >
          <Plus className="w-5 h-5" />
          Create Campaign
        </button>
      </div>

      {/* Filters */}
      <div className="card p-4">
        <div className="flex flex-col md:flex-row items-center gap-4">
          <div className="flex items-center gap-2 w-full md:w-auto">
            <Filter className="w-5 h-5 text-fg-muted" />
            <span className="text-sm font-medium text-fg">Filters:</span>
          </div>
          <input
            type="number"
            placeholder="Filter by Channel ID"
            className="input md:w-64 w-full"
            value={filterChannelId}
            onChange={(e) => setFilterChannelId(e.target.value)}
          />
          <select
            className="input md:w-auto w-full"
            value={filterActive}
            onChange={(e) => setFilterActive(e.target.value)}
          >
            <option value="">All Status</option>
            <option value="true">Active</option>
            <option value="false">Inactive</option>
          </select>
        </div>
      </div>

      {/* Messages List */}
      {loading ? (
        <div className="flex justify-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-purple"></div>
        </div>
      ) : (
        <div className="space-y-4">
          {messages.map((msg) => (
            <div key={msg.Id} className="card p-6 hover:border-purple transition-all duration-300">
              <div className="flex flex-col md:flex-row items-start justify-between gap-4">
                <div className="flex-1 w-full">
                  <div className="flex flex-wrap items-center gap-3 mb-2">
                    <h3 className="text-xl font-bold text-fg">{msg.Title}</h3>
                    <span className={`px-2 py-0.5 rounded text-xs font-semibold uppercase tracking-wider ${msg.IsActive
                        ? 'bg-success/10 text-success border border-success/20'
                        : 'bg-muted text-fg-muted border border-border'
                      }`}>
                      {msg.IsActive ? 'Active' : 'Inactive'}
                    </span>
                    {msg.Recommended && (
                      <span className="px-2 py-0.5 bg-blue/10 text-blue border border-blue/20 rounded text-xs font-semibold uppercase tracking-wider">
                        Recommended
                      </span>
                    )}
                  </div>

                  <p className="text-fg-muted mb-4 line-clamp-2">{msg.Message}</p>

                  <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm bg-muted/30 p-4 rounded-lg border border-border">
                    <div>
                      <span className="text-fg-muted block text-xs uppercase tracking-wide mb-1">Channel ID</span>
                      <p className="font-medium text-fg font-mono">{msg.ChannelId}</p>
                    </div>
                    <div>
                      <span className="text-fg-muted block text-xs uppercase tracking-wide mb-1">URL</span>
                      <p className="font-medium truncate">
                        <a href={msg.Url} target="_blank" rel="noopener noreferrer" className="text-purple hover:underline hover:text-purple/80">
                          {msg.Url}
                        </a>
                      </p>
                    </div>
                    <div>
                      <span className="text-fg-muted block text-xs uppercase tracking-wide mb-1">Frequency</span>
                      <p className="font-medium text-fg">Every {msg.PostsBetween || 10} posts</p>
                    </div>
                    <div>
                      <span className="text-fg-muted block text-xs uppercase tracking-wide mb-1">Expires</span>
                      <p className="font-medium text-fg">{formatDate(msg.ExpiresDate)}</p>
                    </div>
                  </div>

                  <div className="mt-4 flex items-center gap-6 text-sm text-fg-muted">
                    <span className="flex items-center gap-1.5" title="Displays">
                      <BarChart3 className="w-4 h-4" /> {msg.DisplayCount || 0}
                    </span>
                    <span className="flex items-center gap-1.5" title="Clicks">
                      <span className="text-lg leading-none">🖱️</span> {msg.ClickCount || 0}
                    </span>
                    <span className="ml-auto text-xs">Created: {formatDate(msg.CreatedDate)}</span>
                  </div>
                </div>

                <div className="flex gap-2 self-start">
                  <button
                    onClick={() => handleToggleActive(msg.Id)}
                    className={`p-2 rounded-lg border transition-colors ${msg.IsActive
                        ? 'border-yellow text-yellow hover:bg-yellow/10'
                        : 'border-success text-success hover:bg-success/10'
                      }`}
                    title={msg.IsActive ? 'Deactivate' : 'Activate'}
                  >
                    <Power className="w-4 h-4" />
                  </button>
                  <button
                    onClick={() => handleEdit(msg)}
                    className="p-2 rounded-lg border border-border text-fg-muted hover:text-purple hover:border-purple hover:bg-purple/5 transition-colors"
                    title="Edit"
                  >
                    <Edit className="w-4 h-4" />
                  </button>
                  <button
                    onClick={() => handleDelete(msg.Id)}
                    className="p-2 rounded-lg border border-red text-red hover:bg-red/10 transition-colors"
                    title="Delete"
                  >
                    <Trash2 className="w-4 h-4" />
                  </button>
                </div>
              </div>
            </div>
          ))}

          {messages.length === 0 && (
            <div className="text-center py-20 bg-card rounded-xl border border-border border-dashed">
              <div className="w-16 h-16 bg-muted rounded-full flex items-center justify-center mx-auto mb-4">
                <Plus className="w-8 h-8 text-fg-muted" />
              </div>
              <h3 className="text-lg font-medium text-fg mb-1">No campaigns yet</h3>
              <p className="text-fg-muted mb-4">Create your first sponsored message campaign</p>
              <button
                onClick={() => { resetForm(); setShowModal(true); }}
                className="btn btn-primary"
              >
                Create Campaign
              </button>
            </div>
          )}
        </div>
      )}

      {/* Create/Edit Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50 p-4 animate-fade-in">
          <div className="bg-card border border-border text-fg rounded-xl w-full max-w-2xl max-h-[90vh] overflow-y-auto shadow-2xl animate-scale-in">
            <div className="p-6 border-b border-border flex justify-between items-center sticky top-0 bg-card z-10">
              <h2 className="text-2xl font-heading font-bold">
                {editingMessage ? 'Edit Campaign' : 'Create Campaign'}
              </h2>
              <button onClick={() => setShowModal(false)} className="text-fg-muted hover:text-fg transition-colors">
                <X className="w-6 h-6" />
              </button>
            </div>

            <div className="p-6">
              <form onSubmit={handleSubmit} className="space-y-5">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-5">
                  <div>
                    <label className="block text-sm font-medium text-fg mb-1.5">Channel ID <span className="text-red">*</span></label>
                    <input
                      type="number"
                      required
                      className="input w-full"
                      value={formData.channelId}
                      onChange={(e) => setFormData({ ...formData, channelId: e.target.value })}
                      placeholder="-100..."
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-fg mb-1.5">Posts Between Ads</label>
                    <input
                      type="number"
                      min="1"
                      max="100"
                      className="input w-full"
                      value={formData.postsBetween}
                      onChange={(e) => setFormData({ ...formData, postsBetween: e.target.value })}
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-fg mb-1.5">Title <span className="text-red">*</span></label>
                  <input
                    type="text"
                    required
                    maxLength="100"
                    className="input w-full"
                    value={formData.title}
                    onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                    placeholder="Campaign Title"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-fg mb-1.5">Message <span className="text-red">*</span></label>
                  <textarea
                    required
                    maxLength="500"
                    rows="3"
                    className="input w-full resize-none"
                    value={formData.message}
                    onChange={(e) => setFormData({ ...formData, message: e.target.value })}
                    placeholder="Ad content..."
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-fg mb-1.5">URL <span className="text-red">*</span></label>
                  <input
                    type="url"
                    required
                    className="input w-full"
                    value={formData.url}
                    onChange={(e) => setFormData({ ...formData, url: e.target.value })}
                    placeholder="https://..."
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-fg mb-1.5">Button Text <span className="text-red">*</span></label>
                  <input
                    type="text"
                    required
                    maxLength="50"
                    className="input w-full"
                    value={formData.buttonText}
                    onChange={(e) => setFormData({ ...formData, buttonText: e.target.value })}
                    placeholder="e.g. Learn More"
                  />
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-5">
                  <div>
                    <label className="block text-sm font-medium text-fg mb-1.5">Photo URL (optional)</label>
                    <input
                      type="url"
                      className="input w-full"
                      value={formData.photoUrl}
                      onChange={(e) => setFormData({ ...formData, photoUrl: e.target.value })}
                      placeholder="https://..."
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-fg mb-1.5">Expires Date (optional)</label>
                    <input
                      type="date"
                      className="input w-full"
                      value={formData.expiresDate || ''}
                      onChange={(e) => setFormData({ ...formData, expiresDate: e.target.value })}
                    />
                  </div>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-5">
                  <div>
                    <label className="block text-sm font-medium text-fg mb-1.5">Sponsor Info (optional)</label>
                    <input
                      type="text"
                      maxLength="200"
                      className="input w-full"
                      value={formData.sponsorInfo}
                      onChange={(e) => setFormData({ ...formData, sponsorInfo: e.target.value })}
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-fg mb-1.5">Additional Info (optional)</label>
                    <input
                      type="text"
                      maxLength="200"
                      className="input w-full"
                      value={formData.additionalInfo}
                      onChange={(e) => setFormData({ ...formData, additionalInfo: e.target.value })}
                    />
                  </div>
                </div>

                <div className="flex flex-wrap gap-6 pt-2">
                  <label className="flex items-center gap-2 cursor-pointer group">
                    <div className="relative flex items-center">
                      <input
                        type="checkbox"
                        className="peer sr-only"
                        checked={formData.isActive}
                        onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                      />
                      <div className="w-11 h-6 bg-muted rounded-full peer peer-focus:ring-2 peer-focus:ring-purple/20 dark:peer-focus:ring-purple/40 peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-0.5 after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-purple"></div>
                    </div>
                    <span className="text-sm font-medium text-fg group-hover:text-purple transition-colors">Active</span>
                  </label>

                  <label className="flex items-center gap-2 cursor-pointer group">
                    <div className="relative flex items-center">
                      <input
                        type="checkbox"
                        className="peer sr-only"
                        checked={formData.recommended}
                        onChange={(e) => setFormData({ ...formData, recommended: e.target.checked })}
                      />
                      <div className="w-11 h-6 bg-muted rounded-full peer peer-focus:ring-2 peer-focus:ring-purple/20 dark:peer-focus:ring-purple/40 peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-0.5 after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-purple"></div>
                    </div>
                    <span className="text-sm font-medium text-fg group-hover:text-purple transition-colors">Recommended</span>
                  </label>

                  <label className="flex items-center gap-2 cursor-pointer group">
                    <div className="relative flex items-center">
                      <input
                        type="checkbox"
                        className="peer sr-only"
                        checked={formData.canReport}
                        onChange={(e) => setFormData({ ...formData, canReport: e.target.checked })}
                      />
                      <div className="w-11 h-6 bg-muted rounded-full peer peer-focus:ring-2 peer-focus:ring-purple/20 dark:peer-focus:ring-purple/40 peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-0.5 after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-purple"></div>
                    </div>
                    <span className="text-sm font-medium text-fg group-hover:text-purple transition-colors">Can Report</span>
                  </label>
                </div>

                <div className="flex gap-3 pt-6 border-t border-border">
                  <button
                    type="submit"
                    disabled={actionLoading}
                    className="btn btn-primary flex-1 py-2.5 font-semibold text-base"
                  >
                    {actionLoading ? 'Saving...' : (editingMessage ? 'Update Campaign' : 'Create Campaign')}
                  </button>
                  <button
                    type="button"
                    disabled={actionLoading}
                    onClick={() => { setShowModal(false); resetForm(); }}
                    className="btn btn-secondary px-6"
                  >
                    Cancel
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}









