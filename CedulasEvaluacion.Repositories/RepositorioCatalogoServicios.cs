using CedulasEvaluacion.Entities.MCatalogoServicios;
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
    public class RepositorioCatalogoServicios : IRepositorioCatalogoServicios
    {
        private readonly string _connectionString;

        public RepositorioCatalogoServicios(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<List<DashboardCS>> GetDashBoard()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_dashboardContratosServicio", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var response = new List<DashboardCS>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueDashboard(reader));
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

        public async Task<List<CatalogoServicios>> GetCatalogoServicios()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCatalogoServicios", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var response = new List<CatalogoServicios>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueCatalogoServicios(reader));
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

        public async Task<CatalogoServicios> GetServicioById(int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getServicioById", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new CatalogoServicios();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueCatalogoServicios(reader);
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

        public async Task<CatalogoServicios> GetDescripcionServicio(string servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getDescripcionByServicio", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new CatalogoServicios();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueCatalogoServicios(reader);
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

        public async Task<List<CatalogoServicios>> GetServiciosByUsers(int usuario)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getServiciosByUser", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@usuario", usuario));
                        var response = new List<CatalogoServicios>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueCatalogoServicios(reader));
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

        private CatalogoServicios MapToValueCatalogoServicios(SqlDataReader reader)
        {
            return new CatalogoServicios
            {
                Id = (int)reader["Id"],
                Nombre = reader["Nombre"].ToString(),
                Fondo = reader["Fondo"].ToString(),
                Icono  = reader["Icono"].ToString(),
                ServicioBasico = reader["ServicioBasico"] != DBNull.Value ? (bool)reader["ServicioBasico"]:false,
                Descripcion = reader["Descripcion"].ToString(),
            };
        }

        private DashboardCS MapToValueDashboard(SqlDataReader reader)
        {
            return new DashboardCS
            {
                Id = reader["Id"] != DBNull.Value ? (int)reader["Id"] : 0,
                Servicio = reader["Servicio"] != DBNull.Value ? reader["Servicio"].ToString() : "",
                Fondo = reader["Fondo"] != DBNull.Value ? reader["Fondo"].ToString() : "",
                Icono = reader["Icono"] != DBNull.Value ? reader["Icono"].ToString() : "",
                Total = reader["TotalContratos"] != DBNull.Value ? (int)reader["TotalContratos"] : 0,
            };
        }
    }
}
