import { useState, useEffect } from 'react';
import { toast } from 'react-hot-toast';
import { Trash2, Upload, Sparkles, Image as ImageIcon, FileArchive, ChevronDown, Package, Gift, Settings } from 'lucide-react';

const API_URL = import.meta.env.VITE_API_URL || '';

const Attributes = () => {
    const [gifts, setGifts] = useState([]);
    const [selectedGiftId, setSelectedGiftId] = useState('');
    const [attributeSets, setAttributeSets] = useState([]);
    const [loading, setLoading] = useState(true);
    const [uploading, setUploading] = useState(false);

    // Form state
    const [name, setName] = useState('');
    const [modelFile, setModelFile] = useState(null);
    const [patternFile, setPatternFile] = useState(null);
    const [modelName, setModelName] = useState('');
    const [patternName, setPatternName] = useState('');
    const [backdropName, setBackdropName] = useState(''); // Kept for logic, even if auto-generated
    const [modelRarity, setModelRarity] = useState(1000);
    const [patternRarity, setPatternRarity] = useState(1000);
    const [backdropRarity, setBackdropRarity] = useState(1000); // Kept for logic

    // ZIP upload state
    const [zipFile, setZipFile] = useState(null);
    const [zipGiftId, setZipGiftId] = useState('');
    const [uploadResult, setUploadResult] = useState(null);

    useEffect(() => {
        fetchGifts();
        fetchAttributeSets();
    }, []);

    const fetchGifts = async () => {
        try {
            const response = await fetch(`${API_URL}/api/gifts`);
            const data = await response.json();
            setGifts(data);
        } catch (error) {
            console.error('Error fetching gifts:', error);
            toast.error('Failed to load gifts');
        }
    };

    const fetchAttributeSets = async () => {
        try {
            setLoading(true);
            const response = await fetch(`${API_URL}/api/attributes`);
            const data = await response.json();
            setAttributeSets(data);
        } catch (error) {
            console.error('Error fetching attribute sets:', error);
            toast.error('Failed to fetch attribute sets');
        } finally {
            setLoading(false);
        }
    };

    const handleZipUpload = async () => {
        if (!zipFile || !zipGiftId) {
            toast.error('Please select a ZIP file and enter Gift ID');
            return;
        }

        try {
            setUploading(true);
            setUploadResult(null);

            const formData = new FormData();
            formData.append('file', zipFile);
            formData.append('giftId', zipGiftId);

            const response = await fetch(`${API_URL}/api/attributes/upload-zip`, {
                method: 'POST',
                body: formData
            });

            const data = await response.json();

            setUploadResult(data);
            if (data.success) {
                toast.success(`Successfully processed ${data.upgradesProcessed} upgrades!`);
                setZipFile(null);
                setZipGiftId('');
                await fetchAttributeSets();
            } else {
                toast.error(data.error || 'Upload failed');
            }

        } catch (error) {
            console.error('ZIP upload error:', error);
            setUploadResult({
                success: false,
                error: error.message
            });
            toast.error('ZIP upload failed: ' + error.message);
        } finally {
            setUploading(false);
        }
    };

    const handleUpload = async (e) => {
        e.preventDefault();
        if (!selectedGiftId) {
            toast.error('Please select a Gift first!');
            return;
        }
        if (!name) {
            toast.error('Name is required');
            return;
        }

        try {
            setUploading(true);
            const formData = new FormData();
            formData.append('giftId', selectedGiftId);
            formData.append('name', name);
            formData.append('modelName', modelName || `${name} Model`);
            formData.append('patternName', patternName || `${name} Pattern`);
            formData.append('backdropName', backdropName || `${name} Backdrop`);
            formData.append('modelRarity', modelRarity);
            formData.append('patternRarity', patternRarity);
            formData.append('backdropRarity', backdropRarity);

            if (modelFile) formData.append('model', modelFile);
            if (patternFile) formData.append('pattern', patternFile);

            const response = await fetch(`${API_URL}/api/attributes`, {
                method: 'POST',
                body: formData
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.error || 'Failed to upload attribute set');
            }

            // Reset form
            setName('');
            setModelFile(null);
            setPatternFile(null);
            setModelName('');
            setPatternName('');
            setBackdropName('');
            setModelRarity(1000);
            setPatternRarity(1000);
            setBackdropRarity(1000);

            // Refresh list
            fetchAttributeSets();
            toast.success('Attribute set uploaded successfully!');
        } catch (error) {
            console.error('Error uploading attribute set:', error);
            toast.error(error.message);
        } finally {
            setUploading(false);
        }
    };

    const handleDelete = async (id) => {
        if (!confirm('Are you sure you want to delete this attribute set?')) return;

        try {
            const response = await fetch(`${API_URL}/api/attributes/${id}`, {
                method: 'DELETE'
            });

            if (response.ok) {
                setAttributeSets(attributeSets.filter(s => s._id !== id));
                toast.success('Attribute set deleted');
            } else {
                throw new Error('Failed to delete');
            }
        } catch (error) {
            console.error('Error deleting attribute set:', error);
            toast.error('Failed to delete attribute set');
        }
    };

    const selectedGift = gifts.find(g => g.GiftId?.toString() === selectedGiftId);

    return (
        <div className="space-y-6 animate-fade-in">
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                <div>
                    <h1 className="text-3xl font-heading font-bold text-fg flex items-center gap-3">
                        <div className="p-2.5 bg-yellow/10 rounded-xl rounded-tr-none">
                            <Sparkles className="w-8 h-8 text-yellow" />
                        </div>
                        Gift Upgrade Attributes
                    </h1>
                    <p className="text-fg-muted mt-2">Manage models, patterns, and backdrops for gift upgrades</p>
                </div>
            </div>

            {/* ZIP Upload Section */}
            <div className="card bg-gradient-to-br from-bg-panel to-blue/5 border-blue/20">
                <div className="flex items-start justify-between">
                    <div>
                        <h2 className="text-xl font-heading font-bold mb-2 flex items-center gap-2 text-fg">
                            <Package className="text-blue" />
                            Bulk Upload from ZIP
                        </h2>
                        <p className="text-fg-muted text-sm max-w-2xl">
                            Upload a ZIP file containing <code className="bg-bg-app px-1.5 py-0.5 rounded text-xs">info.json</code> and <code className="bg-bg-app px-1.5 py-0.5 rounded text-xs">.tgs</code> files to automatically import multiple upgrades at once.
                        </p>
                    </div>
                </div>

                <div className="mt-6 grid grid-cols-1 md:grid-cols-3 gap-4 items-end">
                    <div>
                        <label className="block text-xs font-bold text-fg-muted uppercase tracking-wider mb-2">Gift ID</label>
                        <input
                            type="number"
                            value={zipGiftId}
                            onChange={(e) => setZipGiftId(e.target.value)}
                            placeholder="e.g., 23"
                            className="input"
                        />
                    </div>

                    <div>
                        <label className="block text-xs font-bold text-fg-muted uppercase tracking-wider mb-2">ZIP File</label>
                        <input
                            type="file"
                            accept=".zip"
                            onChange={(e) => setZipFile(e.target.files[0])}
                            className="w-full text-sm text-fg-muted
                            file:mr-4 file:py-2.5 file:px-4
                            file:rounded-lg file:border-0
                            file:text-sm file:font-semibold
                            file:bg-blue/10 file:text-blue
                            hover:file:bg-blue/20
                            cursor-pointer"
                        />
                    </div>

                    <button
                        onClick={handleZipUpload}
                        disabled={!zipFile || !zipGiftId || uploading}
                        className="btn btn-primary h-[46px]"
                    >
                        {uploading ? 'Processing...' : 'Upload ZIP Archive'}
                    </button>
                </div>

                {zipFile && (
                    <div className="mt-3 flex items-center gap-2 text-sm text-fg-muted bg-bg-app/50 p-2 rounded-lg w-fit">
                        <FileArchive className="w-4 h-4 text-accent" />
                        <span className="font-medium text-fg">{zipFile.name}</span>
                        <span>({(zipFile.size / 1024 / 1024).toFixed(2)} MB)</span>
                    </div>
                )}

                {uploadResult && (
                    <div className={`mt-4 p-4 rounded-xl border ${uploadResult.success ? 'bg-success/10 border-success/20' : 'bg-red-500/10 border-red-500/20'} animate-fade-in`}>
                        {uploadResult.success ? (
                            <div className="flex items-center gap-3">
                                <div className="p-2 bg-success/20 rounded-full text-success">
                                    <Package className="w-5 h-5" />
                                </div>
                                <div>
                                    <p className="font-bold text-success">Import Successful</p>
                                    <p className="text-sm text-fg-muted">Processed <strong>{uploadResult.upgradesProcessed}</strong> upgrades from {uploadResult.title}</p>
                                </div>
                            </div>
                        ) : (
                            <div className="flex items-center gap-3">
                                <div className="p-2 bg-red-500/20 rounded-full text-red-500">
                                    <Trash2 className="w-5 h-5" />
                                </div>
                                <div>
                                    <p className="font-bold text-red-500">Import Failed</p>
                                    <p className="text-sm text-fg-muted opacity-80">{uploadResult.error}</p>
                                </div>
                            </div>
                        )}
                    </div>
                )}
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                {/* Upload Form */}
                <div className="card h-fit lg:sticky lg:top-6">
                    <h2 className="text-xl font-heading font-bold mb-6 flex items-center gap-2 text-fg">
                        <Settings className="w-5 h-5 text-accent" />
                        Manual Creation
                    </h2>
                    <form onSubmit={handleUpload} className="space-y-6">
                        {/* Gift Selection */}
                        <div className="pb-6 border-b border-border">
                            <label className="block text-sm font-bold text-accent mb-2">1. Select Gift for Upgrade</label>
                            <div className="relative">
                                <select
                                    value={selectedGiftId}
                                    onChange={(e) => setSelectedGiftId(e.target.value)}
                                    className="input appearance-none cursor-pointer"
                                    required
                                >
                                    <option value="">-- Choose a Gift --</option>
                                    {gifts.map((gift) => (
                                        <option key={gift._id} value={gift.GiftId}>
                                            {gift.Title || `Gift #${gift.GiftId}`} ({gift.Stars} ⭐)
                                        </option>
                                    ))}
                                </select>
                                <ChevronDown className="absolute right-3 top-1/2 transform -translate-y-1/2 text-fg-muted pointer-events-none" size={18} />
                            </div>
                            {selectedGift && (
                                <div className="mt-3 p-3 bg-accent/5 border border-accent/10 rounded-xl">
                                    <div className="flex items-center gap-2 mb-1">
                                        <Gift className="w-4 h-4 text-accent" />
                                        <span className="text-fg font-bold text-sm">{selectedGift.Title || `Gift #${selectedGift.GiftId}`}</span>
                                    </div>
                                    <div className="text-xs text-fg-muted pl-6">{selectedGift.Stars} Stars • Upgrade Cost: {selectedGift.UpgradeStars} Stars</div>
                                </div>
                            )}
                        </div>

                        <div>
                            <label className="block text-sm font-bold text-accent mb-4">2. Configure Upgrade Attributes</label>
                            <div className="space-y-4">
                                <div>
                                    <label className="block text-xs font-medium text-fg-muted mb-1.5">Set Name</label>
                                    <input
                                        type="text"
                                        value={name}
                                        onChange={(e) => setName(e.target.value)}
                                        className="input"
                                        placeholder="e.g., Golden Rare Set"
                                        required
                                        disabled={!selectedGiftId}
                                    />
                                </div>

                                {/* Model (TGS) */}
                                <div className="bg-bg-app/50 border border-border rounded-xl p-4">
                                    <h3 className="font-bold text-sm text-fg mb-3 flex items-center gap-2">
                                        <FileArchive size={16} className="text-blue" />
                                        Model (Upgraded Gift TGS)
                                    </h3>
                                    <div className="space-y-3">
                                        <div className="relative group">
                                            <input
                                                type="file"
                                                onChange={(e) => setModelFile(e.target.files[0])}
                                                className="absolute inset-0 w-full h-full opacity-0 cursor-pointer disabled:cursor-not-allowed z-10"
                                                accept=".tgs"
                                                disabled={!selectedGiftId}
                                            />
                                            <div className={`border-2 border-dashed rounded-lg p-4 text-center transition-colors ${modelFile ? 'border-success bg-success/5' : 'border-border group-hover:border-blue group-hover:bg-blue/5'
                                                }`}>
                                                {modelFile ? (
                                                    <span className="text-success text-xs font-bold truncate block">{modelFile.name}</span>
                                                ) : (
                                                    <div className="flex flex-col items-center text-fg-muted">
                                                        <Upload size={16} className="mb-1" />
                                                        <span className="text-[10px] uppercase font-bold tracking-wider">Upload TGS</span>
                                                    </div>
                                                )}
                                            </div>
                                        </div>
                                        <div>
                                            <label className="block text-[10px] font-bold text-fg-muted uppercase mb-1">Model Name</label>
                                            <input
                                                type="text"
                                                value={modelName}
                                                onChange={(e) => setModelName(e.target.value)}
                                                className="input py-2 text-sm"
                                                placeholder={`${name || 'Gift'} Model`}
                                                disabled={!selectedGiftId}
                                            />
                                        </div>
                                        <div>
                                            <label className="block text-[10px] font-bold text-fg-muted uppercase mb-1">Rarity (‰)</label>
                                            <input
                                                type="number"
                                                value={modelRarity}
                                                onChange={(e) => setModelRarity(e.target.value)}
                                                className="input py-2 text-sm"
                                                placeholder="1000"
                                                disabled={!selectedGiftId}
                                            />
                                        </div>
                                    </div>
                                </div>

                                {/* Pattern (PNG or TGS) */}
                                <div className="bg-bg-app/50 border border-border rounded-xl p-4">
                                    <h3 className="font-bold text-sm text-fg mb-3 flex items-center gap-2">
                                        <ImageIcon size={16} className="text-purple" />
                                        Pattern (PNG or TGS)
                                    </h3>
                                    <div className="space-y-3">
                                        <div className="relative group">
                                            <input
                                                type="file"
                                                onChange={(e) => setPatternFile(e.target.files[0])}
                                                className="absolute inset-0 w-full h-full opacity-0 cursor-pointer disabled:cursor-not-allowed z-10"
                                                accept=".png,.tgs"
                                                disabled={!selectedGiftId}
                                            />
                                            <div className={`border-2 border-dashed rounded-lg p-4 text-center transition-colors ${patternFile ? 'border-success bg-success/5' : 'border-border group-hover:border-purple group-hover:bg-purple/5'
                                                }`}>
                                                {patternFile ? (
                                                    <span className="text-success text-xs font-bold truncate block">{patternFile.name}</span>
                                                ) : (
                                                    <div className="flex flex-col items-center text-fg-muted">
                                                        <Upload size={16} className="mb-1" />
                                                        <span className="text-[10px] uppercase font-bold tracking-wider">Upload PNG/TGS</span>
                                                    </div>
                                                )}
                                            </div>
                                        </div>
                                        <div>
                                            <label className="block text-[10px] font-bold text-fg-muted uppercase mb-1">Pattern Name</label>
                                            <input
                                                type="text"
                                                value={patternName}
                                                onChange={(e) => setPatternName(e.target.value)}
                                                className="input py-2 text-sm"
                                                placeholder={`${name || 'Gift'} Pattern`}
                                                disabled={!selectedGiftId}
                                            />
                                        </div>
                                        <div>
                                            <label className="block text-[10px] font-bold text-fg-muted uppercase mb-1">Rarity (‰)</label>
                                            <input
                                                type="number"
                                                value={patternRarity}
                                                onChange={(e) => setPatternRarity(e.target.value)}
                                                className="input py-2 text-sm"
                                                placeholder="1000"
                                                disabled={!selectedGiftId}
                                            />
                                        </div>
                                    </div>
                                </div>

                                {/* Backdrop Info */}
                                <div className="bg-blue/5 border border-blue/20 rounded-xl p-4">
                                    <h3 className="font-bold text-sm mb-2 flex items-center gap-2 text-blue">
                                        <Sparkles size={16} />
                                        Backdrop (Auto-generated)
                                    </h3>
                                    <p className="text-xs text-fg-muted leading-relaxed">
                                        ✨ Backdrop colors are <strong>automatically generated</strong> using a deterministic algorithm based on the upgrade instance ID.
                                    </p>
                                </div>

                                <button
                                    type="submit"
                                    disabled={uploading || !selectedGiftId}
                                    className="btn btn-primary w-full"
                                >
                                    {uploading ? 'Uploading...' : 'Create Attribute Set'}
                                </button>
                            </div>
                        </div>
                    </form>
                </div>

                {/* List */}
                <div className="lg:col-span-2">
                    <div className="card overflow-hidden p-0">
                        <div className="overflow-x-auto">
                            <table className="w-full text-left">
                                <thead className="bg-bg-app border-b border-border">
                                    <tr>
                                        <th className="p-4 text-xs font-bold text-fg-muted uppercase tracking-wider">Gift</th>
                                        <th className="p-4 text-xs font-bold text-fg-muted uppercase tracking-wider">Name</th>
                                        <th className="p-4 text-xs font-bold text-fg-muted uppercase tracking-wider">Model</th>
                                        <th className="p-4 text-xs font-bold text-fg-muted uppercase tracking-wider">Pattern</th>
                                        <th className="p-4 text-xs font-bold text-fg-muted uppercase tracking-wider text-right">Actions</th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-border">
                                    {loading ? (
                                        <tr>
                                            <td colSpan="5" className="p-12 text-center text-fg-muted">
                                                <div className="w-8 h-8 border-2 border-accent border-t-transparent rounded-full animate-spin mx-auto mb-2"></div>
                                                Loading attributes...
                                            </td>
                                        </tr>
                                    ) : attributeSets.length === 0 ? (
                                        <tr>
                                            <td colSpan="5" className="p-12 text-center text-fg-muted">
                                                No attribute sets found. Create one to get started.
                                            </td>
                                        </tr>
                                    ) : (
                                        attributeSets.map((set) => (
                                            <tr key={set._id} className="hover:bg-bg-app/50 transition-colors">
                                                <td className="p-4">
                                                    <div className="inline-flex items-center px-2 py-1 rounded bg-accent/10 border border-accent/20 text-accent font-mono text-xs">
                                                        #{set.GiftId}
                                                    </div>
                                                </td>
                                                <td className="p-4 font-medium text-fg">{set.Name}</td>
                                                <td className="p-4">
                                                    {set.Model ? (
                                                        <div className="flex flex-col gap-1">
                                                            <div className="flex items-center gap-1.5 text-xs text-blue font-medium">
                                                                <FileArchive size={12} />
                                                                TGS
                                                            </div>
                                                            <span className="text-[10px] px-1.5 py-0.5 bg-bg-app rounded text-fg-muted w-fit">
                                                                {set.Model.RarityPermille}‰
                                                            </span>
                                                        </div>
                                                    ) : (
                                                        <span className="text-fg-muted opacity-50">-</span>
                                                    )}
                                                </td>
                                                <td className="p-4">
                                                    {set.Pattern ? (
                                                        <div className="flex flex-col gap-1">
                                                            <div className="flex items-center gap-1.5 text-xs text-purple font-medium">
                                                                <ImageIcon size={12} />
                                                                TGS/PNG
                                                            </div>
                                                            <span className="text-[10px] px-1.5 py-0.5 bg-bg-app rounded text-fg-muted w-fit">
                                                                {set.Pattern.RarityPermille}‰
                                                            </span>
                                                        </div>
                                                    ) : (
                                                        <span className="text-fg-muted opacity-50">-</span>
                                                    )}
                                                </td>
                                                <td className="p-4 text-right">
                                                    <button
                                                        onClick={() => handleDelete(set._id)}
                                                        className="p-2 text-fg-muted hover:text-red-500 hover:bg-red-500/10 rounded-lg transition-colors"
                                                        title="Delete Attribute Set"
                                                    >
                                                        <Trash2 size={18} />
                                                    </button>
                                                </td>
                                            </tr>
                                        ))
                                    )}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Attributes;
