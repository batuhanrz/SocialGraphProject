using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SocialGraph.AI
{
    // API tarafinda beklenen DTO siniflari
    public class NodeDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        
        [JsonPropertyName("properties")]
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    public class EdgeDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonPropertyName("sourceId")]
        public string SourceId { get; set; } = string.Empty;
        
        [JsonPropertyName("targetId")]
        public string TargetId { get; set; } = string.Empty;
        
        [JsonPropertyName("relationType")]
        public string RelationType { get; set; } = string.Empty;
        
        [JsonPropertyName("isDirected")]
        public bool IsDirected { get; set; } = true;
        
        [JsonPropertyName("properties")]
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    public class DataGenerator
    {
        private static readonly Random _rnd = new Random();

        // Gemini 3.1 Pro tarafindan uretilmis statik User listesi (Ornek 50 Kisi)
        private static readonly string[] _userNames = {
            "Batuhan Yilmaz", "Fatma Sude Kaya", "Muhammed Furkan Celik", "Isra Nur Demir", "Ozcan Sahin",
            "Ahmet Yilmaz", "Ayse Demir", "Mehmet Kaya", "Fatma Celik", "Mustafa Sahin",
            "Zeynep Koc", "Ali Ozturk", "Elif Aydin", "Huseyin Ozdemir", "Merve Arslan",
            "Hasan Dogan", "Esra Kilic", "Ibrahim Cetin", "Busra Gurbuz", "Halil Gok",
            "Burcu Tekin", "Kemal Polat", "Selin Tarhan", "Caner Bulut", "Eda Yildirim",
            "Tolga Coban", "Cemre Yildiz", "Emre Karaca", "Derya Cakir", "Sinan Tas",
            "Gozde Akin", "Turgut Aslan", "Isil Erdogan", "Oguzhan Gunes", "Tugce Yavuz",
            "Gokhan Kaplan", "Muge Celik", "Serkan Yucel", "Deniz Ozer", "Umut Ekinci",
            "Aslihan Kara", "Koray Avci", "Zehra Basar", "Orhan Veli", "Nazli Cam",
            "Eren Yalcin", "Yasemin Kurt", "Volkan Turk", "Ceren Yilmaz", "Kerem Koca"
        };

        private static readonly string[] _professions = {
            "Software Engineer", "Data Scientist", "UI/UX Designer", "Product Manager", "DevOps Engineer",
            "Marketing Specialist", "Graphic Designer", "Project Manager", "Business Analyst", "CEO"
        };

        // Gemini 3.1 Pro tarafindan uretilmis statik Photo listesi (Ornek 30 Fotograf)
        private static readonly string[] _photoTitles = {
            "Hackathon Hatirasi", "Ofiste Ilk Gun", "Yapay Zeka Zirvesi", "Kahve Molasi", "Yeni Proje Toplantisi",
            "Kodlama Gecesi", "Bahar Senligi", "Mezuniyet Toreni", "Takim Yemegi", "Haftasonu Kacamagi",
            "Doga Yuruyusu", "Konferans Sunumu", "Sertifika Toreni", "Design Thinking Workshop", "Ofis Manzarasi",
            "Evden Calisma Modu", "Yaz Kampi", "Kis Tatili", "Sabah Kosusu", "Kitap ve Kahve",
            "Konser Coskusu", "Gala Gecesi", "Kod Inceleme (Code Review)", "Proje Lansmani", "Musteri Ziyareti",
            "Agile Sprint Planning", "Server Odasi", "Yeni Ofis Masam", "Kedim ve Kod", "Gun Batimi"
        };

        private static readonly string[] _photoTags = {
            "tech", "office", "event", "casual", "nature", "work", "friends", "study", "travel", "art"
        };

        // Gemini 3.1 Pro tarafindan uretilmis statik Event listesi (Ornek 20 Etkinlik)
        private static readonly string[] _eventNames = {
            "Global AI Summit 2026", "Web Summit Europe", "React Developer Conf", "DotNet Days", "Cloud Native Meetup",
            "Data Science Bootcamp", "UX/UI Masterclass", "Startup Weekend", "Cyber Security Expo", "Blockchain Workshop",
            "Tech Career Fair", "Open Source Festival", "Women in Tech", "Game Developers Conference", "IoT Innovators",
            "Fintech Revolution", "Agile Leadership Summit", "Mobile App Developers Meetup", "Deep Learning Symposium", "Tech Makers Hackathon"
        };

        private static readonly string[] _eventLocations = {
            "Istanbul", "Ankara", "Izmir", "London", "Berlin", "San Francisco", "Online", "New York", "Amsterdam", "Paris"
        };

        public (List<NodeDto> Nodes, List<EdgeDto> Edges) GenerateDenseGraph()
        {
            var nodes = GenerateNodes();
            var edges = new List<EdgeDto>();
            
            // Dense: Iliski yogunlugu %15-%20 seviyesine cekildi (Performans optimizasyonu)
            var users = nodes.FindAll(n => n.Type == "User");
            for (int i = 0; i < users.Count; i++)
            {
                for (int j = i + 1; j < users.Count; j++)
                {
                    if (_rnd.NextDouble() < 0.15)
                    {
                        edges.Add(CreateEdge(users[i].Id, users[j].Id, "FRIEND", false));
                        edges.Add(CreateEdge(users[j].Id, users[i].Id, "FRIEND", false));
                    }
                }
            }

            GenerateCommonInteractions(nodes, edges);
            return (nodes, edges);
        }

        public (List<NodeDto> Nodes, List<EdgeDto> Edges) GenerateSparseGraph()
        {
            var nodes = GenerateNodes();
            var edges = new List<EdgeDto>();
            
            // Sparse: Kisi basina en fazla 1-3 arkadas
            var users = nodes.FindAll(n => n.Type == "User");
            foreach (var user in users)
            {
                int numFriends = _rnd.Next(1, 4);
                for (int i = 0; i < numFriends; i++)
                {
                    var friend = users[_rnd.Next(users.Count)];
                    if (friend.Id != user.Id)
                    {
                        edges.Add(CreateEdge(user.Id, friend.Id, "FRIEND", false));
                        edges.Add(CreateEdge(friend.Id, user.Id, "FRIEND", false));
                    }
                }
            }

            GenerateCommonInteractions(nodes, edges, likelihood: 0.1);
            return (nodes, edges);
        }

        public (List<NodeDto> Nodes, List<EdgeDto> Edges) GenerateStarGraph()
        {
            var nodes = GenerateNodes();
            var edges = new List<EdgeDto>();
            
            var users = nodes.FindAll(n => n.Type == "User");
            if (users.Count > 0)
            {
                var influencer = users[0]; // Merkezdeki yildiz
                for (int i = 1; i < users.Count; i++)
                {
                    // Herkes influencera bagli
                    edges.Add(CreateEdge(users[i].Id, influencer.Id, "FRIEND", false));
                    edges.Add(CreateEdge(influencer.Id, users[i].Id, "FRIEND", false));
                }
            }

            GenerateCommonInteractions(nodes, edges);
            return (nodes, edges);
        }

        public (List<NodeDto> Nodes, List<EdgeDto> Edges) GenerateChainGraph()
        {
            var nodes = GenerateNodes();
            var edges = new List<EdgeDto>();
            
            var users = nodes.FindAll(n => n.Type == "User");
            // A -> B -> C -> D zinciri
            for (int i = 0; i < users.Count - 1; i++)
            {
                edges.Add(CreateEdge(users[i].Id, users[i+1].Id, "FRIEND", false));
                edges.Add(CreateEdge(users[i+1].Id, users[i].Id, "FRIEND", false));
            }

            GenerateCommonInteractions(nodes, edges, 0.2);
            return (nodes, edges);
        }

        private List<NodeDto> GenerateNodes()
        {
            var nodes = new List<NodeDto>();

            // 50 User uret
            for (int i = 0; i < _userNames.Length; i++)
            {
                nodes.Add(new NodeDto
                {
                    Id = $"user{i + 1}",
                    Type = "User",
                    Properties = new Dictionary<string, object>
                    {
                        { "Name", _userNames[i] },
                        { "Age", _rnd.Next(18, 60).ToString() },
                        { "Profession", _professions[_rnd.Next(_professions.Length)] }
                    }
                });
            }

            // 30 Photo uret
            for (int i = 0; i < _photoTitles.Length; i++)
            {
                nodes.Add(new NodeDto
                {
                    Id = $"photo{i + 1}",
                    Type = "Photo",
                    Properties = new Dictionary<string, object>
                    {
                        { "Title", _photoTitles[i] },
                        { "Tag", _photoTags[_rnd.Next(_photoTags.Length)] },
                        { "CreatedAt", DateTime.Now.AddDays(-_rnd.Next(1, 300)).ToString("yyyy-MM-dd") }
                    }
                });
            }

            // 20 Event uret
            for (int i = 0; i < _eventNames.Length; i++)
            {
                nodes.Add(new NodeDto
                {
                    Id = $"event{i + 1}",
                    Type = "Event",
                    Properties = new Dictionary<string, object>
                    {
                        { "Name", _eventNames[i] },
                        { "Location", _eventLocations[_rnd.Next(_eventLocations.Length)] },
                        { "Date", DateTime.Now.AddDays(_rnd.Next(1, 100)).ToString("yyyy-MM-dd") }
                    }
                });
            }

            return nodes;
        }

        private void GenerateCommonInteractions(List<NodeDto> nodes, List<EdgeDto> edges, double likelihood = 0.1)
        {
            var users = nodes.FindAll(n => n.Type == "User");
            var photos = nodes.FindAll(n => n.Type == "Photo");
            var events = nodes.FindAll(n => n.Type == "Event");

            // Post & Like Photos
            foreach (var photo in photos)
            {
                var owner = users[_rnd.Next(users.Count)];
                edges.Add(CreateEdge(owner.Id, photo.Id, "POSTED", true));

                foreach (var user in users)
                {
                    if (user.Id != owner.Id && _rnd.NextDouble() < likelihood)
                    {
                        edges.Add(CreateEdge(user.Id, photo.Id, "LIKES", true));
                    }
                }
            }

            // Attends Events
            foreach (var ev in events)
            {
                foreach (var user in users)
                {
                    if (_rnd.NextDouble() < likelihood)
                    {
                        edges.Add(CreateEdge(user.Id, ev.Id, "ATTENDS", true));
                    }
                }
            }
        }

        public (List<NodeDto> Nodes, List<EdgeDto> Edges) GenerateIncrementalData(int nodeCount, int edgeCount)
        {
            var nodes = new List<NodeDto>();
            var edges = new List<EdgeDto>();

            // 1. Yeni Dugumlerin Uretilmesi
            for (int i = 0; i < nodeCount; i++)
            {
                int typeSelector = _rnd.Next(3);
                NodeDto newNode;
                if (typeSelector == 0) // New User
                {
                    newNode = new NodeDto
                    {
                        Id = $"user_new_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
                        Type = "User",
                        Properties = new Dictionary<string, object>
                        {
                            { "Name", _userNames[_rnd.Next(_userNames.Length)] + " (Sim)" },
                            { "Age", _rnd.Next(18, 60).ToString() },
                            { "Profession", _professions[_rnd.Next(_professions.Length)] }
                        }
                    };
                }
                else if (typeSelector == 1) // New Photo
                {
                    newNode = new NodeDto
                    {
                        Id = $"photo_new_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
                        Type = "Photo",
                        Properties = new Dictionary<string, object>
                        {
                            { "Title", _photoTitles[_rnd.Next(_photoTitles.Length)] + " (Sim)" },
                            { "Tag", _photoTags[_rnd.Next(_photoTags.Length)] },
                            { "CreatedAt", DateTime.Now.ToString("yyyy-MM-dd") }
                        }
                    };
                }
                else // New Event
                {
                    newNode = new NodeDto
                    {
                        Id = $"event_new_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
                        Type = "Event",
                        Properties = new Dictionary<string, object>
                        {
                            { "Name", _eventNames[_rnd.Next(_eventNames.Length)] + " (Sim)" },
                            { "Location", _eventLocations[_rnd.Next(_eventLocations.Length)] },
                            { "Date", DateTime.Now.AddDays(_rnd.Next(1, 30)).ToString("yyyy-MM-dd") }
                        }
                    };
                }
                nodes.Add(newNode);

                // 2. Yeni Dugum Iliskilerinin Kurulmasi
                int newRelCount = _rnd.Next(1, 3);
                for (int j = 0; j < newRelCount; j++)
                {
                    string targetId, relType;
                    bool isDirected;

                    if (newNode.Type == "User")
                    {
                        targetId = $"user{_rnd.Next(1, 51)}";
                        relType = "FRIEND";
                        isDirected = false;
                    }
                    else if (newNode.Type == "Photo")
                    {
                        targetId = $"user{_rnd.Next(1, 51)}"; // Foto sahibi
                        relType = "POSTED";
                        isDirected = true;
                        // Yer degistir: User -> Photo
                        edges.Add(CreateEdge(targetId, newNode.Id, relType, isDirected, true));
                        continue;
                    }
                    else // Event
                    {
                        targetId = $"user{_rnd.Next(1, 51)}"; // Katilimci
                        relType = "ATTENDS";
                        isDirected = true;
                        // Yer degistir: User -> Event
                        edges.Add(CreateEdge(targetId, newNode.Id, relType, isDirected, true));
                        continue;
                    }

                    if (newNode.Id != targetId)
                    {
                        edges.Add(CreateEdge(newNode.Id, targetId, relType, isDirected, true));
                        if (!isDirected) edges.Add(CreateEdge(targetId, newNode.Id, relType, isDirected, true));
                    }
                }
            }

            // 3. Mevcut Dugumler Arasi Iliskiler
            int existingRelCount = _rnd.Next(1, 5);
            for (int i = 0; i < existingRelCount; i++)
            {
                int relSelector = _rnd.Next(2); // Sadece FRIEND veya LIKES (Daha dogal artis)
                string sourceId, targetId, relType;
                bool isDirected;

                if (relSelector == 0) // FRIEND
                {
                    sourceId = $"user{_rnd.Next(1, 51)}";
                    targetId = $"user{_rnd.Next(1, 51)}";
                    relType = "FRIEND";
                    isDirected = false;
                }
                else // LIKES
                {
                    sourceId = $"user{_rnd.Next(1, 51)}";
                    targetId = $"photo{_rnd.Next(1, 31)}";
                    relType = "LIKES";
                    isDirected = true;
                }

                if (sourceId != targetId)
                {
                    edges.Add(CreateEdge(sourceId, targetId, relType, isDirected, true));
                    if (!isDirected) edges.Add(CreateEdge(targetId, sourceId, relType, isDirected, true));
                }
            }

            return (nodes, edges);
        }

        private EdgeDto CreateEdge(string source, string target, string relation, bool isDirected, bool isSimulated = false)
        {
            return new EdgeDto
            {
                Id = $"edge_{Guid.NewGuid().ToString("N").Substring(0, 8)}",
                SourceId = source,
                TargetId = target,
                RelationType = relation,
                IsDirected = isDirected,
                Properties = new Dictionary<string, object>
                {
                    { "Timestamp", DateTime.UtcNow.ToString("O") },
                    { "isSimulated", isSimulated }
                }
            };
        }
    }
}
