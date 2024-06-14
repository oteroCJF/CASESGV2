using CedulasEvaluacion.Entities.MCargasMasivas;
using CedulasEvaluacion.Entities.MFacturas;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CedulasEvaluacion.Repositories
{
    public class RepositorioCargasMasivas : IRepositorioCargasMasivas
    {
        private readonly string _connectionString;

        public RepositorioCargasMasivas(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<List<CargasMasivas>> getCargasMasivas()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCargasMasivas", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        var response = new List<CargasMasivas>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueCM(reader));
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

        public async Task<CargasMasivas> getCargaMasivaById(int carga)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCargaMasivaById", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@carga", carga));
                        var response = new CargasMasivas();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueCM(reader);
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

        public async Task<int> insertaCargaMsiva(CargasMasivas carga)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaCargaMasiva", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@usuarioId", carga.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@nombre", carga.Nombre));
                        cmd.Parameters.Add(new SqlParameter("@tipoArchivo", carga.TipoArchivo));
                        cmd.Parameters.Add(new SqlParameter("@anio", carga.Anio));
                        
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

        /********************* Masivo de Faacturas ************************/
        public async Task<List<Facturas>> getFacturas(int carga)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getFacturasByCarga", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@carga", carga));
                        var response = new List<Facturas>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueFacturas(reader));
                            }
                        }

                        foreach (var r in response)
                        {
                            r.concepto = await getConceptosFactura(r.Id);
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
        public async Task<List<Concepto>> getConceptosFactura(int factura)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getConceptosFacturaMasiva", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@factura", factura));
                        var response = new List<Concepto>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueConcepto(reader));
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
        public async Task<Facturas> insertaFacturas(Facturas facturas)
        {
            await copiaFactura(facturas.Xml, facturas.Tipo);
            facturas = desglozaXML(facturas);
            facturas.CedulaExistente = 0;//await buscaFactura(facturas.timbreFiscal.UUID);
            if (facturas.CedulaExistente == 0)
            {
                try
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertaFacturaMasiva", sql))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt)).Direction = ParameterDirection.Output;
                            // el servicio que se enviara determinara a que campo se inserta a traves del SP
                            cmd.Parameters.Add(new SqlParameter("@servicioId", facturas.ServicioId));
                            cmd.Parameters.Add(new SqlParameter("@cargaMasiva", facturas.CargaMasivaId));
                            cmd.Parameters.Add(new SqlParameter("@cedula", facturas.CedulaId));
                            cmd.Parameters.Add(new SqlParameter("@rfc", facturas.emisor.RFC));
                            cmd.Parameters.Add(new SqlParameter("@iva", facturas.traslado.Importe));
                            if (facturas.retencion != null)
                                cmd.Parameters.Add(new SqlParameter("@retencion", facturas.retencion.Importe));
                            if (facturas.cfdiRelacionado != null)
                                cmd.Parameters.Add(new SqlParameter("@uuidRelacionado", facturas.cfdiRelacionado.UUID));
                            cmd.Parameters.Add(new SqlParameter("@nombre", facturas.emisor.Nombre));
                            cmd.Parameters.Add(new SqlParameter("@tipo", facturas.Tipo));
                            cmd.Parameters.Add(new SqlParameter("@usoCFDI", facturas.receptor.usoCFDI));
                            cmd.Parameters.Add(new SqlParameter("@uuid", facturas.timbreFiscal.UUID));
                            cmd.Parameters.Add(new SqlParameter("@serie", facturas.comprobante.Serie));
                            cmd.Parameters.Add(new SqlParameter("@folio", facturas.comprobante.Folio));
                            cmd.Parameters.Add(new SqlParameter("@fechaTimbrado", facturas.timbreFiscal.FechaTimbrado));
                            cmd.Parameters.Add(new SqlParameter("@subtotal", facturas.comprobante.SubTotal));
                            cmd.Parameters.Add(new SqlParameter("@total", facturas.comprobante.Total));
                            cmd.Parameters.Add(new SqlParameter("@archivo", facturas.NombreArchivo));
                            if (!facturas.FechaInicial.ToString("yyyy-MM-dd").Equals("0001-01-01"))
                                cmd.Parameters.Add(new SqlParameter("@fechaInicial", facturas.FechaInicial));
                            if (!facturas.FechaFinal.ToString("yyyy-MM-dd").Equals("0001-01-01"))
                                cmd.Parameters.Add(new SqlParameter("@fechaFinal", facturas.FechaFinal));

                            await sql.OpenAsync();

                            await cmd.ExecuteNonQueryAsync();

                            facturas.Id = Convert.ToInt32(cmd.Parameters["@id"].Value);

                            return facturas;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    return null;
                }
            }
            return facturas;
        }
        public async Task<int> insertaConceptoFacturas(Facturas facturas)
        {
            Facturas factura = await insertaFacturas(facturas);
            double iva = 0;
            if (factura.CedulaExistente == 0)
            {
                try
                {
                    foreach (var fac in factura.concepto)
                    {
                        iva = Convert.ToDouble(fac.Importe) * 0.16;
                        using (SqlConnection sql = new SqlConnection(_connectionString))
                        {
                            using (SqlCommand cmd = new SqlCommand("sp_insertaConceptoFacturaMasiva", sql))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add(new SqlParameter("@factura", factura.Id));
                                cmd.Parameters.Add(new SqlParameter("@cantidad", fac.Cantidad));
                                cmd.Parameters.Add(new SqlParameter("@claveProducto", fac.ClaveProdServ));
                                cmd.Parameters.Add(new SqlParameter("@claveUnidad", fac.ClaveUnidad));
                                cmd.Parameters.Add(new SqlParameter("@unidad", fac.Unidad));
                                cmd.Parameters.Add(new SqlParameter("@descripcion", fac.Descripcion));
                                if (facturas.datosExtra != null)
                                {
                                    cmd.Parameters.Add(new SqlParameter("@folioSap", facturas.datosExtra.FolioSAP));
                                    cmd.Parameters.Add(new SqlParameter("@numeroCliente", facturas.datosExtra.NumeroCliente));
                                    cmd.Parameters.Add(new SqlParameter("@observacion", facturas.datosTotales.observGeneral1));
                                }
                                else if (fac.ObservacionGeneral != null && !fac.ObservacionGeneral.Equals(""))
                                {
                                    cmd.Parameters.Add(new SqlParameter("@observacion", fac.ObservacionGeneral));
                                }
                                cmd.Parameters.Add(new SqlParameter("@precioUnitario", fac.ValorUnitario));
                                cmd.Parameters.Add(new SqlParameter("@subtotal", fac.Importe));
                                cmd.Parameters.Add(new SqlParameter("@descuento", fac.Descuento));
                                cmd.Parameters.Add(new SqlParameter("@iva", iva));

                                await sql.OpenAsync();
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                    return 1;
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    return -1;
                }
            }
            return factura.CedulaExistente;
        }
        public Facturas desglozaXML(Facturas facturas)
        {
            string newPath = "";
            if (facturas.Tipo.Equals("NC"))
            {
                newPath = Directory.GetCurrentDirectory() + "\\Facturas\\Notas de Crédito";
            }
            else
            {
                newPath = Directory.GetCurrentDirectory() + "\\Facturas\\Facturas";
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(newPath + "\\" + facturas.Xml.FileName);
            try
            {
                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.NameTable);
                namespaceManager.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                namespaceManager.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
                namespaceManager.AddNamespace("ext", "http://www.buzone.com.mx/XSD/Addenda/EMEBuzWS");

                XmlNodeList ndComprobante = doc.SelectNodes("//cfdi:Comprobante", namespaceManager);
                if (ndComprobante.Count == 0)
                {
                    namespaceManager.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/4");
                    namespaceManager.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
                    namespaceManager.AddNamespace("ext", "http://www.buzone.com.mx/XSD/Addenda/EMEBuzWS");
                    ndComprobante = doc.SelectNodes("//cfdi:Comprobante", namespaceManager);
                }

                XmlNodeList ndEmisor = doc.SelectNodes("//cfdi:Emisor", namespaceManager);
                XmlNodeList ndCFDIRelacionado = doc.SelectNodes("//cfdi:CfdiRelacionado", namespaceManager);
                XmlNodeList ndConcepto = doc.SelectNodes("//cfdi:Conceptos/cfdi:Concepto", namespaceManager);
                XmlNodeList nDTimbrado = doc.SelectNodes("//cfdi:Complemento/tfd:TimbreFiscalDigital", namespaceManager);
                XmlNodeList nDExtras = doc.SelectNodes("//cfdi:Addenda/ext:ElementosExtra/ext:DatosGenerales", namespaceManager);
                XmlNodeList nDTraslado = doc.SelectNodes("//cfdi:Traslados/cfdi:Traslado", namespaceManager);
                XmlNodeList nDTotales = doc.SelectNodes("//cfdi:Addenda/ext:ElementosExtra/ext:DatosTotales", namespaceManager);
                XmlNodeList nDReceptor = doc.SelectNodes("//cfdi:Receptor", namespaceManager);
                XmlNodeList nDRetenciones = doc.SelectNodes("//cfdi:Retencion", namespaceManager);

                string jsonComprobante = JsonConvert.SerializeXmlNode(ndComprobante[0], Newtonsoft.Json.Formatting.None, true);
                string jsonEmisor = JsonConvert.SerializeXmlNode(ndEmisor[0], Newtonsoft.Json.Formatting.None, true);
                string jsonTraslado = JsonConvert.SerializeXmlNode(nDTraslado[0], Newtonsoft.Json.Formatting.None, true);
                string jsonTimbrado = JsonConvert.SerializeXmlNode(nDTimbrado[0], Newtonsoft.Json.Formatting.None, true);
                string jsonExtra = JsonConvert.SerializeXmlNode(nDExtras[0], Newtonsoft.Json.Formatting.None, true);
                string jsonTotales = JsonConvert.SerializeXmlNode(nDTotales[0], Newtonsoft.Json.Formatting.None, true);
                string jsonReceptor = JsonConvert.SerializeXmlNode(nDReceptor[0], Newtonsoft.Json.Formatting.None, true);
                string jsonRetenciones = JsonConvert.SerializeXmlNode(nDRetenciones[0], Newtonsoft.Json.Formatting.None, true);
                string jsonCFDIRelacionado = "";
                if (ndCFDIRelacionado.Count != 0)
                    jsonCFDIRelacionado = JsonConvert.SerializeXmlNode(ndCFDIRelacionado[0], Newtonsoft.Json.Formatting.None, true);


                List<Concepto> listaConcepto = new List<Concepto>();
                //Concepto concept = null;
                for (int i = 0; i < ndConcepto.Count; i++)
                {
                    string jsonConcepto = JsonConvert.SerializeXmlNode(ndConcepto[i], Newtonsoft.Json.Formatting.None, true);
                    jsonConcepto = jsonConcepto.Replace("@", "");
                    Concepto concept = JsonConvert.DeserializeObject<Concepto>(jsonConcepto);
                    concept.ObservacionGeneral = facturas.TipoServicio ? concept.Descripcion.Split("-")[1].Trim() : "";
                    listaConcepto.Add(concept);
                }

                List<Traslado> listaTraslado = new List<Traslado>();
                decimal iva = 0;
                for (int i = 0; i < nDTraslado.Count - 1; i++)
                {
                    string jsonTraslado2 = JsonConvert.SerializeXmlNode(nDTraslado[i], Newtonsoft.Json.Formatting.None, true);
                    jsonTraslado2 = jsonTraslado2.Replace("@", "");
                    Traslado trasl = JsonConvert.DeserializeObject<Traslado>(jsonTraslado2);
                    iva += trasl.Importe;
                }

                jsonComprobante = jsonComprobante.Replace("@", "");
                jsonEmisor = jsonEmisor.Replace("@", "");
                jsonTraslado = jsonTraslado.Replace("@", "");
                jsonTimbrado = jsonTimbrado.Replace("@", "");
                jsonReceptor = jsonReceptor.Replace("@", "");
                jsonCFDIRelacionado = jsonCFDIRelacionado.Replace("@", "");

                Comprobante comp = JsonConvert.DeserializeObject<Comprobante>(jsonComprobante);
                Emisor emisor = JsonConvert.DeserializeObject<Emisor>(jsonEmisor);
                Traslado traslado = JsonConvert.DeserializeObject<Traslado>(jsonTraslado);
                TimbreFiscal tFiscal = JsonConvert.DeserializeObject<TimbreFiscal>(jsonTimbrado);
                Receptor receptor = JsonConvert.DeserializeObject<Receptor>(jsonReceptor);
                CFDIRelacionado cfdiRelacionado = JsonConvert.DeserializeObject<CFDIRelacionado>(jsonCFDIRelacionado);

                traslado.Importe = iva;

                if (facturas.Retenciones)
                {
                    jsonRetenciones = jsonRetenciones.Replace("@", "");
                    Retencion retencion = JsonConvert.DeserializeObject<Retencion>(jsonRetenciones);
                    facturas.retencion = retencion;
                }

                if (facturas.Retenciones)
                {
                    jsonRetenciones = jsonRetenciones.Replace("@", "");
                    Retencion retencion = JsonConvert.DeserializeObject<Retencion>(jsonRetenciones);
                    facturas.retencion = retencion;
                }

                if (facturas.ServicioId == 3)
                {
                    jsonExtra = jsonExtra.Replace("@", "");
                    jsonTotales = jsonTotales.Replace("@", "");
                    DatosExtra de = JsonConvert.DeserializeObject<DatosExtra>(jsonExtra);
                    DatosTotales tot = JsonConvert.DeserializeObject<DatosTotales>(jsonTotales);
                    facturas.datosExtra = de;
                    facturas.datosTotales = tot;
                }

                facturas.comprobante = comp;
                facturas.emisor = emisor;
                facturas.traslado = traslado;
                facturas.concepto = listaConcepto;
                facturas.timbreFiscal = tFiscal;
                facturas.receptor = receptor;
                facturas.cfdiRelacionado = cfdiRelacionado;

                //File.Delete(newPath + "\\" + facturas.Xml.FileName);
                facturas.NombreArchivo = facturas.Xml.FileName;

                return facturas;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
        public async Task<int> copiaFactura(IFormFile factura, string tipo)
        {
            string newPath = "";
            if (tipo.Equals("NC"))
            {
                newPath = Directory.GetCurrentDirectory() + "\\Facturas\\Notas de Crédito";
            }
            else
            {
                newPath = Directory.GetCurrentDirectory() + "\\Facturas\\Facturas";
            }
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            using (var stream = new FileStream(newPath + "\\" + factura.FileName, FileMode.Create))
            {
                try
                {
                    await (factura).CopyToAsync(stream);
                    return 1;
                }
                catch (Exception ex)
                {
                    string msg = ex.Message.ToString();
                    return -1;
                }
            }
        }
        public async Task<int> buscaFactura(string uuid)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_buscaFacturaMasiva", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@uuid", uuid));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        if (cmd.Parameters["@cedulaId"].Value != DBNull.Value)
                        {
                            return Convert.ToInt32(cmd.Parameters["@cedulaId"].Value);
                        }

                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }
        public async Task<int> procesarArchivos(int carga, string tipo)
        {
            int complete = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_procesarCargaMasiva", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@total", SqlDbType.BigInt)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@carga", carga));
                        cmd.Parameters.Add(new SqlParameter("@tipo", tipo));

                        await sql.OpenAsync();

                        await cmd.ExecuteNonQueryAsync();

                        complete = Convert.ToInt32(cmd.Parameters["@total"].Value);

                        return complete;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }
        private CargasMasivas MapToValueCM(SqlDataReader reader)
        {
            return new CargasMasivas
            {
                Id = (int)reader["Id"],
                Anio = reader["Anio"] != DBNull.Value ? (int)reader["Anio"] : 0,
                Nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : "",
                Usuario = reader["Usuario"] != DBNull.Value ? reader["Usuario"].ToString() : "",
                TipoArchivo = reader["TipoArchivo"] != DBNull.Value ? reader["TipoArchivo"].ToString() : "",
                Estatus = reader["Estatus"] != DBNull.Value ? reader["Estatus"].ToString() : "",
                TotalFacturas = reader["TotalFacturas"] != DBNull.Value ? (int)reader["TotalFacturas"] : 0,
            };
        }
        private Facturas MapToValue(SqlDataReader reader)
        {
            return new Facturas
            {
                Id = (int)reader["Id"],
                CedulaId = reader["CedulaId"] != DBNull.Value ? (int)reader["CedulaId"] : 0,
                SBasicosId = reader["SBasicosId"] != DBNull.Value ? (int)reader["SBasicosId"] : 0,
                Tipo = reader["Tipo"] != DBNull.Value ? reader["Tipo"].ToString() : "",
                NombreArchivo = reader["Archivo"] != DBNull.Value ? reader["Archivo"].ToString() : "",
            };
        }
        private Facturas MapToValueFacturas(SqlDataReader reader)
        {
            Comprobante compro = new Comprobante();
            compro.Folio = (long)reader["Folio"];
            compro.Serie = reader["Serie"].ToString();
            compro.SubTotal = Convert.ToDecimal(reader["Subtotal"]);
            compro.Total = Convert.ToDecimal(reader["Total"]);

            Receptor recept = new Receptor();
            recept.usoCFDI = reader["UsoCFDI"].ToString();

            Emisor emi = new Emisor();
            emi.Nombre = reader["Nombre"].ToString();
            emi.RFC = reader["RFC"].ToString();

            Traslado traslado = new Traslado();
            traslado.Importe = (decimal)reader["IVA"];

            Retencion retencion = new Retencion();
            retencion.Importe = reader["RetencionIVA"] != DBNull.Value ? (decimal)reader["RetencionIVA"] : 0;

            TimbreFiscal timbre = new TimbreFiscal();
            timbre.UUID = reader["UUID"].ToString();
            timbre.FechaTimbrado = Convert.ToDateTime(reader["FechaTimbrado"]);

            CFDIRelacionado cfdiR = new CFDIRelacionado();
            cfdiR.UUID = DBNull.Value != reader["UUIDRelacionado"] ? reader["UUIDRelacionado"].ToString() : "";

            return new Facturas
            {
                Id = (int)reader["Id"],
                Existe = reader["Existe"] != DBNull.Value ? (bool)reader["Existe"] : false,
                Anio = reader["Anio"] != DBNull.Value ? (int)reader["Anio"] : 0,
                TotalDeductivas = reader["TotalDeductivas"] != DBNull.Value ? (decimal)reader["TotalDeductivas"] : 0,
                ServicioId = reader["ServicioId"] != DBNull.Value ? (int)reader["ServicioId"] : 0,
                CedulaId = reader["CedulaId"] != DBNull.Value ? (int)reader["CedulaId"] : 0,
                SBasicosId = reader["SBasicosId"] != DBNull.Value ? (int)reader["SBasicosId"] : 0,
                Descripcion = reader["Descripcion"].ToString(),
                Servicio = reader["Servicio"] != DBNull.Value ? reader["Servicio"].ToString() : "",
                Inmueble = reader["Inmueble"] != DBNull.Value ? reader["Inmueble"].ToString() : "",
                Mes = reader["Mes"] != DBNull.Value ? reader["Mes"].ToString() : "",
                FolioCedula = reader["FolioCedula"] != DBNull.Value ? reader["FolioCedula"].ToString() : "",
                Tipo = reader["Tipo"] != DBNull.Value ? reader["Tipo"].ToString() : "",
                NombreArchivo = reader["Archivo"] != DBNull.Value ? reader["Archivo"].ToString() : "",
                FechaInicial = reader["FechaInicial"] != DBNull.Value ? (DateTime)reader["FechaInicial"] : Convert.ToDateTime("01/01/1990" + ""),
                FechaFinal = reader["FechaFinal"] != DBNull.Value ? (DateTime)reader["FechaFinal"] : Convert.ToDateTime("01/01/1990" + ""),
                comprobante = compro,
                emisor = emi,
                receptor = recept,
                timbreFiscal = timbre,
                traslado = traslado,
                retencion = retencion,
                cfdiRelacionado = cfdiR
            };
        }
        private Concepto MapToValueConcepto(SqlDataReader reader)
        {
            DatosExtra de = new DatosExtra();
            de.FolioSAP = reader["FolioSAP"] != DBNull.Value ? (long)reader["FolioSAP"] : 0;
            de.NumeroCliente = reader["NumeroCliente"] != DBNull.Value ? (long)reader["NumeroCliente"] : 0;

            DatosTotales dt = new DatosTotales();
            dt.observGeneral1 = reader["ObservacionGeneral"] != DBNull.Value ? reader["ObservacionGeneral"].ToString() : "";

            return new Concepto
            {
                FacturaId = (int)reader["FacturaId"],
                Cantidad = (decimal)reader["Cantidad"],
                ClaveProdServ = (int)reader["ClaveProducto"],
                ClaveUnidad = reader["ClaveUnidad"].ToString(),
                Unidad = reader["Unidad"].ToString(),
                Descripcion = reader["Descripcion"].ToString(),
                ValorUnitario = (decimal)reader["PrecioUnitario"],
                Importe = (decimal)reader["Subtotal"],
                IVA = (decimal)reader["IVA"],
                datosExtra = de,
                datosTotales = dt
            };
        }
        private Facturas MapToValuePago(SqlDataReader reader)
        {
            return new Facturas()
            {
                Id = reader["Id"] != DBNull.Value ? (int)reader["Id"] : 0,
                FolioFactura = reader["FolioFactura"] != DBNull.Value ? reader["FolioFactura"].ToString() : "",
            };
        }
        /********************* Masivo de Faacturas ************************/

    }
}
