import { fetchApi } from './apiService';
import type { INode, IRecommendation } from '../types/graph';

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
    },

    chain: async (startNodeId: string, relations: string[]): Promise<INode[]> => {
        const queryParams = relations.map(r => `relations=${encodeURIComponent(r)}`).join('&');
        return fetchApi<INode[]>(`/traversal/chain?startNodeId=${encodeURIComponent(startNodeId)}&${queryParams}`);
    },

    recommendations: async (userId: string): Promise<IRecommendation[]> => {
        return fetchApi<IRecommendation[]>(`/traversal/recommendations?userId=${encodeURIComponent(userId)}`);
    }
};
