using CedulasEvaluacion.Entities.MCedula;
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
    public class RepositorioFiltrado : IRepositorioFiltrado
    {
        private readonly string _connectionString;

        public RepositorioFiltrado(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection"); ;
        }
        public async Task<List<CedulaEvaluacion>> GetEstatusEvaluacion(int servicio, int usuario)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getEstatusEvaluacion", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@usuario", usuario));
                        var response = new List<CedulaEvaluacion>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueEstatus(reader));
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
        public async Task<List<CedulaEvaluacion>> GetMesesEvaluacion(int servicio, int usuario)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getMesesEvaluacion", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@usuario", usuario));
                        var response = new List<CedulaEvaluacion>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueMes(reader));
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
        private CedulaEvaluacion MapToValueEstatus(SqlDataReader reader)
        {
            return new CedulaEvaluacion { 
                Fondo = DBNull.Value != reader["Fondo"] ? reader["Fondo"].ToString():"",
                Estatus = DBNull.Value != reader["Estatus"] ? reader["Estatus"].ToString():"",
                TotalCedulas = DBNull.Value != reader["TotalCedulas"] ? (int) reader["TotalCedulas"]:0
            };
        }
        private CedulaEvaluacion MapToValueMes(SqlDataReader reader)
        {
            return new CedulaEvaluacion
            {
                Mes = DBNull.Value != reader["Mes"] ? reader["Mes"].ToString() : "",
                TotalCedulas = DBNull.Value != reader["TotalCedulas"] ? (int)reader["TotalCedulas"] : 0
            };
        }
    }
}
