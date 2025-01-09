using Microsoft.Data.SqlClient;
using SorteosAPI.Models;
using System.Data;

namespace SorteosAPI.Services
{
    public class ListNumberService : IListNumberService
    {
        private readonly string _connectionString;

        public ListNumberService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException(nameof(_connectionString), "La cadena de conexión no puede ser nula.");
        }

        public async Task<(bool Success, List<ListNumberRaffer> Numbers, int TotalCount, string Message)> GetAssignedNumbersPagedAsync(int pageNumber, int pageSize, string clientFilter, string raffleFilter, string userFilter)
        {
            try
            {
                var numbers = new List<ListNumberRaffer>();
                int totalCount = 0;

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("GetAssignedNumbersPaged", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Filtros
                        command.Parameters.AddWithValue("@ClientName", clientFilter ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@RaffleName", raffleFilter ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@UserName", userFilter ?? (object)DBNull.Value);

                        // Paginación
                        command.Parameters.AddWithValue("@PageNumber", pageNumber);
                        command.Parameters.AddWithValue("@PageSize", pageSize);

                        // Parámetro de salida para obtener el total de registros
                        SqlParameter totalCountParam = new SqlParameter("@TotalCount", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(totalCountParam);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                numbers.Add(new ListNumberRaffer
                                {
                                    IdAssignedNumber = reader.GetInt32(reader.GetOrdinal("IdAssignedNumber")),
                                    IdClient = reader.GetInt32(reader.GetOrdinal("IdClient")),
                                    ClientName = reader.GetString(reader.GetOrdinal("ClientName")),
                                    IdRaffleByClient = reader.GetInt32(reader.GetOrdinal("IdRaffleByClient")),
                                    RaffleName = reader.GetString(reader.GetOrdinal("RaffleName")),
                                    IdUser = reader.GetInt32(reader.GetOrdinal("IdUser")),
                                    UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                    Number = reader.GetInt32(reader.GetOrdinal("Number")),
                                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                                });
                            }
                        }

                        // Obtener el total de registros
                        totalCount = (int)totalCountParam.Value;
                    }
                }

                return (true, numbers, totalCount, "Datos recuperados correctamente.");
            }
            catch (Exception ex)
            {
                return (false, null, 0, $"Ocurrió un error: {ex.Message}");
            }
        }
    }
}