using Microsoft.Data.SqlClient;
using SorteosAPI.Models;
using System.Data;

namespace SorteosAPI.Services
{
    public class AssignedNumberService : IAssignedNumberService
    {
        private readonly string _connectionString;

        public AssignedNumberService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException(nameof(_connectionString), "La cadena de conexión no puede ser nula.");
        }

        public async Task<(bool Success, AssignedNumberRaffer Model, string Message)> AssignRandomNumberAsync(AssignedNumberRaffer model)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("AssignRandomNumber", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.AddWithValue("@IdClient", model.IdClient);
                        command.Parameters.AddWithValue("@IdRaffle", model.IdRaffle);
                        command.Parameters.AddWithValue("@IdUser", model.IdUser);

                        // Parámetros de salida
                        SqlParameter generatedNumberParam = new SqlParameter("@GeneratedNumber", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(generatedNumberParam);

                        SqlParameter successParam = new SqlParameter("@Success", SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(successParam);

                        SqlParameter messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(messageParam);

                        // Ejecutar el procedimiento
                        await command.ExecuteNonQueryAsync();

                        // Recuperar valores de salida
                        int? generatedNumber = generatedNumberParam.Value != DBNull.Value ? (int?)generatedNumberParam.Value : null;
                        bool success = successParam.Value != DBNull.Value && (bool)successParam.Value;
                        string message = messageParam.Value != DBNull.Value ? messageParam.Value.ToString() : string.Empty;

                        // Asignar el número generado al modelo
                        model.Number = generatedNumber ?? 0;

                        return (success, model, message);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, null, $"Ocurrió un error: {ex.Message}");
            }
        }
    }
}