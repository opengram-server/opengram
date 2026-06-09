import { useState, useEffect } from 'react';
import { Search, Shield, Upload, CheckCircle, XCircle, Plus, Edit2, Trash2 } from 'lucide-react';

const API_URL = import.meta.env.VITE_API_URL || '';

export default function Verification() {
  const [activeTab, setActiveTab] = useState('assign'); // assign or manage
  const [verifiers, setVerifiers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [stats, setStats] = useState(null);
  
  // Assign verification state
  const [userId, setUserId] = useState('');
  const [selectedVerifier, setSelectedVerifier] = useState('');
  const [customDescription, setCustomDescription] = useState('');
  const [currentVerification, setCurrentVerification] = useState(null);
  
  // Create verifier state
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [botUserId, setBotUserId] = useState('');
  const [iconFile, setIconFile] = useState(null);
  const [iconDocumentId, setIconDocumentId] = useState('');
  const [companyName, setCompanyName] = useState('');
  const [canModifyDesc, setCanModifyDesc] = useState(false);
  const [uploadProgress, setUploadProgress] = useState(0);

  useEffect(() => {
    loadVerifiers();
    loadStats();
  }, []);

  const loadVerifiers = async () => {
    try {
      const response = await fetch(`${API_URL}/api/verification/bots`);
      const data = await response.json();
      if (data.success) {
        setVerifiers(data.data);
      }
    } catch (error) {
      console.error('Error loading verifiers:', error);
    }
  };

  const loadStats = async () => {
    try {
      const response = await fetch(`${API_URL}/api/verification/stats`);
      const data = await response.json();
      if (data.success) {
        setStats(data.data);
      }
    } catch (error) {
      console.error('Error loading stats:', error);
    }
  };

  const checkUserVerification = async (uid) => {
    if (!uid) return;
    
    try {
      const response = await fetch(`${API_URL}/api/verification/users/${uid}`);
      const data = await response.json();
      if (data.success) {
        setCurrentVerification(data.data);
      }
    } catch (error) {
      console.error('Error checking verification:', error);
    }
  };

  const handleFileUpload = async (file) => {
    if (!file) return;

    const formData = new FormData();
    formData.append('icon', file);

    try {
      setUploadProgress(10);
      const response = await fetch(`${API_URL}/api/verification/upload-icon`, {
        method: 'POST',
        body: formData
      });
      
      setUploadProgress(90);
      const data = await response.json();
      
      if (data.success) {
        setIconDocumentId(data.data.documentId.toString());
        setUploadProgress(100);
        alert(`✅ Файл загружен! Document ID: ${data.data.documentId}`);
        setTimeout(() => setUploadProgress(0), 2000);
      } else {
        alert('❌ Ошибка загрузки: ' + data.error);
        setUploadProgress(0);
      }
    } catch (error) {
      console.error('Upload error:', error);
      alert('❌ Ошибка загрузки файла');
      setUploadProgress(0);
    }
  };

  const createVerifier = async (e) => {
    e.preventDefault();
    
    if (!botUserId || !iconDocumentId || !companyName) {
      alert('Заполните все обязательные поля');
      return;
    }

    setLoading(true);
    try {
      const response = await fetch(`${API_URL}/api/verification/bots`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          botUserId: parseInt(botUserId),
          iconEmojiId: parseInt(iconDocumentId),
          companyName,
          canModifyCustomDescription: canModifyDesc
        })
      });

      const data = await response.json();
      
      if (data.success) {
        alert('✅ Верификатор создан!');
        setShowCreateForm(false);
        setBotUserId('');
        setIconDocumentId('');
        setCompanyName('');
        setCanModifyDesc(false);
        setIconFile(null);
        loadVerifiers();
        loadStats();
      } else {
        alert('❌ Ошибка: ' + data.error);
      }
    } catch (error) {
      console.error('Error creating verifier:', error);
      alert('❌ Ошибка создания верификатора');
    } finally {
      setLoading(false);
    }
  };

  const assignVerification = async (e) => {
    e.preventDefault();
    
    if (!userId || !selectedVerifier) {
      alert('Заполните User ID и выберите верификатора');
      return;
    }

    setLoading(true);
    try {
      const response = await fetch(`${API_URL}/api/verification/users/${userId}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          botUserId: parseInt(selectedVerifier),
          customDescription: customDescription || null
        })
      });

      const data = await response.json();
      
      if (data.success) {
        alert('✅ Верификация выдана!');
        checkUserVerification(userId);
        setCustomDescription('');
        loadStats();
      } else {
        alert('❌ Ошибка: ' + data.error);
      }
    } catch (error) {
      console.error('Error assigning verification:', error);
      alert('❌ Ошибка выдачи верификации');
    } finally {
      setLoading(false);
    }
  };

  const removeVerification = async () => {
    if (!userId || !confirm('Удалить верификацию?')) return;

    setLoading(true);
    try {
      const response = await fetch(`${API_URL}/api/verification/users/${userId}`, {
        method: 'DELETE'
      });

      const data = await response.json();
      
      if (data.success) {
        alert('✅ Верификация удалена!');
        setCurrentVerification(null);
        loadStats();
      } else {
        alert('❌ Ошибка: ' + data.error);
      }
    } catch (error) {
      console.error('Error removing verification:', error);
      alert('❌ Ошибка удаления верификации');
    } finally {
      setLoading(false);
    }
  };

  const deleteVerifier = async (botUserId) => {
    if (!confirm(`Деактивировать верификатора ${botUserId}?`)) return;

    try {
      const response = await fetch(`${API_URL}/api/verification/bots/${botUserId}`, {
        method: 'DELETE'
      });

      const data = await response.json();
      
      if (data.success) {
        alert('✅ Верификатор деактивирован!');
        loadVerifiers();
        loadStats();
      } else {
        alert('❌ Ошибка: ' + data.error);
      }
    } catch (error) {
      console.error('Error deleting verifier:', error);
      alert('❌ Ошибка деактивации');
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-white flex items-center gap-3">
            <Shield className="w-8 h-8 text-blue-400" />
            Third-Party Verification
          </h1>
          <p className="text-[#8b98a5] mt-1">Управление сторонней верификацией пользователей</p>
        </div>
      </div>

      {/* Stats */}
      {stats && (
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="card rounded-lg shadow p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-[#8b98a5] text-sm">Всего верификаторов</p>
                <p className="text-2xl font-bold text-white">{stats.totalVerifiers}</p>
              </div>
              <Shield className="w-8 h-8 text-blue-500" />
            </div>
          </div>
          <div className="card rounded-lg shadow p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-[#8b98a5] text-sm">Активных</p>
                <p className="text-2xl font-bold text-green-600">{stats.activeVerifiers}</p>
              </div>
              <CheckCircle className="w-8 h-8 text-green-500" />
            </div>
          </div>
          <div className="card rounded-lg shadow p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-[#8b98a5] text-sm">Верифицировано пользователей</p>
                <p className="text-2xl font-bold text-purple-600">{stats.userVerifications}</p>
              </div>
              <CheckCircle className="w-8 h-8 text-purple-500" />
            </div>
          </div>
          <div className="card rounded-lg shadow p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-[#8b98a5] text-sm">Верифицировано каналов</p>
                <p className="text-2xl font-bold text-indigo-600">{stats.channelVerifications}</p>
              </div>
              <CheckCircle className="w-8 h-8 text-indigo-500" />
            </div>
          </div>
        </div>
      )}

      {/* Tabs */}
      <div className="card rounded-lg shadow">
        <div className="border-b border-[#2b5278]">
          <nav className="flex -mb-px">
            <button
              onClick={() => setActiveTab('assign')}
              className={`px-6 py-4 text-sm font-medium ${
                activeTab === 'assign'
                  ? 'border-b-2 border-blue-500 text-blue-400'
                  : 'text-[#8b98a5] hover:text-white'
              }`}
            >
              Выдать верификацию
            </button>
            <button
              onClick={() => setActiveTab('manage')}
              className={`px-6 py-4 text-sm font-medium ${
                activeTab === 'manage'
                  ? 'border-b-2 border-blue-500 text-blue-400'
                  : 'text-[#8b98a5] hover:text-white'
              }`}
            >
              Управление верификаторами
            </button>
          </nav>
        </div>

        <div className="p-6">
          {activeTab === 'assign' ? (
            <div className="space-y-6">
              {/* Check user form */}
              <div>
                <label className="block text-sm font-medium text-white mb-2">
                  User ID <span className="text-red-500">*</span>
                </label>
                <div className="flex gap-2">
                  <input
                    type="number"
                    value={userId}
                    onChange={(e) => setUserId(e.target.value)}
                    onBlur={(e) => checkUserVerification(e.target.value)}
                    className="input flex-1"
                    placeholder="Введите User ID"
                  />
                  <button
                    onClick={() => checkUserVerification(userId)}
                    className="px-4 py-2 bg-[#5288c1] text-white rounded-lg hover:bg-[#3d5a7a] flex items-center gap-2"
                  >
                    <Search className="w-4 h-4" />
                    Проверить
                  </button>
                </div>
              </div>

              {/* Current verification status */}
              {userId && (
                <div className={`p-4 rounded-lg ${currentVerification ? 'bg-green-500/20 border border-green-500/30' : 'bg-[#0e1621] border border-[#2b5278]'}`}>
                  {currentVerification ? (
                    <div className="space-y-2">
                      <div className="flex items-center gap-2 text-green-400 font-medium">
                        <CheckCircle className="w-5 h-5" />
                        Пользователь уже верифицирован
                      </div>
                      <div className="text-sm text-[#8b98a5] space-y-1">
                        <p><strong>Bot ID:</strong> {currentVerification.BotVerifierId}</p>
                        <p><strong>Icon ID:</strong> {currentVerification.IconEmojiId}</p>
                        <p><strong>Компания (дефолт):</strong> {currentVerification.Description}</p>
                        {currentVerification.CustomDescription && currentVerification.CustomDescription.trim() !== '' ? (
                          <p><strong>Кастомное описание:</strong> <span className="text-green-400 font-medium">{currentVerification.CustomDescription}</span></p>
                        ) : (
                          <p className="text-[#8b98a5] text-xs">Кастомное описание не задано</p>
                        )}
                        <p className="pt-2 border-t border-[#2b5278]">
                          <strong>Клиент видит:</strong> <span className="text-purple-400 font-semibold">
                            {currentVerification.CustomDescription && currentVerification.CustomDescription.trim() !== '' 
                              ? currentVerification.CustomDescription 
                              : currentVerification.Description}
                          </span>
                        </p>
                        <p><strong>Верифицирован:</strong> {new Date(currentVerification.VerifiedAt).toLocaleString('ru-RU')}</p>
                      </div>
                      <button
                        onClick={removeVerification}
                        disabled={loading}
                        className="mt-2 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 flex items-center gap-2"
                      >
                        <Trash2 className="w-4 h-4" />
                        Удалить верификацию
                      </button>
                    </div>
                  ) : (
                    <div className="flex items-center gap-2 text-[#8b98a5]">
                      <XCircle className="w-5 h-5" />
                      Пользователь не верифицирован
                    </div>
                  )}
                </div>
              )}

              {/* Assign verification form */}
              {userId && !currentVerification && (
                <form onSubmit={assignVerification} className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-white mb-2">
                      Выберите верификатора <span className="text-red-500">*</span>
                    </label>
                    <select
                      value={selectedVerifier}
                      onChange={(e) => setSelectedVerifier(e.target.value)}
                      className="input focus:ring-2 focus:ring-blue-500"
                      required
                    >
                      <option value="">-- Выберите верификатора --</option>
                      {verifiers.map((v) => (
                        <option key={v.BotUserId} value={v.BotUserId}>
                          {v.CompanyName} (Bot ID: {v.BotUserId}, Icon: {v.IconEmojiId})
                        </option>
                      ))}
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-white mb-2">
                      Кастомное описание (опционально)
                    </label>
                    <textarea
                      value={customDescription}
                      onChange={(e) => setCustomDescription(e.target.value)}
                      className="input focus:ring-2 focus:ring-blue-500"
                      rows={3}
                      placeholder="Оставьте пустым для использования названия компании"
                      disabled={!selectedVerifier || !verifiers.find(v => v.BotUserId === parseInt(selectedVerifier))?.CanModifyCustomDescription}
                    />
                    {selectedVerifier && !verifiers.find(v => v.BotUserId === parseInt(selectedVerifier))?.CanModifyCustomDescription && (
                      <p className="text-sm text-red-500 mt-1">⚠️ Этот верификатор не разрешает кастомные описания</p>
                    )}
                    {selectedVerifier && verifiers.find(v => v.BotUserId === parseInt(selectedVerifier))?.CanModifyCustomDescription && (
                      <div className="mt-2 p-3 bg-blue-50 border border-blue-500/30 rounded-lg">
                        <p className="text-sm text-blue-300">
                          <strong>Клиент увидит:</strong> {customDescription && customDescription.trim() !== '' 
                            ? <span className="text-purple-400 font-semibold">{customDescription}</span>
                            : <span className="text-white">{verifiers.find(v => v.BotUserId === parseInt(selectedVerifier))?.CompanyName}</span>
                          }
                        </p>
                      </div>
                    )}
                  </div>

                  <button
                    type="submit"
                    disabled={loading}
                    className="w-full px-6 py-3 bg-green-500 text-white rounded-lg hover:bg-green-600 disabled:bg-[#2b5278]/50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
                  >
                    <CheckCircle className="w-5 h-5" />
                    {loading ? 'Выдаю...' : 'Выдать верификацию'}
                  </button>
                </form>
              )}
            </div>
          ) : (
            <div className="space-y-6">
              {/* Create verifier button */}
              {!showCreateForm && (
                <button
                  onClick={() => setShowCreateForm(true)}
                  className="px-6 py-3 bg-[#5288c1] text-white rounded-lg hover:bg-[#3d5a7a] flex items-center gap-2"
                >
                  <Plus className="w-5 h-5" />
                  Создать верификатора
                </button>
              )}

              {/* Create verifier form */}
              {showCreateForm && (
                <div className="bg-blue-500/20 border border-blue-500/30 rounded-lg p-6">
                  <h3 className="text-lg font-semibold text-white mb-4">Создание нового верификатора</h3>
                  <form onSubmit={createVerifier} className="space-y-4">
                    <div>
                      <label className="block text-sm font-medium text-white mb-2">
                        Bot User ID <span className="text-red-500">*</span>
                      </label>
                      <input
                        type="number"
                        value={botUserId}
                        onChange={(e) => setBotUserId(e.target.value)}
                        className="input focus:ring-2 focus:ring-blue-500"
                        placeholder="ID бота-верификатора"
                        required
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-white mb-2">
                        TGS иконка <span className="text-red-500">*</span>
                      </label>
                      <div className="flex gap-2 items-center">
                        <input
                          type="file"
                          accept=".tgs,.json"
                          onChange={(e) => {
                            const file = e.target.files?.[0];
                            if (file) {
                              setIconFile(file);
                              handleFileUpload(file);
                            }
                          }}
                          className="input flex-1"
                        />
                        {uploadProgress > 0 && uploadProgress < 100 && (
                          <div className="text-sm text-blue-400">Загрузка: {uploadProgress}%</div>
                        )}
                      </div>
                      {iconDocumentId && (
                        <div className="mt-2 space-y-1">
                          <p className="text-sm text-green-600">✅ Document ID: {iconDocumentId}</p>
                          <button
                            type="button"
                            onClick={async () => {
                              try {
                                const response = await fetch(`${API_URL}/api/verification/icons/${iconDocumentId}/check`);
                                const data = await response.json();
                                if (data.success) {
                                  const status = data.data;
                                  alert(
                                    `📊 Проверка документа ${iconDocumentId}:\n\n` +
                                    `MongoDB: ${status.existsInMongoDB ? '✅ Найден' : '❌ Не найден'}\n` +
                                    `MinIO: ${status.existsInMinIO ? '✅ Найден' : '❌ Не найден'}\n` +
                                    (status.document ? `\nРазмер: ${status.document.Size} байт\nMimeType: ${status.document.MimeType}` : '')
                                  );
                                } else {
                                  alert('❌ Ошибка проверки: ' + data.error);
                                }
                              } catch (error) {
                                alert('❌ Ошибка: ' + error.message);
                              }
                            }}
                            className="text-xs text-blue-400 hover:text-blue-300 underline"
                          >
                            🔍 Проверить документ
                          </button>
                        </div>
                      )}
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-white mb-2">
                        Icon Document ID <span className="text-red-500">*</span>
                      </label>
                      <input
                        type="number"
                        value={iconDocumentId}
                        onChange={(e) => setIconDocumentId(e.target.value)}
                        className="input focus:ring-2 focus:ring-blue-500"
                        placeholder="Заполнится автоматически после загрузки"
                        required
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-white mb-2">
                        Название компании <span className="text-red-500">*</span>
                      </label>
                      <input
                        type="text"
                        value={companyName}
                        onChange={(e) => setCompanyName(e.target.value)}
                        className="input focus:ring-2 focus:ring-blue-500"
                        placeholder="Например: Verified Company"
                        required
                      />
                    </div>

                    <div className="flex items-center gap-2">
                      <input
                        type="checkbox"
                        checked={canModifyDesc}
                        onChange={(e) => setCanModifyDesc(e.target.checked)}
                        className="w-4 h-4 text-blue-400 border-[#2b5278] rounded focus:ring-blue-500"
                      />
                      <label className="text-sm text-white">
                        Разрешить изменение кастомного описания
                      </label>
                    </div>

                    <div className="flex gap-2">
                      <button
                        type="submit"
                        disabled={loading}
                        className="flex-1 px-6 py-3 bg-green-500 text-white rounded-lg hover:bg-green-600 disabled:bg-[#2b5278]/50 disabled:cursor-not-allowed"
                      >
                        {loading ? 'Создаю...' : 'Создать'}
                      </button>
                      <button
                        type="button"
                        onClick={() => {
                          setShowCreateForm(false);
                          setBotUserId('');
                          setIconDocumentId('');
                          setCompanyName('');
                          setCanModifyDesc(false);
                          setIconFile(null);
                        }}
                        className="px-6 py-3 bg-[#2b5278] text-white rounded-lg hover:bg-[#3d5a7a]"
                      >
                        Отмена
                      </button>
                    </div>
                  </form>
                </div>
              )}

              {/* Verifiers list */}
              <div>
                <h3 className="text-lg font-semibold text-white mb-4">Список верификаторов</h3>
                {verifiers.length === 0 ? (
                  <p className="text-[#8b98a5] text-center py-8">Нет верификаторов</p>
                ) : (
                  <div className="space-y-3">
                    {verifiers.map((verifier) => (
                      <div
                        key={verifier.BotUserId}
                        className="card border border-[#2b5278] rounded-lg p-4 hover:shadow-md transition-shadow"
                      >
                        <div className="flex items-start justify-between">
                          <div className="flex-1">
                            <div className="flex items-center gap-3">
                              <Shield className="w-6 h-6 text-blue-400" />
                              <div>
                                <h4 className="font-semibold text-white">{verifier.CompanyName}</h4>
                                <p className="text-sm text-[#8b98a5]">Bot User ID: {verifier.BotUserId}</p>
                              </div>
                            </div>
                            <div className="mt-2 text-sm text-[#8b98a5] space-y-1">
                              <p><strong>Icon Emoji ID:</strong> {verifier.IconEmojiId}</p>
                              <p><strong>Кастомное описание:</strong> {verifier.CanModifyCustomDescription ? '✅ Разрешено' : '❌ Запрещено'}</p>
                              <p><strong>Создан:</strong> {new Date(verifier.CreatedAt).toLocaleString('ru-RU')}</p>
                              {verifier.UpdatedAt && (
                                <p><strong>Обновлен:</strong> {new Date(verifier.UpdatedAt).toLocaleString('ru-RU')}</p>
                              )}
                            </div>
                          </div>
                          <div className="flex gap-2">
                            <button
                              onClick={() => deleteVerifier(verifier.BotUserId)}
                              className="p-2 text-red-400 hover:bg-red-500/300/20 rounded-lg"
                              title="Деактивировать"
                            >
                              <Trash2 className="w-5 h-5" />
                            </button>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}










