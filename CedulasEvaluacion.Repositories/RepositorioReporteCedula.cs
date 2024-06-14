using CedulasEvaluacion.Entities.Reportes;
using CedulasEvaluacion.Entities.Vistas;
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
    public class RepositorioReporteCedula :IRepositorioReporteCedula
    {
        private readonly string _connectionString;
        public RepositorioReporteCedula(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }
        public async Task<IEnumerable<ReporteCedula>> getReporteMensajeria(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ReporteMensajeriaById", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
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

        public async Task<IEnumerable<ReporteCedula>> getReporteLimpieza(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ReporteLimpiezaById", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
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

        public async Task<IEnumerable<ReporteFinancieros>> getReporteFinancierosLimpieza()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_reporteFinancierosLimpieza", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var response = new List<ReporteFinancieros>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueFinancieros(reader));
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

        public async Task<IEnumerable<ReporteCedula>> getCedulaByServicio(int servicio, int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_generaCedulaByServicio", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
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

        private ReporteCedula MapToValue(SqlDataReader reader)
        {
            return new ReporteCedula()
            {
                Id = (int)reader["Id"],
                Inmueble = reader["Inmueble"].ToString(),
                Folio = reader["Folio"].ToString(),
                Mes = reader["Mes"].ToString(),
                Anio = (int)reader["Anio"],
                Administracion = reader["Administracion"].ToString(),
                Servicio = reader["Servicio"].ToString(),
                TipoServicio = reader["TipoServicio"] != DBNull.Value ? reader["TipoServicio"].ToString():"",
                Estatus = reader["Estatus"].ToString(),
                Calificacion = reader["Calificacion"].ToString(),
                FechaCreacion = reader["FechaCreacion"].ToString(),
                Periodo = reader["FechaFinal"] != DBNull.Value ? 
                                Convert.ToDateTime(reader["FechaInicial"]).Day+" de "+ Convert.ToDateTime(reader["FechaInicial"]).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) 
                                +" de "+ Convert.ToDateTime(reader["FechaInicial"]).Year+ " al "+ Convert.ToDateTime(reader["FechaFinal"]).Day + " de "+ 
                                reader["Mes"].ToString()+ " del "+
                                Convert.ToDateTime(reader["FechaFinal"]).Year: "",
                Facturas = reader["Facturas"].ToString(),
                MontosFacturas = reader["MontosFacturas"].ToString(),
                Elaboro = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(CultureInfo.InvariantCulture.TextInfo.ToLower(reader["Elaboro"].ToString())),
            };
        }
        private ReporteFinancieros MapToValueFinancieros(SqlDataReader reader)
        {
            return new ReporteFinancieros()
            {
                Id = (int)reader["Id"],
                Inmueble = reader["Inmueble"].ToString(),
                Folio = reader["Folio"].ToString(),
                FolioFactura = reader["FolioFactura"].ToString(),
                Mes = reader["Mes"].ToString(),
                Anio = (int)reader["Anio"],
                Inasistencias = reader["Inasistencias"] != DBNull.Value ? (int)reader["Inasistencias"]:0,
                Calificacion = (decimal) reader["Calificacion"],
                PenaPO = (decimal)reader["PenaPO"],
                PenaUniforme = (decimal)reader["PenaUniforme"],
                PenaCapacitacion = (decimal)reader["PenaCapacitacion"],
                PenaCalificacion = (decimal)reader["PenaCalificacion"],
                PenaEquipo = (decimal)reader["PenaEquipo"],
                TotalInmueble = (decimal)reader["TotalInmueble"],
                Estatus = reader["Estatus"].ToString(),
            };
        }
    }
}
