using CedulasEvaluacion.Entities.MLecturasAgua;
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
    public class RepositorioLecturasAgua : IRepositorioLecturasAgua
    {
        private readonly string _connectionString;

        public RepositorioLecturasAgua(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<List<DashboardLectura>> GetLecturas(int anio,int user)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getLecturasAgua", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@anio", anio));
                        cmd.Parameters.Add(new SqlParameter("@user", user));
                        var response = new List<DashboardLectura>();
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

        public async Task<List<LecturaAgua>> GetLecturasByInmueble(int anio, int inmueble)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getLecturasAguaByInmueble", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@anio", anio));
                        cmd.Parameters.Add(new SqlParameter("@inmueble", inmueble));
                        var response = new List<LecturaAgua>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueInmueble(reader));
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

        public async Task<LecturaAgua> GetLecturaById(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getLecturasAguaById", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        var response = new LecturaAgua();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueInmueble(reader);
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

        public async Task<int> insertaLectura(LecturaAgua lectura)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaLecturaAgua", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", lectura.Id)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@usuario", lectura.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@inmueble", lectura.InmuebleId));
                        cmd.Parameters.Add(new SqlParameter("@lectura", lectura.Lectura));
                        cmd.Parameters.Add(new SqlParameter("@ubicacion", lectura.Ubicacion));
                        cmd.Parameters.Add(new SqlParameter("@cuenta", lectura.Cuenta));
                        cmd.Parameters.Add(new SqlParameter("@mes", lectura.Mes));
                        cmd.Parameters.Add(new SqlParameter("@anio", lectura.Anio));
                        if (lectura.Estatus != null && lectura.Estatus.Equals(""))
                        {
                            cmd.Parameters.Add(new SqlParameter("@estatus", lectura.Estatus));
                        }
                        cmd.Parameters.Add(new SqlParameter("@fechaRegistro", lectura.FechaRegistro));
                        cmd.Parameters.Add(new SqlParameter("@Observaciones", lectura.Observaciones));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        int id = Convert.ToInt32(cmd.Parameters["@id"].Value);
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

        public async Task<int> actualizaLectura(LecturaAgua lectura)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("actualizaLecturaAgua", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", lectura.Id));
                        cmd.Parameters.Add(new SqlParameter("@usuario", lectura.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@inmueble", lectura.InmuebleId));
                        cmd.Parameters.Add(new SqlParameter("@lectura", lectura.Lectura));
                        cmd.Parameters.Add(new SqlParameter("@ubicacion", lectura.Ubicacion));
                        cmd.Parameters.Add(new SqlParameter("@cuenta", lectura.Cuenta));
                        if (lectura.Estatus != null && !lectura.Estatus.Equals(""))
                        {
                            cmd.Parameters.Add(new SqlParameter("@estatus", lectura.Estatus));
                        }
                        cmd.Parameters.Add(new SqlParameter("@mes", lectura.Mes));
                        cmd.Parameters.Add(new SqlParameter("@anio", lectura.Anio));
                        cmd.Parameters.Add(new SqlParameter("@fechaRegistro", lectura.FechaRegistro));
                        cmd.Parameters.Add(new SqlParameter("@observaciones", lectura.Observaciones));

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

        public async Task<int> eliminaLectura(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("eliminaLecturaAgua", sql))
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
                return -1;
            }
        }

        private LecturaAgua MapToValueInmueble(SqlDataReader reader)
        {
            return new LecturaAgua {
                Id = reader["Id"] != DBNull.Value ? (int)reader["Id"] : 0,
                UsuarioId = reader["UsuarioId"] != DBNull.Value ? (int)reader["UsuarioId"] : 0,
                InmuebleId = reader["InmuebleId"] != DBNull.Value ? (int)reader["InmuebleId"] : 0,
                Anio = reader["Anio"] != DBNull.Value ? (int)reader["Anio"] : 0,
                Mes = reader["Mes"] != DBNull.Value ? reader["Mes"].ToString() : "",
                Cuenta = reader["Cuenta"] != DBNull.Value ? reader["Cuenta"].ToString() : "",
                Estatus = reader["Estatus"] != DBNull.Value ? reader["Estatus"].ToString() : "",
                Lectura = reader["Lectura"] != DBNull.Value ? reader["Lectura"].ToString() : "",
                Fondo = reader["Fondo"] != DBNull.Value ? reader["Fondo"].ToString() : "",
                Ubicacion = reader["Ubicacion"] != DBNull.Value ? reader["Ubicacion"].ToString() : "",
                FechaRegistro = reader["FechaRegistro"] != DBNull.Value ? Convert.ToDateTime(reader["FechaRegistro"]) : Convert.ToDateTime("01-01-1991T00:00:00"),
                Observaciones = reader["Observaciones"] != DBNull.Value ? reader["Observaciones"].ToString() : "",
                FechaCreacion = reader["FechaCreacion"] != DBNull.Value ? (DateTime)reader["FechaCreacion"]: Convert.ToDateTime("01-01-1991T00:00:00"),
                FechaActualizacion = reader["FechaActualizacion"] != DBNull.Value ? (DateTime)reader["FechaActualizacion"] :
                    reader["FechaCreacion"] != DBNull.Value ? (DateTime)reader["FechaCreacion"] : Convert.ToDateTime("01-01-1991T00:00:00"),
            };
        }

        private DashboardLectura MapToValue(SqlDataReader reader)
        {
            return new DashboardLectura
            { 
                InmuebleId = reader["InmuebleId"] != DBNull.Value ? (int)reader["InmuebleId"] : 0,
                TotalLecturas = reader["TotalLecturas"] != DBNull.Value ? (int)reader["TotalLecturas"] : 0,
                Inmueble = reader["Inmueble"] != DBNull.Value ? reader["Inmueble"].ToString() : "",
                Fondo = reader["Fondo"] != DBNull.Value ? reader["Fondo"].ToString() : "",
                FondoHexadecimal = reader["FondoHexadecimal"] != DBNull.Value ? reader["FondoHexadecimal"].ToString() : "",
            };
        }
    }
}
