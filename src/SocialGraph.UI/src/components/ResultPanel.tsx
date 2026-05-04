import React, { useEffect, useState } from 'react';
import { Info } from 'lucide-react';
import { nodeService } from '../services/nodeService';
import type { INode } from '../types/graph';

interface ResultPanelProps {
  selectedNodeId: string | null;
}

const ResultPanel: React.FC<ResultPanelProps> = ({ selectedNodeId }) => {
  const [nodeData, setNodeData] = useState<INode | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!selectedNodeId) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setNodeData(null);
      return;
    }

    const fetchNode = async () => {
      setLoading(true);
      try {
        const data = await nodeService.getNode(selectedNodeId);
        setNodeData(data);
      } catch (err) {
        console.error("Failed to load node details", err);
        setNodeData(null);
      } finally {
        setLoading(false);
      }
    };

    fetchNode();
  }, [selectedNodeId]);

  return (
    <div style={{ flex: 1, display: 'flex', flexDirection: 'column' }}>
      <h3 style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', textTransform: 'uppercase', marginBottom: '16px' }}>
        Node Details
      </h3>
      <div className="glass" style={{ 
        padding: '20px', 
        borderRadius: '12px', 
        flex: 1,
        display: 'flex', 
        flexDirection: 'column',
        overflowY: 'auto'
      }}>
        {!selectedNodeId ? (
          <div style={{ margin: 'auto', color: 'var(--text-secondary)', fontSize: '0.9rem', textAlign: 'center' }}>
            Select a node to view properties
          </div>
        ) : loading ? (
          <div style={{ margin: 'auto', color: 'var(--text-secondary)', fontSize: '0.9rem' }}>
            Loading...
          </div>
        ) : nodeData ? (
          <div>
            <div style={{ marginBottom: '20px', paddingBottom: '16px', borderBottom: '1px solid var(--border-color)' }}>
              <div style={{ fontSize: '0.75rem', color: 'var(--accent-color)', fontWeight: 600, textTransform: 'uppercase', marginBottom: '4px' }}>
                {nodeData.type}
              </div>
              <h2 style={{ fontSize: '1.4rem', color: 'white', margin: 0 }}>
                {nodeData.id}
              </h2>
              
              {Object.values(nodeData.properties || {}).some(val => typeof val === 'string' && val.includes('(Sim)')) && (
                <div style={{ 
                  marginTop: '12px', 
                  padding: '8px 12px', 
                  borderRadius: '8px', 
                  background: 'rgba(59, 130, 246, 0.05)', 
                  border: '1px solid rgba(59, 130, 246, 0.2)',
                  fontSize: '0.7rem',
                  color: '#60a5fa',
                  display: 'flex',
                  alignItems: 'center',
                  gap: '8px',
                  lineHeight: 1.4
                }}>
                  <Info size={14} style={{ flexShrink: 0 }} />
                  <span>Bu veri backend simulasyon motoru tarafindan otomatik olarak uretilmistir.</span>
                </div>
              )}
            </div>
            
            <div>
              <h4 style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', marginBottom: '12px', textTransform: 'uppercase' }}>Properties</h4>
              <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
                {Object.entries(nodeData.properties || {}).map(([key, val]) => (
                  <div key={key} style={{ 
                    background: 'rgba(255,255,255,0.03)', 
                    padding: '10px 12px', 
                    borderRadius: '8px',
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '4px'
                  }}>
                    <span style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>{key}</span>
                    <span style={{ fontSize: '0.95rem', color: 'white', wordBreak: 'break-word' }}>{String(val)}</span>
                  </div>
                ))}
                {Object.keys(nodeData.properties || {}).length === 0 && (
                  <span style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>No properties available.</span>
                )}
              </div>
            </div>
          </div>
        ) : (
          <div style={{ margin: 'auto', color: 'var(--text-secondary)', fontSize: '0.9rem' }}>
            Node not found or error loading.
          </div>
        )}
      </div>
    </div>
  );
};

export default ResultPanel;
