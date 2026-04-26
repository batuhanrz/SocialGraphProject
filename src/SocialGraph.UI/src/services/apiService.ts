const BASE_URL = 'http://localhost:5000/api';

/**
 * Native fetch wrapper for API calls.
 * Ensures JSON format and simple error throwing.
 */
export const fetchApi = async <T>(endpoint: string, options?: RequestInit): Promise<T> => {
    try {
        const response = await fetch(`${BASE_URL}${endpoint}`, {
            ...options,
            headers: {
                'Content-Type': 'application/json',
                ...options?.headers,
            },
        });

        if (!response.ok) {
            throw new Error(`API Error: ${response.status} ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        console.error(`[fetchApi] Request failed for endpoint ${endpoint}:`, error);
        throw error;
    }
};
