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
    public class RepositorioIncidenciasTransporte : IRepositorioIncidenciasTransporte
    {
        private readonly string _connectionString;

        public RepositorioIncidenciasTransporte(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection"); ;
        }

        public async Task<List<IncidenciasTransporte>> GetIncidenciasPregunta(int id, int pregunta)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasTransporteByPregunta", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", pregunta));
                        var response = new List<IncidenciasTransporte>();
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
        public async Task<List<IncidenciasTransporte>> GetIncidencias(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasTransporte", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        var response = new List<IncidenciasTransporte>();
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
        public async Task<int> IncidenciasTransporte(IncidenciasTransporte incidenciasTransporte)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaIncidenciasTransporte", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@cedulaTransporte", incidenciasTransporte.CedulaTransporteId));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidenciasTransporte.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", incidenciasTransporte.Pregunta));
                        if (!incidenciasTransporte.FechaIncidencia.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaIncidencia", incidenciasTransporte.FechaIncidencia));
                        if (incidenciasTransporte.HoraPresentada.TotalSeconds != 0)
                            cmd.Parameters.Add(new SqlParameter("@horaPresentada", incidenciasTransporte.HoraPresentada));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", incidenciasTransporte.Comentarios));


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

        public async Task<int> ActualizaIncidencia(IncidenciasTransporte incidenciasTransporte)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaIncidenciasTransporte", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", incidenciasTransporte.Id));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidenciasTransporte.Tipo));
                        if (!incidenciasTransporte.FechaIncidencia.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaIncidencia", incidenciasTransporte.FechaIncidencia));
                        if (incidenciasTransporte.HoraPresentada.TotalSeconds != 0)
                            cmd.Parameters.Add(new SqlParameter("@horaPresentada", incidenciasTransporte.HoraPresentada));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", incidenciasTransporte.Comentarios));


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

        public async Task<int> EliminaIncidencia(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminarIncidenciaTransporte", sql))
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
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaTodaIncidenciaTransporte", sql))
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

        private IncidenciasTransporte MapToValue(SqlDataReader reader)
        {
            return new IncidenciasTransporte
            {
                Id = (int)reader["Id"],
                CedulaTransporteId = (int)reader["CedulaTransporteId"],
                Tipo = reader["Tipo"].ToString(),
                Pregunta = reader["Pregunta"].ToString(),
                FechaIncidencia = reader["FechaIncidencia"] != DBNull.Value ? Convert.ToDateTime(reader["FechaIncidencia"]) : DateTime.Now,
                HoraPresentada= reader["HoraPresentada"] != DBNull.Value ? (TimeSpan)(reader["HoraPresentada"]) : TimeSpan.Parse("00:00:00"),
                Comentarios = reader["Comentarios"] != DBNull.Value ? reader["Comentarios"].ToString() : ""
            };
        }
    }
}
