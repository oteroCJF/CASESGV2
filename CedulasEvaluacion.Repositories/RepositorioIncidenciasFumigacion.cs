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
    public class RepositorioIncidenciasFumigacion : IRepositorioIncidenciasFumigacion
    {
        private readonly string _connectionString;

        public RepositorioIncidenciasFumigacion(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<List<IncidenciasFumigacion>> GetIncidenciasPregunta(int id, int pregunta)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasFumigacionByPregunta", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", pregunta));
                        var response = new List<IncidenciasFumigacion>();
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
        public async Task<List<IncidenciasFumigacion>> GetIncidencias(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasFumigacion", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        var response = new List<IncidenciasFumigacion>();
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

        public async Task<int> IncidenciasFumigacion(IncidenciasFumigacion incidenciasFumigacion)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaIncidenciasFumigacion", sql))
                    {
                        string mm = incidenciasFumigacion.FechaProgramada.ToShortDateString();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@cedulaFumigacion", incidenciasFumigacion.CedulaFumigacionId));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidenciasFumigacion.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", incidenciasFumigacion.Pregunta));
                        if(!incidenciasFumigacion.FechaProgramada.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaProgramada", incidenciasFumigacion.FechaProgramada));
                        if (!incidenciasFumigacion.FechaRealizada.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaRealizada", incidenciasFumigacion.FechaRealizada));
                        if (incidenciasFumigacion.HoraProgramada.TotalSeconds != 0)
                            cmd.Parameters.Add(new SqlParameter("@horaProgramada", incidenciasFumigacion.HoraProgramada));
                        if (incidenciasFumigacion.HoraRealizada.TotalSeconds != 0)
                            cmd.Parameters.Add(new SqlParameter("@horaRealizada", incidenciasFumigacion.HoraRealizada));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", incidenciasFumigacion.Comentarios));


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

        public async Task<int> ActualizaIncidencia(IncidenciasFumigacion incidenciasFumigacion)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaIncidenciasFumigacion", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", incidenciasFumigacion.Id));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidenciasFumigacion.Tipo));
                        if (!incidenciasFumigacion.FechaProgramada.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaProgramada", incidenciasFumigacion.FechaProgramada));
                        if (!incidenciasFumigacion.FechaRealizada.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaRealizada", incidenciasFumigacion.FechaRealizada));
                        if (incidenciasFumigacion.HoraProgramada.TotalSeconds != 0)
                            cmd.Parameters.Add(new SqlParameter("@horaProgramada", incidenciasFumigacion.HoraProgramada));
                        if (incidenciasFumigacion.HoraRealizada.TotalSeconds != 0) 
                            cmd.Parameters.Add(new SqlParameter("@horaRealizada", incidenciasFumigacion.HoraRealizada));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", incidenciasFumigacion.Comentarios));


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
                    using (SqlCommand cmd = new SqlCommand("sp_eliminarIncidenciaFumigacion", sql))
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
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaTodaIncidenciaFumigacion", sql))
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

        private IncidenciasFumigacion MapToValue(SqlDataReader reader)
        {
            return new IncidenciasFumigacion
            {
                Id = (int)reader["Id"],
                CedulaFumigacionId = (int)reader["CedulaFumigacionId"],
                DHAtraso = reader["DHAtraso"] != DBNull.Value ? (int)reader["DHAtraso"] : 0,
                Tipo = reader["Tipo"].ToString(),
                Pregunta = reader["Pregunta"].ToString(),
                FechaProgramada = reader["FechaProgramada"] != DBNull.Value ? Convert.ToDateTime(reader["FechaProgramada"]) : DateTime.Now,
                FechaRealizada = reader["FechaRealizada"] != DBNull.Value ? Convert.ToDateTime(reader["FechaRealizada"]) : DateTime.Now,
                HoraProgramada = reader["HoraProgramada"] != DBNull.Value ? (TimeSpan)(reader["HoraProgramada"]) : TimeSpan.Parse("00:00:00"),
                HoraRealizada = reader["HoraRealizada"] != DBNull.Value ? (TimeSpan)(reader["HoraRealizada"]) : TimeSpan.Parse("00:00:00"),
                Comentarios = reader["Comentarios"] != DBNull.Value ? reader["Comentarios"].ToString() : ""
            };
        }

    }
}
