using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gizindir.model
{
    public class Match
    {
        public int Id { get; set; }
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public DateTime MatchedAt { get; set; }
        
        // Eşleşilen kullanıcının bilgileri
        public string MatchedUserName { get; set; } = "";
        public string MatchedUserEmail { get; set; } = "";
        
        // Dinamik chat modeli için kullanılacak özellikler (UI için)
        public string LastMessage { get; set; } = "";
        public DateTime? LastMessageTime { get; set; }
        public bool IsOnline { get; set; }
        public int UnreadCount { get; set; }
    }
}
