using Microsoft.Data.SqlClient;
using SorteosAPI.Models;
using System.Data;

namespace SorteosAPI.Services
{
    public class RaffleAssignmentService : IRaffleAssignmentService
    {
        private readonly string _connectionString;

        public RaffleAssignmentService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString), "La cadena de conexión no puede ser nula.");
        }

        public async Task<bool> AssignRaffleToClientAsync(RaffleByClientAssign assignment)
        {
            try
            {
                if (!await ClientExistsAsync(assignment.IdClient))
                {
                    throw new Exception("El cliente con el Id proporcionado no existe.");
                }

                if (!await RaffleExistsAsync(assignment.IdRaffle))
                {
                    throw new Exception("El sorteo con el Id proporcionado no existe.");
                }
                if (await RaffleAlreadyAssignedAsync(assignment.IdClient, assignment.IdRaffle))
                {
                    throw new Exception("El sorteo ya ha sido asignado a este cliente.");
                }


                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("AssignRaffleToClient", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdClient", assignment.IdClient);
                        command.Parameters.AddWithValue("@IdRaffle", assignment.IdRaffle);
                        command.Parameters.AddWithValue("@IsActive", assignment.IsActive);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al asignar el sorteo al cliente.", ex);
            }
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

                        var result = (int)(await command.ExecuteScalarAsync() ?? 0);
                        return result > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RaffleExistsAsync(int idRaffle)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = "SELECT COUNT(1) FROM Raffles WHERE IdRaffle = @IdRaffle";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdRaffle", idRaffle);

                        var result = (int)(await command.ExecuteScalarAsync() ?? 0);
                        return result > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RaffleAlreadyAssignedAsync(int idClient, int idRaffle)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = "SELECT COUNT(1) FROM RaffleByClient WHERE IdClient = @IdClient AND IdRaffle = @IdRaffle";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdClient", idClient);
                        command.Parameters.AddWithValue("@IdRaffle", idRaffle);

                        var result = (int)(await command.ExecuteScalarAsync() ?? 0);
                        return result > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<RaffleByClient>> ListRafflesByClientAsync(int? idClient, int? idRaffle)
        {
            var assignments = new List<RaffleByClient>();

            try
            {
                if (idClient.HasValue && !await ClientExistsAsync(idClient.Value))
                {
                    throw new Exception("El cliente con el Id proporcionado no existe.");
                }

                if (idRaffle.HasValue && !await RaffleExistsAsync(idRaffle.Value))
                {
                    throw new Exception("El sorteo con el Id proporcionado no existe.");
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = "SELECT rbc.IdRaffleByClient, rbc.IdClient, rbc.IdRaffle, c.Name AS ClientName, r.Name AS RaffleName, rbc.CreatedAt, rbc.UpdatedAt, rbc.IsActive " +
                            "FROM RaffleByClient rbc " +
                            "INNER JOIN Clients c ON rbc.IdClient = c.IdClient " +
                            "INNER JOIN Raffles r ON rbc.IdRaffle = r.IdRaffle ";
                          

                    if (idClient.HasValue)
                    {
                        query += " AND rbc.IdClient = @IdClient";
                    }
                    if (idRaffle.HasValue)
                    {
                        query += " AND rbc.IdRaffle = @IdRaffle";
                    }

                    query += " ORDER BY rbc.CreatedAt DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        if (idClient.HasValue)
                        {
                            command.Parameters.AddWithValue("@IdClient", idClient.Value);
                        }
                        if (idRaffle.HasValue)
                        {
                            command.Parameters.AddWithValue("@IdRaffle", idRaffle.Value);
                        }

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                assignments.Add(new RaffleByClient
                                {
                                    IdRaffleByClient = reader.GetInt32(reader.GetOrdinal("IdRaffleByClient")),
                                    IdClient = reader.GetInt32(reader.GetOrdinal("IdClient")),
                                    IdRaffle = reader.GetInt32(reader.GetOrdinal("IdRaffle")),
                                    ClientName = reader.GetString(reader.GetOrdinal("ClientName")),
                                    RaffleName = reader.GetString(reader.GetOrdinal("RaffleName")),
                                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al listar los sorteos asignados.", ex);
            }

            return assignments;
        }
    }
}