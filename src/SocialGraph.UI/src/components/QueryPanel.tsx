import React, { useState } from 'react';
import { GitBranch, Link2, Zap, UserPlus } from 'lucide-react';
import { traversalService } from '../services/traversalService';
import type { INode, IRecommendation } from '../types/graph';

interface QueryPanelProps {
  onResultsFound: (nodeIds: string[]) => void;
  startNodeId: string | null;
}

const QueryPanel: React.FC<QueryPanelProps> = ({ onResultsFound, startNodeId }) => {
  const [activeTab, setActiveTab] = useState<'traversal' | 'chain' | 'recommend'>('traversal');
  const [loading, setLoading] = useState(false);
  
  // States for inputs
  const [targetNodeId, setTargetNodeId] = useState('');
  const [chainRelations] = useState<string[]>(['FRIEND', 'ATTENDS', 'UPLOADED']);

  const handleBfs = async () => {
    if (!startNodeId) return;
    setLoading(true);
    try {
      const results = await traversalService.bfs(startNodeId);
      onResultsFound(results);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleDfs = async () => {
    if (!startNodeId) return;
    setLoading(true);
    try {
      const results = await traversalService.dfs(startNodeId);
      onResultsFound(results);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleShortestPath = async () => {
    if (!startNodeId || !targetNodeId) return;
    setLoading(true);
    try {
      const results = await traversalService.shortestPath(startNodeId, targetNodeId);
      onResultsFound(results);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleChainQuery = async () => {
    if (!startNodeId) return;
    setLoading(true);
    try {
      const results: INode[] = await traversalService.chain(startNodeId, chainRelations);
      const nodeIds = results.map((n: INode) => n.id);
      onResultsFound(nodeIds);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleRecommendations = async () => {
    if (!startNodeId) return;
    setLoading(true);
    try {
      const results: IRecommendation[] = await traversalService.recommendations(startNodeId);
      const nodeIds = results.map((r: IRecommendation) => r.node.id);
      onResultsFound(nodeIds);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="query-panel" style={{ marginTop: '24px' }}>
      <div style={{ display: 'flex', gap: '8px', marginBottom: '16px', overflowX: 'auto' }}>
        <button 
          onClick={() => setActiveTab('traversal')}
          className={`tab-btn ${activeTab === 'traversal' ? 'active' : ''}`}
        >
          <GitBranch size={14} /> Traversal
        </button>
        <button 
          onClick={() => setActiveTab('chain')}
          className={`tab-btn ${activeTab === 'chain' ? 'active' : ''}`}
        >
          <Link2 size={14} /> Chain
        </button>
        <button 
          onClick={() => setActiveTab('recommend')}
          className={`tab-btn ${activeTab === 'recommend' ? 'active' : ''}`}
        >
          <UserPlus size={14} /> Recs
        </button>
      </div>

      <div style={{ background: 'rgba(255,255,255,0.03)', borderRadius: '12px', padding: '16px' }}>
        {!startNodeId ? (
          <p style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', textAlign: 'center' }}>
            Select a node from search or graph first.
          </p>
        ) : (
          <>
            <p style={{ fontSize: '0.75rem', color: 'var(--text-secondary)', marginBottom: '12px' }}>
              Current Node: <span style={{ color: 'var(--accent-color)' }}>{startNodeId}</span>
            </p>

            {activeTab === 'traversal' && (
              <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                <div style={{ display: 'flex', gap: '8px' }}>
                  <button onClick={handleBfs} disabled={loading} className="query-btn">BFS</button>
                  <button onClick={handleDfs} disabled={loading} className="query-btn">DFS</button>
                </div>
                <div style={{ marginTop: '8px' }}>
                  <input 
                    type="text" 
                    placeholder="Target Node ID..."
                    value={targetNodeId}
                    onChange={(e) => setTargetNodeId(e.target.value)}
                    style={{ width: '100%', marginBottom: '8px' }}
                  />
                  <button onClick={handleShortestPath} disabled={loading || !targetNodeId} className="query-btn primary">
                    <Zap size={14} /> Shortest Path
                  </button>
                </div>
              </div>
            )}

            {activeTab === 'chain' && (
              <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                <p style={{ fontSize: '0.75rem', opacity: 0.6 }}>Relations to follow:</p>
                <div style={{ display: 'flex', flexWrap: 'wrap', gap: '4px', marginBottom: '8px' }}>
                  {chainRelations.map((r, i) => (
                    <span key={i} className="badge">{r}</span>
                  ))}
                </div>
                <button onClick={handleChainQuery} disabled={loading} className="query-btn primary">
                  Run Chain Query
                </button>
              </div>
            )}

            {activeTab === 'recommend' && (
              <button onClick={handleRecommendations} disabled={loading} className="query-btn primary">
                Get Friend Suggestions
              </button>
            )}
          </>
        )}
      </div>

      <style>{`
        .tab-btn {
          padding: 6px 12px;
          border-radius: 8px;
          border: 1px solid rgba(255,255,255,0.1);
          background: transparent;
          color: var(--text-secondary);
          font-size: 0.8rem;
          display: flex;
          align-items: center;
          gap: 6px;
          cursor: pointer;
          white-space: nowrap;
          transition: all 0.2s;
        }
        .tab-btn.active {
          background: rgba(255,255,255,0.1);
          color: white;
          border-color: rgba(255,255,255,0.3);
        }
        .query-btn {
          flex: 1;
          padding: 8px;
          border-radius: 8px;
          border: 1px solid rgba(255,255,255,0.1);
          background: rgba(255,255,255,0.05);
          color: white;
          font-size: 0.8rem;
          cursor: pointer;
          transition: all 0.2s;
          display: flex;
          align-items: center;
          justify-content: center;
          gap: 8px;
        }
        .query-btn:hover:not(:disabled) {
          background: rgba(255,255,255,0.1);
        }
        .query-btn.primary {
          background: var(--accent-color);
          border: none;
        }
        .query-btn:disabled {
          opacity: 0.4;
          cursor: not-allowed;
        }
        .badge {
          background: rgba(255,255,255,0.1);
          padding: 2px 8px;
          border-radius: 4px;
          font-size: 0.7rem;
        }
        input {
          background: rgba(0,0,0,0.2);
          border: 1px solid rgba(255,255,255,0.1);
          color: white;
          padding: 8px;
          border-radius: 8px;
          font-size: 0.8rem;
        }
      `}</style>
    </div>
  );
};

export default QueryPanel;
