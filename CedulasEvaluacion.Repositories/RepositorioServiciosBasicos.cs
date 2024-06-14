using CedulasEvaluacion.Entities.MServiciosB;
using CedulasEvaluacion.Entities.Vistas;
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
    public class RepositorioServiciosBasicos : IRepositorioServiciosBasicos
    {
        private readonly string _connectionString;

        public RepositorioServiciosBasicos(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<List<VServiciosBasicos>> GetServicioBasico(int anio, int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getServicioBasico", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@anio", anio));
                        var response = new List<VServiciosBasicos>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueSB(reader));
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

        public async Task<ServicioBasico> GetServicioBasicoById(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getServicioBasicoById", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        var response = new ServicioBasico();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueSBId(reader);
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

        public async Task<int> insertaServicioBasico(ServicioBasico sb)
        {
            int id = 0;
            
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaServicioBasico", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", sb.Id)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@servicio", sb.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@usuario", sb.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@inmueble", sb.InmuebleId));
                        cmd.Parameters.Add(new SqlParameter("@mes", sb.Mes));
                        cmd.Parameters.Add(new SqlParameter("@anio", sb.Anio));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        id = Convert.ToInt32(cmd.Parameters["@id"].Value);
                    }
                }
                return id;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        
        public async Task<int> actualizaServicioBasico(ServicioBasico sb)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaServicioBasico", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", sb.Id));
                        cmd.Parameters.Add(new SqlParameter("@servicio", sb.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@usuario", sb.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@inmueble", sb.InmuebleId));
                        cmd.Parameters.Add(new SqlParameter("@mes", sb.Mes));
                        cmd.Parameters.Add(new SqlParameter("@anio", sb.Anio));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return sb.Id;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }

        public async Task<int> enviaServicioBasico(ServicioBasico sb)
        {
            int update = await actualizaServicioBasico(sb);
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_enviaServicioBasico", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", sb.Id));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }

        private VServiciosBasicos MapToValueSB(SqlDataReader reader)
        {
            return new VServiciosBasicos
            {
                Id = (int)reader["Id"],
                Anio = (int)reader["Anio"],
                Servicio = reader["Servicio"] != DBNull.Value ? reader["Servicio"].ToString():"",
                Inmueble = reader["Inmueble"] != DBNull.Value ?  reader["Inmueble"].ToString():"",
                Estatus = reader["Estatus"] != DBNull.Value ?  reader["Estatus"].ToString():"",
                Fondo = reader["Fondo"].ToString(),
                Mes = reader["Mes"].ToString(),
            };
        }
        
        private ServicioBasico MapToValueSBId(SqlDataReader reader)
        {
            return new ServicioBasico
            {
                Id = (int)reader["Id"],
                ServicioId = reader["ServicioId"] != DBNull.Value ? (int)reader["ServicioId"] : 0,
                InmuebleId = reader["InmuebleId"] != DBNull.Value ? (int)reader["InmuebleId"] : 0,
                Anio = (int)reader["Anio"],
                Mes = reader["Mes"].ToString(),
                Estatus = reader["Estatus"] != DBNull.Value ?  reader["Estatus"].ToString():"",
            };
        }
    }
}
