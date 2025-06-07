using gizindir.model;
using Npgsql;
using System;
using System.Collections.Generic;
using BCrypt.Net;
using System.Configuration;
using System.IO;

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
                var cmd = new NpgsqlCommand("SELECT id, full_name, email, password_hash FROM users", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new UserModel
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Email = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        PasswordHash = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    });
                }
            }
            return users;
        }


        public UserModel GetUserByEmail(string email)
        {
            using (var conn = DbContext.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand("SELECT id, name, email, password_hash FROM users WHERE email = @e", conn);
                cmd.Parameters.AddWithValue("e", email);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new UserModel
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Email = reader.GetString(2),
                        Password = reader.GetString(3)
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
                cmd.Parameters.AddWithValue("ph",password_hashed);
                cmd.Parameters.AddWithValue("c", user.CreatedAt);
                cmd.ExecuteNonQuery();

                // Kullanıcıyı kayıt ettikten sonra oturumu kaydet
                System.IO.File.WriteAllText("user_session.txt", user.Email);  // veya user.Id

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
            SELECT u.id, u.name, u.email, u.password_hash
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
                        Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Email = reader.GetString(2),
                        Password = reader.GetString(3)
                    };
                }
            }
            return null;
        }




    }
}
