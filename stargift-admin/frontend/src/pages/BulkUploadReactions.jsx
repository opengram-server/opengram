import React, { useState } from 'react';
import {
  Box,
  Typography,
  Paper,
  Button,
  Alert,
  CircularProgress,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  Checkbox,
  FormControlLabel,
  Divider
} from '@mui/material';
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import ErrorIcon from '@mui/icons-material/Error';
import FolderIcon from '@mui/icons-material/Folder';

const BulkUploadReactions = () => {
  const [file, setFile] = useState(null);
  const [uploading, setUploading] = useState(false);
  const [results, setResults] = useState(null);
  const [error, setError] = useState(null);
  const [premium, setPremium] = useState(false);

  const handleFileChange = (event) => {
    if (event.target.files && event.target.files[0]) {
      setFile(event.target.files[0]);
      setResults(null);
      setError(null);
    }
  };

  const handleUpload = async () => {
    if (!file) return;

    setUploading(true);
    setError(null);
    setResults(null);

    const formData = new FormData();
    formData.append('zipFile', file);
    formData.append('premium', premium);

    try {
      const response = await fetch('http://localhost:3001/api/reactions/bulk-upload', {
        method: 'POST',
        body: formData,
      });

      const data = await response.json();

      if (!response.ok) {
        throw new Error(data.error || 'Upload failed');
      }

      setResults(data.results);
    } catch (err) {
      setError(err.message);
    } finally {
      setUploading(false);
    }
  };

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom>
        Bulk Upload Reactions
      </Typography>

      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="body1" paragraph>
          Upload a ZIP file containing folders named after emojis (e.g., "🙏", "❤️").
          Each folder should contain the reaction assets (static_icon.png, appear_animation.tgs, etc.).
        </Typography>

        <Box sx={{ mb: 3 }}>
          <FormControlLabel
            control={
              <Checkbox
                checked={premium}
                onChange={(e) => setPremium(e.target.checked)}
              />
            }
            label="Mark as Premium Reactions"
          />
        </Box>

        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 3 }}>
          <Button
            variant="contained"
            component="label"
            startIcon={<CloudUploadIcon />}
          >
            Select ZIP File
            <input
              type="file"
              hidden
              accept=".zip"
              onChange={handleFileChange}
            />
          </Button>

          {file && (
            <Typography variant="body2">
              Selected: {file.name} ({(file.size / 1024 / 1024).toFixed(2)} MB)
            </Typography>
          )}
        </Box>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        <Button
          variant="contained"
          color="primary"
          onClick={handleUpload}
          disabled={!file || uploading}
          fullWidth
        >
          {uploading ? <CircularProgress size={24} /> : 'Upload Reactions'}
        </Button>
      </Paper>

      {results && (
        <Paper sx={{ p: 3 }}>
          <Typography variant="h6" gutterBottom>
            Upload Results
          </Typography>
          <List>
            {results.map((result, index) => (
              <React.Fragment key={index}>
                <ListItem>
                  <ListItemIcon>
                    {result.success ? (
                      <CheckCircleIcon color="success" />
                    ) : (
                      <ErrorIcon color="error" />
                    )}
                  </ListItemIcon>
                  <ListItemText
                    primary={
                      <Box display="flex" alignItems="center" gap={1}>
                        {result.success ? (
                          <>
                            <Typography variant="h6">{result.emoji}</Typography>
                            <Typography variant="body2" color="textSecondary">
                              ({result.assets?.length || 0} assets)
                            </Typography>
                          </>
                        ) : (
                          <Typography color="error">
                            {result.emoji || 'Unknown'} - {result.error}
                          </Typography>
                        )}
                      </Box>
                    }
                    secondary={
                      result.success && (
                        <Typography variant="caption" display="block">
                          Assets: {result.assets?.join(', ')}
                        </Typography>
                      )
                    }
                  />
                </ListItem>
                {index < results.length - 1 && <Divider />}
              </React.Fragment>
            ))}
          </List>
        </Paper>
      )}
    </Box>
  );
};

export default BulkUploadReactions;
