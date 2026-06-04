import { useState, useEffect } from 'react';
import { toast } from 'react-hot-toast';
import { Snowflake, Upload, FileJson, Save, Info, CheckCircle2, AlertTriangle, File } from 'lucide-react';

export default function FrozenIconSettings() {
  const [settings, setSettings] = useState({
    type: 'emoji',
    emoji: '❄️',
    documentId: null
  });
  const [loading, setLoading] = useState(false);
  const [file, setFile] = useState(null);

  useEffect(() => {
    loadSettings();
  }, []);

  const loadSettings = async () => {
    try {
      const response = await fetch('http://localhost:3001/api/users/settings/frozen-icon');
      const data = await response.json();
      setSettings(data);
    } catch (error) {
      console.error('Failed to load settings:', error);
      toast.error('Failed to load settings');
    }
  };

  const handleFileChange = (e) => {
    const selectedFile = e.target.files[0];
    if (selectedFile && selectedFile.name.endsWith('.json')) {
      setFile(selectedFile);
    } else {
      toast.error('Please select a .json file');
      setFile(null);
      e.target.value = null; // Reset input
    }
  };

  const handleUpload = async () => {
    if (!file) {
      toast.error('Please select a file first');
      return;
    }

    setLoading(true);
    const toastId = toast.loading('Processing animation...');

    try {
      // Read JSON file content
      const reader = new FileReader();
      reader.onload = async (e) => {
        try {
          const jsonContent = JSON.parse(e.target.result);

          // Validate Lottie JSON structure
          if (!jsonContent.v || !jsonContent.layers) {
            toast.error('Invalid Lottie JSON: missing required fields', { id: toastId });
            setLoading(false);
            return;
          }

          console.log('✅ Valid Lottie JSON:', {
            version: jsonContent.v,
            width: jsonContent.w,
            height: jsonContent.h,
            layers: jsonContent.layers?.length
          });

          // Step 1: Upload JSON as document to Telegram
          const formData = new FormData();
          formData.append('file', file);
          formData.append('type', 'animation');
          formData.append('title', 'Frozen Account Animation');

          const uploadResponse = await fetch('http://localhost:3001/api/upload/document', {
            method: 'POST',
            body: formData
          });

          const uploadData = await uploadResponse.json();

          if (!uploadData.success) {
            toast.error('Failed to upload to Telegram: ' + (uploadData.error || 'Unknown error'), { id: toastId });
            setLoading(false);
            return;
          }

          // Step 2: Save document ID as global frozen animation setting
          const response = await fetch('http://localhost:3001/api/users/settings/frozen-icon', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
              type: 'animation',
              emoji: settings.emoji,
              documentId: uploadData.documentId
            })
          });

          const data = await response.json();

          if (data.success) {
            toast.success(`Animation uploaded successfully! ID: ${uploadData.documentId}`, { id: toastId });
            setSettings({ ...settings, type: 'animation', documentId: uploadData.documentId });
            setFile(null);
          } else {
            toast.error('Failed to save settings: ' + (data.error || 'Unknown error'), { id: toastId });
          }
        } catch (error) {
          console.error('JSON parse error:', error);
          toast.error('Invalid JSON file: ' + error.message, { id: toastId });
        }
        setLoading(false);
      };
      reader.readAsText(file);
    } catch (error) {
      toast.error('Upload failed: ' + error.message, { id: toastId });
      setLoading(false);
    }
  };

  const handleUseEmoji = async () => {
    setLoading(true);
    const toastId = toast.loading('Saving emoji settings...');

    try {
      const response = await fetch('http://localhost:3001/api/users/settings/frozen-icon', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          type: 'emoji',
          emoji: settings.emoji,
          documentId: null
        })
      });

      const data = await response.json();

      if (data.success) {
        toast.success('Switched to emoji mode', { id: toastId });
        setSettings({ ...settings, type: 'emoji', documentId: null });
      } else {
        toast.error('Failed to update settings', { id: toastId });
      }
    } catch (error) {
      toast.error('Update failed: ' + error.message, { id: toastId });
    }
    setLoading(false);
  };

  return (
    <div className="space-y-6 max-w-5xl mx-auto animate-fade-in">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-heading font-bold text-fg flex items-center gap-3">
            <div className="p-2.5 bg-blue/10 rounded-xl rounded-tr-none">
              <Snowflake className="w-8 h-8 text-blue" />
            </div>
            Frozen Account Icon
          </h1>
          <p className="text-fg-muted mt-2">Customize the icon shown on frozen accounts</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Current Settings Card */}
        <div className="card lg:col-span-2">
          <h2 className="text-xl font-heading font-bold text-fg mb-6 flex items-center gap-2">
            <Info className="w-5 h-5 text-accent" />
            Current Configuration
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="p-4 bg-muted/30 rounded-xl border border-border">
              <strong className="text-xs font-bold text-fg-muted uppercase tracking-wider block mb-2">Type</strong>
              <div className="flex items-center gap-2">
                {settings.type === 'emoji' ? (
                  <span className="p-1 bg-yellow/10 rounded text-yellow">
                    <Snowflake className="w-4 h-4" />
                  </span>
                ) : (
                  <span className="p-1 bg-blue/10 rounded text-blue">
                    <FileJson className="w-4 h-4" />
                  </span>
                )}
                <span className="text-fg font-medium capitalize">{settings.type}</span>
              </div>
            </div>
            <div className="p-4 bg-muted/30 rounded-xl border border-border">
              <strong className="text-xs font-bold text-fg-muted uppercase tracking-wider block mb-2">Emoji</strong>
              <span className="text-3xl filter drop-shadow-sm">{settings.emoji}</span>
            </div>
            {settings.documentId && (
              <div className="p-4 bg-muted/30 rounded-xl border border-border md:col-span-1">
                <strong className="text-xs font-bold text-fg-muted uppercase tracking-wider block mb-2">Animation ID</strong>
                <span className="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium bg-success/10 text-success border border-success/20 font-mono">
                  {settings.documentId}
                </span>
              </div>
            )}
          </div>
        </div>

        {/* Upload Animation Card */}
        <div className="card h-full">
          <div className="flex items-center gap-3 mb-4">
            <div className="p-2 bg-purple/10 rounded-lg">
              <FileJson className="w-5 h-5 text-purple" />
            </div>
            <h2 className="text-xl font-heading font-bold text-fg">Upload Animation</h2>
          </div>

          <p className="text-fg-muted mb-6 text-sm">
            Upload a Lottie JSON animation file to use instead of a static emoji.
          </p>

          <div className="space-y-4">
            <div className="relative group">
              <input
                type="file"
                accept=".json"
                onChange={handleFileChange}
                disabled={loading}
                className="absolute inset-0 w-full h-full opacity-0 cursor-pointer disabled:cursor-not-allowed z-10"
              />
              <div className={`border-2 border-dashed rounded-xl p-8 flex flex-col items-center justify-center transition-colors ${file ? 'border-success bg-success/5' : 'border-border group-hover:border-accent group-hover:bg-accent/5'
                }`}>
                {file ? (
                  <>
                    <FileJson className="w-10 h-10 text-success mb-3" />
                    <p className="text-fg font-medium">{file.name}</p>
                    <p className="text-fg-muted text-xs mt-1">{(file.size / 1024).toFixed(2)} KB</p>
                  </>
                ) : (
                  <>
                    <Upload className="w-10 h-10 text-fg-muted mb-3 group-hover:text-accent transition-colors" />
                    <p className="text-fg font-medium">Click to upload JSON</p>
                    <p className="text-fg-muted text-xs mt-1">or drag and drop</p>
                  </>
                )}
              </div>
            </div>

            <button
              onClick={handleUpload}
              disabled={!file || loading}
              className="btn btn-primary w-full flex items-center justify-center gap-2"
            >
              {loading ? (
                <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
              ) : (
                <Upload className="w-5 h-5" />
              )}
              {loading ? 'Uploading...' : 'Upload Animation'}
            </button>
          </div>
        </div>

        {/* Use Emoji Card */}
        <div className="card h-full">
          <div className="flex items-center gap-3 mb-4">
            <div className="p-2 bg-yellow/10 rounded-lg">
              <Snowflake className="w-5 h-5 text-yellow" />
            </div>
            <h2 className="text-xl font-heading font-bold text-fg">Use Emoji</h2>
          </div>

          <p className="text-fg-muted mb-6 text-sm">
            Use a simple emoji character instead of an animation.
          </p>

          <div className="flex flex-col gap-4">
            <div className="flex gap-4 items-center">
              <input
                type="text"
                value={settings.emoji}
                onChange={(e) => setSettings({ ...settings, emoji: e.target.value })}
                maxLength={2}
                className="input text-center text-4xl w-24 h-24 p-0 flex items-center justify-center"
                disabled={loading}
              />
              <div className="text-sm text-fg-muted flex-1">
                Enter a single emoji character to be displayed on frozen accounts.
              </div>
            </div>

            <button
              onClick={handleUseEmoji}
              disabled={loading}
              className="btn btn-secondary w-full flex items-center justify-center gap-2 mt-auto"
            >
              {loading ? (
                <div className="w-5 h-5 border-2 border-current border-t-transparent rounded-full animate-spin" />
              ) : (
                <Save className="w-5 h-5" />
              )}
              {loading ? 'Saving...' : 'Save Emoji'}
            </button>
          </div>
        </div>
      </div>

      <div className="bg-blue/5 border border-blue/20 rounded-xl p-6 flex gap-4 items-start">
        <div className="p-2 bg-blue/10 rounded-lg shrink-0">
          <Info className="w-5 h-5 text-blue" />
        </div>
        <div className="space-y-2">
          <h3 className="text-fg font-bold">How it works</h3>
          <ul className="space-y-1.5 text-sm text-fg-muted">
            <li className="flex items-center gap-2">
              <span className="w-1.5 h-1.5 bg-blue rounded-full"></span>
              <span><strong>Emoji mode:</strong> Simple emoji (❄️) will be added to frozen account names</span>
            </li>
            <li className="flex items-center gap-2">
              <span className="w-1.5 h-1.5 bg-blue rounded-full"></span>
              <span><strong>Animation mode:</strong> Lottie JSON animation will be used (requires client support)</span>
            </li>
            <li className="flex items-center gap-2">
              <span className="w-1.5 h-1.5 bg-blue rounded-full"></span>
              <span><strong>Recommended size:</strong> Under 100KB for best performance</span>
            </li>
          </ul>
        </div>
      </div>
    </div>
  );
}








