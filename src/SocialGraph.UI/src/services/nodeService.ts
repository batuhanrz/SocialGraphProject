import { fetchApi } from './apiService';
import type { INode, ISearchResult } from '../types/graph';

export const nodeService = {
    searchNodes: async (query: string): Promise<ISearchResult[]> => {
        if (!query.trim()) return [];
        return fetchApi<ISearchResult[]>(`/search/autocomplete?query=${encodeURIComponent(query)}`);
    },

    getNode: async (id: string): Promise<INode> => {
        return fetchApi<INode>(`/nodes/${encodeURIComponent(id)}`);
    },

    getAllNodes: async (): Promise<INode[]> => {
        return fetchApi<INode[]>('/nodes');
    }
};
