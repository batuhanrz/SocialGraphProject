import { fetchApi } from './apiService';
import type { IRecommendation, IChainResponse, IPathStep } from '../types/graph';

export const traversalService = {
    bfs: async (startNodeId: string): Promise<string[]> => {
        return fetchApi<string[]>(`/traversal/bfs?startNodeId=${encodeURIComponent(startNodeId)}`);
    },

    dfs: async (startNodeId: string): Promise<string[]> => {
        return fetchApi<string[]>(`/traversal/dfs?startNodeId=${encodeURIComponent(startNodeId)}`);
    },

    shortestPath: async (startNodeId: string, targetNodeId: string, algorithm: string): Promise<IPathStep[]> => {
        return fetchApi<IPathStep[]>(
            `/traversal/shortestpath?startNodeId=${encodeURIComponent(startNodeId)}&targetNodeId=${encodeURIComponent(targetNodeId)}&algorithm=${encodeURIComponent(algorithm)}`
        );
    },

    chain: async (startNodeId: string, relations: string[]): Promise<IChainResponse> => {
        const queryParams = relations.map(r => `relations=${encodeURIComponent(r)}`).join('&');
        return fetchApi<IChainResponse>(`/traversal/chain?startNodeId=${encodeURIComponent(startNodeId)}&${queryParams}`);
    },

    recommendations: async (userId: string): Promise<IRecommendation[]> => {
        return fetchApi<IRecommendation[]>(`/traversal/recommendations?userId=${encodeURIComponent(userId)}`);
    }
};
