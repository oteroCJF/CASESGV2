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
    public class RepositorioIncidenciasCelular : IRepositorioIncidenciasCelular
    {
        private readonly string _connectionString;
        public RepositorioIncidenciasCelular(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }
        public async Task<List<IncidenciasCelular>> getIncidenciasCelular(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasCelular", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        var response = new List<IncidenciasCelular>();
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
        public async Task<int> IncidenciasTipoCelular(int id, string tipo)
        {
            int total = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_totalIncidenciasTipoCelular", sql))
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
        public async Task<List<IncidenciasCelular>> ListIncidenciasTipoCelular(int id, string tipo)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasByTipoCelular", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        cmd.Parameters.Add(new SqlParameter("@tipo", tipo));
                        var response = new List<IncidenciasCelular>();
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
        public async Task<int> IncidenciasCelular(IncidenciasCelular incidenciasCelular)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaIncidenciasCelular", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@cedulaCelular", incidenciasCelular.CedulaCelularId));
                        cmd.Parameters.Add(new SqlParameter("@perfilCelular", incidenciasCelular.PerfilCelularId));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidenciasCelular.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", incidenciasCelular.Pregunta));
                        cmd.Parameters.Add(new SqlParameter("@linea", incidenciasCelular.Linea));
                        cmd.Parameters.Add(new SqlParameter("@fechaSolicitud", incidenciasCelular.FechaSolicitud));
                        cmd.Parameters.Add(new SqlParameter("@fechaAtencion", incidenciasCelular.FechaAtencion));


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
        public async Task<int> ActualizaIncidencia(IncidenciasCelular incidenciasCelular)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaIncidenciaCelular", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", incidenciasCelular.Id));
                        cmd.Parameters.Add(new SqlParameter("@cedulaCelular", incidenciasCelular.CedulaCelularId));
                        cmd.Parameters.Add(new SqlParameter("@perfilCelular", incidenciasCelular.PerfilCelularId));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidenciasCelular.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", incidenciasCelular.Pregunta));
                        cmd.Parameters.Add(new SqlParameter("@linea", incidenciasCelular.Linea));
                        cmd.Parameters.Add(new SqlParameter("@fechaSolicitud", incidenciasCelular.FechaSolicitud));
                        cmd.Parameters.Add(new SqlParameter("@fechaAtencion", incidenciasCelular.FechaAtencion));

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
                    using (SqlCommand cmd = new SqlCommand("sp_eliminarIncidenciaCelular", sql))
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
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaTodaIncidenciaCelular", sql))
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
        private IncidenciasCelular MapToValue(SqlDataReader reader)
        {
            PerfilesCelular pc = new PerfilesCelular();
            pc.Id = reader["PerfilCelularId"] != DBNull.Value ? (int)reader["PerfilCelularId"]:0;
            pc.Nombre = reader["NombrePerfil"] != DBNull.Value ? reader["NombrePerfil"].ToString() : "";


            return new IncidenciasCelular
            {
                Id = (int)reader["Id"],
                PerfilCelularId = reader["PerfilCelularId"] != DBNull.Value ? (int)reader["PerfilCelularId"]:0,
                CedulaCelularId = (int)reader["CedulaCelularId"],
                Tipo = reader["Tipo"].ToString(),
                Linea = reader["Linea"] != DBNull.Value ? reader["Linea"].ToString() : "",
                HorasAtencion = reader["HorasAtencion"] != DBNull.Value ? (int)reader["HorasAtencion"] : 0,
                HorasRetraso = reader["HorasRetraso"] != DBNull.Value ? (int)reader["HorasRetraso"] : 0,
                DiasAtencion = reader["DiasAtencion"] != DBNull.Value ? (int)reader["DiasAtencion"] : 0,
                DiasRetraso = reader["DiasRetraso"] != DBNull.Value ? (int)reader["DiasRetraso"] : 0,
                FechaSolicitud = reader["FechaSolicitud"] != DBNull.Value ? Convert.ToDateTime(reader["FechaSolicitud"]) : DateTime.Now,
                FechaAtencion = reader["FechaAtencion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaAtencion"]) : DateTime.Now,
                MontoPenalizacion = reader["MontoPenalizacion"] != DBNull.Value ? Convert.ToDecimal(reader["MontoPenalizacion"]) : 0,
                Nombre = reader["NombrePerfil"] != DBNull.Value ? reader["NombrePerfil"].ToString() : "",
                perfilesCelular = pc,
            };
        }

    }
}
