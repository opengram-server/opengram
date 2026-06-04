import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import toast from 'react-hot-toast';
import { Plus, Send, Edit, Trash2, Bell, AlertCircle, Info, FileText, CheckCircle2 } from 'lucide-react';
import SendNotificationModal from '../components/SendNotificationModal';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:3001';

export default function ServiceNotifications() {
  const [templates, setTemplates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [sendModalOpen, setSendModalOpen] = useState(false);
  const [selectedTemplate, setSelectedTemplate] = useState(null);

  useEffect(() => {
    loadTemplates();
  }, []);

  const loadTemplates = async () => {
    try {
      setLoading(true);
      const response = await fetch(`${API_URL}/api/service-notifications`);
      const data = await response.json();

      if (data.success) {
        setTemplates(data.templates);
      } else {
        toast.error('Failed to load templates');
      }
    } catch (error) {
      console.error('Error loading templates:', error);
      toast.error('Error loading templates');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (!confirm('Are you sure you want to delete this template?')) {
      return;
    }

    try {
      const response = await fetch(`${API_URL}/api/service-notifications/${id}`, {
        method: 'DELETE'
      });
      const data = await response.json();

      if (data.success) {
        toast.success('Template deleted successfully');
        loadTemplates();
      } else {
        toast.error(data.error || 'Failed to delete template');
      }
    } catch (error) {
      console.error('Error deleting template:', error);
      toast.error('Error deleting template');
    }
  };

  const handleToggleActive = async (template) => {
    try {
      const response = await fetch(`${API_URL}/api/service-notifications/${template.Id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ isActive: !template.IsActive })
      });
      const data = await response.json();

      if (data.success) {
        toast.success(`Template ${!template.IsActive ? 'activated' : 'deactivated'}`);
        loadTemplates();
      } else {
        toast.error(data.error || 'Failed to update template');
      }
    } catch (error) {
      console.error('Error updating template:', error);
      toast.error('Error updating template');
    }
  };

  const handleSendClick = (template) => {
    setSelectedTemplate(template);
    setSendModalOpen(true);
  };

  const handleSendComplete = () => {
    setSendModalOpen(false);
    setSelectedTemplate(null);
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-purple"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6 animate-fade-in">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-heading font-bold text-fg flex items-center gap-3">
            <Bell className="w-8 h-8 text-purple" />
            Service Notifications
          </h1>
          <p className="mt-1 text-fg-muted">
            Manage system notifications and popups for users
          </p>
        </div>
        <Link
          to="/service-notifications/create"
          className="btn btn-primary flex items-center justify-center gap-2"
        >
          <Plus className="w-5 h-5" />
          Create Template
        </Link>
      </div>

      {/* Templates List */}
      <div className="bg-card rounded-xl shadow-lg overflow-hidden border border-border">
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-border">
            <thead className="bg-muted/50">
              <tr>
                <th className="px-6 py-4 text-left text-xs font-semibold text-fg-muted uppercase tracking-wider">
                  Type
                </th>
                <th className="px-6 py-4 text-left text-xs font-semibold text-fg-muted uppercase tracking-wider">
                  Title
                </th>
                <th className="px-6 py-4 text-left text-xs font-semibold text-fg-muted uppercase tracking-wider">
                  Message
                </th>
                <th className="px-6 py-4 text-left text-xs font-semibold text-fg-muted uppercase tracking-wider">
                  Display
                </th>
                <th className="px-6 py-4 text-left text-xs font-semibold text-fg-muted uppercase tracking-wider">
                  Status
                </th>
                <th className="px-6 py-4 text-right text-xs font-semibold text-fg-muted uppercase tracking-wider">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-border">
              {templates.length === 0 ? (
                <tr>
                  <td colSpan="6" className="px-6 py-12 text-center text-fg-muted">
                    <div className="flex flex-col items-center gap-3">
                      <div className="w-12 h-12 bg-muted rounded-full flex items-center justify-center">
                        <Bell className="w-6 h-6 text-fg-muted" />
                      </div>
                      <p>No templates found. Create your first notification template!</p>
                    </div>
                  </td>
                </tr>
              ) : (
                templates.map((template) => (
                  <tr key={template.Id} className="hover:bg-muted/30 transition-colors">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className="px-2.5 py-1 text-xs font-medium bg-blue/10 text-blue border border-blue/20 rounded-lg">
                        {template.Type}
                      </span>
                    </td>
                    <td className="px-6 py-4">
                      <div className="text-sm font-medium text-fg">
                        {template.Title}
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="text-sm text-fg-muted max-w-md truncate" title={template.Message}>
                        {template.Message}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`flex items-center gap-1.5 px-2.5 py-1 text-xs font-medium rounded-lg border ${template.IsPopup
                          ? 'bg-purple/10 text-purple border-purple/20'
                          : 'bg-muted text-fg-muted border-border'
                        }`}>
                        {template.IsPopup ? <AlertCircle className="w-3 h-3" /> : <FileText className="w-3 h-3" />}
                        {template.IsPopup ? 'Popup' : 'Message'}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <button
                        onClick={() => handleToggleActive(template)}
                        className={`flex items-center gap-1.5 px-2.5 py-1 text-xs font-medium rounded-lg border transition-colors ${template.IsActive
                            ? 'bg-success/10 text-success border-success/20 hover:bg-success/20'
                            : 'bg-red/10 text-red border-red/20 hover:bg-red/20'
                          }`}
                      >
                        <CheckCircle2 className="w-3 h-3" />
                        {template.IsActive ? 'Active' : 'Inactive'}
                      </button>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <div className="flex justify-end gap-2">
                        <button
                          onClick={() => handleSendClick(template)}
                          disabled={!template.IsActive}
                          className="p-2 text-blue hover:bg-blue/10 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                          title="Send Notification"
                        >
                          <Send className="w-4 h-4" />
                        </button>
                        <Link
                          to={`/service-notifications/edit/${template.Id}`}
                          className="p-2 text-fg-muted hover:text-purple hover:bg-purple/10 rounded-lg transition-colors"
                          title="Edit Template"
                        >
                          <Edit className="w-4 h-4" />
                        </Link>
                        <button
                          onClick={() => handleDelete(template.Id)}
                          className="p-2 text-red hover:bg-red/10 rounded-lg transition-colors"
                          title="Delete Template"
                        >
                          <Trash2 className="w-4 h-4" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Info Box */}
      <div className="bg-blue/5 border border-blue/20 rounded-xl p-5 flex items-start gap-4">
        <div className="p-2 bg-blue/10 rounded-lg shrink-0">
          <Info className="w-5 h-5 text-blue" />
        </div>
        <div>
          <h3 className="text-base font-semibold text-blue mb-2">About Service Notifications</h3>
          <ul className="text-sm text-fg-muted space-y-1.5">
            <li className="flex items-center gap-2">
              <span className="w-1.5 h-1.5 rounded-full bg-blue/50"></span>
              <span><strong className="text-fg">Popup:</strong> Shows as an alert/popup in the client (user must dismiss)</span>
            </li>
            <li className="flex items-center gap-2">
              <span className="w-1.5 h-1.5 rounded-full bg-blue/50"></span>
              <span><strong className="text-fg">Message:</strong> Saved as a message from "Telegram" (user 777000)</span>
            </li>
            <li className="flex items-center gap-2">
              <span className="w-1.5 h-1.5 rounded-full bg-blue/50"></span>
              <span><strong className="text-fg">Type:</strong> Used for deduplication (same type won't show twice within 15 minutes)</span>
            </li>
            <li className="flex items-center gap-2">
              <span className="w-1.5 h-1.5 rounded-full bg-blue/50"></span>
              <span><strong className="text-fg">Examples:</strong> Premium ads, system announcements, new features</span>
            </li>
          </ul>
        </div>
      </div>

      {/* Send Modal */}
      {sendModalOpen && selectedTemplate && (
        <SendNotificationModal
          template={selectedTemplate}
          onClose={handleSendComplete}
        />
      )}
    </div>
  );
}
