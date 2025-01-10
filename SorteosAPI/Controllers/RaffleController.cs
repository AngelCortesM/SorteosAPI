namespace SorteosAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using SorteosAPI.Models;
    using System;
    using System.Collections.Generic;
    using System.Data;

    [ApiController]
    [Route("api/[controller]")]
    public class RaffleController : ControllerBase
    {
        private readonly string _connectionString;

        public RaffleController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString), "La cadena de conexión no puede ser nula.");
        }

        [HttpPost]
        public IActionResult CreateRaffle([FromBody] RaffleCreate raffleCreate)
        {
            if (raffleCreate == null)
            {
                return BadRequest(new { success = false, message = "El sorteo no puede ser nulo." });
            }

            if (RaffleExists(raffleCreate.Name))
            {
                return BadRequest(new { success = false, message = "Ya existe un sorteo con el mismo nombre." });
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("CreateRaffle", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Name", raffleCreate.Name);
                        command.Parameters.AddWithValue("@IsActive", raffleCreate.IsActive);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok(new { success = true, message = "Sorteo creado exitosamente." });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al crear el sorteo.",
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

        [HttpGet]
        public IActionResult GetAllRaffles()
        {
            try
            {
                var raffles = new List<Raffle>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = @"SELECT
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
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
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

                if (raffles.Count == 0)
                {
                    return NotFound(new { success = false, message = "No se encontraron sorteos." });
                }

                return Ok(new { success = true, data = raffles });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error al obtener los sorteos.",
                    error = ex.Message
                });
            }
        }

        private bool RaffleExists(string name)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = "SELECT COUNT(1) FROM Raffles WHERE Name = @Name";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);

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
    }
}