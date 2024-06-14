using CedulasEvaluacion.Entities.MIncidencias;
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
    public class RepositorioIncidenciasConvencional : IRepositorioIncidenciasConvencional
    {
        private readonly string _connectionString;

        public RepositorioIncidenciasConvencional(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<List<IncidenciasConvencional>> getIncidenciasConvencional(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasConvencional", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        var response = new List<IncidenciasConvencional>();
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
        public async Task<int> IncidenciasTipoConvencional(int id, string tipo)
        {
            int total = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_totalIncidenciasTipoConvencional", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@total", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@tipo", tipo));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        total = (int)cmd.Parameters["@total"].Value;
                        return total;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }
        public async Task<List<IncidenciasConvencional>> ListIncidenciasTipoConvencional(int id, string tipo)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasByTipoConvencional", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        cmd.Parameters.Add(new SqlParameter("@tipo", tipo));
                        var response = new List<IncidenciasConvencional>();
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
        public async Task<int> InsertaIncidencia(IncidenciasConvencional incidenciasConvencional)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaIncidenciasConvencional", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@cedulaConvencional", incidenciasConvencional.CedulaConvencionalId));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidenciasConvencional.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", incidenciasConvencional.Pregunta));
                        cmd.Parameters.Add(new SqlParameter("@linea", incidenciasConvencional.Linea));
                        cmd.Parameters.Add(new SqlParameter("@fechaSolicitud", incidenciasConvencional.FechaSolicitud));
                        cmd.Parameters.Add(new SqlParameter("@fechaAtencion", incidenciasConvencional.FechaAtencion));


                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        id = Convert.ToInt32(cmd.Parameters["@id"].Value);

                        return id;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }
        public async Task<int> ActualizaIncidencia(IncidenciasConvencional incidenciasConvencional)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaIncidenciaConvencional", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", incidenciasConvencional.Id));
                        cmd.Parameters.Add(new SqlParameter("@cedulaConvencional", incidenciasConvencional.CedulaConvencionalId));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidenciasConvencional.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", incidenciasConvencional.Pregunta));
                        cmd.Parameters.Add(new SqlParameter("@linea", incidenciasConvencional.Linea));
                        cmd.Parameters.Add(new SqlParameter("@fechaSolicitud", incidenciasConvencional.FechaSolicitud));
                        cmd.Parameters.Add(new SqlParameter("@fechaAtencion", incidenciasConvencional.FechaAtencion));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }
        public async Task<int> EliminaIncidencia(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminarIncidenciaConvencional", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }
        public async Task<int> EliminaTodaIncidencia(int id, string tipo)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaTodaIncidenciaConvencional", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@tipo", tipo));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }
        private IncidenciasConvencional MapToValue(SqlDataReader reader)
        {
            return new IncidenciasConvencional
            {
                Id = (int)reader["Id"],
                CedulaConvencionalId = (int)reader["CedulaConvencionalId"],
                Tipo = reader["Tipo"].ToString(),
                Linea = reader["Linea"] != DBNull.Value ? reader["Linea"].ToString() : "",
                DiasAtencion = reader["DiasAtencion"] != DBNull.Value ? (int)reader["DiasAtencion"] : 0,
                DiasRetraso = reader["DiasRetraso"] != DBNull.Value ? (int)reader["DiasRetraso"] : 0,
                HorasRetraso = reader["HorasRetraso"] != DBNull.Value ? (int)reader["HorasRetraso"] : 0,
                HorasAtencion= reader["HorasAtencion"] != DBNull.Value ? (int)reader["HorasAtencion"] : 0,
                FechaSolicitud = reader["FechaSolicitud"] != DBNull.Value ? Convert.ToDateTime(reader["FechaSolicitud"]) : DateTime.Now,
                FechaAtencion = reader["FechaAtencion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaAtencion"]) : DateTime.Now,
                MontoPenalizacion = reader["MontoPenalizacion"] != DBNull.Value ? Convert.ToDecimal(reader["MontoPenalizacion"]) : 0
            };
        }
    }
}
