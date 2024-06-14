using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Repositories
{
    public class RepositorioOperaciones : IRepositorioOperaciones
    {

        private readonly string _connectionString;

        public RepositorioOperaciones(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection"); ;
        }

        public async Task<List<Operaciones>> getOperaciones()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getOperaciones", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var response = new List<Operaciones>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValue(reader));
                            }
                        }

                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        private Operaciones MapToValue(SqlDataReader reader)
        {
            return new Operaciones
            {
                Id = (int)reader["Id"],
                ModuloId = (int)reader["ModuloId"],
                Nombre = reader["Nombre"].ToString()
            };
        }
    }
}
