import { useState } from 'react';
import toast from 'react-hot-toast';

const API_URL = import.meta.env.VITE_API_URL || '';

export default function SendNotificationModal({ template, onClose }) {
  const [sendToAll, setSendToAll] = useState(false);
  const [userIds, setUserIds] = useState('');
  const [sending, setSending] = useState(false);

  const handleSend = async () => {
    // Validate input
    if (!sendToAll && !userIds.trim()) {
      toast.error('Please enter user IDs or select "Send to All Users"');
      return;
    }

    // Parse user IDs
    let userIdArray = [];
    if (!sendToAll) {
      userIdArray = userIds
        .split(/[,\s\n]+/)
        .map(id => id.trim())
        .filter(id => id && !isNaN(id))
        .map(id => parseInt(id));

      if (userIdArray.length === 0) {
        toast.error('No valid user IDs found');
        return;
      }
    }

    // Confirm action
    const confirmMessage = sendToAll
      ? 'Are you sure you want to send this notification to ALL users?'
      : `Are you sure you want to send this notification to ${userIdArray.length} user(s)?`;

    if (!confirm(confirmMessage)) {
      return;
    }

    try {
      setSending(true);
      const response = await fetch(`${API_URL}/api/service-notifications/${template.Id}/send`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          sendToAll,
          userIds: sendToAll ? [] : userIdArray
        })
      });

      const data = await response.json();

      if (data.success) {
        toast.success(`Notification queued for ${data.targetUserCount} user(s)`);
        onClose();
      } else {
        toast.error(data.error || 'Failed to send notification');
      }
    } catch (error) {
      console.error('Error sending notification:', error);
      toast.error('Error sending notification');
    } finally {
      setSending(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="p-6 border-b border-gray-200">
          <h2 className="text-2xl font-bold text-gray-900">Send Notification</h2>
          <p className="mt-1 text-sm text-gray-600">
            Send "{template.Title}" to users
          </p>
        </div>

        {/* Content */}
        <div className="p-6 space-y-6">
          {/* Template Preview */}
          <div className="bg-gray-50 border border-gray-200 rounded-lg p-4">
            <h3 className="text-sm font-medium text-gray-900 mb-2">Template Preview</h3>
            <div className="space-y-2 text-sm">
              <div>
                <span className="font-medium text-gray-700">Type:</span>{' '}
                <span className="px-2 py-0.5 bg-blue-100 text-blue-800 rounded text-xs">
                  {template.Type}
                </span>
              </div>
              <div>
                <span className="font-medium text-gray-700">Title:</span>{' '}
                <span className="text-gray-900">{template.Title}</span>
              </div>
              <div>
                <span className="font-medium text-gray-700">Message:</span>{' '}
                <div className="mt-1 text-gray-600 whitespace-pre-wrap">
                  {template.Message}
                </div>
              </div>
              <div>
                <span className="font-medium text-gray-700">Display:</span>{' '}
                <span className={`px-2 py-0.5 rounded text-xs ${
                  template.IsPopup 
                    ? 'bg-purple-100 text-purple-800' 
                    : 'bg-gray-100 text-gray-800'
                }`}>
                  {template.IsPopup ? 'Popup' : 'Message'}
                </span>
              </div>
            </div>
          </div>

          {/* Send Options */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-3">
              Send To
            </label>
            <div className="space-y-3">
              {/* Send to All */}
              <label className="flex items-start p-3 border border-gray-300 rounded-lg cursor-pointer hover:bg-gray-50">
                <input
                  type="radio"
                  checked={sendToAll}
                  onChange={() => setSendToAll(true)}
                  className="mt-1 mr-3"
                />
                <div>
                  <div className="font-medium text-gray-900">All Users</div>
                  <div className="text-sm text-gray-600">
                    Send to every registered user in the system
                  </div>
                </div>
              </label>

              {/* Send to Specific Users */}
              <label className="flex items-start p-3 border border-gray-300 rounded-lg cursor-pointer hover:bg-gray-50">
                <input
                  type="radio"
                  checked={!sendToAll}
                  onChange={() => setSendToAll(false)}
                  className="mt-1 mr-3"
                />
                <div className="flex-1">
                  <div className="font-medium text-gray-900">Specific Users</div>
                  <div className="text-sm text-gray-600 mb-2">
                    Enter user IDs (comma or newline separated)
                  </div>
                  {!sendToAll && (
                    <textarea
                      value={userIds}
                      onChange={(e) => setUserIds(e.target.value)}
                      rows={4}
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm"
                      placeholder="2010001, 2010002, 2010003&#10;or one per line"
                    />
                  )}
                </div>
              </label>
            </div>
          </div>

          {/* Warning */}
          <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
            <div className="flex">
              <div className="flex-shrink-0">
                <svg className="h-5 w-5 text-yellow-400" viewBox="0 0 20 20" fill="currentColor">
                  <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
                </svg>
              </div>
              <div className="ml-3">
                <h3 className="text-sm font-medium text-yellow-800">
                  Important
                </h3>
                <div className="mt-2 text-sm text-yellow-700">
                  <ul className="list-disc list-inside space-y-1">
                    <li>This action cannot be undone</li>
                    <li>Users will receive the notification immediately</li>
                    <li>Popup notifications require user interaction to dismiss</li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Footer */}
        <div className="p-6 border-t border-gray-200 flex justify-end space-x-3">
          <button
            onClick={onClose}
            disabled={sending}
            className="px-4 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Cancel
          </button>
          <button
            onClick={handleSend}
            disabled={sending}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed"
          >
            {sending ? 'Sending...' : '📤 Send Now'}
          </button>
        </div>
      </div>
    </div>
  );
}
