using CedulasEvaluacion.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using CedulasEvaluacion.Entities.MIncidencias;
using CedulasEvaluacion.Entities.Vistas;

namespace CedulasEvaluacion.Repositories
{
    public class RepositorioIncidenciasMensajeria : IRepositorioIncidenciasMensajeria
    {

        private readonly string _connectionString;

        public RepositorioIncidenciasMensajeria(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<List<IncidenciasMensajeria>> getIncidenciasMensajeria(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasMensajeria", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        var response = new List<IncidenciasMensajeria>();
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

        public async Task<List<IncidenciasMensajeria>> getIncidenciasByTipoMensajeria(int id, string tipo)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasByTipoMensajeria", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        cmd.Parameters.Add(new SqlParameter("@tipo", tipo));
                        var response = new List<IncidenciasMensajeria>();
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

        public async Task<int> IncidenciasExcel(IncidenciasMensajeria incidenciasMensajeria)
        {
            int success = 0;
            string file = await generaTXT(incidenciasMensajeria);
            if (!file.Equals(""))
            {
                try
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertaIncidenciasExcel", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@success", SqlDbType.BigInt)).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(new SqlParameter("@cedulaMensajeria", incidenciasMensajeria.CedulaMensajeriaId));
                            cmd.Parameters.Add(new SqlParameter("@file", file));

                            await sql.OpenAsync();
                            await cmd.ExecuteNonQueryAsync();
                            Directory.Delete((Path() + incidenciasMensajeria.Folio), true);

                            success = Convert.ToInt32(cmd.Parameters["@success"].Value);
                        }
                    }
                    return success;
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    Directory.Delete((Path() + incidenciasMensajeria.Folio), true);
                    return -1;
                }
            } else
            {
                Directory.Delete((Path() + incidenciasMensajeria.Folio), true);
                return -1;
            }
        }
        

        public async Task<int> IncidenciasMensajeria(IncidenciasMensajeria incidenciasMensajeria)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaIncidenciasMensajeria", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@cedulaMensajeria", incidenciasMensajeria.CedulaMensajeriaId));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", incidenciasMensajeria.Pregunta));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidenciasMensajeria.Tipo));
                        if (!incidenciasMensajeria.FechaProgramada.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaProgramada", incidenciasMensajeria.FechaProgramada));
                        if (!incidenciasMensajeria.FechaEfectiva.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaEfectiva", incidenciasMensajeria.FechaEfectiva));
                        if (!incidenciasMensajeria.CodigoRastreo.Equals(""))
                            cmd.Parameters.Add(new SqlParameter("@codigoRastreo", incidenciasMensajeria.CodigoRastreo));
                        if (!incidenciasMensajeria.NumeroGuia.Equals(""))
                            cmd.Parameters.Add(new SqlParameter("@numeroGuia", incidenciasMensajeria.NumeroGuia));
                        if (!incidenciasMensajeria.Sobrepeso.Equals(""))
                            cmd.Parameters.Add(new SqlParameter("@sobrepeso", incidenciasMensajeria.Sobrepeso));
                        if (!incidenciasMensajeria.TipoServicio.Equals(""))
                            cmd.Parameters.Add(new SqlParameter("@tipoServicio", incidenciasMensajeria.TipoServicio));
                        if (incidenciasMensajeria.TotalAcuses != 0)
                            cmd.Parameters.Add(new SqlParameter("@totalAcuses", incidenciasMensajeria.TotalAcuses));
                        

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

        public async Task<int> IncidenciaRobada(IncidenciasMensajeria incidenciasMensajeria)
        {
            DateTime date = DateTime.Now;
            string date_str = date.ToString("yyyyMMddHHmmss");
            int id = 0;

            string saveFile = await guardaArchivo(incidenciasMensajeria.ActaRobo, incidenciasMensajeria.Folio, date_str,incidenciasMensajeria.Tipo);

            try
            {
                if (saveFile.Equals("Ok"))
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertaIncidenciasMensajeria", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt)).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(new SqlParameter("@cedulaMensajeria", incidenciasMensajeria.CedulaMensajeriaId));
                            cmd.Parameters.Add(new SqlParameter("@pregunta", incidenciasMensajeria.Pregunta));
                            cmd.Parameters.Add(new SqlParameter("@tipo", incidenciasMensajeria.Tipo));
                            cmd.Parameters.Add(new SqlParameter("@actaRobo", (date_str + "_" + incidenciasMensajeria.ActaRobo.FileName)));
                            if (!incidenciasMensajeria.CodigoRastreo.Equals(""))
                                cmd.Parameters.Add(new SqlParameter("@codigoRastreo", incidenciasMensajeria.CodigoRastreo));
                            if (!incidenciasMensajeria.NumeroGuia.Equals(""))
                                cmd.Parameters.Add(new SqlParameter("@numeroGuia", incidenciasMensajeria.NumeroGuia));
                            if (!incidenciasMensajeria.TipoServicio.Equals(""))
                                cmd.Parameters.Add(new SqlParameter("@tipoServicio", incidenciasMensajeria.TipoServicio));

                            await sql.OpenAsync();
                            await cmd.ExecuteNonQueryAsync();

                            id = Convert.ToInt32(cmd.Parameters["@id"].Value);
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

        public async Task<int> ActualizaIncidencia(IncidenciasMensajeria incidenciasMensajeria)
        {
            if (incidenciasMensajeria.Id != 0)
            {
                int isDeleted = await eliminaArchivo(incidenciasMensajeria);
            }

            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaIncidenciaMensajeria", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", incidenciasMensajeria.Id));
                        cmd.Parameters.Add(new SqlParameter("@cedulaMensajeria", incidenciasMensajeria.CedulaMensajeriaId));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", incidenciasMensajeria.Pregunta));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidenciasMensajeria.Tipo));
                        if (!incidenciasMensajeria.FechaProgramada.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaProgramada", incidenciasMensajeria.FechaProgramada));
                        if (!incidenciasMensajeria.FechaEfectiva.ToShortDateString().Equals("01/01/1990"))
                            cmd.Parameters.Add(new SqlParameter("@fechaEfectiva", incidenciasMensajeria.FechaEfectiva));
                        if (!incidenciasMensajeria.CodigoRastreo.Equals(""))
                            cmd.Parameters.Add(new SqlParameter("@codigoRastreo", incidenciasMensajeria.CodigoRastreo));
                        if (!incidenciasMensajeria.NumeroGuia.Equals(""))
                            cmd.Parameters.Add(new SqlParameter("@numeroGuia", incidenciasMensajeria.NumeroGuia));
                        if (!incidenciasMensajeria.Sobrepeso.Equals(""))
                            cmd.Parameters.Add(new SqlParameter("@sobrepeso", incidenciasMensajeria.Sobrepeso));
                        if (!incidenciasMensajeria.TipoServicio.Equals(""))
                            cmd.Parameters.Add(new SqlParameter("@tipoServicio", incidenciasMensajeria.TipoServicio));
                        if (incidenciasMensajeria.TotalAcuses != 0)
                            cmd.Parameters.Add(new SqlParameter("@totalAcuses", incidenciasMensajeria.TotalAcuses));

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

        public async Task<int> ActualizaRobada(IncidenciasMensajeria incidenciasMensajeria)
        {
            DateTime date = DateTime.Now;
            string date_str = date.ToString("yyyyMMddHHmmss");
            int id = 0;

            if (incidenciasMensajeria.Id != 0)
            {
                int isDeleted = await eliminaArchivo(incidenciasMensajeria);
            }

            string saveFile = await guardaArchivo(incidenciasMensajeria.ActaRobo, incidenciasMensajeria.Folio, date_str,incidenciasMensajeria.Tipo);

            try
            {
                if (saveFile.Equals("Ok"))
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_actualizaIncidenciaMensajeria", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@id", incidenciasMensajeria.Id));
                            cmd.Parameters.Add(new SqlParameter("@cedulaMensajeria", incidenciasMensajeria.CedulaMensajeriaId));
                            cmd.Parameters.Add(new SqlParameter("@pregunta", incidenciasMensajeria.Pregunta));
                            cmd.Parameters.Add(new SqlParameter("@tipo", incidenciasMensajeria.Tipo));
                            cmd.Parameters.Add(new SqlParameter("@actaRobo", (date_str + "_" + incidenciasMensajeria.ActaRobo.FileName)));
                            if (!incidenciasMensajeria.CodigoRastreo.Equals(""))
                                cmd.Parameters.Add(new SqlParameter("@codigoRastreo", incidenciasMensajeria.CodigoRastreo));
                            if (!incidenciasMensajeria.NumeroGuia.Equals(""))
                                cmd.Parameters.Add(new SqlParameter("@numeroGuia", incidenciasMensajeria.NumeroGuia));
                            if (!incidenciasMensajeria.TipoServicio.Equals(""))
                                cmd.Parameters.Add(new SqlParameter("@tipoServicio", incidenciasMensajeria.TipoServicio));

                            await sql.OpenAsync();
                            await cmd.ExecuteNonQueryAsync();

                            id = 1;
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

        public async Task<int> EliminaIncidencia(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminarIncidenciaMensajeria", sql))
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

        public async Task<List<IncidenciasMensajeria>> TotalIncidencias(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_totalIncidenciasMensajeria", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        var response = new List<IncidenciasMensajeria>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueTotal(reader));
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

        public async Task<List<IncidenciasMensajeria>> TotalIncidenciasByPregunta(int id,int pregunta)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasMensajeriaByPregunta", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", pregunta));
                        var response = new List<IncidenciasMensajeria>();
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

        public async Task<int> EliminaTodaIncidencia(int id,string tipo)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaTodaIncidenciaMensajeria", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@tipo", tipo));

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

        public async Task<int> IncidenciasTipo(int id, string tipo)
        {
            int total = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_incidenciasTipo", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@total", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@tipo", tipo));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        total = (int)cmd.Parameters["@total"].Value;
                        return total;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }

        /*Seguimientos Estafeta*/
        public async Task<List<VSeguimiento>> getSeguimientosEstafeta()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getSeguimientosEstafeta", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        var response = new List<VSeguimiento>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueSeguimientos(reader));
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

        public async Task<VSeguimiento> getSeguimientosEstafetaById(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getSeguimientosEstafetaById", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedula", id));
                        var response = new VSeguimiento();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueSeguimientos(reader);
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
        public async Task<List<IncidenciasMensajeria>> getIncidenciasMensajeriaSeguimiento(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasMensajeriaSeguimiento", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedula", id));
                        var response = new List<IncidenciasMensajeria>();
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

        public async Task<List<ReferenciaPago>> getReferenciasPago(int incidencia)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getReferenciasPago", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@incidencia", incidencia));
                        var response = new List<ReferenciaPago>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueReferencias(reader));
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

        public async Task<int> InsertaReferenciasSeguimiento(ReferenciaPago referencia)
        {
            int success = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaReferenciasSeguimiento", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@incidenciaId", referencia.IncidenciaId));
                        cmd.Parameters.Add(new SqlParameter("@referencia", referencia.NumeroReferencia));
                        cmd.Parameters.Add(new SqlParameter("@contrato", referencia.Contrato));
                        cmd.Parameters.Add(new SqlParameter("@cantidad", referencia.Cantidad));
                        cmd.Parameters.Add(new SqlParameter("@precioUnitario", referencia.PrecioUnitario));
                        cmd.Parameters.Add(new SqlParameter("@archivoReferencia", await guardaReferenciasPago(referencia.AReferencia, referencia.NumeroGuia, referencia.FolioCedula,
                                                                                                                DateTime.Now.ToString("yyyyMMddHHmmss"))));
                        //cmd.Parameters.Add(new SqlParameter("@reportePago", await guardaReferenciasPago(referencia.APago, referencia.NumeroGuia, DateTime.Now.ToLongDateString())));    
                        //cmd.Parameters.Add(new SqlParameter("@fechaEmision", referencia.FechaEmision));
                        cmd.Parameters.Add(new SqlParameter("@fechaVencimiento", referencia.FechaVencimiento));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        success = Convert.ToInt32(cmd.Parameters["@id"].Value);
                    }
                }
                return success;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }
        
        public async Task<int> actualizaReferenciasSeguimiento(ReferenciaPago referencia)
        {
            int success = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaReferenciasSeguimiento", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", referencia.Id));
                        cmd.Parameters.Add(new SqlParameter("@incidenciaId", referencia.IncidenciaId));
                        cmd.Parameters.Add(new SqlParameter("@referencia", referencia.NumeroReferencia));
                        cmd.Parameters.Add(new SqlParameter("@contrato", referencia.Contrato));
                        cmd.Parameters.Add(new SqlParameter("@cantidad", referencia.Cantidad));
                        cmd.Parameters.Add(new SqlParameter("@precioUnitario", referencia.PrecioUnitario));
                        if(referencia.AReferencia != null)
                            cmd.Parameters.Add(new SqlParameter("@archivoReferencia", await guardaReferenciasPago(referencia.AReferencia, referencia.NumeroGuia, referencia.FolioCedula,
                                                                                                                DateTime.Now.ToString("yyyyMMddHHmmss"))));
                        else
                            cmd.Parameters.Add(new SqlParameter("@archivoReferencia", referencia.ArchivoReferencia));

                        cmd.Parameters.Add(new SqlParameter("@fechaVencimiento", referencia.FechaVencimiento));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return referencia.Id;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }

        public async Task<int> insertaReciboPago(ReferenciaPago referencia)
        {
            int success = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaReferenciaPago", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", referencia.Id));
                        if (referencia.APago != null)
                            cmd.Parameters.Add(new SqlParameter("@reportePago", await guardaReferenciasPago(referencia.APago, referencia.NumeroGuia, referencia.FolioCedula,
                                                                                                                DateTime.Now.ToString("yyyyMMddHHmmss"))));
                        else
                            cmd.Parameters.Add(new SqlParameter("@reportePago", referencia.ReportePago));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return referencia.Id;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }

        public async Task<string> guardaReferenciasPago(IFormFile archivo, string guia, string cedula, string date)
        {
            long size = archivo.Length;
            string folderCedula = cedula;

            string newPath = Directory.GetCurrentDirectory() + "\\Seguimientos Estafeta\\" + folderCedula+"\\"+guia;
            
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            string file = date + "_" + archivo.FileName;

            using (var stream = new FileStream(newPath + "\\" + file, FileMode.Create))
            {
                try
                {
                    await (archivo).CopyToAsync(stream);
                    return file;
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }
            }
        }

        public async Task<string> guardaActas(IFormFile archivo, string guia, string cedula, string date)
        {
            long size = archivo.Length;
            string folderCedula = cedula;

            string newPath = Directory.GetCurrentDirectory() + "\\Seguimientos Estafeta\\" + folderCedula;

            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            string file = date + "_" + archivo.FileName;

            using (var stream = new FileStream(newPath + "\\" + file, FileMode.Create))
            {
                try
                {
                    await (archivo).CopyToAsync(stream);
                    return file;
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }
            }
        }


        /* Metodo que copia el Excel */
        public async Task<string> guardaExcel(IFormFile archivo, string folio, string date)
        {
            long size = archivo.Length;
            string folderCedula = folio;

            string newPath = Path() + folderCedula;
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            using (var stream = new FileStream(newPath + "\\" + (date + "_" + archivo.FileName), FileMode.Create))
            {
                try
                {
                    await (archivo).CopyToAsync(stream);
                    return (date + "_" + archivo.FileName);
                }
                catch (Exception ex)
                {
                    string msg = ex.Message.ToString();
                    return null;
                }
            }
        }
        /* Fin del metodo que copia el Excel */

        /*Metodo que valida que el excel este correctamente llenado*/
        public bool verificaExcel(string file,string folio)
        {
            //(Path() + folio + "\\" + file)
            bool complete = true;
            var fileName = (Path() + folio + "\\" + file);
            using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    string mm = "";
                    int header = 0;
                    //recorremos cada fila
                    while (reader.Read())
                    {
                        if (header != 0)
                        {
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                if (Convert.ToString(reader.GetValue(i)).Equals(""))
                                {
                                    complete = true;
                                    break;
                                }
                            }
                            mm += "\n";
                            if (complete == false)
                            {
                                break;
                            }
                        }
                        else
                        {
                            header = 1;
                        }
                    }
                }
            }
            return complete;
        }
        /*Fin Metodo que valida que el excel este correctamente llenado*/

        private async Task<string> generaTXT(IncidenciasMensajeria incidenciasMensajeria)
        {
            DateTime date = DateTime.Now;
            string date_str = date.ToString("yyyyMMddHHmmss");
            string headers = "";
            string text = "";
            string txt = "";
            int pregunta = buscaTipo(incidenciasMensajeria.Tipo);
            string path = await guardaExcel(incidenciasMensajeria.Excel, incidenciasMensajeria.Folio, date_str); //devuelve el nombre del archivo
            if (verificaExcel(path,incidenciasMensajeria.Folio) == true)
            {
                txt = path.Replace(".xlsx",".txt");
                FileInfo tfile = new FileInfo((Path() + "\\" + incidenciasMensajeria.Folio + "\\" +txt));
                StreamWriter fichero = tfile.CreateText();
                headers = "CedulaMensajeriaId\tPregunta\tTipo\tFechaProgramada\tFechaEfectiva\tNumeroGuia\tCodigoRastreo\tSobrepeso\tAcuse\tTipoServicio\tValidado\tArchivo\n";
                using (var stream = File.Open((Path() + "\\" + incidenciasMensajeria.Folio + "\\" + path), FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        int header = 0;
                        //recorremos cada fila
                        while (reader.Read())
                        {
                            if (header != 0)
                            {
                                text += incidenciasMensajeria.CedulaMensajeriaId + "\t" + pregunta+"\t"+incidenciasMensajeria.Tipo + "\t";
                                for (var i = 0; i < reader.FieldCount; i++)
                                {
                                    if (Convert.ToString(reader.GetValue(i)).Equals(""))
                                    {
                                        text += "NULL"+"\t";
                                    }
                                    else
                                    {
                                        if (Convert.ToString(reader.GetValue(i)).Contains("/")) {
                                            text += Convert.ToDateTime(reader.GetValue(i)).ToString("yyyy-MM-ddTHH:mm:ss") + "\t";
                                        }
                                        else
                                        {
                                            text += Convert.ToString(reader.GetValue(i)) + "\t";
                                        }
                                    }
                                    
                                }
                                text += txt+"\n";
                            }
                            else
                            {
                                text += headers;
                                header = 1;
                            }
                        }
                        fichero.Write(text);
                    }
                }

                fichero.Close();
                return txt;
            }
            else
            {
                return "";
            }
        }

        private IncidenciasMensajeria MapToValue(SqlDataReader reader)
        {
            return new IncidenciasMensajeria
            {
                Id = (int)reader["Id"],
                CedulaMensajeriaId = (int)reader["CedulaMensajeriaId"],
                Pregunta = reader["Pregunta"] != DBNull.Value ? (int)reader["Pregunta"] :0,
                Sobrepeso = reader["Sobrepeso"] != DBNull.Value ? (decimal)reader["Sobrepeso"]:0,
                Tipo = reader["Tipo"] != DBNull.Value ? reader["Tipo"].ToString():"",
                NumeroGuia = reader["NumeroGuia"] != DBNull.Value ? reader["NumeroGuia"].ToString(): "",
                NombreActa = reader["ActaRobo"] != DBNull.Value ? reader["ActaRobo"].ToString(): "",
                CodigoRastreo = reader["CodigoRastreo"] != DBNull.Value ? Convert.ToInt64(reader["CodigoRastreo"]):0,
                Acuse = reader["Acuse"] != DBNull.Value ? reader["Acuse"].ToString(): "N/A",
                TotalAcuses = reader["TotalAcuses"] != DBNull.Value ? Convert.ToInt32(reader["TotalAcuses"]): 0,
                TipoServicio = reader["TipoServicio"] != DBNull.Value ? reader["TipoServicio"].ToString(): "",
                FechaProgramada = reader["FechaProgramada"] != DBNull.Value ? Convert.ToDateTime(reader["FechaProgramada"]): DateTime.Now,
                FechaEfectiva = reader["FechaEfectiva"] != DBNull.Value ? Convert.ToDateTime(reader["FechaEfectiva"]) : DateTime.Now,
                MontoPenalizacion = reader["MontoPenalizacion"] != DBNull.Value ? Convert.ToDecimal(reader["MontoPenalizacion"]) : 0
            };
        }

        private IncidenciasMensajeria MapToValueTotal(SqlDataReader reader)
        {
            return new IncidenciasMensajeria
            {
                TotalIncidencias = (int)reader["TotalIncidencias"],
                Tipo = reader["Tipo"].ToString(),
            };
        }        
        private VSeguimiento MapToValueSeguimientos(SqlDataReader reader)
        {
            return new VSeguimiento { 
                Id = (int)reader["Id"],
                CedulaId = (int)reader["CedulaId"],
                Anio = (int)reader["Anio"],
                Mes = reader["Mes"].ToString(),
                Folio = reader["Folio"].ToString(),
                Inmueble = reader["Inmueble"].ToString(),
                EstatusCedula = reader["EstatusCedula"].ToString(),
                EstatusSeguimiento = reader["EstatusSeguimiento"].ToString(),
                Usuario = reader["Usuario"].ToString(),
                TotalIncidencias = (int)reader["TotalIncidencias"]
            };
        }
        private ReferenciaPago MapToValueReferencias(SqlDataReader reader)
        {
            return new ReferenciaPago
            { 
                Id = (int)reader["Id"],
                IncidenciaId = reader["IncidenciaId"] != DBNull.Value ? (int)reader["IncidenciaId"]:0,
                NumeroReferencia = reader["Referencia"] != DBNull.Value ? reader["Referencia"].ToString():"",
                Contrato = reader["Contrato"] != DBNull.Value ? reader["Contrato"].ToString():"",
                Cantidad = reader["Cantidad"] != DBNull.Value ? (int)reader["Cantidad"]:0,
                PrecioUnitario = reader["PrecioUnitario"] != DBNull.Value ? (decimal)reader["PrecioUnitario"]:0,
                ArchivoReferencia = reader["ArchivoReferencia"] != DBNull.Value ? reader["ArchivoReferencia"].ToString():"",
                ReportePago = reader["ReportePago"] != DBNull.Value ? reader["ReportePago"].ToString():"",
                Estatus = reader["Estatus"] != DBNull.Value ? reader["Estatus"].ToString():"",
                FechaEmision = reader["FechaEmision"] != DBNull.Value ? Convert.ToDateTime(reader["FechaEmision"]): Convert.ToDateTime("01/01/1990"),
                FechaVencimiento = reader["FechaVencimiento"] != DBNull.Value ? Convert.ToDateTime(reader["FechaVencimiento"]) : Convert.ToDateTime("01/01/1990"),
                FechaCreacion = reader["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaCreacion"]) : Convert.ToDateTime("01/01/1990"),
            };
        }

        private int buscaTipo(string tipo)
        {
            string [] tInci = { "Recoleccion", "Entrega", "Acuses", "Mal Estado", "Extraviadas", "Robadas" };
            for (var i = 0; i< tInci.Length; i++)
            {
                if (tInci[i].Equals(tipo))
                {
                    return (i + 1);
                }
            }
            return 0;
        }

        /*Privates*/
        private string Path () {
            return Directory.GetCurrentDirectory() + "\\IncidenciasMensajeria\\"; 
        }


        /****************** Guarda el Adjunto del Acta de Robo *******************/
        public async Task<string> guardaArchivo(IFormFile archivo, string folio, string date,string tipo)
        {
            long size = archivo.Length;
            string folderCedula = folio;

            string newPath = Directory.GetCurrentDirectory() + "\\Entregables\\" + folderCedula;
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (tipo.Equals("Robadas"))
            {
                newPath = newPath + "\\Actas de Robo";
            }else if (tipo.Equals("Extraviadas"))
            {
                newPath = newPath + "\\Actas de Extravío";
            }

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

        public async Task<int> eliminaArchivo(IncidenciasMensajeria incidenciasMensajeria)
        {
            string newPath = "";
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaActaMensajeria", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", incidenciasMensajeria.Id));
                        cmd.Parameters.Add(new SqlParameter("@archivo", SqlDbType.VarChar, 500)).Direction = System.Data.ParameterDirection.Output;
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        string archivo = (cmd.Parameters["@archivo"].Value).ToString();
                        if (incidenciasMensajeria.Tipo.Equals("Robadas"))
                        {
                            newPath = Directory.GetCurrentDirectory() + "\\Entregables\\" + incidenciasMensajeria.Folio + "\\Actas de Robo\\" + archivo;
                        }else if (incidenciasMensajeria.Tipo.Equals("Extraviadas"))
                        {
                            newPath = Directory.GetCurrentDirectory() + "\\Entregables\\" + incidenciasMensajeria.Folio + "\\Actas de Extravío\\" + archivo;
                        }
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
    }
}
