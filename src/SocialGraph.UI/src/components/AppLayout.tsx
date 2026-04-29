import React, { useState } from 'react';
import SearchBar from './SearchBar';
import GraphCanvas from './GraphCanvas';
import ResultPanel from './ResultPanel';
import QueryPanel from './QueryPanel';

const AppLayout: React.FC = () => {
  const [selectedNodeId, setSelectedNodeId] = useState<string | null>(null);
  const [highlightNodeIds, setHighlightNodeIds] = useState<string[]>([]);

  const handleNodeSelect = (id: string) => {
    setSelectedNodeId(id);
    // When a node is manually selected, clear previous highlights if not part of it
    if (!highlightNodeIds.includes(id)) {
      setHighlightNodeIds([]);
    }
  };

  const handleQueryResults = (nodeIds: string[]) => {
    setHighlightNodeIds(nodeIds);
    if (nodeIds.length > 0) {
      // Auto-select the first result if nothing is selected
      if (!selectedNodeId) {
        setSelectedNodeId(nodeIds[0]);
      }
    }
  };

  return (
    <div style={{ display: 'flex', height: '100vh', width: '100vw', backgroundColor: '#050505', color: 'white' }}>
      {/* Sidebar */}
      <aside 
        className="glass"
        style={{
          width: '360px',
          height: '100%',
          padding: '24px',
          display: 'flex',
          flexDirection: 'column',
          zIndex: 10,
          borderRight: '1px solid rgba(255,255,255,0.05)',
          boxShadow: '20px 0 50px rgba(0,0,0,0.5)'
        }}
      >
        <div style={{ marginBottom: '32px' }}>
          <h1 style={{ fontSize: '1.5rem', fontWeight: 600, letterSpacing: '-0.5px', margin: 0 }}>
            Social<span style={{ color: 'var(--accent-color)' }}>Graph</span>
          </h1>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.8rem', marginTop: '4px' }}>
            ENGINE V2.0 • Sude Framework
          </p>
        </div>

        <div style={{ flex: 1, overflowY: 'auto', paddingRight: '4px' }} className="custom-scrollbar">
          <SearchBar onNodeSelect={handleNodeSelect} />
          
          <QueryPanel 
            startNodeId={selectedNodeId} 
            onResultsFound={handleQueryResults} 
          />

          <ResultPanel selectedNodeId={selectedNodeId} />
        </div>

        <footer style={{ marginTop: '24px', fontSize: '0.7rem', color: 'var(--text-secondary)', opacity: 0.4 }}>
          &copy; 2026 SocialGraph Project • Developed by Sude (Frontend Lead)
        </footer>
      </aside>

      {/* Main Content Area */}
      <main style={{ flex: 1, position: 'relative', display: 'flex' }}>
        <GraphCanvas 
          selectedNodeId={selectedNodeId} 
          onNodeSelect={handleNodeSelect}
          highlightNodeIds={highlightNodeIds}
        />
        
        {/* Floating Legend */}
        <div style={{
          position: 'absolute',
          bottom: '24px',
          right: '24px',
          background: 'rgba(0,0,0,0.6)',
          backdropFilter: 'blur(10px)',
          padding: '12px 20px',
          borderRadius: '12px',
          border: '1px solid rgba(255,255,255,0.1)',
          display: 'flex',
          gap: '20px',
          fontSize: '0.75rem',
          pointerEvents: 'none'
        }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
            <div style={{ width: '10px', height: '10px', borderRadius: '50%', background: '#3b82f6' }} /> User
          </div>
          <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
            <div style={{ width: '10px', height: '10px', borderRadius: '2px', background: '#10b981' }} /> Photo
          </div>
          <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
            <div style={{ width: '10px', height: '0', borderLeft: '5px solid transparent', borderRight: '5px solid transparent', borderBottom: '10px solid #f59e0b' }} /> Event
          </div>
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
          backdrop-filter: blur(20px);
          -webkit-backdrop-filter: blur(20px);
        }
      `}</style>
    </div>
  );
};

export default AppLayout;
