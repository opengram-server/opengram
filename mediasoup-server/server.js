/**
 * MyTelegram Mediasoup SFU Server
 * Production-ready WebRTC SFU for group calls
 */

const mediasoup = require('mediasoup');
const express = require('express');
const http = require('http');
const socketIO = require('socket.io');
const cors = require('cors');
const winston = require('winston');
require('dotenv').config();

// ==================== Configuration ====================

const config = {
  listenIp: process.env.MEDIASOUP_LISTEN_IP || '0.0.0.0',
  announcedIp: process.env.MEDIASOUP_ANNOUNCED_IP || '127.0.0.1',
  httpPort: parseInt(process.env.PORT) || 3200,
  rtcMinPort: parseInt(process.env.RTC_MIN_PORT) || 10000,
  rtcMaxPort: parseInt(process.env.RTC_MAX_PORT) || 10100,
  logLevel: process.env.LOG_LEVEL || 'debug',
};

// ==================== Logger Setup ====================

const logger = winston.createLogger({
  level: config.logLevel,
  format: winston.format.combine(
    winston.format.timestamp(),
    winston.format.printf(({ timestamp, level, message, ...meta }) => {
      return `[${timestamp}] [${level.toUpperCase()}] ${message} ${Object.keys(meta).length ? JSON.stringify(meta) : ''}`;
    })
  ),
  transports: [
    new winston.transports.Console(),
    new winston.transports.File({ filename: 'mediasoup-error.log', level: 'error' }),
    new winston.transports.File({ filename: 'mediasoup-combined.log' })
  ]
});

// ==================== Express App Setup ====================

const app = express();
app.use(cors());
app.use(express.json());

const server = http.createServer(app);
const io = socketIO(server, {
  cors: {
    origin: '*',
    methods: ['GET', 'POST']
  }
});

// ==================== Global State ====================

let worker;
let router;
const rooms = new Map(); // roomId -> Room
const peers = new Map(); // socketId -> Peer

class Room {
  constructor(roomId) {
    this.id = roomId;
    this.peers = new Map(); // peerId -> Peer
    this.router = null;
    logger.info(`Room created: ${roomId}`);
  }

  addPeer(peer) {
    this.peers.set(peer.id, peer);
    logger.info(`Peer ${peer.id} added to room ${this.id}`);
  }

  removePeer(peerId) {
    const peer = this.peers.get(peerId);
    if (peer) {
      peer.close();
      this.peers.delete(peerId);
      logger.info(`Peer ${peerId} removed from room ${this.id}`);
    }
    
    if (this.peers.size === 0) {
      logger.info(`Room ${this.id} is empty, closing...`);
      return true; // Room can be deleted
    }
    return false;
  }

  getPeer(peerId) {
    return this.peers.get(peerId);
  }

  getAllPeers() {
    return Array.from(this.peers.values());
  }

  close() {
    for (const peer of this.peers.values()) {
      peer.close();
    }
    this.peers.clear();
    logger.info(`Room ${this.id} closed`);
  }
}

class Peer {
  constructor(id, socket, roomId) {
    this.id = id;
    this.socket = socket;
    this.roomId = roomId;
    this.transports = new Map(); // transportId -> Transport
    this.producers = new Map(); // producerId -> Producer
    this.consumers = new Map(); // consumerId -> Consumer
    this.rtpCapabilities = null;
    logger.info(`Peer created: ${id} in room ${roomId}`);
  }

  addTransport(transport) {
    this.transports.set(transport.id, transport);
    logger.debug(`Transport ${transport.id} added to peer ${this.id}`);
  }

  addProducer(producer) {
    this.producers.set(producer.id, producer);
    logger.info(`Producer ${producer.id} (${producer.kind}) added to peer ${this.id}`);
  }

  addConsumer(consumer) {
    this.consumers.set(consumer.id, consumer);
    logger.info(`Consumer ${consumer.id} (${consumer.kind}) added to peer ${this.id}`);
  }

  close() {
    logger.info(`Closing peer ${this.id}`);
    
    // Close all consumers
    for (const consumer of this.consumers.values()) {
      consumer.close();
    }
    this.consumers.clear();

    // Close all producers
    for (const producer of this.producers.values()) {
      producer.close();
    }
    this.producers.clear();

    // Close all transports
    for (const transport of this.transports.values()) {
      transport.close();
    }
    this.transports.clear();
  }
}

// ==================== Mediasoup Worker & Router ====================

