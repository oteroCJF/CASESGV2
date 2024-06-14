using CASESGCedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Entities.MFinancieros;
using CedulasEvaluacion.Entities.Models;
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
    public class RepositorioFinancieros : IRepositorioFinancieros
    {
        private readonly string _connectionString;

        public RepositorioFinancieros(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }
        public async Task<List<DashboardFinancieros>> GetCedulasFinancieros()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCedulasFinancieros", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var response = new List<DashboardFinancieros>();
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
        public async Task<List<DashboardFinancieros>> GetDetalleServicio(string servicio, int anio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getDetalleServicioFinancieros", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@anio", anio));
                        var response = new List<DashboardFinancieros>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueServicio(reader));
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
        public async Task<List<Oficio>> GetOficiosFinancieros(string servicio,int anio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getOficiosFinancieros", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@anio", anio));
                        var response = new List<Oficio>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueOficios(reader));
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
        public async Task<Oficio> GetOficioById(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getOficioById", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        var response = new Oficio();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueOficios(reader);
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
        //Obtiene las Cédulas en Trámite de Pago de un Servicio Específico
        public async Task<List<DetalleCedula>> GetCedulasTramitePago(int id, int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCedulasTramitePagoMes", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@oficio", id));
                        var response = new List<DetalleCedula>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueDetalle(reader));
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
        public async Task<List<DetalleCedula>> GetFacturasTramitePago(int id, int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getFacturasTramitePagoMes", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@oficio", id));
                        var response = new List<DetalleCedula>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueFacturas(reader));
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
        /*Filtros para Cedulas*/
        public async Task<List<DetalleCedula>> GetCedulasFiltroPago(int id, string mes,int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCedulasPorFiltroOficio", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@factura", id));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@mes", mes));
                        var response = new List<DetalleCedula>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueDetalle(reader));
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
        public async Task<List<DetalleCedula>> GetFacturasFiltroPago(int id,int servicio, string mes)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getFacturasPorFiltroOficio", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@factura", id));
                        cmd.Parameters.Add(new SqlParameter("@mes", mes));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<DetalleCedula>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueFacturas(reader));
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
        /*Fin de Filtros para Cedulas*/
        public async Task<List<DetalleCedula>> GetFacturasOficio(int id, int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getFacturasOficio", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@oficio", id));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<DetalleCedula>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueFacturas(reader));
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
        public async Task<int> insertarNuevoOficio(Oficio oficio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaOficioFinancieros", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", oficio.Id)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@servicioId", oficio.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@anio", oficio.Anio));
                        cmd.Parameters.Add(new SqlParameter("@numero", oficio.NumeroOficio));

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
                return 0;
            }
        }
        public async Task<int> CancelarOficio(int id, int servicio,int user)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_cancelarOficioDGPPT", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@oficio", id));
                        cmd.Parameters.Add(new SqlParameter("@user", user));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));

                        await sql.OpenAsync();
                        int i = await cmd.ExecuteNonQueryAsync();
                        if (i > 0)
                        {
                            return 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        public async Task<int> PagarOficio(int id, int servicio, DateTime fecha,int user)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_pagarOficio", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@oficio", id));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@fechaPagado", fecha));
                        cmd.Parameters.Add(new SqlParameter("@user", user));

                        await sql.OpenAsync();
                        int i = await cmd.ExecuteNonQueryAsync();
                        if (i > 0)
                        {
                            return 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        public async Task<int> insertarCedulasOficio(List<CedulasOficio> cedulas)
        {
            int id = 0;
            try
            {
                foreach (var ced in cedulas)
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertaCedulaOficio", sql))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@oficio", ced.OficioId));
                            cmd.Parameters.Add(new SqlParameter("@factura", ced.FacturaId));
                            cmd.Parameters.Add(new SqlParameter("@cedula", ced.CedulaId));
                            cmd.Parameters.Add(new SqlParameter("@servicio", ced.ServicioId));

                            await sql.OpenAsync();
                            int i = await cmd.ExecuteNonQueryAsync();
                            if (i > 0)
                            {
                                id = 1;
                            }
                            else
                            {
                                id = -1;
                            }
                        }
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
        public async Task<int> EliminaCedulasOficio(int oficio, int servicio, int factura)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminarCedulaOficio", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@oficio", oficio));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@factura", factura));

                        await sql.OpenAsync();
                        int i = await cmd.ExecuteNonQueryAsync();
                        if (i > 0)
                        {
                            return 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        public async Task<int> TramitarOficioDGPPT(int id, int servicio, int user)
        {
            int success = -1;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_tramitarOficioDGPPT", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@oficio", id));
                        cmd.Parameters.Add(new SqlParameter("@user", user));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));

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

        //Inserta Acuse del Oficio para Pago
        public async Task<int> insertaAcuseOficio(Oficio oficio)
        {
            DateTime date = DateTime.Now;
            string date_str = date.ToString("yyyyMMddHHmmss");
            string saveFile = "";
            int id = 0;


            if (oficio.Id != 0)
            {
                int isDeleted = await eliminaArchivo(oficio);
            }
            
            if(oficio.Archivo != null)
                saveFile = await guardaArchivo(oficio.Archivo, oficio.NumeroOficio.ToString(), date_str);
            try
            {
                if (saveFile.Equals("Ok") || oficio.Archivo == null)
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertarAcuseOficio", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@id", oficio.Id));
                            if (oficio.Archivo != null)
                                cmd.Parameters.Add(new SqlParameter("@archivo", (date_str + "_" + oficio.Archivo.FileName)));
                            cmd.Parameters.Add(new SqlParameter("@fechaTramitado", oficio.FechaTramitado));
                            cmd.Parameters.Add(new SqlParameter("@importe", oficio.ImporteOficio));
                            cmd.Parameters.Add(new SqlParameter("@importePenas", oficio.ImportePenas));

                            await sql.OpenAsync();
                            await cmd.ExecuteNonQueryAsync();
                            id = 1;

                            return id;
                        }
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

        public async Task<string> guardaArchivo(IFormFile archivo, string folio, string date)
        {
            long size = archivo.Length;
            string folderCedula = folio;

            string newPath = Directory.GetCurrentDirectory() + "\\Oficios Financieros\\" + folderCedula;
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

        public async Task<int> eliminaArchivo(Oficio oficio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaAcuseOficioFinancieros", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", oficio.Id));
                        cmd.Parameters.Add(new SqlParameter("@archivo", SqlDbType.VarChar, 500)).Direction = System.Data.ParameterDirection.Output;
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        string archivo = (cmd.Parameters["@archivo"].Value).ToString();
                        string newPath = Directory.GetCurrentDirectory() + "\\Oficios Financieros\\" + oficio.NumeroOficio + "\\" + archivo;
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
        //DashBoard de Financieros
        private DashboardFinancieros MapToValue(SqlDataReader reader)
        {
            return new DashboardFinancieros
            {
                Servicio = reader["Servicio"].ToString(),
                Fondo = reader["Fondo"].ToString(),
                Icono = reader["Icono"].ToString(),
                Total = (int)reader["Total"]
            };
        }
        //Detalle de Servicio
        private DashboardFinancieros MapToValueServicio(SqlDataReader reader)
        {
            return new DashboardFinancieros
            {
                Servicio = reader["Servicio"].ToString(),
                Descripcion = reader["Descripcion"].ToString(),
                Estatus = reader["Estatus"].ToString(),
                Mes = reader["Mes"].ToString(),
                Fondo = reader["Fondo"].ToString(),
                Icono = reader["Icono"].ToString(),
                Total = (int)reader["Total"],
                TotalParcial = (int)reader["TotalParcial"],
                APendientes = (int)reader["APendientes"],
                CedulasPendientes = (int)reader["CedulasPendientes"],
                MemosPendientes = (int)reader["MemosPendientes"],
                ServicioId = (int)reader["ServicioId"]
            };
        }
        private DetalleCedula MapToValueDetalle(SqlDataReader reader)
        {
            return new DetalleCedula
            {
                Id = (int)reader["Id"],
                ServicioId = (int)reader["ServicioId"],
                Servicio = reader["Servicio"].ToString(),
                Inmueble = reader["Inmueble"].ToString(),
                EstatusCedula = reader["EstatusCedula"].ToString(),
                Folio = reader["Folio"].ToString(),
                Mes = reader["Mes"].ToString(),
                Anio = (int)reader["Anio"],
            };
        }
        private DetalleCedula MapToValueFacturas(SqlDataReader reader)
        {
            return new DetalleCedula
            {
                Id = (int)reader["Id"],
                ServicioId = (int)reader["ServicioId"],
                FacturaId = (int)reader["FacturaId"],
                Servicio = reader["Servicio"].ToString(),
                Tipo = reader["Tipo"] != DBNull.Value ? reader["Tipo"].ToString():"",
                Serie = reader["Serie"].ToString(),
                Inmueble = reader["Inmueble"].ToString(),
                EstatusCedula = reader["EstatusCedula"].ToString(),
                EstatusFactura = reader["EstatusFactura"].ToString(),
                Folio = reader["Folio"].ToString(),
                FolioFactura = reader["FolioFactura"].ToString(),
                Mes = reader["Mes"].ToString(),
                Anio = (int)reader["Anio"],
                Subtotal = (decimal)reader["Subtotal"],
                Total = (decimal)reader["Total"],
                IVA = (decimal)reader["IVA"],
            };
        }
        private Oficio MapToValueOficios(SqlDataReader reader)
        {
            return new Oficio
            {
                Id = (int)reader["Id"],
                ServicioId = reader["ServicioId"] != DBNull.Value ? (int)reader["ServicioId"]:0,
                Servicio = reader["Nombre"].ToString(),
                Anio = (int)reader["Anio"],
                NumeroOficio = (int)reader["NumeroOficio"],
                Estatus = reader["Estatus"] != DBNull.Value ? reader["Estatus"].ToString() : "",
                FechaPagado = reader["FechaPagado"] != DBNull.Value ? Convert.ToDateTime(reader["FechaPagado"]) : Convert.ToDateTime("01/01/0001"),
                NombreArchivo = reader["Archivo"] != DBNull.Value ? reader["Archivo"].ToString() : "",
                FechaTramitado = reader["FechaTramitado"] != DBNull.Value ? Convert.ToDateTime(reader["FechaTramitado"]) : Convert.ToDateTime("01/01/0001"),
                ImporteFacturado = reader["ImporteFacturado"] != DBNull.Value ? Convert.ToDecimal(reader["ImporteFacturado"]) : 0,
                ImporteNC = reader["ImporteNC"] != DBNull.Value ? Convert.ToDecimal(reader["ImporteNC"]) : 0,
                ImportePenas = reader["ImportePenas"] != DBNull.Value ? Convert.ToDecimal(reader["ImportePenas"]) : 0,
                TotalOficio = reader["ImportePagar"] != DBNull.Value ? Convert.ToDecimal(reader["ImportePagar"]) : 0,
            };
        }
    }
}
