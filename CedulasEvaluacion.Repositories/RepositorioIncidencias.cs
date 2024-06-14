using CedulasEvaluacion.Entities.MIncidencias;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Repositories
{
    public class RepositorioIncidencias : IRepositorioIncidencias
    {
        private readonly string _connectionString;

        public RepositorioIncidencias(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection"); ;
        }

        public async Task<List<IncidenciasLimpieza>> getIncidencias(int cedulaId)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedulaId));
                        var response = new List<IncidenciasLimpieza>();
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
        public async Task<List<IncidenciasLimpieza>> getIncidenciasByPregunta(int cedulaId, int pregunta)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasLimpiezaByPregunta", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedulaId));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", pregunta));
                        var response = new List<IncidenciasLimpieza>();
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
        public async Task<int> insertarIncidencia(IncidenciasLimpieza incidencia)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaIncidenciasLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", incidencia.Id)).Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", incidencia.CedulaLimpiezaId));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", incidencia.Pregunta));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidencia.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@area", incidencia.Area));
                        cmd.Parameters.Add(new SqlParameter("@fechaIncidencia", incidencia.FechaIncidencia));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", incidencia.Comentarios));
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        int id = (int)cmd.Parameters["@id"].Value;
                        return id;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        public async Task<int> actualizarIncidencia(IncidenciasLimpieza incidencia)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaIncidenciaLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", incidencia.Id));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidencia.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@area", incidencia.Area));
                        cmd.Parameters.Add(new SqlParameter("@fechaIncidencia", incidencia.FechaIncidencia));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", incidencia.Comentarios));
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        
                        return incidencia.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        public async Task<int> eliminarIncidencia(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaIncidenciaLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
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
                return 0;
            }
        }
        public async Task<int> eliminarTodaIncidencia(int cedula, int pregunta)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminarTodaIncidenciaLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
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
                return 0;
            }
        }
        private IncidenciasLimpieza MapToValue(SqlDataReader reader)
        {
            return new IncidenciasLimpieza()
            {
                Id = DBNull.Value != reader["Id"] ? (int)reader["Id"]:0,
                FechaIncidencia = DBNull.Value != reader["FechaIncidencia"] ? (DateTime)reader["FechaIncidencia"]:Convert.ToDateTime("01/01/1990"),
                Tipo = DBNull.Value != reader["Tipo"] ? reader["Tipo"].ToString():"",
                Area = DBNull.Value != reader["Area"] ? reader["Area"].ToString():"",
                Comentarios = DBNull.Value != reader["Comentarios"] ? reader["Comentarios"].ToString():"",
            };
        }
    }
}
