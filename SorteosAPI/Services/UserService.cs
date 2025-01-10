using Microsoft.Data.SqlClient;
using SorteosAPI.Models;
using System.Data;


namespace SorteosAPI.Services
{
    public class UserService : IUserService
    {
        private readonly string _connectionString;

        public UserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString), "La cadena de conexión no puede ser nula.");
        }

        public async Task<bool> CreateUserAsync(UserCreate userCreate)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("CreateUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@IdClient", userCreate.IdClient);
                        command.Parameters.AddWithValue("@Name", userCreate.Name);
                        command.Parameters.AddWithValue("@IsActive", userCreate.IsActive);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al crear el usuario.", ex);
            }
        }

        public async Task<List<User>> GetUsersByClientIdAsync(string? name = null)
        {
            var users = new List<User>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = "SELECT IdUser, IdClient, Name, CreatedAt, UpdatedAt, IsActive FROM Users";
                    if (!string.IsNullOrEmpty(name))
                    {
                        query += " WHERE Name LIKE @Name";
                    }

                    using (var command = new SqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            command.Parameters.AddWithValue("@Name", $"%{name}%");
                        }

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var user = new User
                                {
                                    IdUser = reader.GetInt32(reader.GetOrdinal("IdUser")),
                                    IdClient = reader.GetInt32(reader.GetOrdinal("IdClient")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                                };
                                users.Add(user);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al obtener los usuarios.", ex);
            }

            return users;
        }

        public async Task<bool> ClientExistsAsync(int idClient)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = "SELECT COUNT(1) FROM Clients WHERE IdClient = @IdClient";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdClient", idClient);

                        var result = (int)await command.ExecuteScalarAsync();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar la existencia del cliente.", ex);
            }
        }

        public async Task<bool> UserExistsAsync(int idClient, string name)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = "SELECT COUNT(1) FROM Users WHERE IdClient = @IdClient AND Name = @Name";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdClient", idClient);
                        command.Parameters.AddWithValue("@Name", name);

                        var result = (int)await command.ExecuteScalarAsync();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar la existencia del usuario.", ex);
            }
        }
    }
}

