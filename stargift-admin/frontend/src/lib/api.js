import axios from 'axios'

const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

// Gifts API
export const giftsApi = {
  getAll: (params) => api.get('/gifts', { params }),
  getById: (giftId) => api.get(`/gifts/${giftId}`),
  create: (formData) => api.post('/gifts', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }),
  update: (giftId, formData) => api.put(`/gifts/${giftId}`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }),
  delete: (giftId) => api.delete(`/gifts/${giftId}`),
  markSoldOut: (giftId, soldOut) => api.patch(`/gifts/${giftId}/soldout`, { soldOut }),
  purchase: (giftId) => api.patch(`/gifts/${giftId}/purchase`),
}

// Stats API
export const statsApi = {
  getOverview: () => api.get('/stats'),
  getSentStats: () => api.get('/stats/sent'),
}

// Health check
export const healthCheck = () => api.get('/health')

export default api
