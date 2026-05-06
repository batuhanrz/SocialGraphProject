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
    // 1. Grafi sifirla
    const response = await fetch('http://localhost:5000/api/nodes/reset', { method: 'DELETE' });
    if (!response.ok) throw new Error('System reset failed');

    // 2. Aksiyon akisini temizle
    await fetch('http://localhost:5000/api/simulation/reset', { method: 'DELETE' });
  },

  seedSystem: async () => {
    const response = await fetch('http://localhost:5000/api/nodes/seed', { method: 'POST' });
    if (!response.ok) throw new Error('System seeding failed');

    // Aksiyon akisini da temizle (Seed yeni bir baslangictir)
    await fetch('http://localhost:5000/api/simulation/reset', { method: 'DELETE' });
  },

  runBenchmark: async (iterations: number, onProgress: (msg: string) => void): Promise<BenchmarkResult[]> => {
    const scales = [100, 500, 1000, 5000];
    // Accumulator for sums: scale -> totals
    const accumulators = new Map<number, { seed: number; bfs: number; dfs: number; trie: number; throughput: number }>();
    
    // Initialize accumulators
    scales.forEach(s => accumulators.set(s, { seed: 0, bfs: 0, dfs: 0, trie: 0, throughput: 0 }));

    try {
      // 0. Simulasyonu durdur (Temiz olcum icin)
      onProgress("PREPARING ENVIRONMENT: PAUSING AI WORKER...");
      await fetch('http://localhost:5000/api/simulation/pause', { method: 'POST' });
      await new Promise(r => setTimeout(r, 500));

      for (let i = 1; i <= iterations; i++) {
        const batchPrefix = iterations > 1 ? `[BATCH ${i}/${iterations}] ` : "";

        for (const scale of scales) {
          onProgress(`${batchPrefix}ANALYSING ${scale} NODES...`);
          
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

          // Accumulate
          const acc = accumulators.get(scale)!;
          acc.seed += seedTime;
          acc.bfs += bfsTime;
          acc.dfs += dfsTime;
          acc.trie += trieTime;
          acc.throughput += (scale / (seedTime / 1000));
        }
      }
    } finally {
      // 6. Simulasyonu devam ettir
      await fetch('http://localhost:5000/api/simulation/resume', { method: 'POST' });
    }

    // Calculate averages
    return scales.map(scale => {
      const acc = accumulators.get(scale)!;
      const avgSeedTime = acc.seed / iterations;
      const avgBfsTime = acc.bfs / iterations;
      const avgDfsTime = acc.dfs / iterations;
      const avgTrieTime = acc.trie / iterations;

      return {
        scale,
        seedTime: Math.round(avgSeedTime),
        bfsTime: Math.round(avgBfsTime),
        dfsTime: Math.round(avgDfsTime),
        trieTime: Math.round(avgTrieTime),
        throughput: Math.round(scale / (avgSeedTime / 1000))
      };
    });
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
