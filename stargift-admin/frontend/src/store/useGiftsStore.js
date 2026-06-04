import { create } from 'zustand'
import { giftsApi } from '../lib/api'
import toast from 'react-hot-toast'

const useGiftsStore = create((set, get) => ({
  gifts: [],
  loading: false,
  error: null,
  filters: {
    soldOut: undefined,
    limited: undefined,
    sort: 'stars',
  },

  setFilters: (filters) => set((state) => ({ 
    filters: { ...state.filters, ...filters } 
  })),

  fetchGifts: async () => {
    set({ loading: true, error: null })
    try {
      const response = await giftsApi.getAll(get().filters)
      set({ gifts: response.data, loading: false })
    } catch (error) {
      set({ error: error.message, loading: false })
      toast.error('Failed to load gifts')
    }
  },

  createGift: async (formData) => {
    try {
      const response = await giftsApi.create(formData)
      set((state) => ({ gifts: [...state.gifts, response.data] }))
      toast.success('Gift created successfully!')
      return response.data
    } catch (error) {
      toast.error(error.response?.data?.error || 'Failed to create gift')
      throw error
    }
  },

  updateGift: async (giftId, formData) => {
    try {
      const response = await giftsApi.update(giftId, formData)
      set((state) => ({
        gifts: state.gifts.map((g) => 
          g.GiftId === giftId ? response.data : g
        ),
      }))
      toast.success('Gift updated successfully!')
      return response.data
    } catch (error) {
      toast.error(error.response?.data?.error || 'Failed to update gift')
      throw error
    }
  },

  deleteGift: async (giftId) => {
    try {
      await giftsApi.delete(giftId)
      set((state) => ({
        gifts: state.gifts.filter((g) => g.GiftId !== giftId),
      }))
      toast.success('Gift deleted successfully!')
    } catch (error) {
      toast.error('Failed to delete gift')
      throw error
    }
  },

  toggleSoldOut: async (giftId, soldOut) => {
    try {
      const response = await giftsApi.markSoldOut(giftId, soldOut)
      set((state) => ({
        gifts: state.gifts.map((g) => 
          g.GiftId === giftId ? response.data : g
        ),
      }))
      toast.success(soldOut ? 'Marked as sold out' : 'Marked as available')
    } catch (error) {
      toast.error('Failed to update gift status')
      throw error
    }
  },
}))

export default useGiftsStore
