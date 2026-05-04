# Algoritma Dokümantasyonu (Algorithm Master - Özcan)

Bu doküman, projede kullanılan temel graf algoritmalarının mantığını, pseudocode (sözde kod) karşılıklarını ve ilgili veri yapılarının neden tercih edildiğine dair teknik gerekçeleri içermektedir. Proje savunması (Code Defense) için bir rehber niteliğindedir.

---

## 1. Veri Yapısı Tercih Gerekçeleri

### Neden `CustomHashTable` Kullanıldı?
*   **Gerekçe:** Projenin A.1 kuralı gereği standart .NET koleksiyonlarının (Dictionary vb.) veri saklamak amacıyla kullanımı yasaktır. 
*   **Avantajı:** `CustomHashTable`, Linear Probing algoritması sayesinde bellek lokasyonlarını sıralı kullandığı için önbellek (cache) dostudur. Ziyaret edilen düğümleri O(1) sürede kaydetmek ve kontrol etmek için BFS/DFS algoritmalarında "Visited" tablosu olarak kullanılmıştır.

### Neden `CustomQueue` Kullanıldı?
*   **Gerekçe:** Yine .NET kütüphane yasağı kapsamında, BFS algoritmasının çalışması için gereken FIFO (İlk Giren İlk Çıkar) yapısı sıfırdan oluşturulmalıydı.
*   **Avantajı:** Dairesel dizi (Circular Array) mimarisiyle tasarlanan `CustomQueue`, baştan çıkarma ve sondan ekleme işlemlerini O(1) maliyetle yapar. Bellek kaydırması gerektirmediği için BFS sırasında binlerce düğümü yüksek performansla işleyebilir.

---

## 2. Genişlik Öncelikli Arama (Breadth-First Search - BFS)

**Çalışma Mantığı:** Başlangıç düğümünden başlayarak önce komşularını, sonra komşularının komşularını katman katman ziyaret eden algoritmadır. "En kısa yol" problemlerinde ağırlıksız graflar için en uygun algoritmadır.

**Neden Kullanıldı?** 
Kullanıcıya önerilecek arkadaşların ("mutual friends") veya çok adımlı sorgulardaki (Chain Query) tüm potansiyel yolların eksiksiz taranması için temel mekanizmadır.

**Pseudocode:**
```text
function BFS(graph, startNode):
    queue = new CustomQueue()
    visited = new CustomHashTable()
    
    queue.enqueue(startNode)
    visited.put(startNode, true)
    
    while not queue.isEmpty():
        currentNode = queue.dequeue()
        Process(currentNode)  // Düğümü ziyaret et
        
        for each edge in graph.getEdges(currentNode):
            neighbor = edge.destination
            if not visited.containsKey(neighbor):
                visited.put(neighbor, true)
                queue.enqueue(neighbor)
```

---

## 3. Derinlik Öncelikli Arama (Depth-First Search - DFS)

**Çalışma Mantığı:** Başlangıç düğümünden itibaren gidebileceği en derin noktaya kadar ilerleyen, çıkmaz sokağa girdiğinde geri dönen (backtracking) algoritmadır. Özyinelemeli (recursive) veya CustomStack ile iterative olarak yazılabilir. (Projede özyinelemeli tercih edilmiştir).

**Neden Kullanıldı?** 
Grafın tamamını gezmek, karmaşık ilişkileri tespit etmek veya iki düğüm arasında "herhangi bir" yol olup olmadığını hızlıca anlamak için kullanılmıştır.

**Pseudocode:**
```text
function DFS(graph, startNode):
    visited = new CustomHashTable()
    DFS_Recursive(graph, startNode, visited)

function DFS_Recursive(graph, currentNode, visited):
    visited.put(currentNode, true)
    Process(currentNode)  // Düğümü ziyaret et
    
    for each edge in graph.getEdges(currentNode):
        neighbor = edge.destination
        if not visited.containsKey(neighbor):
            DFS_Recursive(graph, neighbor, visited)
```

---

## 4. En Kısa Yol Algoritması (Shortest Path)

**Çalışma Mantığı:** İki düğüm (Origin ve Target) arasındaki en az atlamalı (hop) yolu bulur. Ağırlıksız (unweighted) graflarda BFS tabanlı çalışır. Bulunan her komşunun "kim tarafından keşfedildiğini" (parent) saklayarak hedef bulunduğunda geriye doğru bir yol haritası çıkartır.

**Neden Kullanıldı?** 
İki kullanıcı arasındaki bağlantı derecesini (degrees of separation) veya bir kullanıcının bir fotoğrafa ulaşma zincirini arayüzde göstermek için.

**Pseudocode:**
```text
function ShortestPath(graph, startNode, targetNode):
    queue = new CustomQueue()
    visited = new CustomHashTable()
    parents = new CustomHashTable()  // Child -> Parent takibi
    
    queue.enqueue(startNode)
    visited.put(startNode, true)
    
    while not queue.isEmpty():
        currentNode = queue.dequeue()
        
        if currentNode == targetNode:
            return ReconstructPath(parents, targetNode)
            
        for each edge in graph.getEdges(currentNode):
            neighbor = edge.destination
            if not visited.containsKey(neighbor):
                visited.put(neighbor, true)
                parents.put(neighbor, currentNode)
                queue.enqueue(neighbor)
                
    return empty_array // Yol bulunamadı

function ReconstructPath(parents, targetNode):
    path = []
    current = targetNode
    while parents.containsKey(current):
        path.add(current)
        current = parents.get(current)
    path.add(current)  // startNode
    return reverse(path)
```
