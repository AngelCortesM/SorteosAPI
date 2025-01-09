using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SorteosAPI.Models;
using System.Data;

namespace SorteosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly string _connectionString;

        public ClientController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString), "La cadena de conexión no puede ser nula.");
        }

        [HttpPost]
        public IActionResult CreateClient([FromBody] ClientCreate clientCreate)
        {
            if (clientCreate == null)
            {
                return BadRequest(new { success = false, message = "El cliente no puede ser nulo." });
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("CreateClient", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Name", clientCreate.Name);
                        command.Parameters.AddWithValue("@IsActive", clientCreate.IsActive);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok(new { success = true, message = "Cliente creado exitosamente." });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al crear el cliente.",
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

        [HttpGet("list")]
        public IActionResult GetClients()
        {
            try
            {
                var clients = new List<Client>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("GetAllClients", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
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

                return Ok(new { success = true, data = clients });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al obtener los clientes.",
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