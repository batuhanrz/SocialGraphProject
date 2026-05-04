import React, { useState, useEffect } from 'react';
import { ChevronDown, ChevronUp, Cpu, User } from 'lucide-react';
import { nodeService } from '../services/nodeService';
import type { INode } from '../types/graph';

interface SimNodeListProps {
  onNodeSelect: (id: string) => void;
}

export const SimNodeList: React.FC<SimNodeListProps> = ({ onNodeSelect }) => {
  const [expanded, setExpanded] = useState(true);
  const [simNodes, setSimNodes] = useState<INode[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (expanded && simNodes.length === 0) {
      loadSimNodes();
    }
  }, [expanded]);

  const loadSimNodes = async () => {
    setLoading(true);
    try {
      const all = await nodeService.getAllNodes();
      // Filter nodes that have (Sim) in ID or Name property
      const filtered = all.filter(n => 
        n.id.includes('(Sim)') || 
        Object.values(n.properties || {}).some(v => typeof v === 'string' && v.includes('(Sim)'))
      );
      setSimNodes(filtered);
    } catch (err) {
      console.error("Failed to load sim nodes", err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{
      position: 'absolute',
      top: '24px',
      right: '24px',
      zIndex: 1000,
      width: '240px',
      maxHeight: '350px',
      display: 'flex',
      flexDirection: 'column',
      pointerEvents: 'auto'
    }}>
      <button 
        onClick={() => setExpanded(!expanded)}
        className="glass"
        style={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          padding: '12px 16px',
          borderRadius: expanded ? '12px 12px 0 0' : '12px',
          border: '1px solid rgba(255,255,255,0.1)',
          cursor: 'pointer',
          width: '100%',
          textAlign: 'left',
          color: 'white',
          background: 'rgba(20, 20, 20, 0.6)',
          backdropFilter: 'blur(20px)',
          transition: 'all 0.3s ease'
        }}
      >
        <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
          <Cpu size={16} color="#60a5fa" />
          <span style={{ fontSize: '0.8rem', fontWeight: 600 }}>Simulation Nodes</span>
        </div>
        {expanded ? <ChevronUp size={16} /> : <ChevronDown size={16} />}
      </button>

      {expanded && (
        <div className="glass custom-scrollbar" style={{
          background: 'rgba(10, 10, 10, 0.8)',
          backdropFilter: 'blur(20px)',
          border: '1px solid rgba(255,255,255,0.1)',
          borderTop: 'none',
          borderRadius: '0 0 12px 12px',
          padding: '8px',
          overflowY: 'auto',
          maxHeight: '280px',
          flex: 1
        }}>
          {loading ? (
            <div style={{ padding: '16px', textAlign: 'center', color: 'rgba(255,255,255,0.4)', fontSize: '0.75rem' }}>
              Loading nodes...
            </div>
          ) : simNodes.length === 0 ? (
            <div style={{ padding: '16px', textAlign: 'center', color: 'rgba(255,255,255,0.4)', fontSize: '0.75rem' }}>
              No simulation nodes found.
            </div>
          ) : (
            <div style={{ display: 'flex', flexDirection: 'column', gap: '4px' }}>
              {simNodes.map(node => (
                <button
                  key={node.id}
                  onClick={() => onNodeSelect(node.id)}
                  style={{
                    display: 'flex',
                    alignItems: 'center',
                    gap: '10px',
                    padding: '8px 12px',
                    borderRadius: '8px',
                    background: 'transparent',
                    border: 'none',
                    color: 'rgba(255,255,255,0.8)',
                    fontSize: '0.75rem',
                    textAlign: 'left',
                    cursor: 'pointer',
                    transition: 'all 0.2s ease',
                    width: '100%'
                  }}
                  onMouseEnter={(e) => e.currentTarget.style.background = 'rgba(255,255,255,0.05)'}
                  onMouseLeave={(e) => e.currentTarget.style.background = 'transparent'}
                >
                  <User size={12} color="#3b82f6" />
                  <span style={{ overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                    {String(node.properties?.Name || node.id).replace(' (Sim)', '')}
                  </span>
                </button>
              ))}
            </div>
          )}
        </div>
      )}
    </div>
  );
};
