// Script to delete old StarsSnapshot from MongoDB
// Run this in MongoDB shell or via mongosh

use MyTelegram;

// Delete all StarsSnapshots with old version
db['eventflow.snapshots'].deleteMany({ 
    'Metadata.AggregateId': { $regex: '^stars-' }
});

print('Old StarsSnapshots deleted');
