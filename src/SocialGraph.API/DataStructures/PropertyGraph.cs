using System;
using System.Collections.Generic; // Sadece IEnumerable icin
using System.Threading;
using SocialGraph.API.Models;

namespace SocialGraph.API.DataStructures
{
    /// <summary>
    /// Adjacency List tabanli Property Graph cekirdek yapisi.
    /// Heterojen dugum destegi: User, Photo, Event ayni graf icerisinde saklanir.
    /// Kenar turleri: FRIEND, LIKES, POSTED, ATTENDS.
    /// Tum ic veri depolama projenin kendi CustomHashTable yapisi ile yapilir.
    /// Thread safety: ReaderWriterLockSlim ile temel okuma/yazma kilitleri.
    ///
    /// Zaman Karmasikligi (genel):
    ///   - AddNode:       O(1) amortized
    ///   - AddEdge:       O(1) amortized
    ///   - GetNode:       O(1)
    ///   - GetNeighbors:  O(k), k = komsu sayisi
    ///   - GetEdgesByType: O(k), k = komsu sayisi
    ///   - RemoveNode:    O(V + E) worst case
    ///   - RemoveEdge:    O(1)
    ///
    /// Uzay Karmasikligi: O(V + E)
    /// </summary>
    public class PropertyGraph
    {
        // --- Gecerli dugum ve kenar turleri ---
        private static readonly string[] ValidNodeTypes = { "User", "Photo", "Event" };
        private static readonly string[] ValidEdgeTypes = { "FRIEND", "LIKES", "POSTED", "ATTENDS" };

        // --- Ic veri yapilari (tumu CustomHashTable) ---

        /// <summary>
        /// Dugum deposu: NodeID -> Node.
        /// Erisim: O(1) amortized.
        /// </summary>
        private readonly CustomHashTable<string, Node> _nodes;

        /// <summary>
        /// Adjacency list: SourceID -> (DestinationID -> Edge).
        /// Dis tablo kaynak dugum ile, ic tablo hedef dugum ile indekslenir.
        /// Herhangi bir kenara O(1) erisim saglar.
        /// </summary>
        private readonly CustomHashTable<string, CustomHashTable<string, Edge>> _adjacency;

        /// <summary>
        /// Toplam kenar sayaci. Yonsuz kenarlarda tek kenar olarak sayilir (cift kayit yapilsa da).
        /// </summary>
        private int _edgeCount;

        /// <summary>
        /// Temel read/write lock altyapisi (Context.md B.1 eszemanlilik gereksinimi).
        /// Okuma islemleri read lock, yazma islemleri write lock kullanir.
        /// Detayli optimizasyon Sprint 3'te yapilacaktir.
        /// </summary>
        private readonly ReaderWriterLockSlim _lock;

        // --- Public ozellikler ---

        /// <summary>
        /// Graftaki toplam dugum sayisi.
        /// Karmasiklik: O(1)
        /// </summary>
        public int NodeCount
        {
            get
            {
                _lock.EnterReadLock();
                try { return _nodes.Count; }
                finally { _lock.ExitReadLock(); }
            }
        }

        /// <summary>
        /// Graftaki toplam kenar sayisi.
        /// Karmasiklik: O(1)
        /// </summary>
        public int EdgeCount
        {
            get
            {
                _lock.EnterReadLock();
                try { return _edgeCount; }
                finally { _lock.ExitReadLock(); }
            }
        }

        // --- Constructor ---

        public PropertyGraph()
        {
            _nodes = new CustomHashTable<string, Node>();
            _adjacency = new CustomHashTable<string, CustomHashTable<string, Edge>>();
            _edgeCount = 0;
            _lock = new ReaderWriterLockSlim();
        }

        // --- Dugum Islemleri ---

