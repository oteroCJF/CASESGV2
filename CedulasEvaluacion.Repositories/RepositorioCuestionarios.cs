using CedulasEvaluacion.Entities.MCedula;
using Microsoft.Data.SqlClient;
using CedulasEvaluacion.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Data;

namespace CedulasEvaluacion.Repositories
{
    public class RepositorioCuestionarios : IRepositorioCuestionarios
    {
        private readonly string _connectionString;

        public RepositorioCuestionarios(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }
        public async Task<List<Preguntas>> GetCuestionarioCompleto(int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCuestionarioCompletoServicio", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<Preguntas>();
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
        public async Task<List<Preguntas>> GetCuestionarioByServicio(int servicio, int cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCuestionarioByServicio", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@cedula", cedula));
                        var response = new List<Preguntas>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueCuestionario(reader));
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
        private Preguntas MapToValue(SqlDataReader reader)
        {
            return new Preguntas()
            {
                Id = (int)reader["Id"],
                NoPregunta = (int)reader["NoPregunta"],
                ServicioId = (int)reader["ServicioId"],
                Pregunta = reader["Pregunta"].ToString(),
                Ayuda = reader["Ayuda"].ToString(),
                Respuesta = reader["Respuesta"] != DBNull.Value ? reader["Respuesta"].ToString(): "|",
                Cierre = (bool)reader["Cierre"],
                NoAplica = (bool)reader["NoAplica"],
                NoEntrego = (bool)reader["NoEntrego"],
                NoRealizo = (bool)reader["NoRealizo"],
                Fecha = (bool)reader["Fecha"],
                Fechas = (bool)reader["Fechas"],
                Incidencias = (bool)reader["Incidencias"],
            };
        }
        private Preguntas MapToValueCuestionario(SqlDataReader reader)
        {
            return new Preguntas()
            {
                Id = (int)reader["Id"],
                NoPregunta = (int)reader["NoPregunta"],
                NoPreguntaUsuario = (int)reader["NoPreguntaUsuario"],
                ServicioId = (int)reader["ServicioId"],
                Pregunta = reader["Pregunta"].ToString(),
                Ayuda = reader["Ayuda"].ToString(),
                Concepto = reader["Concepto"].ToString(),
                Abreviacion = reader["Abreviacion"] != DBNull.Value ? reader["Abreviacion"].ToString() : "",
                Cierre = DBNull.Value != reader["Cierre"] ? (bool)reader["Cierre"]:false,
                NoAplica = DBNull.Value != reader["NoAplica"] ? (bool)reader["NoAplica"]:false,
                NoEntrego = DBNull.Value != reader["NoEntrego"] ? (bool)reader["NoEntrego"]:false,
                NoRealizo = DBNull.Value != reader["NoRealizo"] ? (bool)reader["NoRealizo"]:false,
                Fecha = DBNull.Value != reader["Fecha"] ? (bool)reader["Fecha"]:false,
                Fechas = DBNull.Value != reader["Fechas"] ? (bool)reader["Fechas"]:false,
                SUA = DBNull.Value != reader["SUA"] ? (bool)reader["SUA"] :false,
                Numero = DBNull.Value != reader["Numero"] ? (bool)reader["Numero"] :false,
                Incidencias = DBNull.Value != reader["Incidencias"] ? (bool)reader["Incidencias"]:false,
            };
        }
    }
}
