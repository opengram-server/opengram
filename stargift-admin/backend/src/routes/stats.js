import express from 'express';

const router = express.Router();

// GET statistics
router.get('/', async (req, res) => {
  try {
    const giftsCollection = req.db.collection('AvailableStarGiftReadModel');
    const sentGiftsCollection = req.db.collection('StarGiftReadModel');
    
    const [
      totalGifts,
      limitedGifts,
      soldOutGifts,
      totalSentGifts,
      totalStarsEarned
    ] = await Promise.all([
      giftsCollection.countDocuments(),
      giftsCollection.countDocuments({ Limited: true }),
      giftsCollection.countDocuments({ SoldOut: true }),
      sentGiftsCollection.countDocuments(),
      sentGiftsCollection.aggregate([
        { $group: { _id: null, total: { $sum: '$Stars' } } }
      ]).toArray()
    ]);
    
    const mostPopular = await sentGiftsCollection.aggregate([
      { $group: { _id: '$GiftId', count: { $sum: 1 } } },
      { $sort: { count: -1 } },
      { $limit: 5 }
    ]).toArray();
    
    res.json({
      totalGifts,
      limitedGifts,
      soldOutGifts,
      availableGifts: totalGifts - soldOutGifts,
      totalSentGifts,
      totalStarsEarned: totalStarsEarned[0]?.total || 0,
      mostPopular
    });
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

// GET sent gifts statistics
router.get('/sent', async (req, res) => {
  try {
    const sentGiftsCollection = req.db.collection('StarGiftReadModel');
    
    const stats = await sentGiftsCollection.aggregate([
      {
        $facet: {
          byDay: [
            {
              $group: {
                _id: {
                  $dateToString: { format: '%Y-%m-%d', date: '$CreatedAt' }
                },
                count: { $sum: 1 },
                stars: { $sum: '$Stars' }
              }
            },
            { $sort: { _id: -1 } },
            { $limit: 30 }
          ],
          byGift: [
            {
              $group: {
                _id: '$GiftId',
                count: { $sum: 1 },
                stars: { $sum: '$Stars' }
              }
            },
            { $sort: { count: -1 } }
          ],
          conversion: [
            {
              $group: {
                _id: null,
                total: { $sum: 1 },
                converted: {
                  $sum: { $cond: ['$Converted', 1, 0] }
                },
                saved: {
                  $sum: { $cond: ['$Saved', 1, 0] }
                }
              }
            }
          ]
        }
      }
    ]).toArray();
    
    res.json(stats[0]);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
});

export default router;