async function createWorker() {
  logger.info('Creating Mediasoup worker...');
  
  worker = await mediasoup.createWorker({
    logLevel: 'warn',
    rtcMinPort: config.rtcMinPort,
    rtcMaxPort: config.rtcMaxPort,
  });

  worker.on('died', () => {
    logger.error('Mediasoup worker died, exiting in 2 seconds...');
    setTimeout(() => process.exit(1), 2000);
  });

  logger.info(`Mediasoup worker created [PID: ${worker.pid}]`);
  return worker;
}

async function createRouter() {
  logger.info('Creating Mediasoup router...');
  
  const mediaCodecs = [
    {
      kind: 'audio',
      mimeType: 'audio/opus',
      clockRate: 48000,
      channels: 2
    },
    {
      kind: 'video',
      mimeType: 'video/VP8',
      clockRate: 90000,
      parameters: {
        'x-google-start-bitrate': 1000
      }
    },
    {
      kind: 'video',
      mimeType: 'video/H264',
      clockRate: 90000,
      parameters: {
        'packetization-mode': 1,
        'profile-level-id': '42e01f',
        'level-asymmetry-allowed': 1
      }
    }
  ];

  router = await worker.createRouter({ mediaCodecs });
  logger.info(`Mediasoup router created [ID: ${router.id}]`);
  return router;
}

// ==================== HTTP API Endpoints ====================

// Health check
app.get('/health', (req, res) => {
  res.json({
    status: 'healthy',
    worker: {
      pid: worker?.pid,
      dead: worker?.closed
    },
    router: {
      id: router?.id,
      closed: router?.closed
    },
    rooms: rooms.size,
    peers: peers.size
  });
});

// Get DTLS fingerprint (for MyTelegram backend)
app.get('/api/dtls-fingerprint', (req, res) => {
  if (!router) {
    return res.status(503).json({ error: 'Router not initialized' });
  }

  const fingerprint = router.rtpCapabilities.headerExtensions.length > 0 
    ? router.rtpCapabilities 
    : null;

  // Get DTLS parameters from a temporary transport
  router.createWebRtcTransport({
    listenIps: [{ ip: config.listenIp, announcedIp: config.announcedIp }],
    enableUdp: true,
    enableTcp: false,
    preferUdp: true
  }).then(transport => {
    const dtls = transport.dtlsParameters;
    res.json({
      algorithm: dtls.fingerprints[0].algorithm,
      value: dtls.fingerprints[0].value
    });
    transport.close();
  }).catch(err => {
    logger.error('Failed to get DTLS fingerprint:', err);
    res.status(500).json({ error: err.message });
  });
});

// Get router RTP capabilities
app.get('/api/rtp-capabilities', (req, res) => {
  if (!router) {
    return res.status(503).json({ error: 'Router not initialized' });
  }
  res.json(router.rtpCapabilities);
});

// Get room info
app.get('/api/rooms/:roomId', (req, res) => {
  const room = rooms.get(req.params.roomId);
  if (!room) {
    return res.status(404).json({ error: 'Room not found' });
  }

  res.json({
    id: room.id,
    peers: room.getAllPeers().map(p => ({
      id: p.id,
      producers: p.producers.size,
      consumers: p.consumers.size
    }))
  });
});

// Create WebRTC transport for Telegram MTProto integration
app.post('/api/transports/create', async (req, res) => {
  try {
    const { roomId, peerId, direction } = req.body;
    
    logger.info(`Creating ${direction} transport for peer ${peerId} in room ${roomId}`);
    
    const transport = await router.createWebRtcTransport({
      listenIps: [
        {
          ip: config.listenIp,
          announcedIp: config.announcedIp
        }
      ],
      enableUdp: true,
      enableTcp: false,
      preferUdp: true,
      initialAvailableOutgoingBitrate: 1000000,
      minimumAvailableOutgoingBitrate: 600000,
      maxSctpMessageSize: 262144,
      maxIncomingBitrate: 1500000
    });

    // Store transport info
    const key = `${roomId}:${peerId}:${direction}`;
    if (!global.transports) global.transports = new Map();
    global.transports.set(key, transport);
    global.transports.set(transport.id, transport);

    logger.info(`Transport ${transport.id} created on port ${transport.tuple?.localPort || 'dynamic'}`);

    res.json({
      id: transport.id,
      iceParameters: transport.iceParameters,
      iceCandidates: transport.iceCandidates,
      dtlsParameters: transport.dtlsParameters
    });

  } catch (error) {
    logger.error('Error creating transport:', error);
    res.status(500).json({ error: error.message });
  }
});

