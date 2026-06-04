// Update custom emoji documents to add MinIO file path
const db = db.getSiblingDB('tg');
const collection = db.getCollection('ReadModel-DocumentReadModel');

// Find all custom emoji documents
const customEmojiDocs = collection.find({
  'Attributes2._t': 'TDocumentAttributeCustomEmoji'
});

let updated = 0;
customEmojiDocs.forEach(doc => {
  const documentId = doc.DocumentId;
  const subDir = (documentId % 1000).toString().padStart(3, '0');
  const filePath = `custom_emoji/${subDir}/${documentId}.tgs`;
  
  // Update document with file path
  collection.updateOne(
    { _id: doc._id },
    { 
      $set: { 
        MinIOPath: filePath,
        MinIOBucket: 'tg-files'
      } 
    }
  );
  updated++;
});

print(`Updated ${updated} custom emoji documents with MinIO paths`);
