using CedulasEvaluacion.Entities.MFirmantes;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Reportes;
using CedulasEvaluacion.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Repositories
{
    public class RepositorioFirmantes : IRepositorioFirmantes
    {
        private readonly string _connectionString;

        public RepositorioFirmantes(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }
        public async Task<List<FirmantesServicio>> GetInmueblesFirmante(int servicio, int usuario)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getInmueblesFirmantes", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@usuario", usuario));
                        var response = new List<FirmantesServicio>();
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
        public async Task<List<ReporteCedula>> GetFirmantesByCedula(int cedula, int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getFirmantesCedula", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<ReporteCedula>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueCedula(reader));
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
        public async Task<int> actualizaFirmantes(FirmantesServicio firmante)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaFirmantes", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", firmante.Id));
                        cmd.Parameters.Add(new SqlParameter("@escolaridad", firmante.Escolaridad));
                        cmd.Parameters.Add(new SqlParameter("@usuario", firmante.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@tipoServicio", firmante.TipoServicio));

                        await sql.OpenAsync();
                        int id = await cmd.ExecuteNonQueryAsync();
                        return id;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        public async Task<List<Usuarios>> GetUsuariosByAdministracion(int user)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getUsuariosByAdministracion", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@usuario", user));
                        var response = new List<Usuarios>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueUsuarios(reader));
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
        public async Task<int> GetVerificaFirmantes(string tipo, int inmueble, int servicio, int cedula)
        {
            int r = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getFirmanteTipo", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
                        cmd.Parameters.Add(new SqlParameter("@inmueble", inmueble));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@tipo", tipo));
                        var response = new List<Usuarios>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                r = 1;
                            }
                            return r;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }
        public async Task<int> insertaFirmante(FirmantesServicio firmante)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaFirmanteByCedula", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", firmante.Id)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", firmante.CedulaId));
                        cmd.Parameters.Add(new SqlParameter("@usuario", firmante.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@inmueble", firmante.InmuebleId));
                        cmd.Parameters.Add(new SqlParameter("@servicio", firmante.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@tipo", firmante.Tipo));

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
        private Usuarios MapToValueUsuarios(SqlDataReader reader)
        {
            return new Usuarios()
            {
                Id = (int)reader["Id"],
                NombreCompleto = reader["Empleado"].ToString(),
            };
        }
        private FirmantesServicio MapToValue(SqlDataReader reader)
        {
            return new FirmantesServicio
            {
                Id = reader["Id"] != DBNull.Value ? (int)reader["Id"] : 0,
                ServicioId = reader["ServicioId"] != DBNull.Value ? (int)reader["ServicioId"] : 0,
                InmuebleId = reader["InmuebleId"] != DBNull.Value ? (int)reader["InmuebleId"] : 0,
                UsuarioId = reader["UsuarioId"] != DBNull.Value ? (int)reader["UsuarioId"] : 0,
                Tipo = reader["Tipo"] != DBNull.Value ? reader["Tipo"].ToString() : "",
                TipoServicio = reader["TipoServicio"] != DBNull.Value ? reader["TipoServicio"].ToString() : "",
                Escolaridad = reader["Escolaridad"] != DBNull.Value ? reader["Escolaridad"].ToString() : ""
            };
        }
        private ReporteCedula MapToValueCedula(SqlDataReader reader)
        {
            return new ReporteCedula
            {
                Autoriza = reader["Autoriza"].ToString().Equals("C.P. Elizabeth Carreón Rios") || reader["Autoriza"].ToString().Equals("Lic. Mayte Itzel Vite Villa") ? 
                           CultureInfo.InvariantCulture.TextInfo.ToTitleCase(CultureInfo.InvariantCulture.TextInfo.ToLower(reader["Autoriza"].ToString()) + "\n Administradora") :
                           reader["Autoriza"].ToString().Equals("Mtra. Mónica Elisa Cervantes Ayala") ? CultureInfo.InvariantCulture.TextInfo.ToTitleCase(CultureInfo.InvariantCulture.TextInfo.ToLower(reader["Autoriza"].ToString()) + "\n Directora de Área") :
                           CultureInfo.InvariantCulture.TextInfo.ToTitleCase(CultureInfo.InvariantCulture.TextInfo.ToLower(reader["Autoriza"].ToString()) + "\n Administrador"),
                Reviso = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(CultureInfo.InvariantCulture.TextInfo.ToLower(reader["Reviso"].ToString())),
                PuestoReviso = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(CultureInfo.InvariantCulture.TextInfo.ToLower(reader["PuestoReviso"].ToString())),
                Superviso = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(CultureInfo.InvariantCulture.TextInfo.ToLower(reader["Superviso"].ToString())),
                PuestoSuperviso = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(CultureInfo.InvariantCulture.TextInfo.ToLower(reader["PuestoSuperviso"].ToString())),
            };
        }
    }
}
