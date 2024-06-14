using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Repositories
{
    public class RepositorioAlcancesCedula : IRepositorioAlcancesCedula
    {
        private readonly string _connectionString;

        public RepositorioAlcancesCedula(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<List<CedulaEvaluacion>> getCedulasByMes(int servicio, int anio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getConcentradoCedulasMes", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@anio", anio));
                        var response = new List<CedulaEvaluacion>();
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

        public async Task<int> habilitaAlcancesCedulas(CedulaEvaluacion cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_habilitaAlcances", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@anio", cedula.Anio));
                        cmd.Parameters.Add(new SqlParameter("@mes", cedula.Mes));
                        cmd.Parameters.Add(new SqlParameter("@servicio", cedula.ServicioId));

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

        public async Task<int> capturaHistorial(HistorialEntregables historialEntregables)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaHistorialEntregables", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", historialEntregables.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@tipo", historialEntregables.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@cedula", historialEntregables.CedulaId));
                        cmd.Parameters.Add(new SqlParameter("@usuario", historialEntregables.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@estatus", historialEntregables.Estatus));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", historialEntregables.Comentarios));

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

        public async Task<List<HistorialEntregables>> getHistorialEntregables(int id, int servicioId)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getHistorialEntregables", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedula", id));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicioId));
                        var response = new List<HistorialEntregables>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueHistorial(reader));
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

        public async Task<int> apruebaRechazaAlcance(Entregables entregables)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_autorizarRechazarAlcance", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", entregables.Id));
                        cmd.Parameters.Add(new SqlParameter("@estatus", entregables.Estatus));

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

        private HistorialEntregables MapToValueHistorial(SqlDataReader reader)
        {
            return new HistorialEntregables
            {
                Servicio = reader["Servicio"].ToString(),
                Tipo = reader["Tipo"].ToString(),
                CedulaId = (int)reader["CedulaId"],
                UsuarioId = (int)reader["UsuarioId"],
                Estatus = reader["Estatus"].ToString(),
                Comentarios = reader["Comentarios"].ToString(),
                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"])
            };
        }

        private CedulaEvaluacion MapToValue(SqlDataReader reader)
        {
            return new CedulaEvaluacion
            {
                Mes = reader["Mes"] != DBNull.Value ? reader["Mes"].ToString(): "",
                Fondo = reader["Fondo"].ToString(),
                TotalCedulas = reader["TotalCedulas"] != DBNull.Value ? (int)reader["TotalCedulas"] : 0,
                TotalAlcances = reader["TotalAlcances"] != DBNull.Value ? (int)reader["TotalAlcances"] : 0,
            };
        }


    }
}
