using gizindir.model;
using Npgsql;
using System;
using System.Collections.Generic;
using BCrypt.Net;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace gizindir.data
{
    public class UserRepository
    {
        public List<UserModel> GetAllUsers()
        {
            var users = new List<UserModel>();
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"
                    SELECT id, full_name, email, password_hash, gender, interested_in, 
                           birth_date, bio, profile_image_url, created_at 
                    FROM users", conn);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new UserModel
                    {
                        Id = reader.GetInt32(0),
                        FullName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Email = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        PasswordHash = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        Gender = reader.IsDBNull(4) ? "" : reader.GetString(4),
                        InterestedIn = reader.IsDBNull(5) ? "" : reader.GetString(5),
                        BirthDate = reader.IsDBNull(6) ? default(DateTime) : reader.GetDateTime(6),
                        Bio = reader.IsDBNull(7) ? "" : reader.GetString(7),
                        ProfileImageUrl = reader.IsDBNull(8) ? "" : reader.GetString(8),
                        CreatedAt = reader.IsDBNull(9) ? default(DateTime) : reader.GetDateTime(9)
                    });
                }
            }
            MessageBox.Show($"Toplam {users.Count} kullanıcı bulundu.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return users;
        }



public List<UserModel> GetUnseenUsers(string currentUserEmail)
{
    var users = new List<UserModel>();
    using (var conn = DbContext.GetConnection())
    {
        conn.Open();
        var cmd = new NpgsqlCommand(@"
            SELECT u.id, u.full_name, u.email, u.password_hash, u.gender, 
                   u.interested_in, u.birth_date, u.bio, u.profile_image_url, u.created_at
            FROM users u
            WHERE u.email != @currentEmail
            AND NOT EXISTS (
                SELECT 1 FROM user_interactions ui 
                WHERE ui.user_email = @currentEmail 
                AND ui.shown_user_email = u.email
            )", conn);

        cmd.Parameters.AddWithValue("currentEmail", currentUserEmail);

        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            users.Add(new UserModel
            {
                Id = reader.GetInt32(0),
                FullName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                Email = reader.IsDBNull(2) ? "" : reader.GetString(2),
                PasswordHash = reader.IsDBNull(3) ? "" : reader.GetString(3),
                Gender = reader.IsDBNull(4) ? "" : reader.GetString(4),
                InterestedIn = reader.IsDBNull(5) ? "" : reader.GetString(5),
                BirthDate = reader.IsDBNull(6) ? default(DateTime) : reader.GetDateTime(6),
                Bio = reader.IsDBNull(7) ? "" : reader.GetString(7),
                ProfileImageUrl = reader.IsDBNull(8) ? "" : reader.GetString(8),
                CreatedAt = reader.IsDBNull(9) ? default(DateTime) : reader.GetDateTime(9)
            });
        }
    }
    return users;
}

