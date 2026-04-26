import { fetchApi } from './apiService';

export const traversalService = {
    bfs: async (startNodeId: string): Promise<string[]> => {
        return fetchApi<string[]>(`/traversal/bfs?startNodeId=${encodeURIComponent(startNodeId)}`);
    },

    dfs: async (startNodeId: string): Promise<string[]> => {
        return fetchApi<string[]>(`/traversal/dfs?startNodeId=${encodeURIComponent(startNodeId)}`);
    },

    shortestPath: async (startNodeId: string, targetNodeId: string): Promise<string[]> => {
        return fetchApi<string[]>(
            `/traversal/shortestpath?startNodeId=${encodeURIComponent(startNodeId)}&targetNodeId=${encodeURIComponent(targetNodeId)}`
        );
    }
};
