import React, { useState, useCallback } from 'react';
import SearchBar from './SearchBar';
import GraphCanvas from './GraphCanvas';
import ResultPanel from './ResultPanel';
import QueryPanel from './QueryPanel';
import { SimNodeList } from './SimNodeList';

const AppLayout: React.FC = () => {
  const [selectedNodeId, setSelectedNodeId] = useState<string | null>(null);
  const [targetNodeId, setTargetNodeId] = useState<string>('');
  const [highlightNodeIds, setHighlightNodeIds] = useState<string[]>([]);
  const [highlightEdgeIds, setHighlightEdgeIds] = useState<string[]>([]);
  const [highlightMode, setHighlightMode] = useState<'path' | 'recs' | 'chain'>('path');
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const handleNodeSelect = useCallback((id: string) => {
    setSelectedNodeId(id);
    setHighlightNodeIds(prev => {
      if (!prev.includes(id)) return [];
      return prev;
    });
  }, []);

  const handleNodeRightClick = useCallback((id: string) => {
    setTargetNodeId(prev => prev === id ? "" : id);
  }, []);

  const handleQueryStart = useCallback(() => {
    setHighlightNodeIds([]); // Onceki aramanin / recs glow'larinin kalintilarini hemen temizle
    setHighlightEdgeIds([]);
    setHighlightMode('path'); // Varsayilan moda don
  }, []);

  const handleQueryResults = useCallback((nodeIds: string[], mode: 'path' | 'recs' | 'chain' = 'path') => {
    setIsLoading(true);
    setError(null);
    setHighlightMode(mode);

    setTimeout(() => {
      setHighlightNodeIds(nodeIds);
      setIsLoading(false);
      if (nodeIds.length === 0) {
        setError('Sonuc bulunamadi.');
      } else if (!selectedNodeId) {
        setSelectedNodeId(nodeIds[0]);
      }
    }, 800);
  }, [selectedNodeId]);

  return (
    <div style={{ display: 'flex', height: '100vh', width: '100vw', backgroundColor: '#050505', color: 'white', overflow: 'hidden' }}>
      {/* Sidebar */}
      <aside
        className="sidebar glass"
        style={{
          width: '360px',
          height: '100%',
          padding: '24px',
          display: 'flex',
          flexDirection: 'column',
          zIndex: 10,
          borderRight: '1px solid rgba(255,255,255,0.08)',
          boxShadow: '20px 0 50px rgba(0,0,0,0.8)',
          transition: 'all 0.3s ease'
        }}
      >
        <div style={{ marginBottom: '32px' }}>
          <h1 style={{ fontSize: '1.5rem', fontWeight: 600, letterSpacing: '-0.5px', margin: 0 }}>
            Social<span style={{ color: 'var(--accent-color)' }}>Graph</span>
          </h1>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.8rem', marginTop: '4px' }}>
            ENGINE V2.0
          </p>
        </div>

        <div style={{ flex: 1, overflowY: 'auto', paddingRight: '4px' }} className="custom-scrollbar">
          {isLoading && (
            <div className="status-box" style={{ background: 'rgba(59, 130, 246, 0.1)', color: '#3b82f6', border: '1px solid rgba(59, 130, 246, 0.2)' }}>
              <div className="spinner" /> Analiz ediliyor...
            </div>
          )}

          {error && (
            <div className="status-box" style={{ background: 'rgba(245, 158, 11, 0.1)', color: '#f59e0b', border: '1px solid rgba(245, 158, 11, 0.2)' }}>
              ⚠ {error}
            </div>
          )}

          <SearchBar onNodeSelect={handleNodeSelect} />

          <QueryPanel
            startNodeId={selectedNodeId}
            targetNodeId={targetNodeId}
            onStartChange={handleNodeSelect}
            onTargetChange={setTargetNodeId}
            onResultsFound={handleQueryResults}
            onQueryStart={handleQueryStart}
          />

          <ResultPanel selectedNodeId={selectedNodeId} />
        </div>

        <footer style={{ marginTop: '24px', fontSize: '0.7rem', color: 'var(--text-secondary)', opacity: 0.4 }}>
          &copy; 2026 SocialGraph Project
        </footer>
      </aside>

      {/* Main Content Area */}
      <main style={{ flex: 1, position: 'relative', display: 'flex' }}>
        <GraphCanvas
          selectedNodeId={selectedNodeId}
          targetNodeId={targetNodeId}
          onNodeSelect={handleNodeSelect}
          onNodeRightClick={handleNodeRightClick}
          highlightNodeIds={highlightNodeIds}
          highlightEdgeIds={highlightEdgeIds}
          highlightMode={highlightMode}
        />

        <SimNodeList 
          onNodeSelect={handleNodeSelect} 
          onEdgeSelect={(id) => setHighlightEdgeIds([id])}
        />

        {/* Floating Legend */}
        <div className="legend-box" style={{
          position: 'absolute',
          bottom: '24px',
          right: '24px',
          background: 'rgba(10, 10, 10, 0.7)',
          backdropFilter: 'blur(15px)',
          padding: '14px 20px',
          borderRadius: '14px',
          border: '1px solid rgba(255,255,255,0.08)',
          display: 'flex',
          flexDirection: 'column',
          gap: '10px',
          fontSize: '0.7rem',
          pointerEvents: 'none',
          boxShadow: '0 8px 32px rgba(0,0,0,0.4)'
        }}>
          {/* Node Types */}
          <div style={{ display: 'flex', gap: '16px' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
              <div style={{ width: '8px', height: '8px', borderRadius: '50%', background: '#3b82f6', boxShadow: '0 0 8px #3b82f6' }} /> User
            </div>
            <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
              <div style={{ width: '8px', height: '8px', borderRadius: '2px', background: '#10b981', boxShadow: '0 0 8px #10b981' }} /> Photo
            </div>
            <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
              <div style={{ width: '8px', height: '0', borderLeft: '4px solid transparent', borderRight: '4px solid transparent', borderBottom: '8px solid #f59e0b', filter: 'drop-shadow(0 0 4px #f59e0b)' }} /> Event
            </div>
          </div>
          {/* State Indicators */}
          <div style={{ display: 'flex', gap: '16px', borderTop: '1px solid rgba(255,255,255,0.06)', paddingTop: '8px' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
              <div style={{ width: '8px', height: '8px', borderRadius: '50%', border: '2px solid #3b82f6' }} /> Origin
            </div>
            <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
              <div style={{ width: '8px', height: '8px', borderRadius: '50%', border: '2px solid #ef4444' }} /> Target
            </div>
            <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
              <div style={{ width: '8px', height: '8px', borderRadius: '50%', border: '2px solid #a855f7' }} /> Pinned
            </div>
          </div>
          <div style={{ fontSize: '0.6rem', opacity: 0.4 }}>Left click = Origin • Right click = Target • Shift = Pin</div>
        </div>
      </main>

      <style>{`
        .custom-scrollbar::-webkit-scrollbar {
          width: 4px;
        }
        .custom-scrollbar::-webkit-scrollbar-track {
          background: transparent;
        }
        .custom-scrollbar::-webkit-scrollbar-thumb {
          background: rgba(255,255,255,0.1);
          border-radius: 10px;
        }
        .glass {
          background: rgba(15, 15, 15, 0.7);
          backdrop-filter: blur(25px);
          -webkit-backdrop-filter: blur(25px);
        }
        .status-box {
          padding: 12px;
          border-radius: 12px;
          margin-bottom: 20px;
          font-size: 0.85rem;
          display: flex;
          align-items: center;
          gap: 12px;
          animation: slideIn 0.3s ease;
        }
        .spinner {
          width: 14px;
          height: 14px;
          border: 2px solid rgba(255,255,255,0.3);
          border-top-color: #fff;
          border-radius: 50%;
          animation: spin 0.8s linear infinite;
        }
        @keyframes spin { to { transform: rotate(360deg); } }
        @keyframes slideIn { from { opacity: 0; transform: translateY(-10px); } to { opacity: 1; transform: translateY(0); } }

        @media (max-width: 1024px) {
          .sidebar {
            width: 280px !important;
          }
        }
        @media (max-width: 768px) {
          .sidebar {
            position: absolute;
            left: -360px;
          }
          .sidebar.active {
            left: 0;
          }
        }
      `}</style>
    </div>
  );
};

export default AppLayout;
