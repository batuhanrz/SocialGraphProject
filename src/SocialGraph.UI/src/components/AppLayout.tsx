import React from 'react';
import SearchBar from './SearchBar';
import GraphCanvas from './GraphCanvas';
import ResultPanel from './ResultPanel';

const AppLayout: React.FC = () => {
  return (
    <div style={{ display: 'flex', height: '100vh', width: '100vw' }}>
      {/* Sidebar */}
      <aside 
        className="glass"
        style={{
          width: '320px',
          height: '100%',
          padding: '24px',
          display: 'flex',
          flexDirection: 'column',
          zIndex: 10
        }}
      >
        <div style={{ marginBottom: '40px' }}>
          <h1 style={{ fontSize: '1.5rem', fontWeight: 600, letterSpacing: '-0.5px' }}>
            Social<span style={{ color: 'var(--accent-color)' }}>Graph</span>
          </h1>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.85rem' }}>v1.0 • Explorer Mode</p>
        </div>

        <SearchBar />

        <ResultPanel />

        <footer style={{ marginTop: 'auto', fontSize: '0.75rem', color: 'var(--text-secondary)', opacity: 0.5 }}>
          Built with React + TypeScript • Team SocialGraph
        </footer>
      </aside>

      {/* Main Content Area */}
      <main style={{ flex: 1, position: 'relative', display: 'flex' }}>
        <GraphCanvas />
      </main>
    </div>
  );
};

export default AppLayout;
