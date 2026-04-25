import React from 'react';

const ResultPanel: React.FC = () => {
  return (
    <div style={{ flex: 1 }}>
      <h3 style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', textTransform: 'uppercase', marginBottom: '16px' }}>
        Node Details
      </h3>
      <div className="glass" style={{ padding: '20px', borderRadius: '12px', minHeight: '200px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <p style={{ color: 'var(--text-secondary)', fontSize: '0.9rem', textAlign: 'center' }}>
          Select a node on the canvas to view properties
        </p>
      </div>
    </div>
  );
};

export default ResultPanel;
