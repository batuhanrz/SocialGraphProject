import React, { useEffect, useRef, useCallback, useState } from 'react';
import { Network, type Node as VisNode, type Edge as VisEdge, type Options } from 'vis-network';
import { DataSet } from 'vis-data';
import { nodeService } from '../services/nodeService';

interface GraphCanvasProps {
  selectedNodeId: string | null;
  targetNodeId?: string | null;
  onNodeSelect: (id: string) => void;
  onNodeRightClick?: (id: string) => void;
  highlightNodeIds?: string[];
  highlightEdgeIds?: string[];
  highlightMode?: 'path' | 'recs' | 'chain';
}

const GraphCanvas: React.FC<GraphCanvasProps> = ({
  selectedNodeId,
  targetNodeId,
  onNodeSelect,
  onNodeRightClick,
  highlightNodeIds = [],
  highlightEdgeIds = [],
  highlightMode = 'path'
}) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const networkRef = useRef<Network | null>(null);
  const nodesDataSetRef = useRef<DataSet<VisNode>>(new DataSet([]));
  const edgesDataSetRef = useRef<DataSet<VisEdge>>(new DataSet([]));
  const isFirstFitDoneRef = useRef(false);
  const physicsSlowedRef = useRef(false);

  // dataVersion: initData her calistiginda artar -> highlighting effect'i tetikler
  const [dataVersion, setDataVersion] = useState(0);

  // selectedNodeId'yi ref'te tut - closure sorunu
  const selectedNodeIdRef = useRef<string | null>(null);
  useEffect(() => { selectedNodeIdRef.current = selectedNodeId; }, [selectedNodeId]);

  // highlightNodeIds'yi ref'te tut - afterDrawing animasyonu icin
  const highlightNodeIdsRef = useRef<string[]>([]);
  useEffect(() => { highlightNodeIdsRef.current = highlightNodeIds; }, [highlightNodeIds]);

  const highlightModeRef = useRef<'path' | 'recs' | 'chain'>('path');
  useEffect(() => { highlightModeRef.current = highlightMode; }, [highlightMode]);

  // Kullanicinin kendi tikladigi/surukledigi dugumlere tekrar focus atmayi onlemek icin
  const lastInteractedNodeIdRef = useRef<string | null>(null);
  const isDraggingRef = useRef(false);

  // --- Global Shift: Pin/Unpin Toggle ---
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Shift' && selectedNodeIdRef.current) {
        const nodeId = selectedNodeIdRef.current;
        const node = nodesDataSetRef.current.get(nodeId) as any;
        if (!node) return;

        const wasPinned = node?.fixed === true || node?.fixed?.x === true;
        const nowPinned = !wasPinned;

        // O anki koordinatlari al ki "zink" diye orada kalsin
        const positions = networkRef.current?.getPositions([nodeId]);
        const pos = positions ? positions[nodeId] : null;

        nodesDataSetRef.current.update({
          id: nodeId,
          x: pos?.x,
          y: pos?.y,
          fixed: nowPinned ? { x: true, y: true } : false,
          borderWidth: 5,
          color: { ...(node.color || {}), border: nowPinned ? '#a855f7' : '#3b82f6' },
          title: nowPinned ? 'PIN: Sabitlendi (Shift ile serbest birakin)' : undefined
        });

        // Eger o an surukleniyorsa, suruklemeyi "kirmak" icin etkilesimi anlik kapat-ac
        if (isDraggingRef.current && nowPinned) {
          networkRef.current?.setOptions({ interaction: { dragNodes: false } });
          setTimeout(() => {
            networkRef.current?.setOptions({ interaction: { dragNodes: true } });
          }, 50);
        }
      }
    };
    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, []);

  // --- Veri Yukleme ---
  const initData = useCallback(async () => {
    try {
      const [allNodes, allEdges] = await Promise.all([
        nodeService.getAllNodes(),
        nodeService.getAllEdges()
      ]);

      // 1. Silinen Dugumleri Temizle
      const currentNodeIds = new Set(allNodes.map(n => n.id));
      const nodesToRemove = nodesDataSetRef.current.getIds().filter(id => !currentNodeIds.has(id as string));
      if (nodesToRemove.length > 0) nodesDataSetRef.current.remove(nodesToRemove);

      const formattedNodes: VisNode[] = allNodes.map(node => {
        const existing = nodesDataSetRef.current.get(node.id) as any;

        return {
          id: node.id,
          label: (node.properties.Name as string) || (node.properties.Title as string) || node.id,
          shape: getNodeShape(node.type),
          color: {
            background: getNodeColor(node.type),
            border: existing?.color?.border || '#ffffff', // MEVCUT cerceveyi koru
            highlight: { background: '#ffffff', border: existing?.color?.border || getNodeColor(node.type) }
          },
          font: { color: '#ffffff', size: existing?.font?.size || 14, face: 'Inter' },
          borderWidth: existing?.borderWidth || 2, // MEVCUT kalinligi koru
          shadow: true,
          title: existing?.title,
          fixed: existing?.fixed
        };
      });

      nodesDataSetRef.current.update(formattedNodes);

      // 2. Silinen Kenarlari Temizle
      const currentEdgeIds = new Set(allEdges.map(e => e.id));
      const edgesToRemove = edgesDataSetRef.current.getIds().filter(id => !currentEdgeIds.has(id as string));
      if (edgesToRemove.length > 0) edgesDataSetRef.current.remove(edgesToRemove);

      const processedEdges = new Set<string>();
      const edgesList: VisEdge[] = allEdges
        .filter(edge => {
          const edgeKey = [edge.sourceId, edge.targetId].sort().join('-');
          if (edge.relationType === 'FRIEND' && processedEdges.has(edgeKey)) return false;
          processedEdges.add(edgeKey);
          return true;
        })
        .map(edge => {
          const existingEdge = edgesDataSetRef.current.get(edge.id) as any;
          return {
            id: edge.id,
            from: edge.sourceId,
            to: edge.targetId,
            relationType: edge.relationType,
            arrows: edge.isDirected ? { to: { enabled: true, scaleFactor: 0.5 } } : undefined,
            dashes: getEdgeDashes(edge.relationType),
            color: existingEdge?.color || { color: 'rgba(255,255,255,0.15)', highlight: '#3b82f6' },
            width: existingEdge?.width || 1,
            shadow: existingEdge?.shadow || false,
            smooth: { enabled: true, type: 'continuous', roundness: 0.5 }
          };
        });

      edgesDataSetRef.current.update(edgesList);

      // Highlighting effect'ini tetikle
      setDataVersion(v => v + 1);

    } catch (err) {
      console.error('Failed to load graph data:', err);
    }
  }, []);

  // --- Network Olusturma (Tek Sefer) ---
  useEffect(() => {
    if (!containerRef.current) return;

    const options: Options = {
      nodes: { scaling: { min: 10, max: 30 } },
      edges: {
        smooth: { enabled: true, type: 'continuous', roundness: 0.5 },
        color: { color: 'rgba(255,255,255,0.15)', highlight: '#ffffff' },
        width: 1.5,
        hoverWidth: 3
      },
      physics: {
        forceAtlas2Based: {
          gravitationalConstant: -150,
          centralGravity: 0.005,
          springLength: 150,
          springConstant: 0.04
        },
        maxVelocity: 20,
        solver: 'forceAtlas2Based',
        timestep: 0.25,
        stabilization: false // Hemen basla, bekleme yok
      },
      interaction: {
        hover: true,
        tooltipDelay: 200,
        hideEdgesOnDrag: false,
        multiselect: false, // Shift+Drag (box selection) iptal ederek pinleme cakismasini giderir
        selectable: true,
        dragNodes: true
      }
    };

    networkRef.current = new Network(
      containerRef.current,
      { nodes: nodesDataSetRef.current, edges: edgesDataSetRef.current },
      options
    );
    
    // Senkronizasyon sorunlarini (lag) onlemek icin ref'i aninda guncelleyen yardimci
    const selectNodeImmediate = (id: string) => {
      selectedNodeIdRef.current = id;
      onNodeSelect(id);
    };

    // Sol tik: Current Node sec
    networkRef.current.on('click', (params) => {
      if (params.nodes.length > 0) {
        const id = params.nodes[0];
        lastInteractedNodeIdRef.current = id;
        selectNodeImmediate(id);
      }
    });

    // Surukleme baslangici (Drag): Suruklenen dugumu otomatik olarak sec
    networkRef.current.on('dragStart', (params) => {
      isDraggingRef.current = true;
      if (params.nodes.length > 0) {
        const id = params.nodes[0];
        lastInteractedNodeIdRef.current = id;
        selectNodeImmediate(id);
      }
    });

    networkRef.current.on('dragEnd', () => {
      isDraggingRef.current = false;
    });

    // Sag tik: Target Node sec
    networkRef.current.on('oncontext', (params) => {
      params.event.preventDefault();
      const nodeId = networkRef.current?.getNodeAt(params.pointer.DOM);
      if (nodeId && onNodeRightClick) {
        onNodeRightClick(nodeId as string);
      }
    });

    // Hover Edge: Sadece hover yapilan cizginin label'ini goster
    networkRef.current.on('hoverEdge', (params) => {
      const edgeId = params.edge;
      const edge = edgesDataSetRef.current.get(edgeId) as any;
      if (edge && edge.relationType) {
        edgesDataSetRef.current.update({
          id: edgeId,
          label: edge.relationType,
          font: {
            size: 10,
            align: 'top',
            color: 'rgba(255,255,255,0.8)',
            background: 'rgba(10,10,10,0.8)',
            strokeWidth: 0, // Bolding/stroke efektini engeller
            face: 'Inter'
          }
        });
      }
    });

    networkRef.current.on('blurEdge', (params) => {
      const edgeId = params.edge;
      edgesDataSetRef.current.update({
        id: edgeId,
        label: " " // undefined yerine bosluk karakteri kullaniyoruz
      });
    });

    // --- Animasyonlu Yol (Akis Efekti) ---
    networkRef.current.on('afterDrawing', (ctx) => {
      // Arkadas onerisi (recs) veya Zincir (chain) modunda kan akisi (blood flow) animasyonunu iptal et
      if (highlightModeRef.current === 'recs' || highlightModeRef.current === 'chain') return;

      const pathIds = highlightNodeIdsRef.current;
      if (!pathIds || pathIds.length < 2) return;

      const time = Date.now() / 1000; // Saniye cinsinden zaman

      for (let i = 0; i < pathIds.length - 1; i++) {
        const fromId = pathIds[i];
        const toId = pathIds[i + 1];

        const positions = networkRef.current?.getPositions([fromId, toId]);
        if (!positions) continue;

        const fromPos = positions[fromId];
        const toPos = positions[toId];
        if (!fromPos || !toPos) continue;

        // Iki dugum arasinda hareket eden 3 parcacik (isik huzmesi)
        for (let j = 0; j < 3; j++) {
          // Parcaciklar arasinda mesafe birak
          let progress = (time * 1.5 - j * 0.15) % 1;
          if (progress < 0) progress += 1;

          const x = fromPos.x + (toPos.x - fromPos.x) * progress;
          const y = fromPos.y + (toPos.y - fromPos.y) * progress;

          // Parcacik cizimi
          ctx.beginPath();
          ctx.arc(x, y, 4, 0, 2 * Math.PI, false);

          // Bastaki parcacik daha parlak, arkadakiler sonuk
          const opacity = 1 - (j * 0.3);
          ctx.fillStyle = `rgba(16, 185, 129, ${opacity})`;
          ctx.shadowColor = '#34d399';
          ctx.shadowBlur = 10;
          ctx.fill();
          ctx.closePath();
          ctx.shadowBlur = 0; // diger cizimleri etkilemesin
        }
      }
    });

    // --- Akis Nefesi: durdugu an beklemeden yeniden baslat ---
    networkRef.current.on('stabilized', () => {
      if (!networkRef.current || !physicsSlowedRef.current) return;
      networkRef.current.startSimulation();
    });

    // --- Floating Keeper + Otomatik Fit ---
    const floatingKeeper = setInterval(() => {
      if (!networkRef.current || isDraggingRef.current) return;
      
      // Ilk fit: veri yuklendikten hemen sonra
      if (!isFirstFitDoneRef.current && nodesDataSetRef.current.length > 0) {
        networkRef.current.fit({
          animation: { duration: 400, easingFunction: 'easeInOutQuad' }
        });
        isFirstFitDoneRef.current = true;
      }

      // Yavas suzulme moduna gec (1 kere)
      if (!physicsSlowedRef.current && isFirstFitDoneRef.current) {
        networkRef.current.setOptions({
          physics: { 
            maxVelocity: 2, 
            timestep: 0.2,
            minVelocity: 0.001 // Neredeyse hic durma seviyesi
          }
        });
        physicsSlowedRef.current = true;
      }
    }, 1000);

    // --- Akis Canliligi (Render Loop) ---
    // Vis.js stabilized oldugunda (dugumler durdugunda) render durur, bu da akis animasyonunun donmasina neden olur.
    // Eger bir path veya vurgu varsa, render'i manuel tetikleyerek akisi surekli kiliyoruz.
    let animationFrameId: number;
    const renderLoop = () => {
      if (networkRef.current && highlightNodeIdsRef.current.length > 0) {
        networkRef.current.redraw();
      }
      animationFrameId = requestAnimationFrame(renderLoop);
    };
    animationFrameId = requestAnimationFrame(renderLoop);

    initData();

    return () => {
      cancelAnimationFrame(animationFrameId);
      clearInterval(floatingKeeper);
      networkRef.current?.destroy();
      networkRef.current = null;
    };
  }, []);

  // --- Periyodik Veri Guncelleme ---
  useEffect(() => {
    const interval = setInterval(initData, 15000);
    return () => clearInterval(interval);
  }, [initData]);

  // --- Secim Guncellemesi ---
  useEffect(() => {
    if (networkRef.current && selectedNodeId) {
      networkRef.current.selectNodes([selectedNodeId]);
      
      // Sadece disaridan (orn: arama kutusundan) secildiyse kamera odaklansin
      if (!isDraggingRef.current && lastInteractedNodeIdRef.current !== selectedNodeId) {
        networkRef.current.focus(selectedNodeId, {
          animation: { duration: 1000, easingFunction: 'easeInOutQuad' }
        });
      }
    }
  }, [selectedNodeId]);

  // --- Dugum Renklendirme (HER ZAMAN dogru renkleri uygula) ---
  useEffect(() => {
    if (!networkRef.current) return;

    const allNodes = nodesDataSetRef.current.get();
    const updatedNodes = allNodes.map(node => {
      const id = node.id as string;
      const isOrigin = id === selectedNodeId;
      const isTarget = id === targetNodeId;
      const isPinned = node.fixed === true || (node.fixed as any)?.x === true;
      const isPath = highlightNodeIds.includes(id);

      let borderColor = '#ffffff';
      let bw = 2;
      let fontSize = 14;
      let shadow: any = { enabled: false };

      if (isOrigin && isPinned) {
        borderColor = '#6366f1'; bw = 6; fontSize = 16;
        if (highlightMode) shadow = { enabled: true, color: '#3b82f6', size: 25 };
      } else if (isTarget && isPinned) {
        borderColor = '#c026d3'; bw = 6; fontSize = 16;
        if (highlightMode) shadow = { enabled: true, color: '#ef4444', size: 25 };
      } else if (isOrigin) {
        borderColor = '#3b82f6'; bw = 5; fontSize = 16;
        if (highlightMode) shadow = { enabled: true, color: '#3b82f6', size: 30 };
      } else if (isTarget) {
        borderColor = '#ef4444'; bw = 5; fontSize = 16;
        if (highlightMode) shadow = { enabled: true, color: '#ef4444', size: 30 };
      } else if (isPinned) {
        borderColor = '#a855f7'; bw = 5;
      } else if (isPath) {
        if (highlightMode === 'recs') {
          // Onerilen dugumler guclu bir sekilde parlasin (Yesil)
          borderColor = '#10b981'; bw = 5; fontSize = 16;
          shadow = { enabled: true, color: '#10b981', size: 25 };
        } else if (highlightMode === 'chain') {
          // Zincir/Baginti sorgusu: Elektrik mavisi (Siyan) parlasin - Daha opak ve net
          borderColor = '#06b6d4'; bw = 5; fontSize = 16;
          shadow = { enabled: true, color: '#06b6d4', size: 30 };
        } else {
          borderColor = '#10b981'; bw = 4; fontSize = 15;
        }
      }

      return {
        ...node,
        borderWidth: bw,
        shadow: shadow,
        color: { ...(node.color as any), border: borderColor },
        font: { ...(node.font as any), size: fontSize }
      };
    });

    nodesDataSetRef.current.update(updatedNodes);
  }, [selectedNodeId, targetNodeId, highlightNodeIds, highlightMode, dataVersion]);

  // --- Kenar Glow Efekti ---
  useEffect(() => {
    if (!networkRef.current) return;

    const allEdges = edgesDataSetRef.current.get();
    const updatedEdges = allEdges.map(edge => {
      const fromIdx = highlightNodeIds.indexOf(edge.from as string);
      const toIdx = highlightNodeIds.indexOf(edge.to as string);
      const isPathEdge = highlightMode !== 'recs' && highlightMode !== 'chain'
        && highlightNodeIds.length >= 2
        && fromIdx !== -1 && toIdx !== -1
        && Math.abs(fromIdx - toIdx) === 1;

      const isChainEdge = highlightMode === 'chain'
        && highlightNodeIds.includes(edge.from as string)
        && highlightNodeIds.includes(edge.to as string);

      if (isPathEdge) {
        return {
          ...edge,
          color: { color: 'rgba(16, 185, 129, 0.25)', highlight: 'rgba(16, 185, 129, 0.5)' },
          width: 4,
          smooth: false, // Animasyonun duz cizgi uzerinde dogru calismasi icin
          shadow: false
        };
      } else if (isChainEdge) {
        return {
          ...edge,
          color: { color: 'rgba(6, 182, 212, 0.25)', highlight: 'rgba(6, 182, 212, 0.5)' },
          width: 3,
          shadow: { enabled: true, color: 'rgba(6, 182, 212, 0.4)', size: 10 }
        };
      }

      return {
        ...edge,
        color: { color: 'rgba(255,255,255,0.15)', highlight: '#3b82f6' },
        width: 1,
        shadow: false
      };
    });

    edgesDataSetRef.current.update(updatedEdges);
  }, [highlightNodeIds, highlightMode, dataVersion]);

  // --- Edge Highlighting (Direct Selection) ---
  useEffect(() => {
    if (!networkRef.current) return;

    const allEdges = edgesDataSetRef.current.get();
    const updatedEdges = allEdges.map(edge => {
      const isHighlighted = highlightEdgeIds.includes(edge.id as string);

      if (isHighlighted) {
        return {
          ...edge,
          color: { color: '#f59e0b', highlight: '#f59e0b' },
          width: 5,
          shadow: { enabled: true, color: '#f59e0b', size: 15 }
        };
      }

      // Default state reset (already handled in the effect above, but we ensure it matches)
      return edge;
    });

    edgesDataSetRef.current.update(updatedEdges);
  }, [highlightEdgeIds]);

  return (
    <div
      ref={containerRef}
      style={{
        flex: 1,
        position: 'relative',
        background: 'radial-gradient(circle at center, #111 0%, #050505 100%)',
        overflow: 'hidden'
      }}
    />
  );
};

// Helpers
const getNodeShape = (type: string) => {
  switch (type.toLowerCase()) {
    case 'user': return 'dot';
    case 'photo': return 'square';
    case 'event': return 'triangle';
    default: return 'ellipse';
  }
};

const getNodeColor = (type: string) => {
  switch (type.toLowerCase()) {
    case 'user': return '#3b82f6';
    case 'photo': return '#10b981';
    case 'event': return '#f59e0b';
    default: return '#6b7280';
  }
};

const getEdgeDashes = (relationType: string) => {
  switch (relationType.toUpperCase()) {
    case 'ATTENDS': return [2, 10];
    case 'LIKES': return [5, 5];
    default: return false;
  }
};

export default GraphCanvas;
