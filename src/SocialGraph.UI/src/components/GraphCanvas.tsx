import React, { useEffect, useRef, useCallback } from 'react';
import { Network, type Node as VisNode, type Edge as VisEdge, type Options } from 'vis-network';
import { DataSet } from 'vis-data';
import { nodeService } from '../services/nodeService';
import type { IEdge } from '../types/graph';

interface GraphCanvasProps {
  selectedNodeId: string | null;
  onNodeSelect: (id: string) => void;
  highlightNodeIds?: string[];
}

const GraphCanvas: React.FC<GraphCanvasProps> = ({
  selectedNodeId,
  onNodeSelect,
  highlightNodeIds = []
}) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const networkRef = useRef<Network | null>(null);
  const nodesDataSetRef = useRef<DataSet<VisNode>>(new DataSet([]));
  const edgesDataSetRef = useRef<DataSet<VisEdge>>(new DataSet([]));

  const initData = useCallback(async () => {
    try {
      const allNodes = await nodeService.getAllNodes();

      const formattedNodes: VisNode[] = allNodes.map(node => ({
        id: node.id,
        label: (node.properties.Name as string) || (node.properties.Title as string) || node.id,
        shape: getNodeShape(node.type),
        color: {
          background: getNodeColor(node.type),
          border: '#ffffff',
          highlight: { background: '#ffffff', border: getNodeColor(node.type) }
        },
        font: { color: '#ffffff', size: 14, face: 'Inter' },
        borderWidth: 2,
        shadow: true
      }));

      nodesDataSetRef.current.clear();
      nodesDataSetRef.current.add(formattedNodes);

      const edgesList: VisEdge[] = [];
      const processedEdges = new Set<string>();

      for (const node of allNodes) {
        const nodeEdges = await nodeService.getNodeEdges(node.id);
        nodeEdges.forEach((edge: IEdge) => {
          const edgeKey = [edge.sourceId, edge.targetId].sort().join('-');
          if (edge.relationType === 'FRIEND' && processedEdges.has(edgeKey)) return;

          processedEdges.add(edgeKey);

          edgesList.push({
            id: edge.id,
            from: edge.sourceId,
            to: edge.targetId,
            label: edge.relationType,
            font: { align: 'top', size: 10, color: 'rgba(255,255,255,0.4)' },
            arrows: edge.isDirected ? { to: { enabled: true } } : undefined,
            dashes: getEdgeDashes(edge.relationType),
            color: { color: 'rgba(255,255,255,0.2)', highlight: '#ffffff' },
            width: 1
          });
        });
      }

      edgesDataSetRef.current.clear();
      edgesDataSetRef.current.add(edgesList);
    } catch (err) {
      console.error('Failed to load graph data:', err);
    }
  }, []);

  useEffect(() => {
    if (!containerRef.current) return;

    const options: Options = {
      nodes: {
        scaling: { min: 10, max: 30 }
      },
      edges: {
        smooth: { enabled: true, type: 'continuous', roundness: 0.5 }
      },
      physics: {
        forceAtlas2Based: {
          gravitationalConstant: -50,
          centralGravity: 0.01,
          springLength: 100,
          springConstant: 0.08
        },
        maxVelocity: 50,
        solver: 'forceAtlas2Based',
        timestep: 0.35,
        stabilization: { iterations: 150 }
      },
      interaction: {
        hover: true,
        tooltipDelay: 200
      }
    };

    networkRef.current = new Network(
      containerRef.current,
      { nodes: nodesDataSetRef.current, edges: edgesDataSetRef.current },
      options
    );

    networkRef.current.on('click', (params) => {
      if (params.nodes.length > 0) {
        onNodeSelect(params.nodes[0]);
      }
    });

    initData();

    return () => {
      networkRef.current?.destroy();
    };
  }, [initData, onNodeSelect]);

  // Update selection
  useEffect(() => {
    if (networkRef.current && selectedNodeId) {
      networkRef.current.selectNodes([selectedNodeId]);
      networkRef.current.focus(selectedNodeId, {
        scale: 1.2,
        animation: { duration: 1000, easingFunction: 'easeInOutQuad' }
      });
    }
  }, [selectedNodeId]);

  // Update highlighting
  useEffect(() => {
    if (!networkRef.current) return;

    const allNodes = nodesDataSetRef.current.get();
    const updatedNodes = allNodes.map(node => {
      const isHighlighted = highlightNodeIds.includes(node.id as string);
      return {
        ...node,
        borderWidth: isHighlighted ? 4 : 2,
        color: {
          ...(node.color as { background: string; border: string; highlight: string | { border?: string; background?: string } }),
          border: isHighlighted ? '#ffffff' : '#ffffff'
        },
        font: {
          ...(node.font as { color: string; size: number; face: string }),
          size: isHighlighted ? 18 : 14
        }
      };
    });

    nodesDataSetRef.current.update(updatedNodes);
  }, [highlightNodeIds]);

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
    case 'ATTENDS': return [2, 10]; // Dotted
    case 'LIKES': return [5, 5];    // Dashed
    default: return false;          // Solid
  }
};

export default GraphCanvas;
