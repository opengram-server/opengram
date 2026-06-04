import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { Upload, ArrowLeft, Smile } from 'lucide-react';
import Lottie from 'lottie-react';
import pako from 'pako';

export default function CreateReaction() {
  const navigate = useNavigate();
  const { register, handleSubmit, watch, formState: { errors } } = useForm();
  const [files, setFiles] = useState({
    staticIcon: null,
    appearAnimation: null,
    selectAnimation: null,
    activateAnimation: null,
    effectAnimation: null,
    aroundAnimation: null,
    centerIcon: null
  });
  const [previews, setPreviews] = useState({});
  const [submitting, setSubmitting] = useState(false);

  const premium = watch('premium');

  const handleFileChange = (field, e) => {
    const file = e.target.files[0];
    if (file) {
      setFiles(prev => ({ ...prev, [field]: file }));
      
      // Read file for preview
      const reader = new FileReader();
      reader.onload = (event) => {
        try {
          const arrayBuffer = event.target.result;
          const uint8Array = new Uint8Array(arrayBuffer);
          
          try {
            const decompressed = pako.inflate(uint8Array, { to: 'string' });
            const jsonData = JSON.parse(decompressed);
            setPreviews(prev => ({ ...prev, [field]: jsonData }));
          } catch (gzipErr) {
            const text = new TextDecoder().decode(uint8Array);
            const jsonData = JSON.parse(text);
            setPreviews(prev => ({ ...prev, [field]: jsonData }));
          }
        } catch (err) {
          console.error('Failed to parse JSON:', err);
        }
      };
      reader.readAsArrayBuffer(file);
    }
  };

  const onSubmit = async (data) => {
    setSubmitting(true);
    try {
      const formData = new FormData();
      formData.append('data', JSON.stringify(data));
      
      // Append all animation files
      Object.entries(files).forEach(([key, file]) => {
        if (file) {
          formData.append(key, file);
        }
      });
      
      const response = await fetch('/api/reactions', {
        method: 'POST',
        body: formData
      });

      if (response.ok) {
        navigate('/reactions');
      } else {
        const error = await response.json();
        alert(`Error: ${error.error}`);
      }
    } catch (error) {
      console.error('Create reaction error:', error);
      alert('Failed to create reaction');
    } finally {
      setSubmitting(false);
    }
  };

  const AnimationUpload = ({ field, label, required = false }) => (
    <div>
      <label className="label">
        {label} {required && <span className="text-red-500">*</span>}
      </label>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className={`border-2 border-dashed rounded-lg p-4 text-center transition-colors ${
          files[field] ? 'border-green-500 bg-green-500/20' : 'border-[#2b5278] hover:border-purple-500'
        }`}>
          <input
            type="file"
            accept=".json,.tgs"
            onChange={(e) => handleFileChange(field, e)}
            className="hidden"
            id={`${field}-upload`}
          />
          <label htmlFor={`${field}-upload`} className="cursor-pointer">
            <Upload className={`w-10 h-10 mx-auto mb-2 ${files[field] ? 'text-green-600' : 'text-[#8b98a5]'}`} />
            <p className={`text-sm font-medium ${files[field] ? 'text-green-400' : 'text-[#8b98a5]'}`}>
              {files[field] ? `✓ ${files[field].name}` : 'Click to upload'}
            </p>
            {files[field] && (
              <p className="text-xs text-green-600 mt-1">
                {(files[field].size / 1024).toFixed(2)} KB
              </p>
            )}
            <p className="text-xs text-[#8b98a5] mt-1">TGS or JSON</p>
          </label>
        </div>
        
        {previews[field] && (
          <div className="border-2 border-[#2b5278] rounded-lg p-4 flex items-center justify-center bg-gradient-to-br from-purple-50 to-pink-50">
            <Lottie 
              animationData={previews[field]} 
              loop={true}
              style={{ width: 100, height: 100 }}
            />
          </div>
        )}
      </div>
    </div>
  );

  return (
    <div className="max-w-4xl mx-auto">
      <button onClick={() => navigate('/reactions')} className="btn btn-secondary mb-6 flex items-center gap-2">
        <ArrowLeft className="w-4 h-4" />
        Back to Reactions
      </button>

      <div className="card">
        <div className="flex items-center gap-3 mb-6">
          <div className="bg-gradient-to-br from-purple-500 to-pink-600 p-3 rounded-lg">
            <Smile className="w-6 h-6 text-white" />
          </div>
          <div>
            <h1 className="text-2xl font-bold">Create New Reaction</h1>
            <p className="text-[#8b98a5]">Add a new reaction with animations</p>
          </div>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {/* Basic Info */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="label">Emoji *</label>
              <input
                type="text"
                className="input"
                placeholder="👍"
                {...register('emoji', { required: 'Emoji is required' })}
              />
              {errors.emoji && <p className="text-red-500 text-sm mt-1">{errors.emoji.message}</p>}
            </div>

            <div>
              <label className="label">Title *</label>
              <input
                type="text"
                className="input"
                placeholder="Like, Heart, Fire..."
                {...register('title', { required: 'Title is required' })}
              />
              {errors.title && <p className="text-red-500 text-sm mt-1">{errors.title.message}</p>}
            </div>
          </div>

          {/* Flags */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                id="premium"
                className="w-4 h-4 text-purple-600 rounded"
                {...register('premium')}
              />
              <label htmlFor="premium" className="text-sm font-medium">Premium Only</label>
            </div>
            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                id="inactive"
                className="w-4 h-4 text-[#8b98a5] rounded"
                {...register('inactive')}
              />
              <label htmlFor="inactive" className="text-sm font-medium">Mark as Inactive</label>
            </div>
          </div>

          {/* Animations */}
          <div className="space-y-6">
            <h3 className="text-lg font-semibold border-b pb-2">Animations</h3>
            
            <AnimationUpload field="staticIcon" label="Static Icon" required />
            <AnimationUpload field="appearAnimation" label="Appear Animation" required />
            <AnimationUpload field="selectAnimation" label="Select Animation (Hover)" required />
            <AnimationUpload field="activateAnimation" label="Activate Animation" required />
            <AnimationUpload field="effectAnimation" label="Effect Animation (Background)" required />
            <AnimationUpload field="aroundAnimation" label="Around Animation" />
            <AnimationUpload field="centerIcon" label="Center Icon" />
          </div>

          {/* Info */}
          <div className="bg-blue-50 border border-blue-500/30 rounded-lg p-4">
            <h4 className="font-semibold text-blue-900 mb-2">ℹ️ Animation Guide</h4>
            <ul className="list-disc list-inside text-sm text-blue-300 space-y-1">
              <li><strong>Static Icon:</strong> Small icon shown in reaction picker</li>
              <li><strong>Appear Animation:</strong> Plays when reaction picker opens</li>
              <li><strong>Select Animation:</strong> Plays on hover over reaction</li>
              <li><strong>Activate Animation:</strong> Plays when reaction is clicked</li>
              <li><strong>Effect Animation:</strong> Background effect during activation</li>
              <li><strong>Around/Center:</strong> Optional animations for existing reactions</li>
            </ul>
          </div>

          {/* Buttons */}
          <div className="flex gap-4">
            <button
              type="submit"
              disabled={submitting}
              className="btn btn-primary flex-1"
            >
              {submitting ? 'Creating...' : 'Create Reaction'}
            </button>
            <button
              type="button"
              onClick={() => navigate('/reactions')}
              className="btn btn-secondary"
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}









