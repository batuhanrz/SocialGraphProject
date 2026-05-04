export interface IProperty {
    [key: string]: string | number | boolean;
}

export interface INode {
    id: string;
    type: "User" | "Photo" | "Event" | string;
    properties: IProperty;
}

export interface IEdge {
    id: string;
    sourceId: string;
    targetId: string;
    relationType: "FRIEND" | "LIKES" | "ATTENDS" | string;
    isDirected: boolean;
    properties: IProperty;
}

export interface IGraphData {
    nodes: INode[];
    edges: IEdge[];
}

export interface ISearchResult {
    nodeId: string;
    label: string;
    type: string;
}

export interface IRecommendation {
    node: INode;
    mutualFriendsCount: number;
}

export interface IChainStep {
    relation: string;
    count: number;
}

export interface IChainResponse {
    nodes: INode[];
    steps: IChainStep[];
}
