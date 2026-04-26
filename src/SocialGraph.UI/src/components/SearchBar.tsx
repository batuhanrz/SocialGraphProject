import React, { useState, useEffect, useRef } from 'react';
import { nodeService } from '../services/nodeService';
import type { ISearchResult } from '../types/graph';

interface SearchBarProps {
  onNodeSelect: (nodeId: string) => void;
}

const SearchBar: React.FC<SearchBarProps> = ({ onNodeSelect }) => {
  const [isFocused, setIsFocused] = useState(false);
  const [query, setQuery] = useState('');
  const [results, setResults] = useState<ISearchResult[]>([]);
  const [loading, setLoading] = useState(false);
  
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const fetchResults = async () => {
      if (!query.trim()) {
        setResults([]);
        return;
      }
      
      setLoading(true);
      try {
        const data = await nodeService.searchNodes(query);
        setResults(data || []);
      } catch (err) {
        console.error("Search failed", err);
        setResults([]);
      } finally {
        setLoading(false);
      }
    };

    const timer = setTimeout(fetchResults, 300); // debounce
    return () => clearTimeout(timer);
  }, [query]);

  // Click outside listener for dropdown
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (containerRef.current && !containerRef.current.contains(event.target as Node)) {
        setIsFocused(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const handleSelect = (nodeId: string) => {
    onNodeSelect(nodeId);
    setIsFocused(false);
    setQuery('');
  };

  return (
    <div style={{ marginBottom: '24px', position: 'relative' }} ref={containerRef}>
      <div 
        className="glass"
        style={{
          display: 'flex',
          alignItems: 'center',
          padding: '12px 16px',
          borderRadius: '12px',
          transition: 'all 0.3s ease',
          boxShadow: isFocused ? '0 0 20px var(--accent-glow)' : 'none',
          borderColor: isFocused ? 'var(--accent-color)' : 'var(--border-color)'
        }}
      >
        <input 
          type="text" 
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          placeholder="Search nodes (e.g. Fatma, Photo_1)..."
          onFocus={() => setIsFocused(true)}
          style={{
            background: 'none',
            border: 'none',
            color: 'white',
            width: '100%',
            outline: 'none',
            fontSize: '0.95rem'
          }}
        />
        <span style={{ color: 'var(--text-secondary)', cursor: 'pointer' }}>
          {loading ? '⏳' : '🔍'}
        </span>
      </div>

      {isFocused && (query.trim().length > 0) && (
        <div className="glass" style={{
          position: 'absolute',
          top: '100%',
          left: 0,
          right: 0,
          marginTop: '8px',
          borderRadius: '12px',
          padding: '8px',
          maxHeight: '250px',
          overflowY: 'auto',
          zIndex: 20
        }}>
          {results.length > 0 ? (
            results.map((res) => (
              <div 
                key={res.nodeId}
                onClick={() => handleSelect(res.nodeId)}
                style={{
                  padding: '10px 12px',
                  borderRadius: '8px',
                  cursor: 'pointer',
                  display: 'flex',
                  justifyContent: 'space-between',
                  alignItems: 'center',
                  marginBottom: '4px',
                  transition: 'background 0.2s',
                }}
                onMouseEnter={(e) => e.currentTarget.style.backgroundColor = 'rgba(255,255,255,0.1)'}
                onMouseLeave={(e) => e.currentTarget.style.backgroundColor = 'transparent'}
              >
                <span style={{ color: 'white', fontSize: '0.9rem' }}>{res.label}</span>
                <span style={{ 
                  color: 'var(--text-secondary)', 
                  fontSize: '0.75rem',
                  padding: '2px 8px',
                  borderRadius: '12px',
                  backgroundColor: 'rgba(255,255,255,0.05)'
                }}>
                  {res.type}
                </span>
              </div>
            ))
          ) : (
            <div style={{ padding: '12px', color: 'var(--text-secondary)', fontSize: '0.85rem', textAlign: 'center' }}>
              {loading ? 'Searching...' : 'No results found.'}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default SearchBar;