        /// <summary>
        /// Grafa yeni bir dugum ekler. Dugum turu dogrulanir (User, Photo, Event).
        /// Ayni ID ile tekrar ekleme yapilirsa ArgumentException firlatilir.
        /// Karmasiklik: O(1) amortized (Hash Table Put)
        /// Uzay: O(1)
        /// </summary>
        public void AddNode(Node node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (string.IsNullOrEmpty(node.Id)) throw new ArgumentException("Dugum ID bos olamaz.");

            if (!IsValidNodeType(node.Type))
                throw new ArgumentException($"Gecersiz dugum turu: '{node.Type}'. Gecerli turler: User, Photo, Event.");

            _lock.EnterWriteLock();
            try
            {
                if (_nodes.ContainsKey(node.Id))
                    throw new ArgumentException($"Ayni ID'ye sahip dugum zaten mevcut: '{node.Id}'.");

                _nodes.Put(node.Id, node);
                _adjacency.Put(node.Id, new CustomHashTable<string, Edge>());
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// ID ile dugum getirir. Bulunamazsa null doner.
        /// Karmasiklik: O(1) (Hash Table Get)
        /// Uzay: O(1)
        /// </summary>
        public Node? GetNode(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId)) return null;

            _lock.EnterReadLock();
            try
            {
                if (_nodes.TryGetValue(nodeId, out Node node))
                    return node;
                return null;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Tum dugumleri dizi olarak dondurur.
        /// Karmasiklik: O(n), n = dugum sayisi
        /// Uzay: O(n)
        /// </summary>
        public Node[] GetAllNodes()
        {
            _lock.EnterReadLock();
            try
            {
                int count = _nodes.Count;
                Node[] result = new Node[count];
                int index = 0;

                foreach (var kvp in _nodes)
                {
                    result[index++] = kvp.Value;
                }

                return result;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Belirli bir dugumu ve ona bagli tum kenarlari graftan kaldirir.
        /// Yonsuz kenarlarda karsi taraftaki kayit da silinir.
        /// Karmasiklik: O(V + E) worst case (tum dugumlerin adjacency listesi taranir)
        /// Uzay: O(1)
        /// </summary>
        public bool RemoveNode(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId)) return false;

            _lock.EnterWriteLock();
            try
            {
                if (!_nodes.ContainsKey(nodeId)) return false;

                // 1. Bu dugumden cikan kenarlari say ve sil
                if (_adjacency.TryGetValue(nodeId, out CustomHashTable<string, Edge> outEdges))
                {
                    foreach (var kvp in outEdges)
                    {
                        Edge edge = kvp.Value;

                        // Yonsuz kenar ise karsi taraftaki kaydi da sil
                        if (!edge.IsDirected)
                        {
                            if (_adjacency.TryGetValue(edge.DestinationId, out CustomHashTable<string, Edge> reverseEdges))
                            {
                                reverseEdges.Remove(nodeId);
                            }
                        }

                        _edgeCount--;
                    }

                    _adjacency.Remove(nodeId);
                }

                // 2. Diger dugumlerden bu dugume gelen yonlu kenarlari sil
                foreach (var adjKvp in _adjacency)
                {
                    string sourceId = adjKvp.Key;
                    CustomHashTable<string, Edge> edges = adjKvp.Value;

                    if (edges.ContainsKey(nodeId))
                    {
                        Edge inEdge = edges.Get(nodeId);
                        if (inEdge.IsDirected)
                        {
                            _edgeCount--;
                        }
                        edges.Remove(nodeId);
                    }
                }

                // 3. Dugumu kaldir
                _nodes.Remove(nodeId);

                return true;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        // --- Kenar Islemleri ---

        /// <summary>
        /// Grafa yeni bir kenar ekler. Kenar turu dogrulanir (FRIEND, LIKES, POSTED, ATTENDS).
        /// Kaynak ve hedef dugumlerin grafta mevcut olmasi gerekir.
        /// Yonsuz kenarlar (orn. FRIEND) icin her iki yone de kayit olusturulur.
        /// Karmasiklik: O(1) amortized
        /// Uzay: O(1)
        /// </summary>
        public void AddEdge(Edge edge)
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));
            if (string.IsNullOrEmpty(edge.Id)) throw new ArgumentException("Kenar ID bos olamaz.");
            if (string.IsNullOrEmpty(edge.SourceId)) throw new ArgumentException("Kaynak dugum ID bos olamaz.");
            if (string.IsNullOrEmpty(edge.DestinationId)) throw new ArgumentException("Hedef dugum ID bos olamaz.");

            if (!IsValidEdgeType(edge.RelationType))
                throw new ArgumentException($"Gecersiz kenar turu: '{edge.RelationType}'. Gecerli turler: FRIEND, LIKES, POSTED, ATTENDS.");

            // Yonsuz kenar ise ters yon hazirligini kilit disinda yap (lock suresini azaltmak icin)
            Edge? reverseEdge = null;
            if (!edge.IsDirected)
            {
                reverseEdge = new Edge(
                    edge.Id + "_reverse",
                    edge.DestinationId,
                    edge.SourceId,
                    edge.RelationType,
                    false
                );

                // Orijinal kenarin ek ozelliklerini kopyala
                foreach (var propKvp in edge.Properties)
                {
                    reverseEdge.Properties.Put(propKvp.Key, propKvp.Value);
                }
            }

            _lock.EnterWriteLock();
            try
            {
                if (!_nodes.ContainsKey(edge.SourceId))
                    throw new ArgumentException($"Kaynak dugum bulunamadi: '{edge.SourceId}'.");
                if (!_nodes.ContainsKey(edge.DestinationId))
                    throw new ArgumentException($"Hedef dugum bulunamadi: '{edge.DestinationId}'.");

                // Kaynak -> Hedef yonunde kenar ekle
                if (!_adjacency.ContainsKey(edge.SourceId))
                    _adjacency.Put(edge.SourceId, new CustomHashTable<string, Edge>());

                _adjacency.Get(edge.SourceId).Put(edge.DestinationId, edge);

                // Yonsuz kenar ise onceden hazirlanan ters yonu ekle
                if (reverseEdge != null)
                {
                    if (!_adjacency.ContainsKey(edge.DestinationId))
                        _adjacency.Put(edge.DestinationId, new CustomHashTable<string, Edge>());

                    _adjacency.Get(edge.DestinationId).Put(edge.SourceId, reverseEdge);
                }

                _edgeCount++;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Belirli bir kenari graftan kaldirir.
        /// Yonsuz kenar ise karsi yondeki kayit da silinir.
        /// Karmasiklik: O(1)
        /// Uzay: O(1)
        /// </summary>
        public bool RemoveEdge(string sourceId, string destinationId)
        {
            if (string.IsNullOrEmpty(sourceId) || string.IsNullOrEmpty(destinationId))
                return false;

            _lock.EnterWriteLock();
            try
            {
                if (!_adjacency.TryGetValue(sourceId, out CustomHashTable<string, Edge> sourceEdges))
                    return false;

                if (!sourceEdges.TryGetValue(destinationId, out Edge edge))
                    return false;

                // Kenari sil
                sourceEdges.Remove(destinationId);

                // Yonsuz kenar ise ters yonu de sil
                if (!edge.IsDirected)
                {
                    if (_adjacency.TryGetValue(destinationId, out CustomHashTable<string, Edge> destEdges))
                    {
                        destEdges.Remove(sourceId);
                    }
                }

                _edgeCount--;
                return true;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        // --- Sorgulama Islemleri ---

        /// <summary>
        /// Belirli bir dugumun komsularini (bagli dugumleri) dizi olarak dondurur.
        /// Adjacency list uzerinden O(k) karmasiklikla calisir, k = komsu sayisi.
        /// Karmasiklik: O(k)
        /// Uzay: O(k)
        /// </summary>
        public Node[] GetNeighbors(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId)) return Array.Empty<Node>();

            _lock.EnterReadLock();
            try
            {
                if (!_adjacency.TryGetValue(nodeId, out CustomHashTable<string, Edge> edges))
                    return Array.Empty<Node>();

                int count = edges.Count;
                Node[] neighbors = new Node[count];
                int index = 0;

                foreach (var kvp in edges)
                {
                    string neighborId = kvp.Key;
                    if (_nodes.TryGetValue(neighborId, out Node neighbor))
                    {
                        neighbors[index++] = neighbor;
                    }
                }

                // Eger bazi komsular bulunamadiysa (silinen dugum referansi), diziyi daralt
                if (index < count)
                {
                    Node[] trimmed = new Node[index];
                    Array.Copy(neighbors, trimmed, index);
                    return trimmed;
                }

                return neighbors;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Belirli bir dugumun kenarlarini tur filtresine gore dizi olarak dondurur.
        /// Adjacency list uzerinden tum kenarlari tarar, eslesen turleri toplar.
        /// Karmasiklik: O(k), k = toplam komsu sayisi
        /// Uzay: O(m), m = eslesen kenar sayisi
        /// </summary>
        public Edge[] GetEdgesByType(string nodeId, string relationType)
        {
            if (string.IsNullOrEmpty(nodeId) || string.IsNullOrEmpty(relationType))
                return Array.Empty<Edge>();

            _lock.EnterReadLock();
            try
            {
                if (!_adjacency.TryGetValue(nodeId, out CustomHashTable<string, Edge> edges))
                    return Array.Empty<Edge>();

                // Ilk gecis: eslesen kenar sayisini bul
                int matchCount = 0;
                foreach (var kvp in edges)
                {
                    if (string.Equals(kvp.Value.RelationType, relationType, StringComparison.OrdinalIgnoreCase))
                        matchCount++;
                }

                if (matchCount == 0) return Array.Empty<Edge>();

                // Ikinci gecis: eslesen kenarlari topla
                Edge[] result = new Edge[matchCount];
                int index = 0;
                foreach (var kvp in edges)
                {
                    if (string.Equals(kvp.Value.RelationType, relationType, StringComparison.OrdinalIgnoreCase))
                    {
                        result[index++] = kvp.Value;
                    }
                }

                return result;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Belirli bir dugumun tum kenarlarini dizi olarak dondurur.
        /// Karmasiklik: O(k), k = komsu sayisi
        /// Uzay: O(k)
        /// </summary>
        public Edge[] GetEdges(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId)) return Array.Empty<Edge>();

            _lock.EnterReadLock();
            try
            {
                if (!_adjacency.TryGetValue(nodeId, out CustomHashTable<string, Edge> edges))
                    return Array.Empty<Edge>();

                int count = edges.Count;
                Edge[] result = new Edge[count];
                int index = 0;

                foreach (var kvp in edges)
                {
                    result[index++] = kvp.Value;
                }

                return result;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Graftaki tum kenarlari dizi olarak dondurur.
        /// Yonsuz kenarlarda sadece tek yondeki kaydı dondurur (reverse haric).
        /// Karmasiklik: O(V + E)
        /// Uzay: O(E)
        /// </summary>
        public Edge[] GetAllEdges()
        {
            _lock.EnterReadLock();
            try
            {
                // Optimizasyon: _edgeCount zaten reverse olmayan (lojik) kenarlari sayiyor.
                if (_edgeCount == 0) return Array.Empty<Edge>();

                Edge[] result = new Edge[_edgeCount];
                int index = 0;

                foreach (var adjKvp in _adjacency)
                {
                    foreach (var edgeKvp in adjKvp.Value)
                    {
                        Edge e = edgeKvp.Value;
                        // Reverse kenarlari haric tutarak sadece orijinal/ana kenarlari dondur
                        if (!e.Id.EndsWith("_reverse"))
                        {
                            // Olasi bir yaris durumuna (race condition) karsi dizi siniri kontrolu
                            if (index < _edgeCount)
                            {
                                result[index++] = e;
                            }
                        }
                    }
                }

                // Eger index _edgeCount'tan kucukse (silinme olduysa), diziyi daralt
                if (index < _edgeCount)
                {
                    Edge[] trimmed = new Edge[index];
                    Array.Copy(result, trimmed, index);
                    return trimmed;
                }

                return result;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        // --- Yardimci Metotlar ---

        /// <summary>
        /// Dugum turunun gecerli olup olmadigini kontrol eder.
        /// Gecerli turler: User, Photo, Event.
        /// Karmasiklik: O(1) (sabit boyutlu dizi taramasi)
        /// </summary>
        private static bool IsValidNodeType(string type)
        {
            if (string.IsNullOrEmpty(type)) return false;

            for (int i = 0; i < ValidNodeTypes.Length; i++)
            {
                if (string.Equals(ValidNodeTypes[i], type, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Kenar turunun gecerli olup olmadigini kontrol eder.
        /// Gecerli turler: FRIEND, LIKES, POSTED, ATTENDS.
        /// Karmasiklik: O(1) (sabit boyutlu dizi taramasi)
        /// </summary>
        private static bool IsValidEdgeType(string type)
        {
            if (string.IsNullOrEmpty(type)) return false;

            for (int i = 0; i < ValidEdgeTypes.Length; i++)
            {
                if (string.Equals(ValidEdgeTypes[i], type, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