public void RecordUserInteraction(string userEmail, string shownUserEmail, bool isLiked)
{
    using (var conn = DbContext.GetConnection())
    {
        conn.Open();
        var cmd = new NpgsqlCommand(@"
            INSERT INTO user_interactions (user_email, shown_user_email, is_liked)
            VALUES (@userEmail, @shownUserEmail, @isLiked)
            ON CONFLICT (user_email, shown_user_email) 
            DO UPDATE SET is_liked = @isLiked", conn);

        cmd.Parameters.AddWithValue("userEmail", userEmail);
        cmd.Parameters.AddWithValue("shownUserEmail", shownUserEmail);
        cmd.Parameters.AddWithValue("isLiked", isLiked);
        
        cmd.ExecuteNonQuery();

        // Eğer like yapıldıysa match kontrolü yap
        if (isLiked)
        {
            var matchCmd = new NpgsqlCommand(@"
                SELECT EXISTS (
                    SELECT 1 FROM user_interactions
                    WHERE user_email = @shownUserEmail
                    AND shown_user_email = @userEmail
                    AND is_liked = true
                )", conn);

            matchCmd.Parameters.AddWithValue("userEmail", userEmail);
            matchCmd.Parameters.AddWithValue("shownUserEmail", shownUserEmail);

            bool isMatch = (bool)matchCmd.ExecuteScalar();
            if (isMatch)
            {
                MessageBox.Show("Tebrikler! Bir eşleşme buldunuz!", "Eşleşme", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
        public UserModel GetUserByEmail(string email)
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"
                    SELECT id, full_name, email, password_hash, gender, interested_in, 
                           birth_date, bio, profile_image_url, created_at 
                    FROM users WHERE email = @e", conn);
                cmd.Parameters.AddWithValue("e", email);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new UserModel
                    {
                        Id = reader.GetInt32(0),
                        FullName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Email = reader.GetString(2),
                        PasswordHash = reader.GetString(3),
                        Gender = reader.IsDBNull(4) ? "" : reader.GetString(4),
                        InterestedIn = reader.IsDBNull(5) ? "" : reader.GetString(5),
                        BirthDate = reader.IsDBNull(6) ? default(DateTime) : reader.GetDateTime(6),
                        Bio = reader.IsDBNull(7) ? "" : reader.GetString(7),
                        ProfileImageUrl = reader.IsDBNull(8) ? "" : reader.GetString(8),
                        CreatedAt = reader.IsDBNull(9) ? default(DateTime) : reader.GetDateTime(9)
                    };
                }
            }
            return null;
        }

        public void AddUser(UserModel user)
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"
                    INSERT INTO users 
                    (email, password, password_hash, created_at) 
                    VALUES (@e, @p, @ph, @c)", conn);

                // Hash the password before saving
                string password_hashed = BCrypt.Net.BCrypt.HashPassword(user.Password);

                cmd.Parameters.AddWithValue("e", user.Email);
                cmd.Parameters.AddWithValue("p", user.Password);
                cmd.Parameters.AddWithValue("ph", password_hashed);
                cmd.Parameters.AddWithValue("c", user.CreatedAt);
                cmd.ExecuteNonQuery();

                // Kullanıcıyı kayıt ettikten sonra oturumu kaydet
                System.IO.File.WriteAllText("user_session.txt", user.Email);
            }
        }

        public void UpdateUserProfile(UserModel user)
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"
                    UPDATE users SET 
                        full_name = @fn,
                        gender = @g,
                        interested_in = @i,
                        birth_date = @b,
                        bio = @bio,
                        profile_image_url = @img
                    WHERE email = @e", conn);

                cmd.Parameters.AddWithValue("fn", user.FullName ?? "");
                cmd.Parameters.AddWithValue("g", user.Gender ?? "");
                cmd.Parameters.AddWithValue("i", user.InterestedIn ?? "");
                cmd.Parameters.AddWithValue("b", user.BirthDate);
                cmd.Parameters.AddWithValue("bio", user.Bio ?? "");
                cmd.Parameters.AddWithValue("img", user.ProfileImageUrl ?? "");
                cmd.Parameters.AddWithValue("e", user.Email);

                cmd.ExecuteNonQuery();
            }
        }

        public bool IsProfileCompleted(string email)
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"
                    SELECT full_name, gender, interested_in, birth_date, bio, profile_image_url
                    FROM users WHERE email = @e", conn);
                cmd.Parameters.AddWithValue("e", email);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Eğer herhangi biri boşsa, profil tamamlanmamış demektir
                    return !(reader.IsDBNull(0) || reader.IsDBNull(1) || reader.IsDBNull(2)
                             || reader.IsDBNull(3) || reader.IsDBNull(4) || reader.IsDBNull(5));
                }
            }
            return false;
        }

        public string CreateSession(int userId)
        {
            string token = Guid.NewGuid().ToString();

            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand("INSERT INTO sessions (user_id, session_token) VALUES (@uid, @token)", conn);
                cmd.Parameters.AddWithValue("uid", userId);
                cmd.Parameters.AddWithValue("token", token);
                cmd.ExecuteNonQuery();
            }

            return token;
        }

        public UserModel GetUserBySessionToken(string token)
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"
                    SELECT u.id, u.full_name, u.email, u.password_hash
                    FROM sessions s
                    JOIN users u ON s.user_id = u.id
                    WHERE s.session_token = @token", conn);
                cmd.Parameters.AddWithValue("token", token);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new UserModel
                    {
                        Id = reader.GetInt32(0),
                        FullName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Email = reader.GetString(2),
                        PasswordHash = reader.GetString(3)
                    };
                }
            }
            return null;
        }

        // Beğenme işlemleri için yeni metodlar
        public void AddLike(int fromUserId, int toUserId, bool isLike)
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"
                    INSERT INTO likes (from_user_id, to_user_id, is_like, created_at) 
                    VALUES (@from, @to, @like, @created)
                    ON CONFLICT (from_user_id, to_user_id) 
                    DO UPDATE SET is_like = @like, created_at = @created", conn);

                cmd.Parameters.AddWithValue("from", fromUserId);
                cmd.Parameters.AddWithValue("to", toUserId);
                cmd.Parameters.AddWithValue("like", isLike);
                cmd.Parameters.AddWithValue("created", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }

        public bool CheckForMatch(int userId1, int userId2)
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand(@"
                    SELECT COUNT(*) FROM likes 
                    WHERE ((from_user_id = @u1 AND to_user_id = @u2) OR 
                           (from_user_id = @u2 AND to_user_id = @u1)) 
                    AND is_like = true", conn);

                cmd.Parameters.AddWithValue("u1", userId1);
                cmd.Parameters.AddWithValue("u2", userId2);

                int count = (int)(long)cmd.ExecuteScalar();
                return count == 2; // Her iki kullanıcı da birbirini beğendiyse match
            }
        }
    }
}