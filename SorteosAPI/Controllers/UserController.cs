namespace SorteosAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Data.SqlClient;
    using SorteosAPI.Models;
    using System.Data;

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly string _connectionString;

        public UserController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString), "La cadena de conexión no puede ser nula.");
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserCreate userCreate)
        {
            if (userCreate == null)
            {
                return BadRequest(new { success = false, message = "El usuario no puede ser nulo." });
            }

            if (!ClientExists(userCreate.IdClient))
            {
                return BadRequest(new { success = false, message = "El cliente con el Id proporcionado no existe." });
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("CreateUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@IdClient", userCreate.IdClient);
                        command.Parameters.AddWithValue("@Name", userCreate.Name);
                        command.Parameters.AddWithValue("@IsActive", userCreate.IsActive);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok(new { success = true, message = "Usuario creado exitosamente." });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al crear el usuario.",
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

        [HttpGet("list")]
        public IActionResult GetUsersByClientId([FromQuery] string? name = null)
        {
            try
            {
                var users = new List<User>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = "SELECT IdUser, IdClient, Name, CreatedAt, UpdatedAt, IsActive FROM Users";
                    if (!string.IsNullOrEmpty(name))
                    {
                        query += " Where Name LIKE @Name";
                    }

                    using (var command = new SqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            command.Parameters.AddWithValue("@Name", $"%{name}%");
                        }

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
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

                if (users.Count == 0)
                {
                    return NotFound(new { success = false, message = "No se encontraron usuarios para el cliente proporcionado." });
                }

                return Ok(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error al obtener los usuarios.",
                    error = ex.Message
                });
            }
        }
    }
}