import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Search, UserX, Trash2, Shield, ChevronLeft, ChevronRight, X, Star } from 'lucide-react';

function UserManagement() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [selectedUser, setSelectedUser] = useState(null);
  const [showFreezeModal, setShowFreezeModal] = useState(false);
  const [freezeReason, setFreezeReason] = useState('Account restricted for violating Terms of Service');
  const navigate = useNavigate();

  useEffect(() => {
    loadUsers();
  }, [page, search]);

  const loadUsers = async () => {
    setLoading(true);
    try {
      const response = await fetch(`/api/users?page=${page}&limit=20&search=${search}`);
      const data = await response.json();

      if (data.success) {
        setUsers(data.users);
        setTotalPages(data.pagination.pages);
      }
    } catch (error) {
      console.error('Failed to load users:', error);
      alert('Failed to load users');
    } finally {
      setLoading(false);
    }
  };

  const handleFreeze = (user) => {
    setSelectedUser(user);
    setShowFreezeModal(true);
  };

  const confirmFreeze = async () => {
    if (!selectedUser) return;

    try {
      const response = await fetch(`/api/users/${selectedUser.UserId}/freeze`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ reason: freezeReason })
      });

      const data = await response.json();

      if (data.success) {
        alert(`User ${selectedUser.UserId} has been frozen`);
        setShowFreezeModal(false);
        loadUsers();
      } else {
        alert('Failed to freeze user: ' + data.error);
      }
    } catch (error) {
      console.error('Freeze error:', error);
      alert('Failed to freeze user');
    }
  };

  const handleUnfreeze = async (user) => {
    if (!confirm(`Unfreeze user ${user.UserId} (${user.FirstName})?`)) return;

    try {
      const response = await fetch(`/api/users/${user.UserId}/unfreeze`, {
        method: 'POST'
      });

      const data = await response.json();

      if (data.success) {
        alert(`User ${user.UserId} has been unfrozen`);
        loadUsers();
      } else {
        alert('Failed to unfreeze user: ' + data.error);
      }
    } catch (error) {
      console.error('Unfreeze error:', error);
      alert('Failed to unfreeze user');
    }
  };

  const handleDelete = async (user) => {
    if (!confirm(`Delete user ${user.UserId} (${user.FirstName})? This will mark account as deleted.`)) return;

    try {
      const response = await fetch(`/api/users/${user.UserId}`, {
        method: 'DELETE'
      });

      const data = await response.json();

      if (data.success) {
        alert(`User ${user.UserId} has been deleted`);
        loadUsers();
      } else {
        alert('Failed to delete user: ' + data.error);
      }
    } catch (error) {
      console.error('Delete error:', error);
      alert('Failed to delete user');
    }
  };

  const handleRemovePremium = async (user) => {
    if (!confirm(`Remove Premium from ${user.FirstName} (${user.UserId})?`)) return;

    try {
      const response = await fetch(`/api/users/${user.UserId}/premium`, {
        method: 'DELETE'
      });

      const data = await response.json();

      if (data.success) {
        alert(`Premium removed from user ${user.UserId}`);
        loadUsers();
      } else {
        alert('Failed to remove premium: ' + data.error);
      }
    } catch (error) {
      console.error('Remove premium error:', error);
      alert('Failed to remove premium');
    }
  };

  const handleGivePremium = async (user) => {
    if (!confirm(`Give Premium to ${user.FirstName} (${user.UserId})?`)) return;

    try {
      const response = await fetch(`/api/users/${user.UserId}/premium`, {
        method: 'POST'
      });

      const data = await response.json();

      if (data.success) {
        alert(`Premium given to user ${user.UserId}`);
        loadUsers();
      } else {
        alert('Failed to give premium: ' + data.error);
      }
    } catch (error) {
      console.error('Give premium error:', error);
      alert('Failed to give premium');
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-black font-heading text-fg">User Management</h1>
          <p className="text-fg-muted font-medium mt-1">Manage user accounts and permissions</p>
        </div>
      </div>

      {/* Search Bar */}
      <div className="card p-4">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-fg-muted" />
          <input
            type="text"
            placeholder="Search by name, username, or phone..."
            value={search}
            onChange={(e) => {
              setSearch(e.target.value);
              setPage(1);
            }}
            className="input pl-10 bg-bg-app border-border focus:border-accent w-full"
          />
        </div>
      </div>

      {loading ? (
        <div className="flex items-center justify-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-accent"></div>
        </div>
      ) : (
        <>
          {/* Users Table */}
          <div className="card overflow-hidden p-0 border border-border">
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="bg-bg-panel border-b border-border">
                  <tr>
                    <th className="px-6 py-4 text-left text-xs font-bold text-fg-muted uppercase tracking-wider">User ID</th>
                    <th className="px-6 py-4 text-left text-xs font-bold text-fg-muted uppercase tracking-wider">Name</th>
                    <th className="px-6 py-4 text-left text-xs font-bold text-fg-muted uppercase tracking-wider">Username</th>
                    <th className="px-6 py-4 text-left text-xs font-bold text-fg-muted uppercase tracking-wider">Phone</th>
                    <th className="px-6 py-4 text-left text-xs font-bold text-fg-muted uppercase tracking-wider">Status</th>
                    <th className="px-6 py-4 text-left text-xs font-bold text-fg-muted uppercase tracking-wider">Premium</th>
                    <th className="px-6 py-4 text-right text-xs font-bold text-fg-muted uppercase tracking-wider">Actions</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-border bg-bg-app">
                  {users.map(user => (
                    <tr key={user.UserId} className="hover:bg-bg-panel/50 transition-colors">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className="text-fg font-mono text-sm font-medium">{user.UserId}</span>
                      </td>
                      <td className="px-6 py-4">
                        <div className="flex items-center gap-2">
                          <span className="text-fg font-bold">{user.FirstName} {user.LastName}</span>
                          {user.Bot && <span className="px-2 py-0.5 bg-blue-500/10 text-blue-400 border border-blue-500/20 text-[10px] rounded font-bold uppercase tracking-wider">BOT</span>}
                          {user.Verified && <span className="text-accent" title="Verified">✓</span>}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className="text-accent font-medium text-sm">@{user.UserName || <span className="text-fg-muted font-normal">N/A</span>}</span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className="text-fg-muted font-mono text-sm">{user.PhoneNumber}</span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        {user.Restricted ? (
                          <span className="px-2 py-0.5 bg-red-500/10 text-red-400 border border-red-500/20 text-[10px] rounded-full font-bold uppercase tracking-wider" title={user.RestrictionReason}>
                            🔒 FROZEN
                          </span>
                        ) : user.IsDeleted ? (
                          <span className="px-2 py-0.5 bg-bg-panel text-fg-muted border border-border text-[10px] rounded-full font-bold uppercase tracking-wider">
                            🗑️ DELETED
                          </span>
                        ) : (
                          <span className="px-2 py-0.5 bg-emerald-500/10 text-emerald-400 border border-emerald-500/20 text-[10px] rounded-full font-bold uppercase tracking-wider">
                            ✅ ACTIVE
                          </span>
                        )}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        {user.Premium ? (
                          <span className="px-2 py-0.5 bg-yellow-500/10 text-yellow-400 border border-yellow-500/20 text-[10px] rounded-full font-bold uppercase tracking-wider">
                            ⭐ Premium
                          </span>
                        ) : (
                          <span className="px-2 py-0.5 bg-bg-panel text-fg-muted border border-border text-[10px] rounded-full font-bold uppercase tracking-wider">
                            Free
                          </span>
                        )}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-right">
                        <div className="flex items-center justify-end gap-2">
                          {user.Premium ? (
                            <button
                              onClick={() => handleRemovePremium(user)}
                              className="p-1.5 bg-yellow-500/10 hover:bg-yellow-500/20 text-yellow-400 rounded-lg transition-colors"
                              title="Remove Premium"
                            >
                              <div className="w-4 h-4 flex items-center justify-center font-bold text-xs">−</div>
                            </button>
                          ) : (
                            <button
                              onClick={() => handleGivePremium(user)}
                              className="p-1.5 bg-yellow-500/10 hover:bg-yellow-500/20 text-yellow-400 rounded-lg transition-colors"
                              title="Give Premium"
                            >
                              <div className="w-4 h-4 flex items-center justify-center font-bold text-xs">+</div>
                            </button>
                          )}
                          {user.Restricted ? (
                            <button
                              onClick={() => handleUnfreeze(user)}
                              className="px-3 py-1.5 bg-emerald-500/10 hover:bg-emerald-500/20 text-emerald-400 rounded-lg text-xs font-bold uppercase tracking-wider transition-colors flex items-center gap-1 border border-emerald-500/10"
                            >
                              <Shield className="w-3.5 h-3.5" />
                              Unfreeze
                            </button>
                          ) : (
                            <button
                              onClick={() => handleFreeze(user)}
                              className="px-3 py-1.5 bg-orange-500/10 hover:bg-orange-500/20 text-orange-400 rounded-lg text-xs font-bold uppercase tracking-wider transition-colors flex items-center gap-1 border border-orange-500/10 disabled:opacity-50 disabled:cursor-not-allowed"
                              disabled={user.IsDeleted}
                            >
                              <UserX className="w-3.5 h-3.5" />
                              Freeze
                            </button>
                          )}
                          <button
                            onClick={() => handleDelete(user)}
                            className="p-1.5 text-fg-muted hover:text-red-500 hover:bg-red-500/10 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                            disabled={user.IsDeleted}
                            title="Delete User"
                          >
                            <Trash2 className="w-4 h-4" />
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>

          {/* Pagination */}
          <div className="flex items-center justify-between bg-bg-panel border border-border rounded-lg px-6 py-4">
            <button
              onClick={() => setPage(p => Math.max(1, p - 1))}
              disabled={page === 1}
              className="px-4 py-2 border border-border bg-bg-app hover:bg-bg-panel text-fg rounded-lg flex items-center gap-2 transition-colors disabled:opacity-50 disabled:cursor-not-allowed text-sm font-medium"
            >
              <ChevronLeft className="w-4 h-4" />
              Previous
            </button>
            <span className="text-fg font-medium">
              Page <span className="text-accent font-bold">{page}</span> of <span className="text-fg-muted">{totalPages}</span>
            </span>
            <button
              onClick={() => setPage(p => Math.min(totalPages, p + 1))}
              disabled={page === totalPages}
              className="px-4 py-2 border border-border bg-bg-app hover:bg-bg-panel text-fg rounded-lg flex items-center gap-2 transition-colors disabled:opacity-50 disabled:cursor-not-allowed text-sm font-medium"
            >
              Next
              <ChevronRight className="w-4 h-4" />
            </button>
          </div>
        </>
      )}

      {/* Freeze Modal */}
      {showFreezeModal && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 animate-fade-in" onClick={() => setShowFreezeModal(false)}>
          <div className="card w-full max-w-md mx-4 shadow-2xl shadow-black/50 border-border scale-100 animate-scale-in" onClick={(e) => e.stopPropagation()}>
            <div className="flex items-center justify-between mb-6 pb-4 border-b border-border">
              <h2 className="text-xl font-black font-heading text-fg flex items-center gap-2">
                <UserX className="w-6 h-6 text-orange-400" />
                Freeze Account
              </h2>
              <button onClick={() => setShowFreezeModal(false)} className="text-fg-muted hover:text-fg transition-colors">
                <X className="w-5 h-5" />
              </button>
            </div>

            <div className="bg-bg-app border border-border rounded-xl p-4 mb-6">
              <p className="text-fg-muted text-xs font-bold uppercase tracking-wider mb-1">User</p>
              <p className="text-fg font-bold text-lg">{selectedUser?.FirstName} {selectedUser?.LastName}</p>
              <p className="text-fg-muted text-sm mt-1">ID: <span className="text-accent font-mono">{selectedUser?.UserId}</span></p>
            </div>

            <div className="mb-6">
              <label className="block text-fg-muted text-xs font-bold uppercase tracking-wider mb-2">Restriction Reason</label>
              <textarea
                value={freezeReason}
                onChange={(e) => setFreezeReason(e.target.value)}
                rows={4}
                placeholder="Enter reason for account restriction..."
                className="input w-full resize-none h-32"
              />
            </div>

            <div className="flex gap-3">
              <button
                onClick={confirmFreeze}
                className="flex-1 btn bg-orange-500 hover:bg-orange-600 text-white shadow-[0_0_15px_rgba(249,115,22,0.3)] border-none"
              >
                Freeze Account
              </button>
              <button
                onClick={() => setShowFreezeModal(false)}
                className="flex-1 btn btn-secondary"
              >
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default UserManagement;









