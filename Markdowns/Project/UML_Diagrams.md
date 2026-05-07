# Social Graph Project - UML Diagrams

This document contains the core system diagrams required for the academic defense (Requirement B.2).

## 1. Class Diagram (Core Structure)

The class diagram shows the relationships between the custom data structures and the graph domain models.

```mermaid
classDiagram
    direction TB
    class PropertyGraph {
        -CustomHashTable nodes
        -CustomHashTable adjacency
        -int edgeCount
        +AddNode(Node node)
        +AddEdge(Edge edge)
        +GetNode(string id) Node
        +GetAllNodes() Node[]
        +GetNeighbors(string id) Node[]
        +GetEdgesByType(string id, string type) Edge[]
    }

    class CustomHashTable~TKey, TValue~ {
        -TKey[] keys
        -TValue[] values
        -byte[] states
        -int count
        +Put(TKey key, TValue value)
        +Get(TKey key) TValue
        +TryGetValue(TKey key, out TValue value) bool
        +Remove(TKey key) bool
        -Rehash()
    }

    class CustomTrie {
        -TrieNode root
        +Insert(string word)
        +Search(string word) bool
        +GetSuggestions(string prefix) List~string~
    }

    class CustomQueue~T~ {
        -Node~T~ head
        -Node~T~ tail
        -int count
        +Enqueue(T item)
        +Dequeue() T
        +IsEmpty() bool
    }

    class Node {
        +string Id
        +string Type
        +CustomHashTable properties
    }

    class Edge {
        +string Id
        +string SourceId
        +string DestinationId
        +string RelationType
        +bool IsDirected
        +CustomHashTable properties
    }

    class TrieNode {
        -CustomHashTable children
        -bool IsEndOfWord
    }

    PropertyGraph "1" *-- "many" Node : Contains
    PropertyGraph "1" *-- "many" Edge : Manages
    PropertyGraph ..> CustomHashTable : Uses for storage
    Node "1" *-- "1" CustomHashTable : Stores Properties
    Edge "1" *-- "1" CustomHashTable : Stores Properties
    CustomTrie "1" *-- "1" TrieNode : Root Node
    TrieNode "1" *-- "many" TrieNode : Children
    TrieNode ..> CustomHashTable : Uses for child storage
```

---

## 2. Component Diagram (System Architecture)

The system is designed with a decoupled architecture involving Frontend, API, and a background AI service.

```mermaid
graph TD
    subgraph Frontend
        UI["React Frontend (UI)"]
    end

    subgraph Backend
        API["SocialGraph.API (Core Engine)"]
        DB[("In-Memory Graph (HashTable)")]
    end

    subgraph Simulation_Engine
        AI["SocialGraph.AI (Worker)"]
    end

    UI -- "REST API (JSON)" --> API
    AI -- "Async Data Stream" --> API
    API --- DB
```

---

## 3. Sequence Diagram (Multi-step Chain Query Flow)

Traces the execution of a sequential pipeline query (e.g., User -> FRIEND -> User -> ATTENDS -> Event).

```mermaid
sequenceDiagram
    autonumber
    actor User
    participant UI as React UI
    participant API as TraversalController
    participant Engine as RelationalQueryEngine
    participant Graph as PropertyGraph

    User->>UI: Select Start Node & Relations (FRIEND, ATTENDS)
    UI->>API: GET /api/traversal/chain (nodeId, relations[])
    API->>Engine: ExecuteChainQuery(startId, relations)
    
    loop For each relation in sequence
        Engine->>Graph: GetNeighbors(currentNodes, relationType)
        Graph-->>Engine: List of target nodes
        Engine->>Engine: Filter & Aggregate results
    end

    Engine-->>API: ChainResponseDto (Nodes, Steps)
    API-->>UI: JSON Result
    UI->>UI: Render Spider-Web & Report
    UI-->>User: Visual Feedback
```
