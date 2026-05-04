import React, { useEffect, useRef, useCallback, useState } from 'react';
import { Network, type Node as VisNode, type Edge as VisEdge, type Options } from 'vis-network';
import { DataSet } from 'vis-data';
import { nodeService } from '../services/nodeService';
import type { IEdge } from '../types/graph';

interface GraphCanvasProps {
  selectedNodeId: string | null;
  targetNodeId?: string | null;
  onNodeSelect: (id: string) => void;
  onNodeRightClick?: (id: string) => void;
  highlightNodeIds?: string[];
  highlightMode?: 'path' | 'recs' | 'chain';
}

const GraphCanvas: React.FC<GraphCanvasProps> = ({
  selectedNodeId,
  targetNodeId,
  onNodeSelect,
  onNodeRightClick,
  highlightNodeIds = [],
  highlightMode = 'path'
}) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const networkRef = useRef<Network | null>(null);
  const nodesDataSetRef = useRef<DataSet<VisNode>>(new DataSet([]));
  const edgesDataSetRef = useRef<DataSet<VisEdge>>(new DataSet([]));
  const isFirstFitDoneRef = useRef(false);
  const physicsSlowedRef = useRef(false);

  // dataVersion: initData her çalıştığında artar → highlighting effect'i tetikler
  const [dataVersion, setDataVersion] = useState(0);

  // selectedNodeId'yi ref'te tut — closure sorunu
  const selectedNodeIdRef = useRef<string | null>(null);
  useEffect(() => { selectedNodeIdRef.current = selectedNodeId; }, [selectedNodeId]);

  // highlightNodeIds'yi ref'te tut — afterDrawing animasyonu için
  const highlightNodeIdsRef = useRef<string[]>([]);
  useEffect(() => { highlightNodeIdsRef.current = highlightNodeIds; }, [highlightNodeIds]);

  const highlightModeRef = useRef<'path' | 'recs' | 'chain'>('path');
  useEffect(() => { highlightModeRef.current = highlightMode; }, [highlightMode]);

  // Kullanıcının kendi tıkladığı/sürüklediği düğümlere tekrar focus atmayı önlemek için
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

        // O anki koordinatları al ki "zınk" diye orada kalsın
        const positions = networkRef.current?.getPositions([nodeId]);
        const pos = positions ? positions[nodeId] : null;

        nodesDataSetRef.current.update({
          id: nodeId,
          x: pos?.x,
          y: pos?.y,
          fixed: nowPinned ? { x: true, y: true } : false,
          borderWidth: 5,
          color: { ...(node.color || {}), border: nowPinned ? '#a855f7' : '#3b82f6' },
          title: nowPinned ? '📌 Sabitlendi (Shift ile serbest bırakın)' : undefined
        });

        // Eğer o an sürükleniyorsa, sürüklemeyi "kırmak" için etkileşimi anlık kapat-aç
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

  // --- Veri Yükleme ---
  const initData = useCallback(async () => {
    try {
      const allNodes = await nodeService.getAllNodes();

      const formattedNodes: VisNode[] = allNodes.map(node => {
        const existing = nodesDataSetRef.current.get(node.id) as any;

        return {
          id: node.id,
          label: (node.properties.Name as string) || (node.properties.Title as string) || node.id,
          shape: getNodeShape(node.type),
          color: {
            background: getNodeColor(node.type),
            border: existing?.color?.border || '#ffffff', // MEVCUT çerçeveyi koru
            highlight: { background: '#ffffff', border: existing?.color?.border || getNodeColor(node.type) }
          },
          font: { color: '#ffffff', size: existing?.font?.size || 14, face: 'Inter' },
          borderWidth: existing?.borderWidth || 2, // MEVCUT kalınlığı koru
          shadow: true,
          title: existing?.title,
          fixed: existing?.fixed
        };
      });

      nodesDataSetRef.current.update(formattedNodes);

      const edgesList: VisEdge[] = [];
      const processedEdges = new Set<string>();

      for (const node of allNodes) {
        const nodeEdges = await nodeService.getNodeEdges(node.id);
        nodeEdges.forEach((edge: IEdge) => {
          const edgeKey = [edge.sourceId, edge.targetId].sort().join('-');
          if (edge.relationType === 'FRIEND' && processedEdges.has(edgeKey)) return;
          processedEdges.add(edgeKey);

          // Mevcut edge stilini koru
          const existingEdge = edgesDataSetRef.current.get(edge.id) as any;

          edgesList.push({
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
          } as any);
        });
      }

      edgesDataSetRef.current.update(edgesList);

      // Highlighting effect'ini tetikle
      setDataVersion(v => v + 1);

    } catch (err) {
      console.error('Failed to load graph data:', err);
    }
  }, []);

  // --- Network Oluşturma (Tek Sefer) ---
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
        stabilization: false // Hemen başla, bekleme yok
      },
      interaction: {
        hover: true,
        tooltipDelay: 200,
        hideEdgesOnDrag: false,
        multiselect: false, // Shift+Drag (box selection) iptal ederek pinleme çakışmasını giderir
        selectable: true,
        dragNodes: true
      }
    };

    networkRef.current = new Network(
      containerRef.current,
      { nodes: nodesDataSetRef.current, edges: edgesDataSetRef.current },
      options
    );
    
    // Senkronizasyon sorunlarını (lag) önlemek için ref'i anında güncelleyen yardımcı
    const selectNodeImmediate = (id: string) => {
      selectedNodeIdRef.current = id;
      onNodeSelect(id);
    };

    // Sol tık: Current Node seç
    networkRef.current.on('click', (params) => {
      if (params.nodes.length > 0) {
        const id = params.nodes[0];
        lastInteractedNodeIdRef.current = id;
        selectNodeImmediate(id);
      }
    });

    // Sürükleme başlangıcı (Drag): Sürüklenen düğümü otomatik olarak seç
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

    // Sağ tık: Target Node seç
    networkRef.current.on('oncontext', (params) => {
      params.event.preventDefault();
      const nodeId = networkRef.current?.getNodeAt(params.pointer.DOM);
      if (nodeId && onNodeRightClick) {
        onNodeRightClick(nodeId as string);
      }
    });

    // Hover Edge: Sadece hover yapılan çizginin label'ını göster
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
        label: " " // undefined yerine boşluk karakteri kullanıyoruz
      });
    });

    // --- Animasyonlu Yol (Akış Efekti) ---
    networkRef.current.on('afterDrawing', (ctx) => {
      // Arkadaş önerisi (recs) modunda kan akışı (blood flow) animasyonunu iptal et
      if (highlightModeRef.current === 'recs') return;

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

        // İki düğüm arasında hareket eden 3 parçacık (ışık hüzmesi)
        for (let j = 0; j < 3; j++) {
          // Parçacıklar arasında mesafe bırak
          let progress = (time * 1.5 - j * 0.15) % 1;
          if (progress < 0) progress += 1;

          const x = fromPos.x + (toPos.x - fromPos.x) * progress;
          const y = fromPos.y + (toPos.y - fromPos.y) * progress;

          // Parçacık çizimi
          ctx.beginPath();
          ctx.arc(x, y, 4, 0, 2 * Math.PI, false);

          // Baştaki parçacık daha parlak, arkadakiler sönük
          const opacity = 1 - (j * 0.3);
          ctx.fillStyle = `rgba(16, 185, 129, ${opacity})`;
          ctx.shadowColor = '#34d399';
          ctx.shadowBlur = 10;
          ctx.fill();
          ctx.closePath();
          ctx.shadowBlur = 0; // diğer çizimleri etkilemesin
        }
      }
    });

    // --- Akış Nefesi: durduğu an beklemeden yeniden başlat ---
    networkRef.current.on('stabilized', () => {
      if (!networkRef.current || !physicsSlowedRef.current) return;
      networkRef.current.startSimulation();
    });

    // --- Floating Keeper + Otomatik Fit ---
    const floatingKeeper = setInterval(() => {
      if (!networkRef.current || isDraggingRef.current) return;
      
      // İlk fit: veri yüklendikten hemen sonra
      if (!isFirstFitDoneRef.current && nodesDataSetRef.current.length > 0) {
        networkRef.current.fit({
          animation: { duration: 400, easingFunction: 'easeInOutQuad' }
        });
        isFirstFitDoneRef.current = true;
      }

      // Yavaş süzülme moduna geç (1 kere)
      if (!physicsSlowedRef.current && isFirstFitDoneRef.current) {
        networkRef.current.setOptions({
          physics: { 
            maxVelocity: 2, 
            timestep: 0.2,
            minVelocity: 0.001 // Neredeyse hiç durma seviyesi
          }
        });
        physicsSlowedRef.current = true;
      }
    }, 1000);

    // --- Akış Canlılığı (Render Loop) ---
    // Vis.js stabilized olduğunda (düğümler durduğunda) render durur, bu da akış animasyonunun donmasına neden olur.
    // Eğer bir path veya vurgu varsa, render'ı manuel tetikleyerek akışı sürekli kılıyoruz.
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

  // --- Periyodik Veri Güncelleme ---
  useEffect(() => {
    const interval = setInterval(initData, 30000);
    return () => clearInterval(interval);
  }, [initData]);

  // --- Seçim Güncellemesi ---
  useEffect(() => {
    if (networkRef.current && selectedNodeId) {
      networkRef.current.selectNodes([selectedNodeId]);
      
      // Sadece dışarıdan (örn: arama kutusundan) seçildiyse kamera odaklansın
      if (!isDraggingRef.current && lastInteractedNodeIdRef.current !== selectedNodeId) {
        networkRef.current.focus(selectedNodeId, {
          animation: { duration: 1000, easingFunction: 'easeInOutQuad' }
        });
      }
    }
  }, [selectedNodeId]);

  // --- Düğüm Renklendirme (HER ZAMAN doğru renkleri uygula) ---
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
      } else if (isTarget && isPinned) {
        borderColor = '#c026d3'; bw = 6; fontSize = 16;
      } else if (isOrigin) {
        borderColor = '#3b82f6'; bw = 5; fontSize = 16;
      } else if (isTarget) {
        borderColor = '#ef4444'; bw = 5; fontSize = 16;
      } else if (isPinned) {
        borderColor = '#a855f7'; bw = 5;
      } else if (isPath) {
        if (highlightMode === 'recs') {
          // Önerilen düğümler güçlü bir şekilde parlasın (Yeşil)
          borderColor = '#10b981'; bw = 5; fontSize = 16;
          shadow = { enabled: true, color: '#10b981', size: 25 };
        } else if (highlightMode === 'chain') {
          // Zincir/Bağıntı sorgusu: Elektrik mavisi (Siyan) parlasın - Daha opak ve net
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
          smooth: false, // Animasyonun düz çizgi üzerinde doğru çalışması için
          shadow: false
        };
      } else if (isChainEdge) {
        return {
          ...edge,
          color: { color: 'rgba(6, 182, 212, 0.15)', highlight: 'rgba(6, 182, 212, 0.4)' },
          width: 2,
          shadow: { enabled: true, color: 'rgba(6, 182, 212, 0.3)', size: 5 }
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
