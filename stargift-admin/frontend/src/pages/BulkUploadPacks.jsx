import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowLeft, Upload, FileArchive, CheckCircle, XCircle } from 'lucide-react';
import { toast } from 'react-hot-toast';

export default function BulkUploadPacks() {
    const [file, setFile] = useState(null);
    const [uploading, setUploading] = useState(false);
    const [progress, setProgress] = useState({ current: 0, total: 0, pack: '' });
    const [result, setResult] = useState(null);
    const navigate = useNavigate();

    const handleFileChange = (e) => {
        const selectedFile = e.target.files[0];
        if (selectedFile && selectedFile.name.endsWith('.zip')) {
            setFile(selectedFile);
        } else {
            toast.error('Please select a ZIP file');
            setFile(null);
        }
    };

    const handleUpload = async () => {
        if (!file) {
            toast.error('Please select a file first');
            return;
        }

        setUploading(true);
        setResult(null);
        setProgress({ current: 0, total: 0, pack: 'Starting upload...' });

        const formData = new FormData();
        formData.append('zipFile', file);

        try {
            const response = await fetch('/api/stickerpacks/bulk-upload-nested', {
                method: 'POST',
                body: formData,
            });

            const data = await response.json();

            if (response.ok) {
                setResult(data);
                setProgress({ current: data.createdPacks, total: data.totalPacks, pack: 'Complete!' });
                toast.success('Bulk upload completed!');
            } else {
                toast.error(data.error || 'Upload failed');
            }
        } catch (err) {
            toast.error(err.message || 'Network error');
        } finally {
            setUploading(false);
        }
    };

    return (
        <div className="max-w-4xl mx-auto space-y-6 animate-fade-in">
            {/* Header */}
            <div>
                <button
                    onClick={() => navigate('/stickerpacks')}
                    className="inline-flex items-center text-fg-muted hover:text-purple mb-4 transition-colors"
                >
                    <ArrowLeft className="w-5 h-5 mr-2" />
                    Back to Sticker Packs
                </button>
                <h1 className="text-3xl font-heading font-bold text-fg flex items-center gap-3">
                    <FileArchive className="w-8 h-8 text-purple" />
                    Bulk Upload Sticker Packs
                </h1>
                <p className="text-fg-muted mt-1">
                    Upload a ZIP file containing multiple pack ZIPs with .tgs files
                </p>
            </div>

            {/* Upload Card */}
            <div className="card p-6">
                <div className="mb-6">
                    <h2 className="text-xl font-semibold text-fg mb-4">
                        📦 Select Main ZIP File
                    </h2>
                    <p className="text-sm text-fg-muted mb-4">
                        Structure: <code className="bg-muted px-2 py-1 rounded">featured_packs.zip</code> →
                        <code className="bg-muted px-2 py-1 rounded ml-2">DuckEmoji.zip</code> →
                        <code className="bg-muted px-2 py-1 rounded ml-2">🦆_12345.tgs</code>
                    </p>

                    <input
                        type="file"
                        accept=".zip"
                        onChange={handleFileChange}
                        disabled={uploading}
                        className="input w-full p-2 cursor-pointer"
                    />

                    {file && (
                        <div className="mt-3 p-3 bg-purple/10 border border-purple/20 rounded-lg">
                            <p className="text-sm text-purple">
                                ✓ Selected: <strong>{file.name}</strong> ({(file.size / 1024 / 1024).toFixed(2)} MB)
                            </p>
                        </div>
                    )}
                </div>

                <button
                    onClick={handleUpload}
                    disabled={!file || uploading}
                    className="btn btn-primary w-full py-3 text-lg font-semibold flex items-center justify-center gap-2"
                >
                    {uploading ? (
                        <>
                            <Upload className="w-5 h-5 animate-spin" />
                            Processing...
                        </>
                    ) : (
                        <>
                            <Upload className="w-5 h-5" />
                            Upload and Process
                        </>
                    )}
                </button>
            </div>

            {/* Progress */}
            {uploading && (
                <div className="card p-6">
                    <h3 className="text-lg font-semibold text-fg mb-4">Processing...</h3>
                    <div className="space-y-3">
                        <div className="flex justify-between text-sm text-fg-muted">
                            <span>{progress.pack}</span>
                            <span>{progress.current}/{progress.total}</span>
                        </div>
                        <div className="w-full bg-muted rounded-full h-3">
                            <div
                                className="bg-purple h-3 rounded-full transition-all duration-300"
                                style={{ width: progress.total > 0 ? `${(progress.current / progress.total) * 100}%` : '0%' }}
                            />
                        </div>
                    </div>
                </div>
            )}

            {/* Results */}
            {result && (
                <div className="card p-6 animate-fade-in">
                    <h3 className="text-2xl font-bold text-fg mb-6 flex items-center gap-2">
                        <CheckCircle className="w-8 h-8 text-success" />
                        Upload Complete!
                    </h3>

                    <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-8">
                        <div className="bg-success/5 border border-success/20 rounded-xl p-4 text-center">
                            <div className="text-3xl font-bold text-success">{result.createdPacks}</div>
                            <div className="text-sm text-fg-muted mt-1">Packs Created</div>
                        </div>
                        <div className="bg-blue/5 border border-blue/20 rounded-xl p-4 text-center">
                            <div className="text-3xl font-bold text-blue">{result.totalStickers}</div>
                            <div className="text-sm text-fg-muted mt-1">Stickers Uploaded</div>
                        </div>
                        <div className="bg-purple/5 border border-purple/20 rounded-xl p-4 text-center">
                            <div className="text-3xl font-bold text-purple">{result.totalPacks}</div>
                            <div className="text-sm text-fg-muted mt-1">Total Packs</div>
                        </div>
                        <div className="bg-red/5 border border-red/20 rounded-xl p-4 text-center">
                            <div className="text-3xl font-bold text-red">{result.failedPacks}</div>
                            <div className="text-sm text-fg-muted mt-1">Failed</div>
                        </div>
                    </div>

                    {/* Created Packs List */}
                    {result.packs && result.packs.length > 0 && (
                        <div className="mb-6">
                            <h4 className="font-semibold text-fg mb-3">Created Packs:</h4>
                            <div className="max-h-96 overflow-y-auto space-y-2 custom-scrollbar pr-2">
                                {result.packs.map((pack, idx) => (
                                    <div key={idx} className="flex justify-between items-center p-3 border border-border rounded-lg bg-card hover:border-purple transition-colors">
                                        <div>
                                            <div className="font-medium text-fg">{pack.title}</div>
                                            <div className="text-sm text-fg-muted">
                                                {pack.shortName} • ID: {pack.stickersetId}
                                            </div>
                                        </div>
                                        <div className="text-sm font-semibold text-purple">
                                            {pack.stickersCount} stickers
                                        </div>
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}

                    {/* Failed Packs */}
                    {result.failures && result.failures.length > 0 && (
                        <div>
                            <h4 className="font-semibold text-red mb-3">Failed Packs:</h4>
                            <div className="max-h-48 overflow-y-auto space-y-2 custom-scrollbar pr-2">
                                {result.failures.map((fail, idx) => (
                                    <div key={idx} className="p-3 bg-red/5 border border-red/20 rounded-lg">
                                        <div className="font-medium text-fg">{fail.pack}</div>
                                        <div className="text-sm text-red">{fail.reason}</div>
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}

                    <button
                        onClick={() => navigate('/stickerpacks')}
                        className="mt-6 w-full btn btn-secondary"
                    >
                        View All Sticker Packs →
                    </button>
                </div>
            )}
        </div>
    );
}
