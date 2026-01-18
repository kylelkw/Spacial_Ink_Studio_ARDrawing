const express = require('express');
const cors = require('cors');
const bodyParser = require('body-parser');
const fs = require('fs');
const path = require('path');

const app = express();
const PORT = process.env.PORT || 3000;

// Middleware
app.use(cors());
app.use(bodyParser.json({ limit: '50mb' }));
app.use(bodyParser.urlencoded({ extended: true, limit: '50mb' }));

// Create metadata directory
const metadataDir = path.join(__dirname, 'metadata');
if (!fs.existsSync(metadataDir)) {
  fs.mkdirSync(metadataDir, { recursive: true });
}

// Logging middleware
app.use((req, res, next) => {
  console.log(`[${new Date().toISOString()}] ${req.method} ${req.path}`);
  next();
});

// Health check endpoint
app.get('/health', (req, res) => {
  res.json({ 
    status: 'ok',
    uptime: process.uptime(),
    timestamp: new Date().toISOString()
  });
});

// Upload metadata endpoint
app.post('/metadata', (req, res) => {
  try {
    const metadata = req.body;
    
    // Validate metadata
    if (!metadata || typeof metadata !== 'object') {
      return res.status(400).json({ 
        error: 'Invalid metadata format' 
      });
    }
    
    // Generate filename
    const timestamp = Date.now();
    const filename = `metadata_${timestamp}.json`;
    const filepath = path.join(metadataDir, filename);
    
    // Add server metadata
    const enhancedMetadata = {
      ...metadata,
      server_timestamp: new Date().toISOString(),
      server_id: 'graffiti-metadata-server-v1'
    };
    
    // Save to file
    fs.writeFileSync(filepath, JSON.stringify(enhancedMetadata, null, 2));
    
    // Generate URI
    const uri = `http://localhost:${PORT}/metadata/${filename}`;
    
    console.log(`✅ Metadata saved: ${filename} (${JSON.stringify(metadata).length} bytes)`);
    
    res.json({
      success: true,
      uri: uri,
      filename: filename,
      size: JSON.stringify(enhancedMetadata).length,
      timestamp: enhancedMetadata.server_timestamp
    });
  } catch (error) {
    console.error('❌ Error saving metadata:', error);
    res.status(500).json({ 
      error: error.message,
      success: false
    });
  }
});

// Get metadata by filename
app.get('/metadata/:filename', (req, res) => {
  try {
    const filename = req.params.filename;
    const filepath = path.join(metadataDir, filename);
    
    if (!fs.existsSync(filepath)) {
      return res.status(404).json({ 
        error: 'Metadata not found' 
      });
    }
    
    const metadata = JSON.parse(fs.readFileSync(filepath, 'utf8'));
    res.json(metadata);
  } catch (error) {
    console.error('❌ Error reading metadata:', error);
    res.status(500).json({ 
      error: error.message 
    });
  }
});

// List all metadata files
app.get('/metadata', (req, res) => {
  try {
    const files = fs.readdirSync(metadataDir)
      .filter(f => f.endsWith('.json'))
      .map(f => {
        const filepath = path.join(metadataDir, f);
        const stats = fs.statSync(filepath);
        return {
          filename: f,
          uri: `http://localhost:${PORT}/metadata/${f}`,
          size: stats.size,
          created: stats.birthtime,
          modified: stats.mtime
        };
      })
      .sort((a, b) => b.modified - a.modified);
    
    res.json({
      total: files.length,
      files: files
    });
  } catch (error) {
    console.error('❌ Error listing metadata:', error);
    res.status(500).json({ 
      error: error.message 
    });
  }
});

// Delete metadata (for testing)
app.delete('/metadata/:filename', (req, res) => {
  try {
    const filename = req.params.filename;
    const filepath = path.join(metadataDir, filename);
    
    if (!fs.existsSync(filepath)) {
      return res.status(404).json({ 
        error: 'Metadata not found' 
      });
    }
    
    fs.unlinkSync(filepath);
    console.log(`🗑️  Metadata deleted: ${filename}`);
    
    res.json({
      success: true,
      message: 'Metadata deleted',
      filename: filename
    });
  } catch (error) {
    console.error('❌ Error deleting metadata:', error);
    res.status(500).json({ 
      error: error.message 
    });
  }
});

// Statistics endpoint
app.get('/stats', (req, res) => {
  try {
    const files = fs.readdirSync(metadataDir).filter(f => f.endsWith('.json'));
    
    let totalSize = 0;
    let totalStrokes = 0;
    
    files.forEach(f => {
      const filepath = path.join(metadataDir, f);
      const stats = fs.statSync(filepath);
      totalSize += stats.size;
      
      try {
        const metadata = JSON.parse(fs.readFileSync(filepath, 'utf8'));
        totalStrokes += metadata.totalStrokes || 0;
      } catch (e) {
        // Skip invalid files
      }
    });
    
    res.json({
      total_metadata_files: files.length,
      total_size_bytes: totalSize,
      total_size_mb: (totalSize / 1024 / 1024).toFixed(2),
      total_strokes: totalStrokes,
      storage_path: metadataDir
    });
  } catch (error) {
    console.error('❌ Error getting stats:', error);
    res.status(500).json({ 
      error: error.message 
    });
  }
});

// 404 handler
app.use((req, res) => {
  res.status(404).json({ 
    error: 'Not found',
    path: req.path,
    available_endpoints: [
      'GET /health',
      'POST /metadata',
      'GET /metadata',
      'GET /metadata/:filename',
      'DELETE /metadata/:filename',
      'GET /stats'
    ]
  });
});

// Error handler
app.use((err, req, res, next) => {
  console.error('❌ Server error:', err);
  res.status(500).json({ 
    error: 'Internal server error',
    message: err.message
  });
});

// Start server
app.listen(PORT, () => {
  console.log('\n🚀 Graffiti Metadata Server');
  console.log('━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━');
  console.log(`📡 Server running on http://localhost:${PORT}`);
  console.log(`💾 Metadata directory: ${metadataDir}`);
  console.log(`\n📋 Available endpoints:`);
  console.log(`   POST   /metadata          - Upload new metadata`);
  console.log(`   GET    /metadata          - List all metadata`);
  console.log(`   GET    /metadata/:id      - Get specific metadata`);
  console.log(`   DELETE /metadata/:id      - Delete metadata`);
  console.log(`   GET    /stats             - Server statistics`);
  console.log(`   GET    /health            - Health check`);
  console.log('━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n');
  console.log('Ready to receive metadata uploads! 🎨\n');
});

// Graceful shutdown
process.on('SIGINT', () => {
  console.log('\n\n👋 Shutting down server...');
  process.exit(0);
});
