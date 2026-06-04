import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Upload, FileArchive, ArrowLeft, CheckCircle, XCircle, Loader } from 'lucide-react';
import { Link } from 'react-router-dom';
import { toast } from 'react-hot-toast';

export default function BulkUploadStickers() {
  const navigate = useNavigate();
  const { packId } = useParams();
  const [loading, setLoading] = useState(false);
  const [zipFile, setZipFile] = useState(null);
  const [uploadProgress, setUploadProgress] = useState([]);
  const [completed, setCompleted] = useState(false);
  const [uploadStats, setUploadStats] = useState(null);
  const [packs, setPacks] = useState([]);
  const [selectedPackId, setSelectedPackId] = useState(packId || '');
  const [loadingPacks, setLoadingPacks] = useState(false);

  useEffect(() => {
    fetchPacks();
  }, []);

  const fetchPacks = async () => {
    try {
      setLoadingPacks(true);
      const response = await fetch('/api/stickerpacks?limit=100');
      const data = await response.json();
      setPacks(data.packs || []);
      if (packId) {
        setSelectedPackId(packId);
      }
    } catch (error) {
      console.error('Error fetching packs:', error);
      toast.error('Failed to load sticker packs');
    } finally {
      setLoadingPacks(false);
    }
  };

  const handleFileChange = (e) => {
    const file = e.target.files[0];
    if (file && file.name.endsWith('.zip')) {
      setZipFile(file);
      setUploadProgress([]);
      setCompleted(false);
    } else {
      toast.error('Please select a ZIP file');
    }
  };

  const handleUpload = async () => {
    if (!zipFile) {
      toast.error('Please select a ZIP file first');
      return;
    }

    if (!selectedPackId) {
      toast.error('Please select a sticker pack');
      return;
    }

    setLoading(true);
    setUploadProgress([]);

    try {
      const formData = new FormData();
      formData.append('zipFile', zipFile);
      formData.append('packId', selectedPackId);

      const response = await fetch('/api/stickerpacks/bulk-upload', {
        method: 'POST',
        body: formData
      });

      if (response.ok) {
        const data = await response.json();
        setUploadProgress(data.results || []);
        setUploadStats({
          total: data.successCount || 0,
          premium: data.premiumCount || 0,
          regular: data.regularCount || 0
        });
        setCompleted(true);
        toast.success('Upload completed successfully');

        if (data.packId) {
          setTimeout(() => {
            navigate(`/stickerpacks/${data.packId}`);
          }, 2000);
        }
      } else {
        const error = await response.json();
        toast.error(`Error: ${error.error}`);
      }
    } catch (error) {
      console.error('Error uploading:', error);
      toast.error('Failed to upload ZIP file');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-4xl mx-auto space-y-6 animate-fade-in">
      {/* Header */}
      <div>
        <Link
          to={packId ? `/stickerpacks/${packId}` : '/stickerpacks'}
          className="inline-flex items-center text-fg-muted hover:text-purple mb-4 transition-colors"
        >
          <ArrowLeft className="w-5 h-5 mr-2" />
          Back
        </Link>
        <h1 className="text-3xl font-heading font-bold text-fg flex items-center gap-3">
          <FileArchive className="w-8 h-8 text-blue" />
          Bulk Upload Stickers from ZIP
        </h1>
        <p className="text-fg-muted mt-1">
          Upload a ZIP file containing TGS/WEBP/PNG/WEBM sticker files. Random emojis will be assigned automatically.
        </p>
        <div className="mt-4 p-4 bg-blue/5 border border-blue/20 rounded-lg">
          <p className="text-sm text-blue font-medium">✨ Premium Stickers Support:</p>
          <p className="text-xs text-blue/80 mt-1">
            Name files as <code className="bg-black/30 px-1 rounded">000_main.tgs</code> + <code className="bg-black/30 px-1 rounded">000_effect.tgs</code> for premium stickers with effects
          </p>
        </div>
      </div>

      {/* Upload Form */}
      <div className="card p-6 space-y-6">
        {/* Pack Selection */}
        <div>
          <label className="block text-sm font-medium text-fg mb-2">
            Select Sticker Pack <span className="text-red">*</span>
          </label>
          <select
            value={selectedPackId}
            onChange={(e) => setSelectedPackId(e.target.value)}
            disabled={loading || loadingPacks || !!packId}
            className="input w-full focus:ring-2 focus:ring-purple/50 disabled:opacity-50"
          >
            <option value="">-- Select a pack --</option>
            {packs.map((pack) => (
              <option key={pack.StickerSetId || pack.stickerset_id} value={pack.StickerSetId || pack.stickerset_id}>
                {pack.Title || pack.title} ({pack.ShortName || pack.short_name})
              </option>
            ))}
          </select>
          <p className="mt-2 text-sm text-fg-muted">
            {packId ? 'Pack is pre-selected from URL' : 'Choose which pack to add stickers to'}
          </p>
          {!packId && (
            <Link
              to="/stickerpacks/create"
              className="mt-2 inline-flex items-center text-sm text-purple hover:text-purple/80"
            >
              + Create new sticker pack first
            </Link>
          )}
        </div>

        {/* File Input */}
        <div>
          <label className="block text-sm font-medium text-fg mb-2">
            Select ZIP File <span className="text-red">*</span>
          </label>
          <div className="flex items-center gap-4">
            <input
              type="file"
              accept=".zip"
              onChange={handleFileChange}
              disabled={loading}
              className="input flex-1 p-2 focus:ring-2 focus:ring-purple/50"
            />
            {zipFile && (
              <span className="text-sm text-fg-muted">
                {zipFile.name} ({(zipFile.size / 1024 / 1024).toFixed(2)} MB)
              </span>
            )}
          </div>
          <p className="mt-2 text-sm text-fg-muted">
            ZIP file should contain .tgs, .webp, .png or .webm files
          </p>
        </div>

        {/* Info Box */}
        <div className="bg-purple/5 border border-purple/20 rounded-lg p-4">
          <h4 className="font-semibold text-purple mb-2">ℹ️ How it works</h4>
          <ul className="list-disc list-inside text-sm text-purple/80 space-y-1">
            <li>Upload a ZIP file containing TGS, WEBP, PNG or WEBM sticker files</li>
            <li>Each file will be extracted and processed</li>
            <li>Random emoji will be assigned as fallback (alt) for each sticker</li>
            <li>All stickers will be added to the selected pack</li>
            <li>You can edit emoji assignments later</li>
            <li>Supported formats: TGS (animated), WEBP (recommended), PNG</li>
            <li className="text-purple font-medium">✨ Premium: Use <code className="bg-black/30 px-1 rounded">XXX_main.tgs</code> + <code className="bg-black/30 px-1 rounded">XXX_effect.tgs</code> naming</li>
          </ul>
        </div>

        {/* Upload Button */}
        <button
          onClick={handleUpload}
          disabled={!zipFile || !selectedPackId || loading}
          className="btn btn-primary w-full py-3 text-lg font-semibold flex items-center justify-center gap-2"
        >
          {loading ? (
            <>
              <Loader className="w-5 h-5 animate-spin" />
              Processing...
            </>
          ) : (
            <>
              <Upload className="w-5 h-5" />
              Upload and Process
            </>
          )}
        </button>

        {/* Progress */}
        {uploadProgress.length > 0 && (
          <div className="mt-6 space-y-2 animate-fade-in">
            <h3 className="font-semibold text-fg">Upload Results:</h3>
            <div className="max-h-96 overflow-y-auto space-y-2 pr-2 custom-scrollbar">
              {uploadProgress.map((item, index) => (
                <div
                  key={index}
                  className={`flex items-center gap-3 p-3 rounded-lg border ${item.success
                    ? 'bg-success/5 border-success/20'
                    : 'bg-red/5 border-red/20'
                    }`}
                >
                  {item.success ? (
                    <CheckCircle className="w-5 h-5 text-success flex-shrink-0" />
                  ) : (
                    <XCircle className="w-5 h-5 text-red flex-shrink-0" />
                  )}
                  <div className="flex-1 min-w-0">
                    <div className="font-medium text-fg truncate">{item.filename}</div>
                    {item.emoji && (
                      <div className="text-sm text-fg-muted truncate">
                        Emoji: {item.emoji} | Document ID: {item.documentId}
                        {item.premium && <span className="ml-2 px-2 py-0.5 bg-blue/10 text-blue text-[10px] rounded border border-blue/20">✨ PREMIUM</span>}
                      </div>
                    )}
                    {item.error && (
                      <div className="text-sm text-red">{item.error}</div>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Success Message */}
        {completed && uploadStats && (
          <div className="bg-success/5 border border-success/20 rounded-lg p-4 animate-scale-in">
            <h4 className="font-semibold text-success mb-2 flex items-center gap-2">
              <CheckCircle className="w-5 h-5" />
              Upload Complete!
            </h4>
            <div className="text-sm text-success/80 space-y-1">
              <p>✅ Total: {uploadStats.total} stickers uploaded successfully</p>
              {uploadStats.premium > 0 && (
                <p>✨ Premium: {uploadStats.premium} stickers with effects</p>
              )}
              {uploadStats.regular > 0 && (
                <p>📦 Regular: {uploadStats.regular} stickers</p>
              )}
              {packId && <p className="mt-2 font-medium">Redirecting to pack...</p>}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
