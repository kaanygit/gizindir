using gizindir.model;
using Npgsql;
using System;
using System.Collections.Generic;
using BCrypt.Net;
using System.Configuration;

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
                        Name = reader.GetString(1),
                        Email = reader.GetString(2),
                        Password = reader.GetString(3)
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
                var cmd = new NpgsqlCommand("SELECT id, name, email, password FROM users WHERE email = @e", conn);
                cmd.Parameters.AddWithValue("e", email);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new UserModel
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
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
            }
        }

    }
}
