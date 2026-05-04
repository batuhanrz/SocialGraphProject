import React, { useState } from 'react';
import { GitBranch, Link2, Zap, UserPlus } from 'lucide-react';
import { traversalService } from '../services/traversalService';
import { nodeService } from '../services/nodeService';
import type { INode, IRecommendation } from '../types/graph';

interface QueryPanelProps {
  onResultsFound: (nodeIds: string[]) => void;
  startNodeId: string | null;
  targetNodeId: string;
  onTargetChange: (id: string) => void;
  onStartChange?: (id: string) => void;
}

const QueryPanel: React.FC<QueryPanelProps> = ({ onResultsFound, startNodeId, targetNodeId, onTargetChange, onStartChange }) => {
  const [activeTab, setActiveTab] = useState<'traversal' | 'chain' | 'recommend'>('traversal');
  const [loading, setLoading] = useState(false);
  const [selectedAlgo, setSelectedAlgo] = useState<'BFS' | 'DFS' | null>(null);
  const [nodeLabels, setNodeLabels] = useState<Record<string, string>>({});
  const [chainRelations] = useState<string[]>(['FRIEND', 'ATTENDS', 'UPLOADED']);
  const [showDirectionWarning, setShowDirectionWarning] = useState(false);

  // ID → İsim çözümleme
  React.useEffect(() => {
    setShowDirectionWarning(false);
    const resolveNames = async () => {
      const idsToResolve = [startNodeId, targetNodeId].filter(id => id && !nodeLabels[id]) as string[];
      if (idsToResolve.length === 0) return;

      const newLabels = { ...nodeLabels };
      for (const id of idsToResolve) {
        try {
          const node = await nodeService.getNode(id);
          newLabels[id] = (node.properties.Name as string) || (node.properties.Title as string) || id;
        } catch {
          newLabels[id] = id;
        }
      }
      setNodeLabels(newLabels);
    };
    resolveNames();
  }, [startNodeId, targetNodeId]);

  // BFS/DFS butonları: Sadece algoritma seç (Toggle)
  const handleSelectAlgo = (algo: 'BFS' | 'DFS') => {
    setSelectedAlgo(prev => prev === algo ? null : algo);
  };

  // Shortest Path: Seçili algoritmayı kullanarak sorgu at
  const handleShortestPath = async () => {
    if (!startNodeId || !targetNodeId || !selectedAlgo) return;
    setLoading(true);
    setShowDirectionWarning(false);
    try {
      const results = await traversalService.shortestPath(startNodeId, targetNodeId, selectedAlgo);
      if (results.length === 0) {
        setShowDirectionWarning(true);
      }
      onResultsFound(results);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleSwap = () => {
    if (startNodeId && targetNodeId && onStartChange) {
      const temp = startNodeId;
      onStartChange(targetNodeId);
      onTargetChange(temp);
      setShowDirectionWarning(false);
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

  const canRunQuery = !!startNodeId && !!targetNodeId && !!selectedAlgo;

  return (
    <div className="query-panel" style={{ marginTop: '24px' }}>
      {/* Tab Seçiciler */}
      <div style={{ display: 'flex', gap: '8px', marginBottom: '16px', overflowX: 'auto' }}>
        <button onClick={() => setActiveTab('traversal')} className={`tab-btn ${activeTab === 'traversal' ? 'active' : ''}`}>
          <GitBranch size={14} /> Traversal
        </button>
        <button onClick={() => setActiveTab('chain')} className={`tab-btn ${activeTab === 'chain' ? 'active' : ''}`}>
          <Link2 size={14} /> Chain
        </button>
        <button onClick={() => setActiveTab('recommend')} className={`tab-btn ${activeTab === 'recommend' ? 'active' : ''}`}>
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
            {/* Current + Target Node Bilgisi */}
            <div style={{ marginBottom: '16px' }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '6px' }}>
                <div style={{ width: '8px', height: '8px', borderRadius: '50%', background: '#3b82f6', boxShadow: '0 0 8px #3b82f6' }} />
                <p style={{ fontSize: '0.7rem', color: 'var(--text-secondary)', margin: 0, opacity: 0.6 }}>Origin:</p>
                <p style={{ fontSize: '0.85rem', fontWeight: 600, color: '#3b82f6', margin: 0 }}>
                  {nodeLabels[startNodeId] || startNodeId}
                </p>
              </div>
              {targetNodeId && (
                <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                  <div style={{ width: '8px', height: '8px', borderRadius: '50%', background: '#ef4444', boxShadow: '0 0 8px #ef4444' }} />
                  <p style={{ fontSize: '0.7rem', color: 'var(--text-secondary)', margin: 0, opacity: 0.6 }}>Target:</p>
                  <p style={{ fontSize: '0.85rem', fontWeight: 600, color: '#ef4444', margin: 0 }}>
                    {nodeLabels[targetNodeId] || targetNodeId}
                  </p>
                </div>
              )}
            </div>

            {/* Traversal Tab */}
            {activeTab === 'traversal' && (
              <div style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                {/* Algoritma Seçicileri */}
                <p style={{ fontSize: '0.7rem', color: 'var(--text-secondary)', opacity: 0.6, margin: 0 }}>Algorithm:</p>
                <div style={{ display: 'flex', gap: '8px' }}>
                  <button
                    onClick={() => handleSelectAlgo('BFS')}
                    className={`query-btn ${selectedAlgo === 'BFS' ? 'algo-active' : ''}`}
                  >
                    BFS
                  </button>
                  <button
                    onClick={() => handleSelectAlgo('DFS')}
                    className={`query-btn ${selectedAlgo === 'DFS' ? 'algo-active' : ''}`}
                  >
                    DFS
                  </button>
                </div>

                {/* Target input — isim veya ID göster */}
                <input
                  type="text"
                  placeholder="Target Node ID (or right-click a node)..."
                  value={nodeLabels[targetNodeId] || targetNodeId}
                  onChange={(e) => onTargetChange(e.target.value)}
                  style={{ width: '100%' }}
                />

                {/* Shortest Path Butonu */}
                <button
                  onClick={handleShortestPath}
                  disabled={loading || !canRunQuery}
                  className="query-btn primary"
                  title={!canRunQuery ? 'Origin, Target ve Algoritma seçilmeli' : ''}
                >
                  <Zap size={14} /> {selectedAlgo ? `Run ${selectedAlgo} Path` : 'Find Path'}
                </button>

                {showDirectionWarning && (
                  <div style={{ background: 'rgba(245, 158, 11, 0.1)', border: '1px solid rgba(245, 158, 11, 0.2)', padding: '10px', borderRadius: '8px', marginTop: '8px' }}>
                    <p style={{ fontSize: '0.75rem', color: '#f59e0b', margin: '0 0 8px 0', lineHeight: 1.4 }}>
                      Sonuç bulunamadı. LIKES ve ATTENDS gibi ilişkiler tek yönlüdür. Hedef ve Başlangıç düğümlerini yer değiştirip tekrar deneyin.
                    </p>
                    {onStartChange && (
                      <button onClick={handleSwap} className="query-btn" style={{ width: '100%', borderColor: '#f59e0b', color: '#f59e0b' }}>
                        Swap Origin & Target
                      </button>
                    )}
                  </div>
                )}

                {!canRunQuery && startNodeId && !showDirectionWarning && (
                  <p style={{ fontSize: '0.65rem', color: 'var(--text-secondary)', opacity: 0.5, textAlign: 'center', margin: 0 }}>
                    {!selectedAlgo ? '↑ Bir algoritma seçin' : !targetNodeId ? '↑ Bir hedef düğüm seçin (sağ tık)' : ''}
                  </p>
                )}
              </div>
            )}

            {/* Chain Tab */}
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

            {/* Recommend Tab */}
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
        .query-btn.algo-active {
          background: rgba(59, 130, 246, 0.2);
          border-color: #3b82f6;
          color: white;
          box-shadow: 0 0 15px rgba(59, 130, 246, 0.3);
        }
        .query-btn.primary {
          background: var(--accent-color);
          border: none;
        }
        .query-btn.primary:disabled {
          background: rgba(255,255,255,0.05);
          border: 1px solid rgba(255,255,255,0.05);
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
