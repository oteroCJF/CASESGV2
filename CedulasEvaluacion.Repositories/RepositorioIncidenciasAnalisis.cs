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
    public class RepositorioIncidenciasAnalisis : IRepositorioIncidenciasAnalisis
    {
        private readonly string _connectionString;

        public RepositorioIncidenciasAnalisis(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }
        public async Task<List<IncidenciasAnalisis>> GetIncidenciasPregunta(int id, int pregunta)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasAnalisisByPregunta", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", pregunta));
                        var response = new List<IncidenciasAnalisis>();
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
        public async Task<List<IncidenciasAnalisis>> GetIncidencias(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasAnalisis", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        var response = new List<IncidenciasAnalisis>();
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
        public async Task<int> IncidenciasAnalisis(IncidenciasAnalisis incidenciasAnalisis)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaIncidenciasAnalisis", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@cedulaAnalisis", incidenciasAnalisis.CedulaAnalisisId));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidenciasAnalisis.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", incidenciasAnalisis.Pregunta));
                        if (!incidenciasAnalisis.FechaIncidencia.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaIncidencia", incidenciasAnalisis.FechaIncidencia));

                        cmd.Parameters.Add(new SqlParameter("@comentarios", incidenciasAnalisis.Comentarios));


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
        public async Task<int> ActualizaIncidencia(IncidenciasAnalisis incidenciasAnalisis)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaIncidenciasAnalisis", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", incidenciasAnalisis.Id));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidenciasAnalisis.Tipo));
                        if (!incidenciasAnalisis.FechaIncidencia.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaIncidencia", incidenciasAnalisis.FechaIncidencia));

                        cmd.Parameters.Add(new SqlParameter("@comentarios", incidenciasAnalisis.Comentarios));
                        
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

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
        public async Task<int> EliminaIncidencia(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminarIncidenciaAnalisis", sql))
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
        public async Task<int> EliminaTodaIncidencia(int id, int pregunta)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaTodaIncidenciaAnalisis", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", pregunta));

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
        private IncidenciasAnalisis MapToValue(SqlDataReader reader)
        {
            return new IncidenciasAnalisis
            {
                Id = (int)reader["Id"],
                CedulaAnalisisId = (int)reader["CedulaAnalisisId"],
                Tipo = reader["Tipo"].ToString(),
                Pregunta = (int) reader["Pregunta"],
                FechaIncidencia = reader["FechaIncidencia"] != DBNull.Value ? Convert.ToDateTime(reader["FechaIncidencia"]) : DateTime.Now,
                Comentarios = reader["Comentarios"] != DBNull.Value ? reader["Comentarios"].ToString() : ""
            };
        }



    }
}
