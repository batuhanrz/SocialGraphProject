import React, { useState, useEffect } from 'react';
import { ChevronDown, ChevronUp, Cpu, User, Share2, Activity, Zap, Box, Link2, UserMinus, HeartOff, PlusCircle } from 'lucide-react';
import { nodeService } from '../services/nodeService';
import type { INode, IEdge } from '../types/graph';

interface ISimulationAction {
  id: string;
  type: number; // 0: NodeAdded, 1: EdgeAdded, 2: Unfriend, 3: Unlike
  sourceId: string;
  sourceName: string;
  targetId: string;
  targetName: string;
  description: string;
  timestamp: string;
}

interface SimNodeListProps {
  onNodeSelect: (id: string) => void;
  onEdgeSelect?: (id: string) => void;
}

export const SimNodeList: React.FC<SimNodeListProps> = ({ onNodeSelect, onEdgeSelect }) => {
  const [expanded, setExpanded] = useState(true);
  const [simNodes, setSimNodes] = useState<INode[]>([]);
  const [simEdgeCount, setSimEdgeCount] = useState(0);
  const [totalNodes, setTotalNodes] = useState(0);
  const [totalEdges, setTotalEdges] = useState(0);
  const [loading, setLoading] = useState(false);
  const [hoveredNodeId, setHoveredNodeId] = useState<string | null>(null);
  const [nodeRelations, setNodeRelations] = useState<Record<string, IEdge[]>>({});
  const [nodeLabels, setNodeLabels] = useState<Record<string, string>>({});
  const [simActions, setSimActions] = useState<ISimulationAction[]>([]);
  const [allRawEdges, setAllRawEdges] = useState<IEdge[]>([]);

  useEffect(() => {
    loadSimData();
    const interval = setInterval(loadSimData, 5000);
    return () => clearInterval(interval);
  }, []);

  const loadSimData = async () => {
    try {
      const [allNodes, allEdges, actions] = await Promise.all([
        nodeService.getAllNodes(),
        nodeService.getAllEdges(),
        nodeService.getSimulationActions()
      ]);
      
      setAllRawEdges(allEdges);

      const labels: Record<string, string> = {};
      allNodes.forEach(n => {
        labels[n.id] = String(n.properties?.Name || n.properties?.Title || n.id).replace(' (Sim)', '');
      });
      setNodeLabels(labels);

      // Filter nodes (Simulated ones)
      const filteredNodes = allNodes.filter(n => 
        n.id.includes('_new_') || 
        n.id.includes('(Sim)') ||
        Object.values(n.properties || {}).some(v => typeof v === 'string' && v.includes('(Sim)'))
      );

      // Filter edges (isSimulated: true)
      const filteredEdges = allEdges.filter(e => e.properties?.isSimulated === true);

      // Map relations
      const relMap: Record<string, IEdge[]> = {};
      filteredEdges.forEach(e => {
        if (!relMap[e.sourceId]) relMap[e.sourceId] = [];
        if (!relMap[e.targetId]) relMap[e.targetId] = [];
        if (!relMap[e.sourceId].some(existing => existing.id === e.id)) relMap[e.sourceId].push(e);
        if (!relMap[e.targetId].some(existing => existing.id === e.id)) relMap[e.targetId].push(e);
      });

      setSimNodes(filteredNodes);
      setSimEdgeCount(filteredEdges.length);
      setTotalNodes(allNodes.length);
      setTotalEdges(allEdges.length);
      setNodeRelations(relMap);
      setSimActions(actions);
    } catch (err) {
      console.error("Failed to load sim data", err);
    } finally {
      setLoading(false);
    }
  };

  const getRelationText = (nodeId: string, edge: IEdge) => {
    const isSource = edge.sourceId === nodeId;
    const otherId = isSource ? edge.targetId : edge.sourceId;
    const otherName = nodeLabels[otherId] || otherId;
    
    if (edge.relationType === 'FRIEND') return `Arkadaslik: ${otherName}`;
    if (edge.relationType === 'LIKES') return isSource ? `Begendi: ${otherName}` : `Begenildi: ${otherName}`;
    if (edge.relationType === 'ATTENDS') return isSource ? `Katildi: ${otherName}` : `Katilimci: ${otherName}`;
    if (edge.relationType === 'POSTED') return isSource ? `Paylasti: ${otherName}` : `Paylasildi: ${otherName}`;
    return `${edge.relationType}: ${otherName}`;
  };

  const getActionIcon = (type: number) => {
    switch(type) {
      case 0: return <PlusCircle size={12} color="#3b82f6" />;
      case 1: return <Share2 size={12} color="#10b981" />;
      case 2: return <UserMinus size={12} color="#ef4444" />;
      case 3: return <HeartOff size={12} color="#ef4444" />;
      default: return <Activity size={12} color="#94a3b8" />;
    }
  };

  const getActionDescription = (action: ISimulationAction) => {
    // If it's a node addition, we already have the formatted text from worker
    if (action.type === 0) return action.description;

    const sName = nodeLabels[action.sourceId] || action.sourceId;
    const tName = nodeLabels[action.targetId] || action.targetId;

    if (action.type === 1) {
      // EdgeAdded
      const relType = action.description.match(/\((.*?)\)/)?.[1] || "Baglanti";
      return `${sName} -> ${tName} (${relType})`;
    }
    if (action.type === 2) return `${sName} & ${tName}: Arkadaslik sona erdi`;
    if (action.type === 3) return `${sName} -> ${tName}: Begeni geri cekildi`;

    return action.description;
  };

  return (
    <div style={{
      position: 'absolute',
      top: '24px',
      right: '24px',
      zIndex: 1000,
      width: '260px',
      maxHeight: '480px',
      display: 'flex',
      flexDirection: 'column',
      pointerEvents: 'auto'
    }}>
      {/* Hover Report Popover */}
      {hoveredNodeId && nodeRelations[hoveredNodeId] && (
        <div style={{
          position: 'absolute',
          right: '270px',
          top: '0',
          width: '220px',
          background: 'rgba(15, 15, 15, 0.95)',
          backdropFilter: 'blur(30px)',
          border: '1px solid rgba(59, 130, 246, 0.4)',
          borderRadius: '12px',
          padding: '12px',
          boxShadow: '0 10px 40px rgba(0,0,0,0.6)',
          animation: 'fadeIn 0.2s ease-out'
        }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '8px', borderBottom: '1px solid rgba(255,255,255,0.1)', paddingBottom: '6px' }}>
            <Zap size={14} color="#f59e0b" />
            <span style={{ fontSize: '0.75rem', fontWeight: 700, color: 'white' }}>Simulasyon Raporu</span>
          </div>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '6px' }}>
            {nodeRelations[hoveredNodeId].slice(0, 6).map((edge, idx) => (
               <div key={idx} style={{ display: 'flex', alignItems: 'center', gap: '8px', fontSize: '0.65rem', color: 'rgba(255,255,255,0.8)' }}>
                  <Link2 size={10} color="#10b981" />
                  <span style={{ whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
                    {getRelationText(hoveredNodeId, edge)}
                  </span>
               </div>
            ))}
          </div>
        </div>
      )}

      <button onClick={() => setExpanded(!expanded)} className="glass" style={{
          display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '12px 16px', borderRadius: expanded ? '12px 12px 0 0' : '12px',
          border: '1px solid rgba(255,255,255,0.1)', cursor: 'pointer', width: '100%', color: 'white', background: 'rgba(20, 20, 20, 0.6)', backdropFilter: 'blur(20px)', transition: 'all 0.3s ease'
        }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
          <Cpu size={16} color="#60a5fa" />
          <span style={{ fontSize: '0.8rem', fontWeight: 600 }}>Simulation Engine</span>
        </div>
        {expanded ? <ChevronUp size={16} /> : <ChevronDown size={16} />}
      </button>

      {expanded && (
        <div className="glass custom-scrollbar" style={{
          background: 'rgba(10, 10, 10, 0.8)', backdropFilter: 'blur(20px)', border: '1px solid rgba(255,255,255,0.1)', borderTop: 'none', borderRadius: '0 0 12px 12px', padding: '0', overflowY: 'auto', maxHeight: '380px', flex: 1
        }}>
          {/* Stats Header */}
          <div style={{ padding: '10px 12px', background: 'rgba(255,255,255,0.02)', borderBottom: '1px solid rgba(255,255,255,0.05)', display: 'flex', flexDirection: 'column', gap: '6px' }}>
             <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <span style={{ fontSize: '0.6rem', color: 'rgba(255,255,255,0.4)', textTransform: 'uppercase' }}>System Pulse</span>
                <Activity size={10} color="#f59e0b" style={{ opacity: 0.5 }} />
             </div>
             <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                <div style={{ display: 'flex', gap: '10px' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '3px' }} title="Total Nodes">
                        <Box size={10} color="#94a3b8" />
                        <span style={{ fontSize: '0.7rem', fontWeight: 600 }}>{totalNodes}</span>
                    </div>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '3px' }} title="Total Relations">
                        <Share2 size={10} color="#94a3b8" />
                        <span style={{ fontSize: '0.7rem', fontWeight: 600 }}>{totalEdges}</span>
                    </div>
                </div>
                <div style={{ display: 'flex', gap: '10px', borderLeft: '1px solid rgba(255,255,255,0.1)', paddingLeft: '10px' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '3px' }} title="New Nodes">
                        <User size={10} color="#3b82f6" />
                        <span style={{ fontSize: '0.7rem', fontWeight: 600, color: '#3b82f6' }}>+{simNodes.length}</span>
                    </div>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '3px' }} title="New Relations">
                        <Share2 size={10} color="#10b981" />
                        <span style={{ fontSize: '0.7rem', fontWeight: 600, color: '#10b981' }}>+{simEdgeCount}</span>
                    </div>
                </div>
             </div>
          </div>

          <div style={{ padding: '8px 12px', borderBottom: '1px solid rgba(255,255,255,0.05)', background: 'rgba(59, 130, 246, 0.05)' }}>
            <span style={{ fontSize: '0.65rem', fontWeight: 700, color: 'rgba(255,255,255,0.6)' }}>LIVE ACTION FEED</span>
          </div>

          {loading && simActions.length === 0 ? (
            <div style={{ padding: '24px', textAlign: 'center', color: 'rgba(255,255,255,0.4)', fontSize: '0.75rem' }}>Synchronizing pulse...</div>
          ) : simActions.length === 0 ? (
            <div style={{ padding: '24px', textAlign: 'center', color: 'rgba(255,255,255,0.4)', fontSize: '0.75rem' }}>Waiting for system events...</div>
          ) : (
            <div style={{ display: 'flex', flexDirection: 'column', padding: '4px' }}>
              {simActions.map(action => (
                <div key={action.id} 
                     onMouseEnter={() => setHoveredNodeId(action.sourceId)}
                     onMouseLeave={() => setHoveredNodeId(null)}
                       onClick={() => {
                        // Grafı anında güncellemeye zorla (Yeni düğümlerin hemen yüklenmesi için)
                        window.dispatchEvent(new CustomEvent('refresh-graph'));
                        
                        // Her durumda source node'u sec
                        onNodeSelect(action.sourceId);
                        
                        if (action.type === 1 || action.type === 2 || action.type === 3) {
                          const matchedEdge = allRawEdges.find(e => 
                            (e.sourceId === action.sourceId && e.targetId === action.targetId) ||
                            (e.sourceId === action.targetId && e.targetId === action.sourceId)
                          );
                          
                          if (matchedEdge && onEdgeSelect) {
                            onEdgeSelect(matchedEdge.id);
                          }
                        }
                      }}
                     style={{
                        padding: '10px 12px', borderRadius: '8px', marginBottom: '2px', display: 'flex', alignItems: 'flex-start', gap: '10px', transition: 'all 0.2s',
                        background: hoveredNodeId === action.sourceId ? 'rgba(255,255,255,0.05)' : 'transparent',
                        borderLeft: action.type >= 2 ? '2px solid #ef4444' : '2px solid transparent',
                        cursor: 'pointer'
                     }}>
                  <div style={{ marginTop: '2px' }}>{getActionIcon(action.type)}</div>
                  <div style={{ display: 'flex', flexDirection: 'column', gap: '2px', flex: 1 }}>
                    <span style={{ fontSize: '0.7rem', color: 'rgba(255,255,255,0.9)', lineHeight: '1.2' }}>
                        {getActionDescription(action)}
                    </span>
                    <span style={{ fontSize: '0.55rem', color: 'rgba(255,255,255,0.3)' }}>{new Date(action.timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit' })}</span>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}
      <style>{`
        @keyframes fadeIn { from { opacity: 0; transform: translateX(10px); } to { opacity: 1; transform: translateX(0); } }
      `}</style>
    </div>
  );
};
