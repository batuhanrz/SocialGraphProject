import { nodeService } from './nodeService';
import { traversalService } from './traversalService';

export interface BenchmarkResult {
  scale: number;
  seedTime: number;
  bfsTime: number;
  dfsTime: number;
  trieTime: number;
  throughput: number;
}

export const benchmarkService = {
  resetSystem: async () => {
    const response = await fetch('http://localhost:5000/api/nodes/reset', { method: 'DELETE' });
    if (!response.ok) throw new Error('System reset failed');
  },

  seedSystem: async () => {
    const response = await fetch('http://localhost:5000/api/nodes/seed', { method: 'POST' });
    if (!response.ok) throw new Error('System seeding failed');
  },

  runBenchmark: async (onProgress: (msg: string) => void): Promise<BenchmarkResult[]> => {
    const scales = [100, 500, 1000, 5000];
    const results: BenchmarkResult[] = [];

    for (const scale of scales) {
      onProgress(`SYSTEM AUDIT: ANALYSING ${scale} NODES...`);
      
      // 1. Reset
      await benchmarkService.resetSystem();

      // 2. Measure Seeding
      const startSeed = performance.now();
      await seedNodes(scale);
      const seedTime = performance.now() - startSeed;

      // 3. Measure BFS
      const startBfs = performance.now();
      await traversalService.shortestPath(`user1`, `user${scale}`, 'bfs');
      const bfsTime = performance.now() - startBfs;

      // 4. Measure DFS
      const startDfs = performance.now();
      await traversalService.shortestPath(`user1`, `user${scale}`, 'dfs');
      const dfsTime = performance.now() - startDfs;

      // 5. Measure Trie (Search)
      const startTrie = performance.now();
      await nodeService.searchNodes('user');
      const trieTime = performance.now() - startTrie;

      results.push({
        scale,
        seedTime: Math.round(seedTime),
        bfsTime: Math.round(bfsTime),
        dfsTime: Math.round(dfsTime),
        trieTime: Math.round(trieTime),
        throughput: Math.round(scale / (seedTime / 1000))
      });
    }

    return results;
  }
};

async function seedNodes(count: number) {
  const nodes = [];
  const edges = [];

  for (let i = 1; i <= count; i++) {
    nodes.push({
      id: `user${i}`,
      type: 'User',
      properties: { Name: `Benchmark User ${i}` }
    });

    if (i > 1) {
      edges.push({
        id: `e${i}`,
        sourceId: `user${i - 1}`,
        targetId: `user${i}`,
        relationType: 'FRIEND',
        isDirected: false,
        properties: {}
      });
    }
  }

  // Batch push
  await fetch('http://localhost:5000/api/nodes/batch', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(nodes)
  });

  await fetch('http://localhost:5000/api/edges/batch', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(edges)
  });
}
