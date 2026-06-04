import { useState, useEffect } from 'react';
import { toast } from 'react-hot-toast';
import { Snowflake, Upload, Save, Info, CheckCircle2, AlertTriangle, Cloud, FileCode, Server } from 'lucide-react';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:3001';

export default function FrozenSettings() {
  const [settings, setSettings] = useState(null);
  const [loading, setLoading] = useState(true);
  const [uploading, setUploading] = useState(false);
  const [file, setFile] = useState(null);
  const [documentId, setDocumentId] = useState('');

  useEffect(() => {
    fetchSettings();
  }, []);

  const fetchSettings = async () => {
    try {
      setLoading(true);
      const response = await fetch(`${API_URL}/api/frozen-settings`);
      const data = await response.json();
      setSettings(data.settings);
      if (data.settings.snowflakeEmojiId) {
        setDocumentId(data.settings.snowflakeEmojiId.toString());
      }
    } catch (error) {
      console.error('Error fetching settings:', error);
      toast.error('Failed to load settings');
    } finally {
      setLoading(false);
    }
  };

  const handleFileChange = (e) => {
    const selectedFile = e.target.files[0];
    if (selectedFile) {
      // Check if it's a TGS file
      if (!selectedFile.name.endsWith('.tgs')) {
        toast.error('Please select a .tgs file');
        return;
      }
      setFile(selectedFile);
    }
  };

  const handleUpload = async () => {
    if (!file) {
      toast.error('Please select a TGS file first');
      return;
    }

    try {
      setUploading(true);
      const formData = new FormData();
      formData.append('snowflake', file);

      const response = await fetch(`${API_URL}/api/frozen-settings/upload-snowflake`, {
        method: 'POST',
        body: formData
      });
      const data = await response.json();

      if (response.ok) {
        toast.success('Snowflake TGS uploaded successfully! Now upload it to Telegram and set the Document ID.');
        fetchSettings();
        setFile(null);
      } else {
        throw new Error(data.error || 'Failed to upload file');
      }
    } catch (error) {
      console.error('Error uploading:', error);
      toast.error(error.message);
    } finally {
      setUploading(false);
    }
  };

  const handleSetEmojiId = async () => {
    if (!documentId || isNaN(documentId)) {
      toast.error('Please enter a valid Document ID');
      return;
    }

    try {
      const response = await fetch(`${API_URL}/api/frozen-settings/set-emoji-id`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          documentId: parseInt(documentId)
        })
      });
      const data = await response.json();

      if (response.ok) {
        toast.success('Emoji ID updated! Restart QueryServer to apply changes.');
        fetchSettings();
      } else {
        throw new Error(data.error || 'Failed to set emoji ID');
      }
    } catch (error) {
      console.error('Error setting emoji ID:', error);
      toast.error(error.message);
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6 max-w-5xl mx-auto animate-fade-in">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-heading font-bold text-fg flex items-center gap-3">
            <div className="p-2.5 bg-blue/10 rounded-xl rounded-tr-none">
              <Server className="w-8 h-8 text-blue" />
            </div>
            Server-Side Frozen Settings
          </h1>
          <p className="text-fg-muted mt-2">Configure the snowflake animation for the server</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Current Configuration */}
        <div className="card lg:col-span-2">
          <h2 className="text-xl font-heading font-bold text-fg mb-6 flex items-center gap-2">
            <Info className="w-5 h-5 text-accent" />
            Current Configuration
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
            <div className="p-4 bg-muted/30 rounded-xl border border-border">
              <strong className="text-xs font-bold text-fg-muted uppercase tracking-wider block mb-2">Snowflake File</strong>
              <div className="flex items-center gap-2">
                <FileCode className="w-4 h-4 text-purple" />
                <span className="text-fg font-medium font-mono text-sm truncate">
                  {settings?.snowflakeFileName || 'Not uploaded'}
                </span>
              </div>
            </div>
            <div className="p-4 bg-muted/30 rounded-xl border border-border">
              <strong className="text-xs font-bold text-fg-muted uppercase tracking-wider block mb-2">Uploaded At</strong>
              <div className="flex items-center gap-2">
                <span className="text-fg font-medium text-sm">
                  {settings?.snowflakeUploadedAt
                    ? new Date(settings.snowflakeUploadedAt).toLocaleDateString()
                    : 'N/A'}
                </span>
              </div>
            </div>
            <div className="p-4 bg-muted/30 rounded-xl border border-border">
              <strong className="text-xs font-bold text-fg-muted uppercase tracking-wider block mb-2">Document ID</strong>
              <div className="flex items-center gap-2">
                <span className="text-fg font-medium font-mono text-sm underline decoration-dotted">
                  {settings?.snowflakeEmojiId || 'Not set'}
                </span>
              </div>
            </div>
            <div className="p-4 bg-muted/30 rounded-xl border border-border">
              <strong className="text-xs font-bold text-fg-muted uppercase tracking-wider block mb-2">Status</strong>
              <div className="flex items-center gap-2">
                <span className={`inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium border ${settings?.snowflakeEmojiId
                    ? 'bg-success/10 text-success border-success/20'
                    : 'bg-yellow/10 text-yellow border-yellow/20'
                  }`}>
                  {settings?.snowflakeEmojiId ? (
                    <>
                      <CheckCircle2 className="w-3 h-3 mr-1" /> Active
                    </>
                  ) : (
                    <>
                      <AlertTriangle className="w-3 h-3 mr-1" /> Not Configured
                    </>
                  )}
                </span>
              </div>
            </div>
          </div>
        </div>

        {/* Step 1: Upload TGS */}
        <div className="card h-full">
          <div className="flex items-center gap-3 mb-4">
            <div className="w-8 h-8 rounded-full bg-blue text-white flex items-center justify-center font-bold text-sm">1</div>
            <h2 className="text-xl font-heading font-bold text-fg">Upload Snowflake TGS</h2>
          </div>

          <p className="text-fg-muted mb-6 text-sm">
            Upload an animated snowflake sticker file (.tgs format, max 64KB).
          </p>

          <div className="space-y-4">
            <div className="relative group">
              <input
                type="file"
                accept=".tgs"
                onChange={handleFileChange}
                className="absolute inset-0 w-full h-full opacity-0 cursor-pointer z-10"
              />
              <div className={`border-2 border-dashed rounded-xl p-8 flex flex-col items-center justify-center transition-colors ${file ? 'border-success bg-success/5' : 'border-border group-hover:border-accent group-hover:bg-accent/5'
                }`}>
                {file ? (
                  <>
                    <FileCode className="w-10 h-10 text-success mb-3" />
                    <p className="text-fg font-medium">{file.name}</p>
                    <p className="text-fg-muted text-xs mt-1">{(file.size / 1024).toFixed(2)} KB</p>
                  </>
                ) : (
                  <>
                    <Upload className="w-10 h-10 text-fg-muted mb-3 group-hover:text-accent transition-colors" />
                    <p className="text-fg font-medium">Click to upload TGS</p>
                    <p className="text-fg-muted text-xs mt-1">or drag and drop</p>
                  </>
                )}
              </div>
            </div>

            <button
              onClick={handleUpload}
              disabled={!file || uploading}
              className="btn btn-primary w-full flex items-center justify-center gap-2"
            >
              {uploading ? (
                <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
              ) : (
                <Upload className="w-5 h-5" />
              )}
              {uploading ? 'Uploading...' : 'Upload TGS'}
            </button>

            {settings?.snowflakeFilePath && (
              <div className="p-3 bg-muted/50 rounded-lg border border-border">
                <div className="text-xs text-fg-muted uppercase font-bold mb-1">Filesystem Path</div>
                <code className="text-xs font-mono text-success break-all block">
                  {settings.snowflakeFilePath}
                </code>
              </div>
            )}
          </div>
        </div>

        <div className="flex flex-col gap-6">
          {/* Step 2: Upload to Telegram */}
          <div className="card">
            <div className="flex items-center gap-3 mb-4">
              <div className="w-8 h-8 rounded-full bg-blue text-white flex items-center justify-center font-bold text-sm">2</div>
              <h2 className="text-xl font-heading font-bold text-fg">Upload to Telegram</h2>
            </div>
            <p className="text-fg-muted mb-4 text-sm">
              After uploading the TGS above, upload it to Telegram manually:
            </p>

            <div className="bg-muted/80 p-4 rounded-xl font-mono text-xs space-y-2 border border-border shadow-inner">
              <div className="text-fg-muted"># Use Bot API to upload as custom emoji</div>
              <div className="text-success">POST /bot{'<token>'}/uploadStickerFile</div>
              <div className="text-fg-muted"># Or use @Stickers bot / admin panel</div>
              <div className="text-fg-muted"># Get the Document ID from response</div>
            </div>
          </div>

          {/* Step 3: Set Document ID */}
          <div className="card flex-1">
            <div className="flex items-center gap-3 mb-4">
              <div className="w-8 h-8 rounded-full bg-blue text-white flex items-center justify-center font-bold text-sm">3</div>
              <h2 className="text-xl font-heading font-bold text-fg">Set Document ID</h2>
            </div>
            <p className="text-fg-muted mb-4 text-sm">
              Enter the Document ID from Telegram to link it.
            </p>

            <div className="space-y-4">
              <input
                type="text"
                value={documentId}
                onChange={(e) => setDocumentId(e.target.value)}
                placeholder="e.g., 1234567890"
                className="input w-full font-mono"
              />

              <button
                onClick={handleSetEmojiId}
                disabled={!documentId}
                className="btn btn-secondary w-full flex items-center justify-center gap-2"
              >
                <Save className="w-5 h-5" />
                Save Document ID
              </button>
            </div>

            <div className="mt-4 p-3 bg-yellow/10 border border-yellow/20 text-yellow text-xs rounded-lg flex items-start gap-2">
              <AlertTriangle className="w-4 h-4 shrink-0 mt-0.5" />
              <span>Restart QueryServer after saving to apply changes.</span>
            </div>
          </div>
        </div>
      </div>

      {/* Steps Guide */}
      <div className="bg-blue/5 border border-blue/20 rounded-xl p-6">
        <h3 className="text-lg font-bold font-heading text-fg mb-3 flex items-center gap-2">
          <Cloud className="w-5 h-5 text-blue" />
          Workflow Guide
        </h3>
        <ol className="list-decimal list-inside space-y-2 text-sm text-fg-muted">
          <li>Upload your animated snowflake <code className="bg-muted px-1.5 py-0.5 rounded text-fg">.tgs</code> file here.</li>
          <li>Upload the same TGS to your Telegram server as a custom emoji (using tools or bot).</li>
          <li>Copy the <strong>Document ID</strong> from the Telegram upload response.</li>
          <li>Enter that ID in step 3 and save.</li>
          <li>The system updates <code className="bg-muted px-1.5 py-0.5 rounded text-fg">queryserver.appsettings.json</code> automatically.</li>
          <li><strong>Restart QueryServer</strong> - frozen accounts will now show the snowflake emoji!</li>
        </ol>
      </div>
    </div>
  );
}
