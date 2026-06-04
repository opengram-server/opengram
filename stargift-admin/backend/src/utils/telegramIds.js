import crypto from 'crypto';

/**
 * Generate FileReference (24 bytes)
 * Format: [4 bytes timestamp][20 bytes random]
 */
export function generateFileReference() {
  const buffer = Buffer.allocUnsafe(24);
  
  // First 4 bytes: current UNIX timestamp
  buffer.writeUInt32BE(Math.floor(Date.now() / 1000), 0);
  
  // Remaining 20 bytes: cryptographically secure random
  const randomBytes = crypto.randomBytes(20);
  randomBytes.copy(buffer, 4);
  
  return buffer;
}

/**
 * Generate AccessHash (int64)
 * Must be cryptographically random
 */
export function generateAccessHash() {
  // Generate 8 random bytes
  const buffer = crypto.randomBytes(8);
  
  // Convert to signed int64 string for MongoDB
  return buffer.readBigInt64BE(0).toString();
}

/**
 * Generate DocumentId using timestamp + random
 * Ensures uniqueness without DB roundtrip
 */
export function generateDocumentId() {
  // Timestamp in milliseconds * 10000 + random component
  return Date.now() * 10000 + Math.floor(Math.random() * 10000);
}
