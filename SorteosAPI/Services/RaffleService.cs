using Microsoft.Data.SqlClient;
using SorteosAPI.Models;
using System.Data;

namespace SorteosAPI.Services
{
    public class RaffleService : IRaffleService
    {
        private readonly string _connectionString;

        public RaffleService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString), "La cadena de conexión no puede ser nula.");
        }

        public async Task<bool> CreateRaffleAsync(RaffleCreate raffleCreate)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("CreateRaffle", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Name", raffleCreate.Name);
                        command.Parameters.AddWithValue("@IsActive", raffleCreate.IsActive);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al crear el sorteo.", ex);
            }
        }

        public async Task<List<Raffle>> GetAllRafflesAsync()
        {
            var raffles = new List<Raffle>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = @"
                        SELECT
                            r.IdRaffle,
                            ISNULL(rc.IdClient, 0) AS IdClient,
                            r.Name,
                            r.CreatedAt,
                            r.UpdatedAt,
                            r.IsActive
                        FROM
                            Raffles r
                        LEFT JOIN
                            RaffleByClient rc ON r.IdRaffle = rc.IdRaffle";

                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var raffle = new Raffle
                                {
                                    IdRaffle = reader.GetInt32(reader.GetOrdinal("IdRaffle")),
                                    IdClient = reader.GetInt32(reader.GetOrdinal("IdClient")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                                };
                                raffles.Add(raffle);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al obtener los sorteos.", ex);
            }

            return raffles;
        }

        public async Task<bool> RaffleExistsAsync(string name)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = "SELECT COUNT(1) FROM Raffles WHERE Name = @Name";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);

                        var result = (int?)(await command.ExecuteScalarAsync());
                        return result.HasValue && result.Value > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar la existencia del sorteo.", ex);
            }
        }
    }
}