// Connect transport with DTLS parameters from client
app.post('/api/transports/connect', async (req, res) => {
  try {
    const { transportId, dtlsParameters } = req.body;
    
    logger.info(`Connecting transport ${transportId}`);
    
    const transport = global.transports?.get(transportId);
    if (!transport) {
      return res.status(404).json({ error: 'Transport not found' });
    }

    await transport.connect({ dtlsParameters });
    
    logger.info(`Transport ${transportId} connected`);
    res.json({ success: true });

  } catch (error) {
    logger.error('Error connecting transport:', error);
    res.status(500).json({ error: error.message });
  }
});

// Create producer (for sending media)
app.post('/api/produce', async (req, res) => {
  try {
    const { transportId, kind, rtpParameters } = req.body;
    
    logger.info(`Creating producer on transport ${transportId}, kind: ${kind}`);
    
    const transport = global.transports?.get(transportId);
    if (!transport) {
      return res.status(404).json({ error: 'Transport not found' });
    }

    const producer = await transport.produce({
      kind,
      rtpParameters
    });

    logger.info(`Producer ${producer.id} created`);
    res.json({ producerId: producer.id });

  } catch (error) {
    logger.error('Error creating producer:', error);
    res.status(500).json({ error: error.message });
  }
});

// Create consumer (for receiving media)
app.post('/api/consume', async (req, res) => {
  try {
    const { transportId, producerId, rtpCapabilities } = req.body;
    
    logger.info(`Creating consumer on transport ${transportId} for producer ${producerId}`);
    
    const transport = global.transports?.get(transportId);
    if (!transport) {
      return res.status(404).json({ error: 'Transport not found' });
    }

    // Check if can consume
    if (!router.canConsume({ producerId, rtpCapabilities })) {
      return res.status(400).json({ error: 'Cannot consume' });
    }

    const consumer = await transport.consume({
      producerId,
      rtpCapabilities,
      paused: false
    });

    logger.info(`Consumer ${consumer.id} created`);
    
    res.json({
      id: consumer.id,
      producerId: producerId,
      kind: consumer.kind,
      rtpParameters: consumer.rtpParameters
    });

  } catch (error) {
    logger.error('Error creating consumer:', error);
    res.status(500).json({ error: error.message });
  }
});

// ==================== Socket.IO Signaling ====================

