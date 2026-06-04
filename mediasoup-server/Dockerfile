FROM node:18-alpine

# Install dependencies for Mediasoup
RUN apk add --no-cache \
    python3 \
    make \
    g++ \
    gcc \
    linux-headers

WORKDIR /app

# Copy package files
COPY package*.json ./

# Install npm dependencies
RUN npm ci --only=production

# Copy application files
COPY . .

# Expose ports
EXPOSE 3200
EXPOSE 10000-10100/udp

# Start server
CMD ["node", "server.js"]
