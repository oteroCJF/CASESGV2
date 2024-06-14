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
    public class RepositorioIncidenciasAgua : IRepositorioIncidenciasAgua
    {
        private readonly string _connectionString;

        public RepositorioIncidenciasAgua(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }
        public async Task<List<IncidenciasAgua>> GetIncidenciasPregunta(int id, int pregunta)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasAguaByPregunta", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", pregunta));
                        var response = new List<IncidenciasAgua>();
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
        public async Task<List<IncidenciasAgua>> GetIncidencias(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasAgua", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        var response = new List<IncidenciasAgua>();
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

        public async Task<int> IncidenciasAgua(IncidenciasAgua incidenciasAgua)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaIncidenciasAgua", sql))
                    {
                        string mm = incidenciasAgua.FechaProgramada.ToShortDateString();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@cedulaAgua", incidenciasAgua.CedulaAguaId));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidenciasAgua.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", incidenciasAgua.Pregunta));
                        if (!incidenciasAgua.FechaProgramada.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaProgramada", incidenciasAgua.FechaProgramada));
                        if (!incidenciasAgua.FechaRealizada.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaRealizada", incidenciasAgua.FechaRealizada));
                        if (incidenciasAgua.HoraProgramada.TotalSeconds != 0)
                            cmd.Parameters.Add(new SqlParameter("@horaProgramada", incidenciasAgua.HoraProgramada));
                        if (incidenciasAgua.HoraRealizada.TotalSeconds != 0)
                            cmd.Parameters.Add(new SqlParameter("@horaRealizada", incidenciasAgua.HoraRealizada));
                        if (incidenciasAgua.Garrafones != 0)
                            cmd.Parameters.Add(new SqlParameter("@garrafones", incidenciasAgua.Garrafones));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", incidenciasAgua.Comentarios));


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

        public async Task<int> ActualizaIncidencia(IncidenciasAgua incidenciasAgua)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaIncidenciasAgua", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", incidenciasAgua.Id));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidenciasAgua.Tipo));
                        if (!incidenciasAgua.FechaProgramada.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaProgramada", incidenciasAgua.FechaProgramada));
                        if (!incidenciasAgua.FechaRealizada.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaRealizada", incidenciasAgua.FechaRealizada));
                        if (incidenciasAgua.HoraProgramada.TotalSeconds != 0)
                            cmd.Parameters.Add(new SqlParameter("@horaProgramada", incidenciasAgua.HoraProgramada));
                        if (incidenciasAgua.HoraRealizada.TotalSeconds != 0)
                            cmd.Parameters.Add(new SqlParameter("@horaRealizada", incidenciasAgua.HoraRealizada));
                        if (incidenciasAgua.Garrafones != 0)
                            cmd.Parameters.Add(new SqlParameter("@garrafones", incidenciasAgua.Garrafones));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", incidenciasAgua.Comentarios));


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
                    using (SqlCommand cmd = new SqlCommand("sp_eliminarIncidenciaAgua", sql))
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
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaTodaIncidenciaAgua", sql))
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

        private IncidenciasAgua MapToValue(SqlDataReader reader)
        {
            return new IncidenciasAgua
            {
                Id = (int)reader["Id"],
                CedulaAguaId = (int)reader["CedulaAguaId"],
                Tipo = reader["Tipo"].ToString(),
                Pregunta = reader["Pregunta"].ToString(),
                Garrafones = reader["Garrafones"] != DBNull.Value ? (int)reader["Garrafones"] : 0,
                FechaProgramada = reader["FechaProgramada"] != DBNull.Value ? Convert.ToDateTime(reader["FechaProgramada"]) : DateTime.Now,
                FechaRealizada = reader["FechaRealizada"] != DBNull.Value ? Convert.ToDateTime(reader["FechaRealizada"]) : DateTime.Now,
                HoraProgramada = reader["HoraProgramada"] != DBNull.Value ? (TimeSpan)(reader["HoraProgramada"]) : TimeSpan.Parse("00:00:00"),
                HoraRealizada = reader["HoraRealizada"] != DBNull.Value ? (TimeSpan)(reader["HoraRealizada"]) : TimeSpan.Parse("00:00:00"),
                Comentarios = reader["Comentarios"] != DBNull.Value ? reader["Comentarios"].ToString() : "",
                Penalizable = reader["Penalizable"] != DBNull.Value ? (bool)reader["Penalizable"] : false,
                MontoPenalizacion = reader["MontoPenalizacion"] != DBNull.Value ? (decimal)reader["MontoPenalizacion"] : 0,
            };
        }


    }
}
