import { useState, useEffect } from 'react'
import Lottie from 'lottie-react'
import { AlertCircle } from 'lucide-react'
import pako from 'pako'

export default function StickerPreview({ stickerBase64, documentId, size = 120 }) {
  const [animationData, setAnimationData] = useState(null)
  const [error, setError] = useState(false)
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    // If documentId is provided (new format), load from MinIO
    if (documentId && typeof documentId === 'number') {
      loadStickerFromMinIO(documentId)
      return
    }

    // Legacy: If stickerBase64 is provided (old format)
    if (!stickerBase64) {
      setAnimationData(null)
      return
    }

    try {
      let jsonString
      
      // Check if it's a Buffer object (from MongoDB Binary)
      if (typeof stickerBase64 === 'object' && stickerBase64.type === 'Buffer' && Array.isArray(stickerBase64.data)) {
        // Convert Buffer array to string
        jsonString = String.fromCharCode.apply(null, stickerBase64.data)
      } else if (typeof stickerBase64 === 'string') {
        // Try to decode as base64
        try {
          jsonString = atob(stickerBase64)
        } catch {
          // If it fails, assume it's already a JSON string
          jsonString = stickerBase64
        }
      } else {
        throw new Error('Unknown sticker format')
      }
      
      const data = JSON.parse(jsonString)
      setAnimationData(data)
      setError(false)
    } catch (err) {
      console.error('Failed to parse sticker data:', err)
      setError(true)
    }
  }, [stickerBase64, documentId])

  const loadStickerFromMinIO = async (docId) => {
    setLoading(true)
    setError(false)
    
    try {
      // For now, show placeholder since we can't directly access MinIO from frontend
      // In production, you'd need a backend endpoint to proxy MinIO files
      console.log(`Sticker DocumentId: ${docId}`)
      
      // Show placeholder emoji for now
      setAnimationData(null)
      setError(false)
    } catch (err) {
      console.error('Failed to load sticker from MinIO:', err)
      setError(true)
    } finally {
      setLoading(false)
    }
  }

  if (!stickerBase64 && !documentId) {
    return (
      <div 
        className="flex items-center justify-center bg-gray-100 rounded-lg"
        style={{ width: size, height: size }}
      >
        <span className="text-gray-400 text-2xl">🎁</span>
      </div>
    )
  }

  if (error) {
    return (
      <div 
        className="flex flex-col items-center justify-center bg-red-50 rounded-lg border border-red-200"
        style={{ width: size, height: size }}
      >
        <AlertCircle className="w-6 h-6 text-red-500 mb-1" />
        <span className="text-xs text-red-600">Invalid sticker</span>
      </div>
    )
  }

  if (!animationData) {
    return (
      <div 
        className="flex items-center justify-center bg-gray-100 rounded-lg"
        style={{ width: size, height: size }}
      >
        <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-primary-600"></div>
      </div>
    )
  }

  return (
    <div 
      className="flex items-center justify-center bg-gradient-to-br from-primary-50 to-purple-50 rounded-lg"
      style={{ width: size, height: size }}
    >
      <Lottie 
        animationData={animationData} 
        loop={true}
        style={{ width: size * 0.9, height: size * 0.9 }}
      />
    </div>
  )
}
