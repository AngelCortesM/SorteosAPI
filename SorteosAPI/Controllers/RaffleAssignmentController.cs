using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SorteosAPI.Models;
using System.Data;

namespace SorteosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaffleAssignmentController : ControllerBase
    {
        private readonly string _connectionString;

        public RaffleAssignmentController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString), "La cadena de conexión no puede ser nula.");
        }

        [HttpPost]
        public IActionResult AssignRaffleToClient([FromBody] RaffleByClientAssign assignment)
        {
            if (assignment == null)
            {
                return BadRequest(new { success = false, message = "Los datos del sorteo no pueden ser nulos." });
            }

            try
            {
                if (!ClientExists(assignment.IdClient))
                {
                    return BadRequest(new { success = false, message = "El cliente con el Id proporcionado no existe." });
                }

                if (!RaffleExists(assignment.IdRaffle))
                {
                    return BadRequest(new { success = false, message = "El sorteo con el Id proporcionado no existe." });
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("AssignRaffleToClient", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdClient", assignment.IdClient);
                        command.Parameters.AddWithValue("@IdRaffle", assignment.IdRaffle);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok(new { success = true, message = "Sorteo asignado al cliente exitosamente." });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al asignar el sorteo al cliente.",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error inesperado.",
                    error = ex.Message
                });
            }
        }

        private bool ClientExists(int idClient)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = "SELECT COUNT(1) FROM Clients WHERE IdClient = @IdClient";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdClient", idClient);

                        var result = (int)command.ExecuteScalar();
                        return result > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool RaffleExists(int idRaffle)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = "SELECT COUNT(1) FROM Raffles WHERE IdRaffle = @IdRaffle";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdRaffle", idRaffle);

                        var result = (int)command.ExecuteScalar();
                        return result > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpGet("list")]
        public IActionResult ListRafflesByClient([FromQuery] int? idClient, [FromQuery] int? idRaffle)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = "SELECT rbc.IdRaffleByClient, rbc.IdClient, rbc.IdRaffle, c.Name AS ClientName, r.Name AS RaffleName, rbc.CreatedAt, rbc.UpdatedAt, rbc.IsActive " +
                            "FROM RaffleByClient rbc " +
                            "INNER JOIN Clients c ON rbc.IdClient = c.IdClient " +
                            "INNER JOIN Raffles r ON rbc.IdRaffle = r.IdRaffle " +
                            "WHERE rbc.IsActive = 1";

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

                        using (var reader = command.ExecuteReader())
                        {
                            var assignments = new List<RaffleByClient>();

                            while (reader.Read())
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

                            return Ok(assignments);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al listar los sorteos asignados.",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error inesperado.",
                    error = ex.Message
                });
            }
        }
    }
}