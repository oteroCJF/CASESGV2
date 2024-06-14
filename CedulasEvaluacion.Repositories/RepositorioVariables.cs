using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.MVariables;
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
    public class RepositorioVariables : IRepositorioVariables
    {
        private readonly string _connectionString;

        public RepositorioVariables(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection"); ;
        }

        public async Task<List<Variables>> GetAniosEvaluacion()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getAniosEvaluacion", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var response = new List<Variables>();
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
        public async Task<List<Variables>> GetTipoServicio(int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getTiposServicio", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<Variables>();
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
        public async Task<List<Variables>> GetListadoEntregables(int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getListadoEntregables", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<Variables>();
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
        public async Task<List<Variables>> GetListadoAlcances(int servicio, int cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getAlcances", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
                        var response = new List<Variables>();
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
        public async Task<List<Variables>> GetEntregablesByServicioCAE(int servicio, int cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetEntregablesByServicioCAE", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
                        var response = new List<Variables>();
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
        public async Task<List<Variables>> GetEntregablesByServicioCAR(int servicio, int cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetEntregablesByServicioCAR", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
                        var response = new List<Variables>();
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
        public async Task<List<Variables>> GetEntregablesUpdateByServicio(int servicio,string tipo)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetEntregablesUpdateByCedula", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@tipo", tipo));
                        var response = new List<Variables>();
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
        public async Task<List<Variables>> GetTiposIncidenciasByServicio(int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetTiposIncidencias", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicioId", servicio));
                        var response = new List<Variables>();
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
        public async Task<List<Variables>> GetRubrosEvaluar(int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getRubrosEvaluar", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicioId", servicio));
                        var response = new List<Variables>();
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
        public async Task<List<Variables>> GetDetalleIncidenciaByTipo(int servicio, string tipo)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetDetalleIncidenciaByTipo", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicioId", servicio));
                        cmd.Parameters.Add(new SqlParameter("@tipo", tipo));
                        var response = new List<Variables>();
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
        public async Task<Variables> GetHorarioComedor(int comedor, int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetHorarioComedor", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@comedor", comedor));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new Variables();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValue(reader);
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
        public async Task<List<Variables>> GetPestanasRevisionCedula(int servicio, int cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetPestanasRevisionCedula", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
                        var response = new List<Variables>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueRevision(reader));
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
        public async Task<List<Meses>> GetMesesEvaluacion()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getMeses", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var response = new List<Meses>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueMeses(reader));
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
        private Variables MapToValue(SqlDataReader reader)
        {
            return new Variables {
                Id = DBNull.Value != reader["Id"] ? (int) reader["Id"]:0,
                ServicioId = DBNull.Value != reader["ServicioId"] ? (int)reader["ServicioId"]:0,
                Tipo = DBNull.Value != reader["Tipo"] ? reader["Tipo"].ToString():"",
                Abreviacion = DBNull.Value != reader["Abreviacion"] ?  reader["Abreviacion"].ToString():"",
                Valor = DBNull.Value != reader["Valor"] ? reader["Valor"].ToString():"",
            };
        }
        private Variables MapToValueRevision(SqlDataReader reader)
        {
            return new Variables
            {
                Id = reader["Id"] != DBNull.Value ? (int)reader["Id"] : 0,
                NoPregunta = reader["NoPregunta"] != DBNull.Value ? (int)reader["NoPregunta"] : 0,
                ServicioId = reader["ServicioId"] != DBNull.Value ? (int)reader["ServicioId"] : 0,
                Tipo = reader["Tipo"] != DBNull.Value ? reader["Tipo"].ToString() : "",
                Abreviacion = reader["Abreviacion"] != DBNull.Value ? reader["Abreviacion"].ToString() : "",
                Valor = reader["Valor"] != DBNull.Value ? reader["Valor"].ToString() : "",
                Cierre = reader["Cierre"] != DBNull.Value ? (bool) reader["Cierre"] : false,
                NoAplica = reader["NoAplica"] != DBNull.Value ? (bool) reader["NoAplica"] : false,
                Fecha = reader["Fecha"] != DBNull.Value ? (bool) reader["Fecha"] : false,
                Fechas = reader["Fechas"] != DBNull.Value ? (bool) reader["Fechas"] : false,
                NoEntrego = reader["NoEntrego"] != DBNull.Value ? (bool) reader["NoEntrego"] : false,
                NoRealizo = reader["NoRealizo"] != DBNull.Value ? (bool) reader["NoRealizo"] : false,
                SUA = reader["SUA"] != DBNull.Value ? (bool) reader["SUA"] : false,
                Numero = reader["Numero"] != DBNull.Value ? (bool) reader["Numero"] : false,
                Incidencias = reader["Incidencias"] != DBNull.Value ? (bool) reader["Incidencias"] : false,
            };
        }
        private Meses MapToValueMeses(SqlDataReader reader)
        {
            return new Meses
            {
                Id = reader["Id"] != DBNull.Value ? (int)reader["Id"] : 0,
                Nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : "",
                Fondo = reader["Fondo"] != DBNull.Value ? reader["Fondo"].ToString() : "",
            };
        }
    }
}
