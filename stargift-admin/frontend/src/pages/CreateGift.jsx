import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { Upload, ArrowLeft, Sparkles } from 'lucide-react'
import useGiftsStore from '../store/useGiftsStore'
import Lottie from 'lottie-react'
import pako from 'pako'

export default function CreateGift() {
  const navigate = useNavigate()
  const { createGift } = useGiftsStore()
  const { register, handleSubmit, watch, formState: { errors } } = useForm()
  const [stickerFile, setStickerFile] = useState(null)
  const [stickerPreview, setStickerPreview] = useState(null)
  const [submitting, setSubmitting] = useState(false)

  const limited = watch('limited')

  const handleFileChange = (e) => {
    const file = e.target.files[0]
    if (file) {
      setStickerFile(file)

      // Read file for preview (TGS files are gzipped JSON)
      const reader = new FileReader()
      reader.onload = (event) => {
        try {
          const arrayBuffer = event.target.result
          const uint8Array = new Uint8Array(arrayBuffer)

          // Try to decompress as gzip (TGS format)
          try {
            const decompressed = pako.inflate(uint8Array, { to: 'string' })
            const jsonData = JSON.parse(decompressed)
            setStickerPreview(jsonData)
          } catch (gzipErr) {
            // If not gzipped, try to parse as plain JSON
            const text = new TextDecoder().decode(uint8Array)
            const jsonData = JSON.parse(text)
            setStickerPreview(jsonData)
          }
        } catch (err) {
          console.error('Failed to parse JSON:', err)
          setStickerPreview(null)
          // toast.error('Invalid TGS file format')
        }
      }
      reader.readAsArrayBuffer(file)
    }
  }

  const onSubmit = async (rawData) => {
    setSubmitting(true)
    try {
      // Process data to ensure correct types
      const stars = Number(rawData.stars);
      const data = {
        ...rawData,
        giftId: Number(rawData.giftId),
        stars: stars,
        convertStars: rawData.limited
          ? Number(rawData.convertStars)
          : Math.floor(stars * 0.7) || 1,

        upgradeStars: rawData.limited
          ? Number(rawData.upgradeStars)
          : null, // Explicitly null for non-limited

        stickerId: rawData.stickerId ? rawData.stickerId : undefined
      }

      if (rawData.limited) {
        data.availabilityTotal = Number(rawData.availabilityTotal)
        data.availabilityRemains = rawData.availabilityRemains
          ? Number(rawData.availabilityRemains)
          : Number(rawData.availabilityTotal)
      } else {
        delete data.availabilityTotal
        delete data.availabilityRemains
      }

      const formData = new FormData()
      formData.append('data', JSON.stringify(data))
      if (stickerFile) {
        formData.append('sticker', stickerFile)
        console.log('Uploading sticker:', stickerFile.name, stickerFile.size, 'bytes')
      } else {
        console.warn('No sticker file selected')
      }

      await createGift(formData)
      navigate('/gifts')
    } catch (error) {
      console.error('Create gift error:', error)
      console.error('Error response:', error.response?.data)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="max-w-3xl mx-auto">
      <button onClick={() => navigate('/gifts')} className="btn btn-secondary mb-6 flex items-center gap-2">
        <ArrowLeft className="w-4 h-4" />
        Back to Gifts
      </button>

      <div className="card">
        <div className="flex items-center gap-3 mb-6">
          <div className="bg-gradient-to-br from-primary-500 to-primary-600 p-3 rounded-lg">
            <Sparkles className="w-6 h-6 text-white" />
          </div>
          <div>
            <h1 className="text-2xl font-bold">Create New Gift</h1>
            <p className="text-[#8b98a5]">Add a new gift to the catalog</p>
          </div>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {/* Gift ID */}
          <div>
            <label className="label">Gift ID *</label>
            <input
              type="number"
              className="input"
              {...register('giftId', { required: 'Gift ID is required', min: 1 })}
            />
            {errors.giftId && <p className="text-red-500 text-sm mt-1">{errors.giftId.message}</p>}
          </div>

          {/* Title */}
          <div>
            <label className="label">Title</label>
            <input
              type="text"
              className="input"
              placeholder="e.g., Golden Star, Blue Diamond..."
              {...register('title')}
            />
          </div>

          {/* Sticker Upload */}
          <div>
            <label className="label">Sticker Animation (JSON/TGS)</label>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className={`border-2 border-dashed rounded-lg p-6 text-center transition-colors ${stickerFile ? 'border-green-500 bg-green-500/20' : 'border-[#2b5278] hover:border-primary-500'
                }`}>
                <input
                  type="file"
                  accept=".json,.tgs"
                  onChange={handleFileChange}
                  className="hidden"
                  id="sticker-upload"
                />
                <label htmlFor="sticker-upload" className="cursor-pointer">
                  <Upload className={`w-12 h-12 mx-auto mb-2 ${stickerFile ? 'text-green-600' : 'text-[#8b98a5]'}`} />
                  <p className={`text-[#8b98a5] font-medium ${stickerFile ? 'text-green-400' : ''}`}>
                    {stickerFile ? `✓ ${stickerFile.name}` : 'Click to upload sticker'}
                  </p>
                  {stickerFile && (
                    <p className="text-sm text-green-600 mt-1">
                      {(stickerFile.size / 1024).toFixed(2)} KB
                    </p>
                  )}
                  <p className="text-sm text-[#8b98a5] mt-1">JSON or TGS (Max: 10MB)</p>
                </label>
              </div>

              {/* Preview */}
              {stickerPreview && (
                <div className="border border-border rounded-lg p-6 flex items-center justify-center bg-bg-app shadow-inner">
                  <Lottie
                    animationData={stickerPreview}
                    loop={true}
                    style={{ width: 150, height: 150 }}
                  />
                </div>
              )}
            </div>

            {/* Manual Sticker ID */}
            <div className="mt-4">
              <label className="label uppercase tracking-wider text-xs font-bold text-fg-muted mb-2 block">Or Enter Existing Sticker ID (Debug)</label>
              <input
                type="text"
                className="input font-mono text-sm"
                placeholder="e.g. 17673446594580884"
                {...register('stickerId')}
              />
              <p className="text-xs text-fg-muted mt-1">Overrides file upload if set. Use to reuse existing stickers.</p>
            </div>
          </div>

          {/* Stars Pricing */}
          {/* Stars Pricing */}
          <div className={`grid grid-cols-1 ${limited ? 'md:grid-cols-3' : 'md:grid-cols-1'} gap-6`}>
            <div>
              <label className="label uppercase tracking-wider text-xs font-bold text-fg-muted mb-2 block">Price (Stars) *</label>
              <input
                type="number"
                className="input text-yellow-500 font-bold"
                {...register('stars', { required: true, min: 1 })}
              />
            </div>

            {limited && (
              <>
                <div className="animate-fade-in">
                  <label className="label uppercase tracking-wider text-xs font-bold text-fg-muted mb-2 block">Convert Stars *</label>
                  <input
                    type="number"
                    className="input"
                    {...register('convertStars', { required: 'Convert price is required for limited gifts', min: 1 })}
                  />
                </div>
                <div className="animate-fade-in">
                  <label className="label uppercase tracking-wider text-xs font-bold text-fg-muted mb-2 block">Upgrade Stars</label>
                  <input
                    type="number"
                    className="input text-purple-400"
                    {...register('upgradeStars', { min: 0 })}
                  />
                </div>
              </>
            )}
          </div>

          {/* Gift Type */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                id="limited"
                className="w-4 h-4 text-primary-600 rounded"
                {...register('limited')}
              />
              <label htmlFor="limited" className="text-sm font-medium">Limited Edition</label>
            </div>
            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                id="birthday"
                className="w-4 h-4 text-primary-600 rounded"
                {...register('birthday')}
              />
              <label htmlFor="birthday" className="text-sm font-medium">Birthday Gift</label>
            </div>
            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                id="soldOut"
                className="w-4 h-4 text-primary-600 rounded"
                {...register('soldOut')}
              />
              <label htmlFor="soldOut" className="text-sm font-medium">Mark as Sold Out</label>
            </div>
          </div>

          {/* Limited Edition Supply */}
          {limited && (
            <div className="bg-purple-50 border border-purple-200 rounded-lg p-4">
              <h3 className="font-medium text-purple-900 mb-3">Limited Edition Settings</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="label">Total Supply *</label>
                  <input
                    type="number"
                    className="input"
                    {...register('availabilityTotal', { min: 1 })}
                  />
                </div>
                <div>
                  <label className="label">Remaining Supply</label>
                  <input
                    type="number"
                    className="input"
                    placeholder="Leave empty to use total"
                    {...register('availabilityRemains', { min: 0 })}
                  />
                </div>
              </div>
            </div>
          )}

          {/* Buttons */}
          <div className="flex gap-4">
            <button
              type="submit"
              disabled={submitting}
              className="btn btn-primary flex-1"
            >
              {submitting ? 'Creating...' : 'Create Gift'}
            </button>
            <button
              type="button"
              onClick={() => navigate('/gifts')}
              className="btn btn-secondary"
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}









