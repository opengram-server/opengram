import { useState } from 'react'
import { Download, Upload, AlertCircle, CheckCircle, Loader } from 'lucide-react'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:3001'

export default function EmojiPacksBackup() {
  const [importing, setImporting] = useState(false)
  const [exporting, setExporting] = useState(false)
  const [result, setResult] = useState(null)
  const [error, setError] = useState(null)

  const handleExport = async () => {
    try {
      setExporting(true)
      setError(null)
      setResult(null)

      const response = await fetch(`${API_URL}/api/emojipacks/export`)
      
      if (!response.ok) {
        throw new Error('Export failed')
      }

      // Get filename from Content-Disposition header
      const contentDisposition = response.headers.get('Content-Disposition')
      const filenameMatch = contentDisposition?.match(/filename="(.+)"/)
      const filename = filenameMatch ? filenameMatch[1] : `emoji-packs-export-${Date.now()}.json`

      // Download file
      const blob = await response.blob()
      const url = window.URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = filename
      document.body.appendChild(a)
      a.click()
      window.URL.revokeObjectURL(url)
      document.body.removeChild(a)

      setResult({
        type: 'export',
        message: 'Export successful! File downloaded.'
      })
    } catch (err) {
      setError(err.message)
    } finally {
      setExporting(false)
    }
  }

  const handleImportMetadata = async (e) => {
    const file = e.target.files?.[0]
    if (!file) return

    try {
      setImporting(true)
      setError(null)
      setResult(null)

      const formData = new FormData()
      formData.append('file', file)

      const response = await fetch(`${API_URL}/api/emojipacks/import/metadata`, {
        method: 'POST',
        body: formData
      })

      const data = await response.json()

      if (!response.ok) {
        throw new Error(data.error || 'Import failed')
      }

      setResult({
        type: 'metadata',
        data
      })
    } catch (err) {
      setError(err.message)
    } finally {
      setImporting(false)
      e.target.value = ''
    }
  }

  const handleImportFiles = async (e) => {
    const file = e.target.files?.[0]
    if (!file) return

    try {
      setImporting(true)
      setError(null)

      const formData = new FormData()
      formData.append('file', file)

      const response = await fetch(`${API_URL}/api/emojipacks/import/files`, {
        method: 'POST',
        body: formData
      })

      const data = await response.json()

      if (!response.ok) {
        throw new Error(data.error || 'Import failed')
      }

      setResult({
        type: 'files',
        data
      })
    } catch (err) {
      setError(err.message)
    } finally {
      setImporting(false)
      e.target.value = ''
    }
  }

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-2xl font-bold mb-2">Emoji Packs Backup</h1>
        <p className="text-[#8b98a5]">
          Export all emoji packs to JSON file or import them back after docker-compose down
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {/* Export Card */}
        <div className="card rounded-lg shadow p-6">
          <div className="flex items-center gap-3 mb-4">
            <Download className="w-6 h-6 text-blue-400" />
            <h2 className="text-xl font-semibold">Export Packs</h2>
          </div>
          
          <p className="text-[#8b98a5] mb-4">
            Download all emoji packs with metadata and TGS files as a ZIP archive. 
            Use this before running docker-compose down.
          </p>

          <button
            onClick={handleExport}
            disabled={exporting}
            className="w-full bg-[#5288c1] text-white px-4 py-2 rounded hover:bg-[#3d5a7a] disabled:bg-gray-400 disabled:cursor-not-allowed flex items-center justify-center gap-2"
          >
            {exporting ? (
              <>
                <Loader className="w-4 h-4 animate-spin" />
                Exporting...
              </>
            ) : (
              <>
                <Download className="w-4 h-4" />
                Export All Packs
              </>
            )}
          </button>
        </div>

        {/* Import Card */}
        <div className="card rounded-lg shadow p-6">
          <div className="flex items-center gap-3 mb-4">
            <Upload className="w-6 h-6 text-green-600" />
            <h2 className="text-xl font-semibold">Import Packs (2 Steps)</h2>
          </div>
          
          <p className="text-[#8b98a5] mb-4">
            Step 1: Upload metadata.json<br/>
            Step 2: Upload ZIP with files folder
          </p>

          <div className="space-y-3">
            <label className="w-full bg-[#5288c1] text-white px-4 py-2 rounded hover:bg-[#3d5a7a] disabled:bg-gray-400 disabled:cursor-not-allowed flex items-center justify-center gap-2 cursor-pointer">
              {importing ? (
                <>
                  <Loader className="w-4 h-4 animate-spin" />
                  Importing...
                </>
              ) : (
                <>
                  <Upload className="w-4 h-4" />
                  1. Import Metadata (JSON)
                </>
              )}
              <input
                type="file"
                accept=".json"
                onChange={handleImportMetadata}
                disabled={importing}
                className="hidden"
              />
            </label>

            <label className="w-full bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600 disabled:bg-gray-400 disabled:cursor-not-allowed flex items-center justify-center gap-2 cursor-pointer">
              {importing ? (
                <>
                  <Loader className="w-4 h-4 animate-spin" />
                  Importing...
                </>
              ) : (
                <>
                  <Upload className="w-4 h-4" />
                  2. Import Files (ZIP)
                </>
              )}
              <input
                type="file"
                accept=".zip"
                onChange={handleImportFiles}
                disabled={importing}
                className="hidden"
              />
            </label>
          </div>
        </div>
      </div>

      {/* Results */}
      {result && (
        <div className="mt-6 bg-green-500/20 border border-green-500/30 rounded-lg p-4">
          <div className="flex items-start gap-3">
            <CheckCircle className="w-5 h-5 text-green-600 mt-0.5" />
            <div className="flex-1">
              <h3 className="font-semibold text-green-900 mb-2">
                {result.type === 'export' && 'Export Successful'}
                {result.type === 'metadata' && 'Metadata Imported'}
                {result.type === 'files' && 'Files Imported'}
              </h3>
              
              {result.type === 'export' && (
                <p className="text-green-400">{result.message}</p>
              )}
              
              {result.type === 'metadata' && (
                <div className="text-green-400 space-y-1">
                  <p>✅ Imported {result.data.imported.packs} packs</p>
                  <p>✅ Imported {result.data.imported.documents} documents</p>
                  {result.data.skipped.packs > 0 && (
                    <p>⏭️  Skipped {result.data.skipped.packs} existing packs</p>
                  )}
                  <p className="mt-2 font-semibold">➡️ Now upload TGS files (Step 2)</p>
                </div>
              )}
              
              {result.type === 'files' && (
                <div className="text-green-400 space-y-1">
                  <p>✅ Uploaded {result.data.uploaded} TGS files</p>
                  {result.data.skipped > 0 && (
                    <p>⏭️  Skipped {result.data.skipped} existing files</p>
                  )}
                  <p className="mt-2 font-semibold">🎉 Import complete!</p>
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Error */}
      {error && (
        <div className="mt-6 bg-red-500/20 border border-red-500/30 rounded-lg p-4">
          <div className="flex items-start gap-3">
            <AlertCircle className="w-5 h-5 text-red-400 mt-0.5" />
            <div>
              <h3 className="font-semibold text-red-900 mb-1">Error</h3>
              <p className="text-red-700">{error}</p>
            </div>
          </div>
        </div>
      )}

      {/* Instructions */}
      <div className="mt-8 bg-blue-50 border border-blue-500/30 rounded-lg p-6">
        <h3 className="font-semibold text-blue-900 mb-3">📝 How to use:</h3>
        <ol className="list-decimal list-inside space-y-2 text-blue-300">
          <li>
            <strong>Before docker-compose down:</strong> Click "Export All Packs" to download a ZIP backup
          </li>
          <li>
            <strong>After restarting:</strong> Click "Import from ZIP" and select your backup file
          </li>
          <li>
            All packs, documents, and TGS files will be restored automatically
          </li>
          <li>
            Existing packs and files will be skipped (no duplicates)
          </li>
        </ol>
        
        <div className="mt-4 p-3 bg-yellow-500/20 border border-yellow-500/30 rounded">
          <p className="text-yellow-400 text-sm">
            <strong>✅ Complete Backup:</strong> The ZIP file contains both MongoDB data (metadata) 
            and MinIO files (TGS animations). You can safely delete Docker volumes after export.
          </p>
        </div>
      </div>
    </div>
  )
}









