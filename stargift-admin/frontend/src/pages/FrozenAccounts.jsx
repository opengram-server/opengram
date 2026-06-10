import { useState, useEffect } from 'react';
import { toast } from 'react-hot-toast';
import { Search, Info, Shield, Filter, User, Calendar, CheckCircle2, XCircle, AlertTriangle, Snowflake } from 'lucide-react';

const API_URL = import.meta.env.VITE_API_URL || '/api';

const FrozenAccounts = () => {
  const [frozenAccounts, setFrozenAccounts] = useState([]);
  const [appeals, setAppeals] = useState([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState('accounts'); // 'accounts' or 'appeals'
  const [showFreezeModal, setShowFreezeModal] = useState(false);
  const [freezeForm, setFreezeForm] = useState({
    userId: '',
    reason: 4,
    durationDays: 7,
    note: ''
  });

  const reasons = {
    1: 'Spam',
    2: 'Malicious Links',
    3: 'Mass Reports',
    4: 'ToS Violation',
    99: 'Other'
  };

  const statuses = {
    1: { label: 'Active', color: 'red' },
    2: { label: 'Appeal Pending', color: 'yellow' },
    3: { label: 'Approved', color: 'green' },
    4: { label: 'Rejected', color: 'gray' },
    5: { label: 'Expired', color: 'gray' }
  };

  useEffect(() => {
    fetchData();
  }, [activeTab]);

  const fetchData = async () => {
    setLoading(true);
    try {
      if (activeTab === 'accounts') {
        // In a real scenario, this endpoint might need to be adjusted or proxied
        const response = await fetch(`${API_URL}/frozen-accounts`);
        const data = await response.json();
        if (data.success) {
          setFrozenAccounts(data.data);
        }
      } else {
        const response = await fetch(`${API_URL}/frozen-accounts/appeals`);
        const data = await response.json();
        if (data.success) {
          setAppeals(data.data);
        }
      }
    } catch (error) {
      toast.error('Error loading data');
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const handleFreezeAccount = async (e) => {
    e.preventDefault();

    try {
      const response = await fetch(`${API_URL}/frozen-accounts/freeze`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(freezeForm)
      });

      const data = await response.json();

      if (data.success) {
        toast.success('Account frozen successfully');
        setShowFreezeModal(false);
        setFreezeForm({ userId: '', reason: 4, durationDays: 7, note: '' });
        fetchData();
      } else {
        toast.error(data.error || 'Failed to freeze account');
      }
    } catch (error) {
      toast.error('Error freezing account');
      console.error(error);
    }
  };

  const handleUnfreeze = async (userId) => {
    if (!confirm(`Unfreeze account for user ${userId}?`)) return;

    try {
      const response = await fetch(`${API_URL}/frozen-accounts/unfreeze`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ userId })
      });

      const data = await response.json();

      if (data.success) {
        toast.success('Account unfrozen successfully');
        fetchData();
      } else {
        toast.error(data.error || 'Failed to unfreeze account');
      }
    } catch (error) {
      toast.error('Error unfreezing account');
      console.error(error);
    }
  };

  const handleReviewAppeal = async (appealId, status) => {
    const statusText = status === 2 ? 'approve' : 'reject';
    if (!confirm(`${statusText.charAt(0).toUpperCase() + statusText.slice(1)} this appeal?`)) return;

    try {
      const response = await fetch(`${API_URL}/frozen-accounts/appeals/${appealId}/review`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ status, moderatorUserId: 777000 })
      });

      const data = await response.json();

      if (data.success) {
        toast.success(data.message);
        fetchData();
      } else {
        toast.error(data.error || 'Failed to review appeal');
      }
    } catch (error) {
      toast.error('Error reviewing appeal');
      console.error(error);
    }
  };

  const formatDate = (timestamp) => {
    if (!timestamp) return 'N/A';
    const date = new Date(timestamp * 1000);
    return date.toLocaleString();
  };

  return (
    <div className="space-y-6 animate-fade-in">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-heading font-bold text-fg flex items-center gap-3">
            <div className="p-2.5 bg-blue/10 rounded-xl rounded-tr-none">
              <Snowflake className="w-8 h-8 text-blue" />
            </div>
            Frozen Accounts Management
          </h1>
          <p className="text-fg-muted mt-2">Manage frozen accounts and review appeals</p>
        </div>
      </div>

      {/* Tabs */}
      <div className="flex border-b border-border space-x-6">
        <button
          onClick={() => setActiveTab('accounts')}
          className={`pb-3 border-b-2 font-medium text-sm transition-colors flex items-center gap-2 ${activeTab === 'accounts'
              ? 'border-blue text-blue'
              : 'border-transparent text-fg-muted hover:text-fg'
            }`}
        >
          <Shield className="w-4 h-4" />
          Frozen Accounts ({frozenAccounts.length})
        </button>
        <button
          onClick={() => setActiveTab('appeals')}
          className={`pb-3 border-b-2 font-medium text-sm transition-colors flex items-center gap-2 ${activeTab === 'appeals'
              ? 'border-blue text-blue'
              : 'border-transparent text-fg-muted hover:text-fg'
            }`}
        >
          <Info className="w-4 h-4" />
          Appeals ({appeals.length})
        </button>
      </div>

      {/* Freeze Account Button */}
      {activeTab === 'accounts' && (
        <div className="flex justify-end">
          <button
            onClick={() => setShowFreezeModal(true)}
            className="btn bg-red text-white hover:bg-red/90 flex items-center gap-2"
          >
            <Snowflake className="w-4 h-4" />
            Freeze Account
          </button>
        </div>
      )}

      {/* Content */}
      {loading ? (
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
        </div>
      ) : (
        <>
          {activeTab === 'accounts' ? (
            <div className="card border-border overflow-hidden">
              <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-border">
                  <thead className="bg-muted/50">
                    <tr>
                      <th className="px-6 py-4 text-left text-xs font-semibold text-fg-muted uppercase tracking-wider">User ID</th>
                      <th className="px-6 py-4 text-left text-xs font-semibold text-fg-muted uppercase tracking-wider">Reason</th>
                      <th className="px-6 py-4 text-left text-xs font-semibold text-fg-muted uppercase tracking-wider">Status</th>
                      <th className="px-6 py-4 text-left text-xs font-semibold text-fg-muted uppercase tracking-wider">Frozen Since</th>
                      <th className="px-6 py-4 text-left text-xs font-semibold text-fg-muted uppercase tracking-wider">Until</th>
                      <th className="px-6 py-4 text-left text-xs font-semibold text-fg-muted uppercase tracking-wider">Actions</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-border bg-card">
                    {frozenAccounts.length === 0 ? (
                      <tr>
                        <td colSpan="6" className="px-6 py-12 text-center text-fg-muted">
                          <div className="flex flex-col items-center gap-3">
                            <div className="w-12 h-12 bg-muted rounded-full flex items-center justify-center">
                              <Snowflake className="w-6 h-6 text-fg-muted" />
                            </div>
                            <p>No frozen accounts</p>
                          </div>
                        </td>
                      </tr>
                    ) : (
                      frozenAccounts.map((account) => (
                        <tr key={account._id} className="hover:bg-muted/30 transition-colors">
                          <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-fg">
                            {account.UserId}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-fg-muted">
                            {reasons[account.Reason] || 'Unknown'}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <span className={`px-2.5 py-1 inline-flex text-xs leading-5 font-semibold rounded-full border ${account.Status === 1 ? 'bg-red/10 text-red border-red/20' :
                                account.Status === 2 ? 'bg-yellow/10 text-yellow border-yellow/20' :
                                  account.Status === 3 ? 'bg-success/10 text-success border-success/20' :
                                    'bg-muted text-fg-muted border-border'
                              }`}>
                              {statuses[account.Status]?.label || 'Unknown'}
                            </span>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-fg-muted font-mono">
                            {formatDate(account.FreezeSinceDate)}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-fg-muted font-mono">
                            {formatDate(account.FreezeUntilDate)}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                            {account.Status === 1 ? (
                              <button
                                onClick={() => handleUnfreeze(account.UserId)}
                                className="text-success hover:text-success/80 flex items-center gap-1 transition-colors"
                              >
                                <CheckCircle2 className="w-4 h-4" />
                                Unfreeze
                              </button>
                            ) : (
                              <span className="text-fg-muted opacity-50">
                                {account.Status === 3 ? 'Already unfrozen' : '-'}
                              </span>
                            )}
                          </td>
                        </tr>
                      ))
                    )}
                  </tbody>
                </table>
              </div>
            </div>
          ) : (
            <div className="grid gap-6">
              {appeals.length === 0 ? (
                <div className="card p-12 text-center text-fg-muted border-dashed">
                  <div className="w-16 h-16 bg-muted rounded-full flex items-center justify-center mx-auto mb-4">
                    <Info className="w-8 h-8 text-fg-muted" />
                  </div>
                  No appeals yet
                </div>
              ) : (
                appeals.map((appeal) => (
                  <div key={appeal._id} className="card p-6 border-border hover:border-purple/50 transition-colors">
                    <div className="flex flex-col md:flex-row md:justify-between md:items-start mb-4 gap-4">
                      <div>
                        <h3 className="text-lg font-bold text-fg flex items-center gap-2">
                          Appeal #{appeal.AppealId}
                        </h3>
                        <div className="flex flex-wrap gap-x-4 gap-y-1 mt-1 text-sm text-fg-muted">
                          <span className="flex items-center gap-1">
                            <User className="w-3.5 h-3.5" />
                            ID: {appeal.UserId}
                          </span>
                          <span className="flex items-center gap-1">
                            <span className="w-1 h-1 rounded-full bg-fg-muted"></span>
                            Name: {appeal.UserName}
                          </span>
                          <span className="flex items-center gap-1">
                            <Calendar className="w-3.5 h-3.5" />
                            Submitted: {new Date(appeal.SubmittedDate).toLocaleString()}
                          </span>
                        </div>
                      </div>
                      <span className={`px-3 py-1 text-xs font-semibold rounded-full border w-fit ${appeal.Status === 1 ? 'bg-yellow/10 text-yellow border-yellow/20' :
                          appeal.Status === 2 ? 'bg-success/10 text-success border-success/20' :
                            'bg-red/10 text-red border-red/20'
                        }`}>
                        {appeal.Status === 1 ? '⏳ Pending' : appeal.Status === 2 ? '✅ Approved' : '❌ Rejected'}
                      </span>
                    </div>

                    <div className="mb-4 bg-muted/30 p-4 rounded-lg border border-border">
                      <p className="text-xs font-bold text-fg-muted uppercase tracking-wider mb-2">Appeal Text</p>
                      <p className="text-sm text-fg">{appeal.AppealText}</p>
                    </div>

                    {appeal.Status === 1 && (
                      <div className="flex gap-3">
                        <button
                          onClick={() => handleReviewAppeal(appeal.AppealId, 2)}
                          className="btn bg-success/10 text-success border border-success/20 hover:bg-success/20 flex items-center gap-2"
                        >
                          <CheckCircle2 className="w-4 h-4" />
                          Approve
                        </button>
                        <button
                          onClick={() => handleReviewAppeal(appeal.AppealId, 3)}
                          className="btn bg-red/10 text-red border border-red/20 hover:bg-red/20 flex items-center gap-2"
                        >
                          <XCircle className="w-4 h-4" />
                          Reject
                        </button>
                      </div>
                    )}

                    {appeal.ReviewNote && (
                      <div className="mt-4 pt-4 border-t border-border">
                        <p className="text-xs font-bold text-fg-muted uppercase tracking-wider mb-1">Review Note</p>
                        <p className="text-sm text-fg-muted italic">{appeal.ReviewNote}</p>
                      </div>
                    )}
                  </div>
                ))
              )}
            </div>
          )}
        </>
      )}

      {/* Freeze Modal */}
      {showFreezeModal && (
        <div className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50 animate-fade-in">
          <div className="bg-card w-full max-w-md rounded-2xl border border-border shadow-2xl p-6 m-4 animate-scale-in">
            <div className="flex justify-between items-center mb-6">
              <h2 className="text-xl font-heading font-bold text-fg flex items-center gap-2">
                <Snowflake className="w-5 h-5 text-red" />
                Freeze Account
              </h2>
              <button onClick={() => setShowFreezeModal(false)} className="text-fg-muted hover:text-fg">
                <XCircle className="w-6 h-6" />
              </button>
            </div>

            <form onSubmit={handleFreezeAccount} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-fg mb-1.5">
                  User ID <span className="text-red">*</span>
                </label>
                <input
                  type="number"
                  value={freezeForm.userId}
                  onChange={(e) => setFreezeForm({ ...freezeForm, userId: e.target.value })}
                  className="input w-full"
                  placeholder="Enter User ID"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-fg mb-1.5">
                  Reason
                </label>
                <select
                  value={freezeForm.reason}
                  onChange={(e) => setFreezeForm({ ...freezeForm, reason: parseInt(e.target.value) })}
                  className="input w-full"
                >
                  {Object.entries(reasons).map(([value, label]) => (
                    <option key={value} value={value}>{label}</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-fg mb-1.5">
                  Duration (days)
                </label>
                <input
                  type="number"
                  value={freezeForm.durationDays}
                  onChange={(e) => setFreezeForm({ ...freezeForm, durationDays: parseInt(e.target.value) })}
                  className="input w-full"
                  min="1"
                  max="365"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-fg mb-1.5">
                  Note (optional)
                </label>
                <textarea
                  value={freezeForm.note}
                  onChange={(e) => setFreezeForm({ ...freezeForm, note: e.target.value })}
                  className="input w-full resize-none"
                  rows="3"
                  placeholder="Add internal note..."
                />
              </div>

              <div className="flex gap-3 pt-2">
                <button
                  type="button"
                  onClick={() => setShowFreezeModal(false)}
                  className="btn btn-secondary flex-1"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="btn bg-red text-white hover:bg-red/90 flex-1"
                >
                  Confirm Freeze
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default FrozenAccounts;
