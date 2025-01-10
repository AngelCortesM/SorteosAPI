using Microsoft.Data.SqlClient;

using SorteosAPI.Models;

using System.Data;

namespace SorteosAPI.Services
{
    public class ClientService : IClientService
    {
        private readonly string _connectionString;

        public ClientService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString), "La cadena de conexión no puede ser nula.");
        }

        public async Task<bool> CreateClientAsync(ClientCreate clientCreate)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("CreateClient", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Name", clientCreate.Name);
                        command.Parameters.AddWithValue("@IsActive", clientCreate.IsActive);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al crear el cliente.", ex);
            }
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            var clients = new List<Client>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("GetAllClients", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var client = new Client
                                {
                                    IdClient = reader.IsDBNull(reader.GetOrdinal("IdClient")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdClient")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                    UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                                };

                                clients.Add(client);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al obtener los clientes.", ex);
            }

            return clients;
        }
    }
}