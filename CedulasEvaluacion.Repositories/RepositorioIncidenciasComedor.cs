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
    public class RepositorioIncidenciasComedor : IRepositorioIncidenciasComedor
    {
        private readonly string _connectionString;

        public RepositorioIncidenciasComedor(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }
        public async Task<List<IncidenciasComedor>> GetIncidenciasPregunta(int id, int pregunta)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasComedorByPregunta", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", pregunta));
                        var response = new List<IncidenciasComedor>();
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
        public async Task<List<IncidenciasComedor>> GetIncidencias(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasComedor", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        var response = new List<IncidenciasComedor>();
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
        public async Task<int> IncidenciasComedor(IncidenciasComedor incidencia)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaIncidenciasComedor", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@cedulaComedor", incidencia.CedulaComedorId));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidencia.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@cumplio", incidencia.Cumplio));
                        if (!incidencia.Incumplimiento.Equals(""))
                            cmd.Parameters.Add(new SqlParameter("@incumplimiento", incidencia.Incumplimiento));
                        if (!incidencia.FechaProgramada.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaProgramada", incidencia.FechaProgramada));
                        if (!incidencia.FechaEntrega.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaEntrega", incidencia.FechaEntrega));
                        if (!incidencia.FechaIncidencia.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaIncidencia", incidencia.FechaIncidencia));
                        if (incidencia.TotalMuestras != 0)
                            cmd.Parameters.Add(new SqlParameter("@totalMuestras", incidencia.TotalMuestras));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", incidencia.Comentarios));

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
        public async Task<int> ActualizaIncidencia(IncidenciasComedor incidencia)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaIncidenciasComedor", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", incidencia.Id));
                        cmd.Parameters.Add(new SqlParameter("@cedulaComedor", incidencia.CedulaComedorId));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidencia.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@cumplio", incidencia.Cumplio));
                        if (!incidencia.Incumplimiento.Equals(""))
                            cmd.Parameters.Add(new SqlParameter("@incumplimiento", incidencia.Incumplimiento));
                        if (!incidencia.FechaProgramada.ToString("dd/MM/yyyy").Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaProgramada", incidencia.FechaProgramada));
                        if (!incidencia.FechaEntrega.ToString("dd/MM/yyyy").Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaEntrega", incidencia.FechaEntrega));
                        if (!incidencia.FechaIncidencia.ToString("dd/MM/yyyy").Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaIncidencia", incidencia.FechaIncidencia));
                        if (incidencia.TotalMuestras != 0)
                            cmd.Parameters.Add(new SqlParameter("@totalMuestras", incidencia.TotalMuestras));
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
                return -1;
            }
        }
        public async Task<int> EliminaIncidencia(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminarIncidenciaComedor", sql))
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
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaTodaIncidenciaComedor", sql))
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
        private IncidenciasComedor MapToValue(SqlDataReader reader)
        {
            return new IncidenciasComedor
            {
                Id = (int)reader["Id"],
                CedulaComedorId = (int)reader["CedulaComedorId"],
                Tipo = reader["Tipo"].ToString(),
                TipoIncidencia = reader["TipoIncidencia"].ToString(),
                Incumplimiento = reader["Incumplimiento"] != DBNull.Value ? reader["Incumplimiento"].ToString() : "",
                Pregunta = (int)reader["Pregunta"],
                FechaProgramada = reader["FechaProgramada"] != DBNull.Value ? Convert.ToDateTime(reader["FechaProgramada"]) : Convert.ToDateTime("01/01/1990"),
                FechaEntrega = reader["FechaEntrega"] != DBNull.Value ? Convert.ToDateTime(reader["FechaEntrega"]) : Convert.ToDateTime("01/01/1990"),
                FechaIncidencia = reader["FechaIncidencia"] != DBNull.Value ? Convert.ToDateTime(reader["FechaIncidencia"]) : Convert.ToDateTime("01/01/1990"),
                DiasAtraso = reader["DiasAtraso"] != DBNull.Value ? (int)reader["DiasAtraso"] :0,
                TotalMuestras = reader["TotalMuestras"] != DBNull.Value ? (int)reader["TotalMuestras"]:0,
                Comentarios = reader["Comentarios"] != DBNull.Value ? reader["Comentarios"].ToString() : "",
                Cumplio = reader["Cumplio"] != DBNull.Value ? (bool)reader["Cumplio"] : false,
                Penalizable = reader["Penalizable"] != DBNull.Value ? (bool)reader["Penalizable"] : false,
                MontoPenalizacion = reader["MontoPenalizacion"] != DBNull.Value ? (decimal)reader["MontoPenalizacion"] : 0,
            };
        }



    }
}
