import React, { useState } from 'react';
import { GitBranch, Link2, Zap, UserPlus, Network } from 'lucide-react';
import { traversalService } from '../services/traversalService';
import { nodeService } from '../services/nodeService';
import type { INode, IRecommendation, IChainResponse } from '../types/graph';

interface QueryPanelProps {
  onResultsFound: (nodeIds: string[], mode?: 'path' | 'recs' | 'chain') => void;
  startNodeId: string | null;
  targetNodeId: string;
  onTargetChange: (id: string) => void;
  onStartChange?: (id: string) => void;
  onQueryStart?: () => void;
}

const QueryPanel: React.FC<QueryPanelProps> = ({ onResultsFound, startNodeId, targetNodeId, onTargetChange, onStartChange, onQueryStart }) => {
  const [activeTab, setActiveTab] = useState<'traversal' | 'chain' | 'recommend'>('traversal');
  const [loading, setLoading] = useState(false);
  const [selectedAlgo, setSelectedAlgo] = useState<'BFS' | 'DFS' | null>(null);
  const [nodeLabels, setNodeLabels] = useState<Record<string, string>>({});
  const [chainRelations, setChainRelations] = useState<string[]>(['FRIEND', 'ATTENDS', 'UPLOADED']);
  const [showDirectionWarning, setShowDirectionWarning] = useState(false);
  const [reportData, setReportData] = useState<{ path: string[], algo: 'BFS' | 'DFS' } | null>(null);
  const [recsReportData, setRecsReportData] = useState<IRecommendation[] | null>(null);
  const [chainReportData, setChainReportData] = useState<IChainResponse | null>(null);
  const [isReportOpen, setIsReportOpen] = useState(false);
  const [nodeTypes, setNodeTypes] = useState<Record<string, string>>({});

  // ID → İsim çözümleme
  React.useEffect(() => {
    const resolveNames = async () => {
      const idsToResolve = [
        startNodeId, 
        targetNodeId, 
        ...(reportData?.path || []), 
        ...(recsReportData?.map(r => r.node.id) || [])
      ].filter(id => id && !nodeLabels[id]) as string[];
      
      if (idsToResolve.length === 0) return;

      const newLabels = { ...nodeLabels };
      const newTypes = { ...nodeTypes };
      for (const id of idsToResolve) {
        try {
          const node = await nodeService.getNode(id);
          newLabels[id] = (node.properties.Name as string) || (node.properties.Title as string) || id;
          newTypes[id] = node.type;
        } catch {
          newLabels[id] = id;
          newTypes[id] = 'Unknown';
        }
      }
      setNodeLabels(newLabels);
      setNodeTypes(newTypes);
    };
    resolveNames();
  }, [startNodeId, targetNodeId, reportData?.path, recsReportData]);

  // BFS/DFS butonları: Sadece algoritma seç (Toggle)
  const handleSelectAlgo = (algo: 'BFS' | 'DFS') => {
    setSelectedAlgo(prev => prev === algo ? null : algo);
    setShowDirectionWarning(false);
  };

  // Shortest Path: Seçili algoritmayı kullanarak sorgu at
  const handleShortestPath = async () => {
    if (!startNodeId || !targetNodeId || !selectedAlgo) return;
    if (onQueryStart) onQueryStart();
    setLoading(true);
    setShowDirectionWarning(false);
    try {
      const results = await traversalService.shortestPath(startNodeId, targetNodeId, selectedAlgo);
      if (results.length === 0) {
        setShowDirectionWarning(true);
        setReportData(null);
      } else {
        setReportData({ path: results, algo: selectedAlgo });
        setIsReportOpen(true);
      }
      onResultsFound(results, 'path');
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
    if (onQueryStart) onQueryStart();
    setLoading(true);
    try {
      const response: IChainResponse = await traversalService.chain(startNodeId, chainRelations);
      const nodeIds = response.nodes.map((n: INode) => n.id);
      setChainReportData(response);
      onResultsFound(nodeIds, 'chain');
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleRecommendations = async () => {
    if (!startNodeId) return;
    if (onQueryStart) onQueryStart();
    setLoading(true);
    setRecsReportData(null);
    try {
      const results: IRecommendation[] = await traversalService.recommendations(startNodeId);
      const nodeIds = results.map((r: IRecommendation) => r.node.id);
      
      setRecsReportData(results);
      setIsReportOpen(true);
      onResultsFound(nodeIds, 'recs');
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
              {targetNodeId && activeTab === 'traversal' && (
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

                {/* Algorithm Report Accordion */}
                {reportData && (
                  <div className="algo-report">
                    <button 
                      className="algo-report-header" 
                      onClick={() => setIsReportOpen(!isReportOpen)}
                    >
                      <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                        <Zap size={14} color="var(--accent-color)" />
                        <span>{reportData.algo} Algorithm Report</span>
                      </div>
                      <span>{isReportOpen ? '▲' : '▼'}</span>
                    </button>
                    
                    {isReportOpen && (
                      <div className="algo-report-content custom-scrollbar">
                        <p className="report-intro">
                          {reportData.algo === 'BFS' 
                            ? 'Genişlik Öncelikli Arama (BFS), grafı katman katman (level-by-level) tarayarak en kısa yolu garantiler.'
                            : 'Derinlik Öncelikli Arama (DFS), hedefe ulaşana kadar ilk daldan en derine (backtracking) inerek yolu arar.'}
                        </p>
                        <div className="report-steps">
                          <div className="step">
                            <span className="step-dot origin"></span>
                            <span><strong>{nodeLabels[reportData.path[0]] || reportData.path[0]}</strong> düğümünden arama başlatıldı.</span>
                          </div>
                          
                          {reportData.path.length > 2 && (
                            <div className="step">
                              <span className="step-dot"></span>
                              <span>
                                {reportData.algo === 'BFS'
                                  ? 'Ara katmanlardaki komşular genişleterek taranıyor...'
                                  : 'Hedefe doğru derinlemesine (deep-dive) iniliyor...'}
                              </span>
                            </div>
                          )}

                          {reportData.path.slice(1, -1).map((stepId) => (
                            <div className="step" key={stepId}>
                              <span className="step-dot"></span>
                              <span><strong>{nodeLabels[stepId] || stepId}</strong> düğümüne ulaşıldı.</span>
                            </div>
                          ))}

                          <div className="step">
                            <span className="step-dot target"></span>
                            <span>Hedef düğüm <strong>{nodeLabels[reportData.path[reportData.path.length - 1]] || reportData.path[reportData.path.length - 1]}</strong> bulundu!</span>
                          </div>
                        </div>
                        <p className="report-outro">
                          Yol başarıyla oluşturuldu. Toplam Derinlik: {reportData.path.length - 1} sekme.
                        </p>
                      </div>
                    )}
                  </div>
                )}
              </div>
            )}

            {/* Chain Tab */}
            {activeTab === 'chain' && (
              <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
                <div style={{ padding: '12px', background: 'rgba(255,255,255,0.03)', borderRadius: '12px', border: '1px solid rgba(255,255,255,0.05)' }}>
                  <p style={{ fontSize: '0.75rem', fontWeight: 600, margin: '0 0 8px 0', display: 'flex', alignItems: 'center', gap: '6px' }}>
                    <Network size={14} color="#06b6d4" /> Sequential Pipeline
                  </p>
                  <p style={{ fontSize: '0.65rem', opacity: 0.5, lineHeight: 1.4, margin: '0 0 12px 0' }}>
                    Bu sorgu, seçilen ilişkileri **sırasıyla** takip ederek bir "bağıntı zinciri" oluşturur. Zincir koparsa (eşleşme yoksa) arama durur.
                  </p>

                  {/* Active Steps */}
                  <div style={{ display: 'flex', flexDirection: 'column', gap: '4px' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px', padding: '6px 10px', background: 'rgba(59, 130, 246, 0.1)', borderRadius: '8px', border: '1px dashed rgba(59, 130, 246, 0.3)' }}>
                      <div style={{ width: '12px', height: '12px', borderRadius: '50%', background: '#3b82f6' }} />
                      <span style={{ fontSize: '0.7rem', color: '#3b82f6', fontWeight: 600 }}>Origin: {nodeLabels[startNodeId || ''] || startNodeId || 'Seçilmedi'}</span>
                    </div>

                    {chainRelations.map((r, i) => (
                      <React.Fragment key={i}>
                        <div style={{ height: '10px', width: '1px', background: 'rgba(255,255,255,0.2)', marginLeft: '16px' }} />
                        <div 
                          onClick={() => setChainRelations(prev => prev.filter((_, idx) => idx !== i))}
                          className="badge" 
                          style={{ 
                            padding: '6px 12px', 
                            cursor: 'pointer', 
                            display: 'flex', 
                            alignItems: 'center', 
                            justifyContent: 'space-between',
                            background: 'rgba(6, 182, 212, 0.1)',
                            border: '1px solid rgba(6, 182, 212, 0.2)',
                            color: '#06b6d4'
                          }}
                          title="Tıkla ve Kaldır"
                        >
                          <span style={{ fontSize: '0.7rem', fontWeight: 700 }}>STEP {i+1}: {r}</span>
                          <span style={{ fontSize: '0.6rem', opacity: 0.5 }}>✕</span>
                        </div>
                      </React.Fragment>
                    ))}
                  </div>

                  {/* Available Relations to Add */}
                  <div style={{ marginTop: '16px' }}>
                    <p style={{ fontSize: '0.65rem', opacity: 0.4, marginBottom: '6px' }}>Add next step:</p>
                    <div style={{ display: 'flex', flexWrap: 'wrap', gap: '4px' }}>
                      {['FRIEND', 'ATTENDS', 'UPLOADED', 'LIKES', 'LOCATED_IN'].map(r => (
                        <button
                          key={r}
                          onClick={() => setChainRelations(prev => [...prev, r])}
                          disabled={chainRelations.length >= 5 || chainRelations.includes(r)}
                          style={{
                            padding: '4px 8px',
                            borderRadius: '6px',
                            background: 'rgba(255,255,255,0.05)',
                            border: '1px solid rgba(255,255,255,0.1)',
                            color: chainRelations.includes(r) ? 'rgba(255,255,255,0.2)' : 'white',
                            fontSize: '0.65rem',
                            cursor: chainRelations.includes(r) ? 'not-allowed' : 'pointer'
                          }}
                        >
                          + {r}
                        </button>
                      ))}
                    </div>
                  </div>
                </div>

                <button 
                  onClick={handleChainQuery} 
                  disabled={loading || !startNodeId || chainRelations.length === 0} 
                  className="query-btn primary"
                  style={{ background: 'linear-gradient(135deg, #0891b2, #06b6d4)' }}
                >
                  <Network size={14} /> Run Chain Query
                </button>

                {/* Chain Algorithm Report */}
                {chainReportData && (
                  <div className="algo-report" style={{ marginTop: '12px', borderLeft: '3px solid #06b6d4' }}>
                    <button 
                      className="algo-report-header" 
                      onClick={() => setIsReportOpen(!isReportOpen)}
                    >
                      <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                        <Network size={14} color="#06b6d4" />
                        <span>Chain Pipeline Report</span>
                      </div>
                      <span>{isReportOpen ? '▲' : '▼'}</span>
                    </button>
                    
                    {isReportOpen && (
                      <div className="algo-report-content custom-scrollbar">
                        <p className="report-intro">
                          Bu rapor, seçtiğiniz ilişkilerin (Sequential Follow) katman katman nasıl işlendiğini gösterir.
                        </p>
                        <div className="report-steps">
                          <div className="step">
                            <span className="step-dot origin" style={{ background: '#3b82f6' }}></span>
                            <span><strong>{nodeLabels[startNodeId || ''] || startNodeId}</strong> üzerinden zincir başlatıldı.</span>
                          </div>
                          
                          {chainReportData.steps.map((step, idx) => (
                            <div className="step" key={idx}>
                              <span className="step-dot" style={{ background: '#06b6d4' }}></span>
                              <span>
                                <strong>{step.relation}</strong> ilişkisi takip edildi: 
                                <span style={{ color: step.count > 0 ? '#06b6d4' : '#ef4444', fontWeight: 600, marginLeft: '4px' }}>
                                  {step.count} düğüm bulundu.
                                </span>
                              </span>
                            </div>
                          ))}

                          {/* Skipped Steps */}
                          {chainReportData.steps.length < chainRelations.length && chainRelations.slice(chainReportData.steps.length).map((rel, idx) => (
                            <div className="step" key={`skipped-${idx}`} style={{ opacity: 0.4 }}>
                              <span className="step-dot" style={{ background: '#666' }}></span>
                              <span>
                                <strong>{rel}</strong> adımı <span style={{ fontStyle: 'italic' }}>atlandı</span>. 
                                (Önceki adımda 0 sonuç bulunduğu için zincir koptu.)
                              </span>
                            </div>
                          ))}

                          <div className="step">
                            <span className="step-dot target" style={{ background: '#10b981' }}></span>
                            <span>İşlem tamamlandı. Toplam <strong>{chainReportData.nodes.length}</strong> bağıntılı düğümden oluşan bir ağ keşfedildi.</span>
                          </div>
                        </div>
                        {chainReportData.nodes.length === 0 && (
                          <div style={{ marginTop: '8px', padding: '8px', background: 'rgba(239, 68, 68, 0.1)', borderRadius: '6px', fontSize: '0.65rem', color: '#ef4444' }}>
                            DİKKAT: Zincir herhangi bir adımda koptuğu için sonuç boş döndü. (Sequential Break)
                          </div>
                        )}
                      </div>
                    )}
                  </div>
                )}
              </div>
            )}

            {/* Recommend Tab */}
            {activeTab === 'recommend' && (
              <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                {startNodeId && nodeTypes[startNodeId] && nodeTypes[startNodeId] !== 'User' && (
                  <p style={{ fontSize: '0.75rem', color: '#f59e0b', margin: '0 0 8px 0', lineHeight: 1.4 }}>
                    Arkadaş önerisi sistemi sadece User (Kullanıcı) düğümleri için çalışır. Mevcut origin tipi: {nodeTypes[startNodeId]}
                  </p>
                )}
                <button 
                  onClick={handleRecommendations} 
                  disabled={loading || (!!startNodeId && !!nodeTypes[startNodeId] && nodeTypes[startNodeId] !== 'User')} 
                  className="query-btn primary"
                >
                  <UserPlus size={14} /> Get Friend Suggestions
                </button>
                
                {/* Recs Report Accordion */}
                {recsReportData && (
                  <div className="algo-report">
                    <button 
                      className="algo-report-header" 
                      onClick={() => setIsReportOpen(!isReportOpen)}
                    >
                      <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                        <Zap size={14} color="var(--accent-color)" />
                        <span>Triadic Closure Report</span>
                      </div>
                      <span>{isReportOpen ? '▲' : '▼'}</span>
                    </button>
                    
                    {isReportOpen && (
                      <div className="algo-report-content custom-scrollbar">
                        <p className="report-intro">
                          Triadic Closure algoritması, graf üzerindeki ortak arkadaş bağlarını inceleyerek "<strong>{nodeLabels[startNodeId!] || startNodeId}</strong>" için potansiyel yeni arkadaşlıklar tespit etti.
                        </p>
                        <div className="report-steps">
                          {recsReportData.length === 0 && (
                            <p style={{ margin: 0, opacity: 0.7 }}>Önerilecek ortak bağlantı bulunamadı.</p>
                          )}
                          {recsReportData.map((rec) => (
                            <div className="step" key={rec.node.id}>
                              <span className="step-dot origin" style={{ background: '#10b981', boxShadow: '0 0 5px #10b981' }}></span>
                              <span>
                                <strong>{nodeLabels[rec.node.id] || rec.node.id}</strong> 
                                <span style={{ opacity: 0.6 }}> ({rec.mutualFriendsCount} ortak arkadaş)</span>
                              </span>
                            </div>
                          ))}
                        </div>
                      </div>
                    )}
                  </div>
                )}
              </div>
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
        .algo-report {
          margin-top: 12px;
          border-radius: 8px;
          background: rgba(0, 0, 0, 0.2);
          border: 1px solid rgba(255, 255, 255, 0.05);
          overflow: hidden;
        }
        .algo-report-header {
          width: 100%;
          padding: 10px 12px;
          background: rgba(255, 255, 255, 0.03);
          border: none;
          border-bottom: 1px solid rgba(255, 255, 255, 0.05);
          color: white;
          font-size: 0.8rem;
          display: flex;
          align-items: center;
          justify-content: space-between;
          cursor: pointer;
          transition: background 0.2s;
        }
        .algo-report-header:hover {
          background: rgba(255, 255, 255, 0.06);
        }
        .algo-report-content {
          padding: 12px;
          font-size: 0.75rem;
          color: rgba(255, 255, 255, 0.7);
          max-height: 250px;
          overflow-y: auto;
        }
        .report-intro {
          margin: 0 0 12px 0;
          font-style: italic;
          color: rgba(255, 255, 255, 0.5);
          line-height: 1.4;
        }
        .report-outro {
          margin: 12px 0 0 0;
          padding-top: 12px;
          border-top: 1px dashed rgba(255, 255, 255, 0.1);
          color: var(--accent-color);
          font-weight: 600;
        }
        .report-steps {
          display: flex;
          flex-direction: column;
          gap: 8px;
        }
        .step {
          display: flex;
          align-items: flex-start;
          gap: 8px;
          line-height: 1.4;
        }
        .step strong {
          color: white;
        }
        .step-dot {
          width: 6px;
          height: 6px;
          border-radius: 50%;
          background: rgba(255, 255, 255, 0.3);
          margin-top: 5px;
          flex-shrink: 0;
        }
        .step-dot.origin { background: #3b82f6; box-shadow: 0 0 5px #3b82f6; }
        .step-dot.target { background: #ef4444; box-shadow: 0 0 5px #ef4444; }
      `}</style>
    </div>
  );
};

export default QueryPanel;
