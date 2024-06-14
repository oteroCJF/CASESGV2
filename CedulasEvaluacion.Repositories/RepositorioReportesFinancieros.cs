using CedulasEvaluacion.Entities.MDeductivas;
using CedulasEvaluacion.Entities.Reportes;
using CedulasEvaluacion.Entities.Vistas;
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
    public class RepositorioReportesFinancieros : IRepositorioReportesFinancieros
    {
        private readonly string _connectionString;

        public RepositorioReportesFinancieros(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<List<ReporteCedula>> GetCedulasFinancieros(string mes, int anio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_generaReporteMensualPAT", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@mes", mes));
                        cmd.Parameters.Add(new SqlParameter("@anio", anio));
                        var response = new List<ReporteCedula>();
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
        public async Task<List<ReporteCedula>> GetReportePagos(string mes, int anio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_generaReportePagos", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@mes", mes));
                        cmd.Parameters.Add(new SqlParameter("@anio", anio));
                        var response = new List<ReporteCedula>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValuePagos(reader));
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
        public async Task<List<ReporteCedula>> GetReporteServiciosFacturas(int servicio, string mes)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ReporteServiciosFactura", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@mes", mes));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<ReporteCedula>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValuePagos(reader));
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
        public async Task<List<DesgloceDeductivas>> GeneraDesgloceCedula(int cedula,int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getDesglocePenalizaciones", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<DesgloceDeductivas>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueDesgloce(reader));
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
        public async Task<List<VFacturas>> GetReporteFacturas(int cedula, int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getFacturas", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedula", cedula));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<VFacturas>();
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
        public async Task<List<VConceptos>> GetReporteConceptosFactura(int factura)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getConceptosFactura", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@factura", factura));
                        var response = new List<VConceptos>();
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
        private VConceptos MapToValueConcepto (SqlDataReader reader)
        {
            return new VConceptos
            {
                Cantidad = (decimal)reader["Cantidad"],
                Descripcion = reader["Descripcion"].ToString(),
                ObservacionGeneral = reader["ObservacionGeneral"].ToString(),
                PrecioUnitario = (decimal)reader["PrecioUnitario"],
                Subtotal = (decimal)reader["Subtotal"],
                IVA = (decimal)reader["IVA"],
            };
        }
        private VFacturas MapToValueFacturas(SqlDataReader reader)
        {
            return new VFacturas
            {
                Id = (int)reader["Id"],
                CedulaId = reader["CedulaId"] != DBNull.Value ? (int)reader["CedulaId"] : 0,
                Tipo = reader["Tipo"] != DBNull.Value ? reader["Tipo"].ToString() : "",
                UUID = reader["UUID"].ToString(),
                FechaTimbrado = Convert.ToDateTime(reader["FechaTimbrado"]),
                IVA = (decimal)reader["IVA"],
                Nombre = reader["Nombre"].ToString(),
                RFC = reader["RFC"].ToString(),
                Folio = reader["Serie"].ToString()+reader["Folio"].ToString(),
                FechaInicial = reader["FechaInicial"] != DBNull.Value ? (DateTime)reader["FechaInicial"] : Convert.ToDateTime("01/01/1990" + ""),
                FechaFinal = reader["FechaFinal"] != DBNull.Value ? (DateTime)reader["FechaFinal"] : Convert.ToDateTime("01/01/1990" + ""),
                Estatus = reader["Estatus"].ToString(),
                Subtotal = Convert.ToDecimal(reader["Subtotal"]),
                Total = Convert.ToDecimal(reader["Total"])
            };
        }
        private ReporteCedula MapToValue(SqlDataReader reader)
        {
            return new ReporteCedula
            {
                Id = (int)reader["Id"],
                Servicio = reader["Servicio"] != DBNull.Value ? reader["Servicio"].ToString() : "",
                Inmueble = reader["Inmueble"] != DBNull.Value ? reader["Inmueble"].ToString() : "",
                Folio = reader["Folio"] != DBNull.Value ? reader["Folio"].ToString() : "",
                Mes = reader["Mes"] != DBNull.Value ? reader["Mes"].ToString() : "",
                Anio = reader["Anio"] != DBNull.Value ? (int)reader["Anio"] : 0,
                Estatus = reader["Estatus"] != DBNull.Value ? reader["Estatus"].ToString() : "",
                Empresa = reader["Empresa"] != DBNull.Value ? reader["Empresa"].ToString() : "",
            };
        }
        private ReporteCedula MapToValuePagos(SqlDataReader reader)
        {
            return new ReporteCedula
            {
                Id = (int) reader["Id"],
                Servicio = reader["Servicio"] != DBNull.Value ? reader["Servicio"].ToString():"",
                Anio = reader["Anio"] != DBNull.Value ? (int)reader["Anio"] : 0,
                Mes = reader["Mes"] != DBNull.Value ? reader["Mes"].ToString() : "",
                TotalCedulas = reader["TotalCedulas"] != DBNull.Value ? (int) reader["TotalCedulas"] : 0,
                DiasTranscurridos = reader["DiasTranscurridos"] != DBNull.Value ? (int) reader["DiasTranscurridos"] : 0,
                NumeroOficio = reader["NumeroOficio"] != DBNull.Value ? reader["NumeroOficio"].ToString():"",
                FechaTramitado = reader["FechaTramitado"] != DBNull.Value ? Convert.ToDateTime(reader["FechaTramitado"]):Convert.ToDateTime("01/01/1990"),
                FechaPagado = reader["FechaPagado"] != DBNull.Value ? Convert.ToDateTime(reader["FechaPagado"]):Convert.ToDateTime("01/01/1990"),
                Estatus = reader["Estatus"] != DBNull.Value ? reader["Estatus"].ToString() : "",
            };
        }
        private DesgloceDeductivas MapToValueDesgloce(SqlDataReader reader)
        {
            return new DesgloceDeductivas
            {
                Id = reader["Id"] != DBNull.Value ? (int)reader["Id"] : 0,
                Pregunta = reader["Pregunta"] != DBNull.Value ? (int) reader["Pregunta"] : 0,
                Tipo = reader["Tipo"] != DBNull.Value ? reader["Tipo"].ToString() : "",
                FechaProgramada = reader["FechaProgramada"] != DBNull.Value ? Convert.ToDateTime(reader["FechaProgramada"]): Convert.ToDateTime("01/01/1990"),
                FechaEntrega = reader["FechaEntrega"] != DBNull.Value ? Convert.ToDateTime(reader["FechaEntrega"]) : Convert.ToDateTime("01/01/1990"),
                FechaIncidencia = reader["FechaIncidencia"] != DBNull.Value ? Convert.ToDateTime(reader["FechaIncidencia"]) : Convert.ToDateTime("01/01/1990"),
                Comentarios = reader["Comentarios"] != DBNull.Value ? reader["Comentarios"].ToString() : "",
                DiasAtraso = reader["DiasAtraso"] != DBNull.Value ? (int)reader["DiasAtraso"] : 0,
                MontoPenalizacion = reader["MontoPenalizacion"] != DBNull.Value ? (decimal)reader["MontoPenalizacion"] : 0,
            };
        }
    }
}
