using gizindir.model;
using Npgsql;
using System;
using System.Collections.Generic;

namespace gizindir.data
{
    public class MessageRepository
    {
        public List<Message> GetMessagesBetweenUsers(string userEmail1, string userEmail2)
        {
            var messages = new List<Message>();
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"
                    SELECT m.id, m.sender_id, m.receiver_id, m.content, m.sent_at, m.is_read
                    FROM messages m
                    JOIN users u1 ON m.sender_id = u1.id
                    JOIN users u2 ON m.receiver_id = u2.id
                    WHERE (u1.email = @email1 AND u2.email = @email2)
                    OR (u1.email = @email2 AND u2.email = @email1)
                    ORDER BY m.sent_at ASC
                ", conn);

                cmd.Parameters.AddWithValue("email1", userEmail1);
                cmd.Parameters.AddWithValue("email2", userEmail2);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        messages.Add(new Message
                        {
                            Id = reader.GetInt32(0),
                            SenderId = reader.GetInt32(1),
                            ReceiverId = reader.GetInt32(2),
                            Content = reader.GetString(3),
                            SentAt = reader.GetDateTime(4),
                            IsRead = reader.GetBoolean(5)
                        });
                    }
                }
            }
            
            return messages;
        }
        
        public void SendMessage(int senderId, int receiverId, string content)
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"
                    INSERT INTO messages (sender_id, receiver_id, content, sent_at, is_read)
                    VALUES (@sender_id, @receiver_id, @content, @sent_at, @is_read)
                ", conn);

                cmd.Parameters.AddWithValue("sender_id", senderId);
                cmd.Parameters.AddWithValue("receiver_id", receiverId);
                cmd.Parameters.AddWithValue("content", content);
                cmd.Parameters.AddWithValue("sent_at", DateTime.Now);
                cmd.Parameters.AddWithValue("is_read", false);

                cmd.ExecuteNonQuery();
            }
        }
        
        public void MarkAsRead(int messageId)
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"
                    UPDATE messages 
                    SET is_read = true
                    WHERE id = @id
                ", conn);

                cmd.Parameters.AddWithValue("id", messageId);
                cmd.ExecuteNonQuery();
            }
        }
        
        public void MarkAllAsRead(int senderId, int receiverId)
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"
                    UPDATE messages 
                    SET is_read = true
                    WHERE sender_id = @sender_id AND receiver_id = @receiver_id
                ", conn);

                cmd.Parameters.AddWithValue("sender_id", senderId);
                cmd.Parameters.AddWithValue("receiver_id", receiverId);
                cmd.ExecuteNonQuery();
            }
        }
        
        public Message GetLastMessage(string userEmail1, string userEmail2)
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"
                    SELECT m.id, m.sender_id, m.receiver_id, m.content, m.sent_at, m.is_read
                    FROM messages m
                    JOIN users u1 ON m.sender_id = u1.id
                    JOIN users u2 ON m.receiver_id = u2.id
                    WHERE (u1.email = @email1 AND u2.email = @email2)
                    OR (u1.email = @email2 AND u2.email = @email1)
                    ORDER BY m.sent_at DESC
                    LIMIT 1
                ", conn);

                cmd.Parameters.AddWithValue("email1", userEmail1);
                cmd.Parameters.AddWithValue("email2", userEmail2);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Message
                        {
                            Id = reader.GetInt32(0),
                            SenderId = reader.GetInt32(1),
                            ReceiverId = reader.GetInt32(2),
                            Content = reader.GetString(3),
                            SentAt = reader.GetDateTime(4),
                            IsRead = reader.GetBoolean(5)
                        };
                    }
                }
            }
            
            return null;
        }
        
        public int GetUnreadCount(int receiverId, int senderId)
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"
                    SELECT COUNT(*) 
                    FROM messages
                    WHERE receiver_id = @receiver_id 
                    AND sender_id = @sender_id
                    AND is_read = false
                ", conn);

                cmd.Parameters.AddWithValue("receiver_id", receiverId);
                cmd.Parameters.AddWithValue("sender_id", senderId);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
}
