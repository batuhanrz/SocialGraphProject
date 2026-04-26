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
            "Batuhan Yılmaz", "Fatma Sude Kaya", "Muhammed Furkan Çelik", "İsra Nur Demir", "Özcan Şahin",
            "Ahmet Yılmaz", "Ayşe Demir", "Mehmet Kaya", "Fatma Çelik", "Mustafa Şahin",
            "Zeynep Koç", "Ali Öztürk", "Elif Aydın", "Hüseyin Özdemir", "Merve Arslan",
            "Hasan Doğan", "Esra Kılıç", "İbrahim Çetin", "Büşra Gürbüz", "Halil Gök",
            "Burcu Tekin", "Kemal Polat", "Selin Tarhan", "Caner Bulut", "Eda Yıldırım",
            "Tolga Çoban", "Cemre Yıldız", "Emre Karaca", "Derya Çakır", "Sinan Taş",
            "Gözde Akın", "Turgut Aslan", "Işıl Erdoğan", "Oğuzhan Güneş", "Tuğçe Yavuz",
            "Gökhan Kaplan", "Müge Çelik", "Serkan Yücel", "Deniz Özer", "Umut Ekinci",
            "Aslıhan Kara", "Koray Avcı", "Zehra Başar", "Orhan Veli", "Nazlı Çam",
            "Eren Yalçın", "Yasemin Kurt", "Volkan Türk", "Ceren Yılmaz", "Kerem Koca"
        };

        private static readonly string[] _professions = {
            "Software Engineer", "Data Scientist", "UI/UX Designer", "Product Manager", "DevOps Engineer",
            "Marketing Specialist", "Graphic Designer", "Project Manager", "Business Analyst", "CEO"
        };

        // Gemini 3.1 Pro tarafindan uretilmis statik Photo listesi (Ornek 30 Fotograf)
        private static readonly string[] _photoTitles = {
            "Hackathon Hatırası", "Ofiste İlk Gün", "Yapay Zeka Zirvesi", "Kahve Molası", "Yeni Proje Toplantısı",
            "Kodlama Gecesi", "Bahar Şenliği", "Mezuniyet Töreni", "Takım Yemeği", "Haftasonu Kaçamağı",
            "Doğa Yürüyüşü", "Konferans Sunumu", "Sertifika Töreni", "Design Thinking Workshop", "Ofis Manzarası",
            "Evden Çalışma Modu", "Yaz Kampı", "Kış Tatili", "Sabah Koşusu", "Kitap ve Kahve",
            "Konser Coşkusu", "Gala Gecesi", "Kod İnceleme (Code Review)", "Proje Lansmanı", "Müşteri Ziyareti",
            "Agile Sprint Planning", "Server Odası", "Yeni Ofis Masam", "Kedim ve Kod", "Gün Batımı"
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
            
            // Dense: Neredeyse herkes herkesle arkadas (Yogunluk %30-%40 arasi)
            var users = nodes.FindAll(n => n.Type == "User");
            for (int i = 0; i < users.Count; i++)
            {
                for (int j = i + 1; j < users.Count; j++)
                {
                    if (_rnd.NextDouble() < 0.35)
                    {
                        edges.Add(CreateEdge(users[i].Id, users[j].Id, "FRIEND", false));
                        edges.Add(CreateEdge(users[j].Id, users[i].Id, "FRIEND", false)); // Iki yonlu arkadaslik simulasyonu (cunku API'ye tek tek post edilecek)
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

        private void GenerateCommonInteractions(List<NodeDto> nodes, List<EdgeDto> edges, double likelihood = 0.3)
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

        private EdgeDto CreateEdge(string source, string target, string relation, bool isDirected)
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
                    { "Timestamp", DateTime.UtcNow.ToString("O") }
                }
            };
        }
    }
}