io.on('connection', (socket) => {
  logger.info(`Socket connected: ${socket.id}`);

  socket.on('join-room', async ({ roomId, peerId }, callback) => {
    try {
      logger.info(`Peer ${peerId} joining room ${roomId}`);

      // Get or create room
      let room = rooms.get(roomId);
      if (!room) {
        room = new Room(roomId);
        room.router = router;
        rooms.set(roomId, room);
      }

      // Create peer
      const peer = new Peer(peerId, socket, roomId);
      room.addPeer(peer);
      peers.set(socket.id, peer);

      socket.join(roomId);

      callback({
        success: true,
        rtpCapabilities: router.rtpCapabilities
      });

    } catch (error) {
      logger.error('Error joining room:', error);
      callback({ success: false, error: error.message });
    }
  });

  socket.on('create-transport', async ({ roomId, direction }, callback) => {
    try {
      const peer = peers.get(socket.id);
      if (!peer) {
        throw new Error('Peer not found');
      }

      const transport = await router.createWebRtcTransport({
        listenIps: [
          {
            ip: config.listenIp,
            announcedIp: config.announcedIp
          }
        ],
        enableUdp: true,
        enableTcp: false,
        preferUdp: true,
        initialAvailableOutgoingBitrate: 1000000,
        minimumAvailableOutgoingBitrate: 600000,
        maxSctpMessageSize: 262144,
        maxIncomingBitrate: 1500000
      });

      peer.addTransport(transport);

      transport.on('dtlsstatechange', (dtlsState) => {
        logger.debug(`Transport ${transport.id} DTLS state: ${dtlsState}`);
        if (dtlsState === 'closed') {
          transport.close();
        }
      });

      transport.on('close', () => {
        logger.info(`Transport ${transport.id} closed`);
      });

      callback({
        success: true,
        transport: {
          id: transport.id,
          iceParameters: transport.iceParameters,
          iceCandidates: transport.iceCandidates,
          dtlsParameters: transport.dtlsParameters
        }
      });

    } catch (error) {
      logger.error('Error creating transport:', error);
      callback({ success: false, error: error.message });
    }
  });

  socket.on('connect-transport', async ({ transportId, dtlsParameters }, callback) => {
    try {
      const peer = peers.get(socket.id);
      if (!peer) {
        throw new Error('Peer not found');
      }

      const transport = peer.transports.get(transportId);
      if (!transport) {
        throw new Error('Transport not found');
      }

      await transport.connect({ dtlsParameters });
      logger.info(`Transport ${transportId} connected for peer ${peer.id}`);

      callback({ success: true });

    } catch (error) {
      logger.error('Error connecting transport:', error);
      callback({ success: false, error: error.message });
    }
  });

  socket.on('produce', async ({ transportId, kind, rtpParameters }, callback) => {
    try {
      const peer = peers.get(socket.id);
      if (!peer) {
        throw new Error('Peer not found');
      }

      const transport = peer.transports.get(transportId);
      if (!transport) {
        throw new Error('Transport not found');
      }

      const producer = await transport.produce({
        kind,
        rtpParameters
      });

      peer.addProducer(producer);

      producer.on('transportclose', () => {
        logger.info(`Producer ${producer.id} transport closed`);
        producer.close();
        peer.producers.delete(producer.id);
      });

      // Notify other peers in room about new producer
      const room = rooms.get(peer.roomId);
      if (room) {
        socket.to(peer.roomId).emit('new-producer', {
          peerId: peer.id,
          producerId: producer.id,
          kind: producer.kind
        });
      }

      callback({
        success: true,
        producerId: producer.id
      });

    } catch (error) {
      logger.error('Error producing:', error);
      callback({ success: false, error: error.message });
    }
  });

  socket.on('consume', async ({ transportId, producerId, rtpCapabilities }, callback) => {
    try {
      const peer = peers.get(socket.id);
      if (!peer) {
        throw new Error('Peer not found');
      }

      const transport = peer.transports.get(transportId);
      if (!transport) {
        throw new Error('Transport not found');
      }

      // Find the producer peer
      const room = rooms.get(peer.roomId);
      let producer = null;
      for (const otherPeer of room.getAllPeers()) {
        producer = otherPeer.producers.get(producerId);
        if (producer) break;
      }

      if (!producer) {
        throw new Error('Producer not found');
      }

      // Check if router can consume
      if (!router.canConsume({ producerId, rtpCapabilities })) {
        throw new Error('Cannot consume');
      }

      const consumer = await transport.consume({
        producerId,
        rtpCapabilities,
        paused: false
      });

      peer.addConsumer(consumer);

      consumer.on('transportclose', () => {
        logger.info(`Consumer ${consumer.id} transport closed`);
        consumer.close();
        peer.consumers.delete(consumer.id);
      });

      consumer.on('producerclose', () => {
        logger.info(`Consumer ${consumer.id} producer closed`);
        socket.emit('consumer-closed', { consumerId: consumer.id });
        consumer.close();
        peer.consumers.delete(consumer.id);
      });

      callback({
        success: true,
        consumer: {
          id: consumer.id,
          producerId: producerId,
          kind: consumer.kind,
          rtpParameters: consumer.rtpParameters
        }
      });

    } catch (error) {
      logger.error('Error consuming:', error);
      callback({ success: false, error: error.message });
    }
  });

  socket.on('disconnect', () => {
    logger.info(`Socket disconnected: ${socket.id}`);
    
    const peer = peers.get(socket.id);
    if (peer) {
      const room = rooms.get(peer.roomId);
      if (room) {
        const isEmpty = room.removePeer(peer.id);
        if (isEmpty) {
          rooms.delete(room.id);
        }
        
        // Notify other peers
        socket.to(peer.roomId).emit('peer-left', { peerId: peer.id });
      }
      peers.delete(socket.id);
    }
  });
});

// ==================== Server Startup ====================

async function startServer() {
  try {
    await createWorker();
    await createRouter();

    server.listen(config.httpPort, () => {
      logger.info(`==============================================`);
      logger.info(`MyTelegram Mediasoup SFU Server Started`);
      logger.info(`==============================================`);
      logger.info(`HTTP API: http://localhost:${config.httpPort}`);
      logger.info(`WebRTC RTC Ports: ${config.rtcMinPort}-${config.rtcMaxPort}`);
      logger.info(`Announced IP: ${config.announcedIp}`);
      logger.info(`==============================================`);
    });

  } catch (error) {
    logger.error('Failed to start server:', error);
    process.exit(1);
  }
}

// Graceful shutdown
process.on('SIGINT', async () => {
  logger.info('Shutting down gracefully...');
  
  // Close all rooms
  for (const room of rooms.values()) {
    room.close();
  }
  rooms.clear();
  peers.clear();

  if (worker) {
    worker.close();
  }

  server.close(() => {
    logger.info('Server closed');
    process.exit(0);
  });
});

startServer();
