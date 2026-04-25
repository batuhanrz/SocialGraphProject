import React, { useState } from 'react';

const SearchBar: React.FC = () => {
  const [isFocused, setIsFocused] = useState(false);

  return (
    <div style={{ marginBottom: '24px' }}>
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
          placeholder="Search nodes (e.g. Fatma, Photo_1)..."
          onFocus={() => setIsFocused(true)}
          onBlur={() => setIsFocused(false)}
          style={{
            background: 'none',
            border: 'none',
            color: 'white',
            width: '100%',
            outline: 'none',
            fontSize: '0.95rem'
          }}
        />
        <span style={{ color: 'var(--text-secondary)', cursor: 'pointer' }}>🔍</span>
      </div>
    </div>
  );
};

export default SearchBar;
