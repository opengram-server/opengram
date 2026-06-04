import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Upload, FileArchive, ArrowLeft, CheckCircle, XCircle, Loader } from 'lucide-react';
import { Link } from 'react-router-dom';
import { toast } from 'react-hot-toast';

export default function BulkUploadEmojis() {
  const navigate = useNavigate();
  const { packId } = useParams();
  const [loading, setLoading] = useState(false);
  const [zipFile, setZipFile] = useState(null);
  const [uploadProgress, setUploadProgress] = useState([]);
  const [completed, setCompleted] = useState(false);
  const [packs, setPacks] = useState([]);
  const [selectedPackId, setSelectedPackId] = useState(packId || '');
  const [loadingPacks, setLoadingPacks] = useState(false);

  useEffect(() => {
    fetchPacks();
  }, []);

  const fetchPacks = async () => {
    try {
      setLoadingPacks(true);
      const response = await fetch('/api/emojipacks?limit=100');
      const data = await response.json();
      setPacks(data.packs || []);
      if (packId) {
        setSelectedPackId(packId);
      }
    } catch (error) {
      console.error('Error fetching packs:', error);
      toast.error('Failed to load emoji packs');
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
      toast.error('Please select an emoji pack');
      return;
    }

    setLoading(true);
    setUploadProgress([]);

    try {
      const formData = new FormData();
      formData.append('zipFile', zipFile);
      formData.append('packId', selectedPackId);

      const response = await fetch('/api/emojipacks/bulk-upload', {
        method: 'POST',
        body: formData
      });

      if (response.ok) {
        const data = await response.json();
        setUploadProgress(data.results || []);
        setCompleted(true);
        toast.success('Upload completed successfully');

        if (data.packId) {
          setTimeout(() => {
            navigate(`/emojipacks/${data.packId}`);
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
          to={packId ? `/emojipacks/${packId}` : '/emojipacks'}
          className="inline-flex items-center text-fg-muted hover:text-purple mb-4 transition-colors"
        >
          <ArrowLeft className="w-5 h-5 mr-2" />
          Back
        </Link>
        <h1 className="text-3xl font-heading font-bold text-fg flex items-center gap-3">
          <FileArchive className="w-8 h-8 text-purple" />
          Bulk Upload Emojis from ZIP
        </h1>
        <p className="text-fg-muted mt-1">
          Upload a ZIP file containing TGS, WebP, or PNG emoji files. Random emojis will be assigned automatically.
        </p>
      </div>

      {/* Upload Form */}
      <div className="card p-6 space-y-6">
        {/* Pack Selection */}
        <div>
          <label className="block text-sm font-medium text-fg mb-2">
            Select Emoji Pack <span className="text-red">*</span>
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
            {packId ? 'Pack is pre-selected from URL' : 'Choose which pack to add emojis to'}
          </p>
          {!packId && (
            <Link
              to="/emojipacks/create"
              className="mt-2 inline-flex items-center text-sm text-purple hover:text-purple/80"
            >
              + Create new emoji pack first
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
            ZIP file should contain .tgs, .webp, or .png files
          </p>
        </div>

        {/* Info Box */}
        <div className="bg-blue/5 border border-blue/20 rounded-lg p-4">
          <h4 className="font-semibold text-blue mb-2">ℹ️ How it works</h4>
          <ul className="list-disc list-inside text-sm text-blue/80 space-y-1">
            <li>Upload a ZIP file containing TGS, WebP, or PNG emoji files</li>
            <li>Each file will be extracted and processed</li>
            <li>Random emoji will be assigned as fallback (alt) for each file</li>
            <li>All emojis will be added to the selected pack</li>
            <li>You can edit emoji assignments later</li>
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
        {completed && (
          <div className="bg-success/5 border border-success/20 rounded-lg p-4 animate-scale-in">
            <h4 className="font-semibold text-success mb-2 flex items-center gap-2">
              <CheckCircle className="w-5 h-5" />
              Upload Complete!
            </h4>
            <p className="text-sm text-success/80">
              {uploadProgress.filter(p => p.success).length} emojis uploaded successfully.
              {packId && ' Redirecting to pack...'}
            </p>
          </div>
        )}
      </div>
    </div>
  );
}










