import React from 'react';

const GraphCanvas: React.FC = () => {
  return (
    <div 
      style={{
        flex: 1,
        position: 'relative',
        background: 'radial-gradient(circle at center, #111 0%, #050505 100%)',
        overflow: 'hidden'
      }}
    >
      {/* Grid Pattern Background */}
      <div 
        style={{
          position: 'absolute',
          top: 0, left: 0, right: 0, bottom: 0,
          backgroundImage: `radial-gradient(circle, rgba(255,255,255,0.05) 1px, transparent 1px)`,
          backgroundSize: '30px 30px'
        }} 
      />
      
      <div 
        style={{
          position: 'absolute',
          top: '50%', left: '50%',
          transform: 'translate(-50%, -50%)',
          textAlign: 'center',
          color: 'var(--text-secondary)'
        }}
      >
        <p style={{ fontSize: '1.2rem', marginBottom: '8px' }}>Graph Visualizer Active</p>
        <p style={{ fontSize: '0.9rem', opacity: 0.6 }}>Waiting for query results...</p>
      </div>
    </div>
  );
};

export default GraphCanvas;
