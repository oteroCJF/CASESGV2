using CedulasEvaluacion.Entities.MContratos;
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
    public class RepositorioContratosServicio : IRepositorioContratosServicio
    {
        private readonly string _connectionString;

        public RepositorioContratosServicio(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }
        public async Task<List<ContratosServicio>> GetContratosServicios(int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getContratosByServicio", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<ContratosServicio>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueContratos(reader));
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
        public async Task<ContratosServicio> GetContratoServicioActivo(int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getContratoServicioActivo", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new ContratosServicio();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueContratos(reader);
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
        public async Task<ContratosServicio> GetContratoServicioById(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getContratoServicioById", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        var response = new ContratosServicio();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueContratos(reader);
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
        public async Task<List<ConveniosContrato>> GetConveniosByContrato(int contrato)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getConveniosByContrato", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        var response = new List<ConveniosContrato>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueConvenios(reader));
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
        public async Task<int> InsertaContrato(ContratosServicio contratosServicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaContratoServicio", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", contratosServicio.Id)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@usuarioId", contratosServicio.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@servicioId", contratosServicio.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@contrato", contratosServicio.NumeroContrato));
                        cmd.Parameters.Add(new SqlParameter("@empresa", contratosServicio.Empresa));
                        cmd.Parameters.Add(new SqlParameter("@rfc", contratosServicio.RFC));
                        cmd.Parameters.Add(new SqlParameter("@direccion", contratosServicio.Direccion));
                        cmd.Parameters.Add(new SqlParameter("@representante", contratosServicio.Representante));
                        cmd.Parameters.Add(new SqlParameter("@fechaInicio", contratosServicio.FechaInicio));
                        cmd.Parameters.Add(new SqlParameter("@fechaFin", contratosServicio.FechaFin));
                        cmd.Parameters.Add(new SqlParameter("@activo", contratosServicio.Activo));
                        cmd.Parameters.Add(new SqlParameter("@montoMin", contratosServicio.MontoMin));
                        cmd.Parameters.Add(new SqlParameter("@montoMax", contratosServicio.MontoMax));
                        cmd.Parameters.Add(new SqlParameter("@volumetriaMin", contratosServicio.VolumetriaMin));
                        cmd.Parameters.Add(new SqlParameter("@volumetriaMax", contratosServicio.VolumetriaMax));


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
        public async Task<int> ActualizaContrato(ContratosServicio contratosServicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaContratoServicio", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", contratosServicio.Id));
                        cmd.Parameters.Add(new SqlParameter("@usuarioId", contratosServicio.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@servicioId", contratosServicio.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@contrato", contratosServicio.NumeroContrato));
                        cmd.Parameters.Add(new SqlParameter("@empresa", contratosServicio.Empresa));
                        cmd.Parameters.Add(new SqlParameter("@rfc", contratosServicio.RFC));
                        cmd.Parameters.Add(new SqlParameter("@direccion", contratosServicio.Direccion));
                        cmd.Parameters.Add(new SqlParameter("@representante", contratosServicio.Representante));
                        cmd.Parameters.Add(new SqlParameter("@fechaInicio", contratosServicio.FechaInicio));
                        cmd.Parameters.Add(new SqlParameter("@fechaFin", contratosServicio.FechaFin));
                        cmd.Parameters.Add(new SqlParameter("@activo", contratosServicio.Activo));
                        cmd.Parameters.Add(new SqlParameter("@montoMin", contratosServicio.MontoMin));
                        cmd.Parameters.Add(new SqlParameter("@montoMax", contratosServicio.MontoMax));
                        cmd.Parameters.Add(new SqlParameter("@volumetriaMin", contratosServicio.VolumetriaMin));
                        cmd.Parameters.Add(new SqlParameter("@volumetriaMax", contratosServicio.VolumetriaMax));


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
        public async Task<int> InsertaConvenio(ConveniosContrato convenio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaConvenioContrato", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", convenio.Id)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@contratoId", convenio.ContratoId));
                        cmd.Parameters.Add(new SqlParameter("@contrato", convenio.NoContrato));
                        cmd.Parameters.Add(new SqlParameter("@fechaInicial", convenio.FechaInicio));
                        cmd.Parameters.Add(new SqlParameter("@fechaFinal", convenio.FechaFin));
                        cmd.Parameters.Add(new SqlParameter("@montoMin", convenio.MontoMin));
                        cmd.Parameters.Add(new SqlParameter("@montoMax", convenio.MontoMax));
                        cmd.Parameters.Add(new SqlParameter("@volumetriaMin", convenio.VolumetriaMin));
                        cmd.Parameters.Add(new SqlParameter("@volumetriaMax", convenio.VolumetriaMax));
                        cmd.Parameters.Add(new SqlParameter("@observaciones", convenio.Observaciones));


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
        public async Task<int> ActualizaConvenio(ConveniosContrato convenio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizarConvenioContrato", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", convenio.Id));
                        cmd.Parameters.Add(new SqlParameter("@contratoId", convenio.ContratoId));
                        cmd.Parameters.Add(new SqlParameter("@contrato", convenio.NoContrato));
                        cmd.Parameters.Add(new SqlParameter("@fechaInicial", convenio.FechaInicio));
                        cmd.Parameters.Add(new SqlParameter("@fechaFinal", convenio.FechaFin));
                        cmd.Parameters.Add(new SqlParameter("@montoMin", convenio.MontoMin));
                        cmd.Parameters.Add(new SqlParameter("@montoMax", convenio.MontoMax));
                        cmd.Parameters.Add(new SqlParameter("@volumetriaMin", convenio.VolumetriaMin));
                        cmd.Parameters.Add(new SqlParameter("@volumetriaMax", convenio.VolumetriaMax));
                        cmd.Parameters.Add(new SqlParameter("@observaciones", convenio.Observaciones));


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
        public async Task<int> InsertarActualizarDocumentacion(EntregablesContrato entregables)
        {
            DateTime date = DateTime.Now;
            string date_str = date.ToString("yyyyMMddHHmmss");
            int id = 0;
            string saveFile = "Ok";

            if (entregables.Archivo != null)
            {
                int isDeleted = await eliminaArchivo(entregables);
                saveFile = await guardaArchivo(entregables.Archivo, entregables.Id + "", date_str);
            }

            try
            {
                if (saveFile.Equals("Ok"))
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertarActualizarDocumentacionContrato", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@id", entregables.Id));
                            cmd.Parameters.Add(new SqlParameter("@tipo", entregables.Tipo));
                            if (entregables.Archivo != null)
                            {
                                cmd.Parameters.Add(new SqlParameter("@archivo", (date_str + "_" + entregables.Archivo.FileName)));
                            }

                            await sql.OpenAsync();
                            id = await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                return id;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }
        public async Task<string> guardaArchivo(IFormFile archivo, string folio, string date)
        {
            long size = archivo.Length;
            string folderCedula = folio;

            string newPath = Directory.GetCurrentDirectory() + "\\ObligacionesPS\\Contrato_" + folderCedula+"\\Documentacion";
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            using (var stream = new FileStream(newPath + "\\" + (date + "_" + archivo.FileName), FileMode.Create))
            {
                try
                {
                    await (archivo).CopyToAsync(stream);
                    return "Ok";
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }
            }
        }
        public async Task<int> eliminaArchivo(EntregablesContrato entregable)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaDocumentacionContrato", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", entregable.Id));
                        cmd.Parameters.Add(new SqlParameter("@tipo", entregable.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@archivo", SqlDbType.VarChar,1024)).Direction = ParameterDirection.Output;
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        string archivo = (cmd.Parameters["@archivo"].Value).ToString();
                        string newPath = Directory.GetCurrentDirectory() + "\\ObligacionesPS\\Contrato_" + entregable.Id + "\\Documentacion\\" + archivo;
                        File.Delete(newPath);

                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        public async Task<int> eliminarContrato(int contrato)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaContrato", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", contrato));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        public async Task<int> eliminarConvenio(int convenio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaConvenio", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", convenio));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        private ContratosServicio MapToValueContratos(SqlDataReader reader)
        {
            return new ContratosServicio
            {
                Id = (int)reader["Id"],
                ServicioId = (int)reader["ServicioId"],
                NumeroContrato = reader["NoContrato"] != DBNull.Value ? reader["NoContrato"].ToString() : "",
                Empresa = reader["Empresa"] != DBNull.Value ? reader["Empresa"].ToString() : "",
                RFC = reader["RFC"] != DBNull.Value ? reader["RFC"].ToString() : "",
                Direccion = reader["Direccion"] != DBNull.Value ? reader["Direccion"].ToString() : "",
                Representante = reader["Representante"] != DBNull.Value ? reader["Representante"].ToString() : "",
                ContratoPDF = reader["ContratoPDF"] != DBNull.Value ? reader["ContratoPDF"].ToString() : "",
                Anexos = reader["Anexos"] != DBNull.Value ? reader["Anexos"].ToString() : "",
                JuntaAclaraciones = reader["JuntaAclaraciones"] != DBNull.Value ? reader["JuntaAclaraciones"].ToString() : "",
                FechaInicio = reader["FechaInicio"] != DBNull.Value ? Convert.ToDateTime(reader["FechaInicio"]) : DateTime.Now,
                FechaFin = reader["FechaFin"] != DBNull.Value ? Convert.ToDateTime(reader["FechaFin"]) : DateTime.Now,
                Activo = reader["Activo"] != DBNull.Value ? (bool)reader["Activo"] : false,
                MontoMin = reader["MontoMin"] != DBNull.Value ? (decimal)reader["MontoMin"] : 0,
                MontoMax = reader["MontoMax"] != DBNull.Value ? (decimal)reader["MontoMax"] : 0,
                VolumetriaMin = reader["VolumetriaMin"] != DBNull.Value ? (int)reader["VolumetriaMin"] : 0,
                VolumetriaMax = reader["VolumetriaMax"] != DBNull.Value ? (int)reader["VolumetriaMax"] : 0,
            };
        }
        private ConveniosContrato MapToValueConvenios(SqlDataReader reader)
        {
            return new ConveniosContrato
            {
                Id = (int)reader["Id"],
                ContratoId= (int)reader["ContratoId"],
                NoContrato = reader["NoConvenio"] != DBNull.Value ? reader["NoConvenio"].ToString() : "",
                FechaInicio = reader["FechaInicio"] != DBNull.Value ? Convert.ToDateTime(reader["FechaInicio"]) : DateTime.Now,
                FechaFin = reader["FechaFinal"] != DBNull.Value ? Convert.ToDateTime(reader["FechaFinal"]) : DateTime.Now,
                MontoMin = reader["MontoMin"] != DBNull.Value ? (decimal)reader["MontoMin"] : 0,
                MontoMax = reader["MontoMax"] != DBNull.Value ? (decimal)reader["MontoMax"] : 0,
                VolumetriaMin = reader["VolumetriaMin"] != DBNull.Value ? (int)reader["VolumetriaMin"] : 0,
                VolumetriaMax = reader["VolumetriaMax"] != DBNull.Value ? (int) reader["VolumetriaMax"] : 0,
                Observaciones = reader["Observaciones"] != DBNull.Value ? reader["Observaciones"].ToString() : "",
            };
        }
        
    }
}
