using gizindir.model;
using Npgsql;
using System;
using System.Collections.Generic;

namespace gizindir.data
{
    public class MatchRepository
    {
        public List<Match> GetMatchesByEmail(string email)
        {
            var matches = new List<Match>();
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                
                // Bu sorgu, hem user_interactions tablosundan eşleşmeleri alır
                var cmd = new NpgsqlCommand(@"
                    SELECT u1.id, u1.email, u1.full_name, u2.id, u2.email, u2.full_name,
                           ui1.created_at as matched_at
                    FROM user_interactions ui1
                    JOIN user_interactions ui2 ON ui1.user_email = ui2.shown_user_email 
                                              AND ui1.shown_user_email = ui2.user_email
                    JOIN users u1 ON ui1.user_email = u1.email
                    JOIN users u2 ON ui1.shown_user_email = u2.email
                    WHERE (ui1.user_email = @email OR ui1.shown_user_email = @email)
                    AND ui1.is_liked = true AND ui2.is_liked = true
                ", conn);

                cmd.Parameters.AddWithValue("email", email);
                
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int user1Id = reader.GetInt32(0);
                        string user1Email = reader.GetString(1);
                        string user1Name = reader.IsDBNull(2) ? "" : reader.GetString(2);
                        
                        int user2Id = reader.GetInt32(3);
                        string user2Email = reader.GetString(4);
                        string user2Name = reader.IsDBNull(5) ? "" : reader.GetString(5);
                        
                        DateTime matchedAt = reader.GetDateTime(6);
                        
                        // Eğer mevcut kullanıcı user1 ise, eşleşme user2 ile
                        // Eğer mevcut kullanıcı user2 ise, eşleşme user1 ile
                        if (user1Email == email)
                        {
                            matches.Add(new Match
                            {
                                Id = 0, // Veritabanında match tablosu olmadığı için ID 0
                                User1Id = user1Id,
                                User2Id = user2Id,
                                MatchedAt = matchedAt,
                                MatchedUserName = user2Name,
                                MatchedUserEmail = user2Email
                            });
                        }
                        else
                        {
                            matches.Add(new Match
                            {
                                Id = 0, // Veritabanında match tablosu olmadığı için ID 0
                                User1Id = user2Id,
                                User2Id = user1Id,
                                MatchedAt = matchedAt,
                                MatchedUserName = user1Name,
                                MatchedUserEmail = user1Email
                            });
                        }
                    }
                }
            }
            
            return matches;
        }
        
        public bool IsMatch(string userEmail1, string userEmail2)
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"
                    SELECT EXISTS (
                        SELECT 1 FROM user_interactions ui1
                        JOIN user_interactions ui2 ON ui1.user_email = ui2.shown_user_email
                                                 AND ui1.shown_user_email = ui2.user_email
                        WHERE ui1.user_email = @email1
                        AND ui1.shown_user_email = @email2
                        AND ui1.is_liked = true
                        AND ui2.is_liked = true
                    )", conn);

                cmd.Parameters.AddWithValue("email1", userEmail1);
                cmd.Parameters.AddWithValue("email2", userEmail2);

                return (bool)cmd.ExecuteScalar();
            }
        }
        
        public void AddMatch(string userEmail1, string userEmail2)
        {
            // Bu metod, ileride gerçek bir matches tablosu oluşturulduğunda kullanılabilir
            // Şu an için user_interactions tablosu ile eşleşmeler yönetiliyor
            
            // Örnek implementasyon:
            /*
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"
                    INSERT INTO matches (user1_email, user2_email, matched_at)
                    VALUES (@email1, @email2, @matched_at)
                    ON CONFLICT (user1_email, user2_email) DO NOTHING
                ", conn);

                cmd.Parameters.AddWithValue("email1", userEmail1);
                cmd.Parameters.AddWithValue("email2", userEmail2);
                cmd.Parameters.AddWithValue("matched_at", DateTime.Now);
                
                cmd.ExecuteNonQuery();
            }
            */
        }
    }
}
