import { useState, useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { Upload, ArrowLeft, Sparkles } from 'lucide-react'
import { giftsApi } from '../lib/api'
import useGiftsStore from '../store/useGiftsStore'
import toast from 'react-hot-toast'

export default function EditGift() {
  const { giftId } = useParams()
  const navigate = useNavigate()
  const { updateGift } = useGiftsStore()
  const { register, handleSubmit, watch, setValue, formState: { errors } } = useForm()
  const [stickerFile, setStickerFile] = useState(null)
  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)

  const limited = watch('Limited')

  useEffect(() => {
    loadGift()
  }, [giftId])

  const loadGift = async () => {
    try {
      const response = await giftsApi.getById(giftId)
      const gift = response.data

      setValue('title', gift.Title)
      setValue('stars', gift.Stars)
      setValue('convertStars', gift.ConvertStars)
      setValue('upgradeStars', gift.UpgradeStars)
      setValue('Limited', gift.Limited)
      setValue('birthday', gift.Birthday)
      setValue('SoldOut', gift.SoldOut)
      setValue('availabilityTotal', gift.AvailabilityTotal)
      setValue('AvailabilityRemains', gift.AvailabilityRemains)

      // Force infinite availability if not limited
      if (!gift.Limited) {
        setValue('availabilityTotal', 2000000000) // 2 Billion (effectively infinite)
        setValue('AvailabilityRemains', 2000000000)
        setValue('convertStars', 0)
        setValue('upgradeStars', 0)
      }
      setLoading(false)
    } catch (error) {
      toast.error('Failed to load gift')
      navigate('/gifts')
    }
  }

  const onSubmit = async (rawData) => {
    setSubmitting(true)
    try {
      // Process data to ensure correct types
      const stars = Number(rawData.stars);
      const data = {
        ...rawData,
        stars: stars,
        convertStars: rawData.Limited
          ? Number(rawData.convertStars)
          : Math.floor(stars * 0.7) || 1,

        upgradeStars: rawData.Limited
          ? Number(rawData.upgradeStars)
          : (stars * 10) || 100,
      }

      if (rawData.Limited) {
        data.availabilityTotal = Number(rawData.availabilityTotal)
        data.AvailabilityRemains = rawData.AvailabilityRemains
          ? Number(rawData.AvailabilityRemains)
          : Number(rawData.availabilityTotal)
      } else {
        delete data.availabilityTotal
        delete data.AvailabilityRemains
      }

      const formData = new FormData()
      formData.append('data', JSON.stringify(data))
      if (stickerFile) {
        formData.append('sticker', stickerFile)
      }

      await updateGift(parseInt(giftId), formData)
      navigate('/gifts')
    } catch (error) {
      console.error(error)
    } finally {
      setSubmitting(false)
    }
  }

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    )
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
            <h1 className="text-2xl font-bold">Edit Gift #{giftId}</h1>
            <p className="text-[#8b98a5]">Update gift details</p>
          </div>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          <div>
            <label className="label">Title</label>
            <input type="text" className="input" {...register('title')} />
          </div>

          <div>
            <label className="label">Update Sticker (JSON/TGS)</label>
            <div className="border-2 border-dashed border-[#2b5278] rounded-lg p-6 text-center">
              <input
                type="file"
                accept=".json,.tgs"
                onChange={(e) => setStickerFile(e.target.files[0])}
                className="hidden"
                id="sticker-upload"
              />
              <label htmlFor="sticker-upload" className="cursor-pointer">
                <Upload className="w-12 h-12 text-[#8b98a5] mx-auto mb-2" />
                <p className="text-[#8b98a5]">{stickerFile ? stickerFile.name : 'Click to upload new sticker'}</p>
              </label>
            </div>
          </div>

          <div className={`grid grid-cols-1 ${limited ? 'md:grid-cols-3' : 'md:grid-cols-1'} gap-6`}>
            <div>
              <label className="label uppercase tracking-wider text-xs font-bold text-fg-muted mb-2 block">Price (Stars)</label>
              <input type="number" className="input text-yellow-500 font-bold" {...register('stars', { min: 1 })} />
            </div>

            {limited && (
              <>
                <div className="animate-fade-in">
                  <label className="label uppercase tracking-wider text-xs font-bold text-fg-muted mb-2 block">Convert Stars</label>
                  <input type="number" className="input" {...register('convertStars', { required: 'Required for limited gifts', min: 1 })} />
                </div>
                <div className="animate-fade-in">
                  <label className="label uppercase tracking-wider text-xs font-bold text-fg-muted mb-2 block">Upgrade Stars</label>
                  <input type="number" className="input text-purple-400" {...register('upgradeStars')} />
                </div>
              </>
            )}
          </div>

          <div className="grid grid-cols-3 gap-4">
            <div className="flex items-center gap-2">
              <input type="checkbox" id="Limited" className="w-4 h-4 text-primary-600 rounded" {...register('Limited')} />
              <label htmlFor="Limited" className="text-sm font-medium">Limited</label>
            </div>
            <div className="flex items-center gap-2">
              <input type="checkbox" id="birthday" className="w-4 h-4 text-primary-600 rounded" {...register('birthday')} />
              <label htmlFor="birthday" className="text-sm font-medium">Birthday</label>
            </div>
            <div className="flex items-center gap-2">
              <input type="checkbox" id="SoldOut" className="w-4 h-4 text-primary-600 rounded" {...register('SoldOut')} />
              <label htmlFor="SoldOut" className="text-sm font-medium">Sold Out</label>
            </div>
          </div>

          {limited && (
            <div className="bg-purple-50 border border-purple-200 rounded-lg p-4">
              <h3 className="font-medium text-purple-900 mb-3">Limited Edition Settings</h3>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="label">Total Supply</label>
                  <input type="number" className="input" {...register('availabilityTotal')} />
                </div>
                <div>
                  <label className="label">Remaining</label>
                  <input type="number" className="input" {...register('AvailabilityRemains')} />
                </div>
              </div>
            </div>
          )}

          <div className="flex gap-4">
            <button type="submit" disabled={submitting} className="btn btn-primary flex-1">
              {submitting ? 'Saving...' : 'Save Changes'}
            </button>
            <button type="button" onClick={() => navigate('/gifts')} className="btn btn-secondary">
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}









