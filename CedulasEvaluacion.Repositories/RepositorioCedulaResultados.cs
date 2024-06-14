using CedulasEvaluacion.Entities.MCedulaR;
using CedulasEvaluacion.Entities.MVariables;
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
    public class RepositorioCedulaResultados : IRepositorioCedulaResultados
    {
        private readonly string _connectionString;

        public RepositorioCedulaResultados(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<IEnumerable<PeriodoEvaluacion>> GetPeriodoEvaluacion(int anio, int mesI, int mesF, int servicio, int contrato)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_CR_getPeriodoEvaluacion", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@anio", anio));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@mesInicial", mesI));
                        cmd.Parameters.Add(new SqlParameter("@mesFinal", mesF));
                        var response = new List<PeriodoEvaluacion>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValuePE(reader));
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

        public async Task<IEnumerable<MIndiceEfectividad>> GetIndiceEfectividad(int servicio, int anio, int mesI, int mesF, string rubros, int contrato)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_CR_generaIndiceEfectividad", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@anio", anio));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@mesInicial", mesI));
                        cmd.Parameters.Add(new SqlParameter("@mesFinal", mesF));
                        cmd.Parameters.Add(new SqlParameter("@rubros", rubros));                        
                        var response = new List<MIndiceEfectividad>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueIE(reader));
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

        public async Task<IEnumerable<Variables>> GetRubrosaEvaluar(string rubros)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_CR_getRubrosEvaluar", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@rubros", rubros));
                        var response = new List<Variables>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueRE(reader));
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

        public async Task<IEnumerable<MIndiceEfectividad>> GetIncidenciasInmueble(int servicio, int anio, int mesI, int mesF, string rubros, int contrato)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_CR_getIncidencias", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@anio", anio));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@mesInicial", mesI));
                        cmd.Parameters.Add(new SqlParameter("@mesFinal", mesF));
                        cmd.Parameters.Add(new SqlParameter("@rubros", rubros));
                        var response = new List<MIndiceEfectividad>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueII(reader));
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

        public async Task<IEnumerable<AvanceFinanciero>> GetAvanceFinanciero(int servicio, int contrato)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_CR_getAvanceFinanciero", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<AvanceFinanciero>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueAvanceFinanciero(reader));
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
        public async Task<IEnumerable<AvanceFisico>> GetAvanceFisico(int servicio, int contrato)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_CR_getAvanceFisico", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<AvanceFisico>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueAvanceFisico(reader));
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
        private MIndiceEfectividad MapToValueIE(SqlDataReader reader)
        {
            return new MIndiceEfectividad {
                Id = DBNull.Value != reader["Id"] ? (int)reader["Id"] : 0,
                MesId = DBNull.Value != reader["MesId"] ? (int)reader["MesId"] : 0,
                TipoServicio = DBNull.Value != reader["TipoServicio"] ? reader["TipoServicio"].ToString() : "",
                Mes = DBNull.Value != reader["Mes"] ? reader["Mes"].ToString() : "",
                CedulasIncidencias = DBNull.Value != reader["CedulasIncidencias"] ? (int)reader["CedulasIncidencias"] : 0,
                CedulasSIncidencias = DBNull.Value != reader["CedulasSIncidencias"] ? (int)reader["CedulasSIncidencias"] : 0,
                TotalIncidencias = DBNull.Value != reader["TotalIncidencias"] ? (int)reader["TotalIncidencias"] : 0,
                IndiceEfectividad = DBNull.Value != reader["IndiceEfectividad"] ? (decimal)reader["IndiceEfectividad"] : 0,
                Calificacion = DBNull.Value != reader["Calificacion"] ? Convert.ToDecimal(reader["Calificacion"]) : Convert.ToDecimal(0),
            };
        }
        private Variables MapToValueRE(SqlDataReader reader)
        {
            return new Variables
            {
                Id = DBNull.Value != reader["Id"] ? (int)reader["Id"] : 0,
                ServicioId = DBNull.Value != reader["ServicioId"] ? (int)reader["ServicioId"] : 0,
                Tipo = DBNull.Value != reader["Tipo"] ? reader["Tipo"].ToString() : "",
                Abreviacion = DBNull.Value != reader["Abreviacion"] ? reader["Abreviacion"].ToString() : "",
                Valor = DBNull.Value != reader["Valor"] ? reader["Valor"].ToString() : "",
            };
        }
        private PeriodoEvaluacion MapToValuePE(SqlDataReader reader)
        {
            return new PeriodoEvaluacion
            {
                Empresa = DBNull.Value != reader["Empresa"] ? reader["Empresa"].ToString() : "",
                Contrato = DBNull.Value != reader["Contrato"] ? reader["Contrato"].ToString() : "",
                Anio = DBNull.Value != reader["Anio"] ? (int)reader["Anio"] : 0,
                Servicio = DBNull.Value != reader["Servicio"] ? reader["Servicio"].ToString() : "",
                Convenios = DBNull.Value != reader["Convenios"] ? reader["Convenios"].ToString() : "",
                MesInicial = DBNull.Value != reader["MesInicial"] ? reader["MesInicial"].ToString() : "",
                MesFinal = DBNull.Value != reader["MesFinal"] ? reader["MesFinal"].ToString() : "",
                FechaInicial = DBNull.Value != reader["FechaInicial"] ? Convert.ToDateTime(reader["FechaInicial"]) : Convert.ToDateTime("01/01/1990"),
                FechaFinal = DBNull.Value != reader["FechaFinal"] ? Convert.ToDateTime(reader["FechaFinal"]) : Convert.ToDateTime("01/01/1990"),
                Calificacion = DBNull.Value != reader["Calificacion"] ? Convert.ToDecimal(reader["Calificacion"]) : Convert.ToDecimal(0),
            };
        }
        private MIndiceEfectividad MapToValueII(SqlDataReader reader)
        {
            return new MIndiceEfectividad
            {
                Id = DBNull.Value != reader["Id"] ? (int)reader["Id"] : 0,
                Inmueble = DBNull.Value != reader["Inmueble"] ? reader["Inmueble"].ToString() : "",
                Tipo = DBNull.Value != reader["Tipo"] ? reader["Tipo"].ToString() : "",
                Valor = DBNull.Value != reader["Valor"] ? reader["Valor"].ToString() : "",
                TotalIncidencias = DBNull.Value != reader["TotalIncidencias"] ? (int)reader["TotalIncidencias"] : 0,
            };
        }
        private AvanceFinanciero MapToValueAvanceFinanciero(SqlDataReader reader)
        {
            return new AvanceFinanciero
            {
                MontoContratado = reader["MontoContratado"] != DBNull.Value ? (decimal)reader["MontoContratado"] : 0,
                MontoFacturado = reader["MontoFacturado"] != DBNull.Value ? (decimal)reader["MontoFacturado"] : 0,
                MontoDeductivas = reader["MontoDeductivas"] != DBNull.Value ? (decimal)reader["MontoDeductivas"] : 0,
                MontoPenalizacion = reader["MontoPenalizacion"] != DBNull.Value ? (decimal)reader["MontoPenalizacion"] : 0,
                MontoPagado = reader["MontoPagado"] != DBNull.Value ? (decimal)reader["MontoPagado"] : 0,
                MontoPorEjercer = reader["MontoPorEjercer"] != DBNull.Value ? (decimal)reader["MontoPorEjercer"] : 0,
                MontoPendientePago = reader["MontoPendientePago"] != DBNull.Value ? (decimal)reader["MontoPendientePago"] : 0
            };
        }
        private AvanceFisico MapToValueAvanceFisico(SqlDataReader reader)
        {
            return new AvanceFisico
            {
                ServiciosContratados = reader["ServiciosContratados"] != DBNull.Value ? Convert.ToInt64(reader["ServiciosContratados"]) : 0,
                ServiciosDevengados = reader["ServiciosDevengados"] != DBNull.Value ? Convert.ToInt64(reader["ServiciosDevengados"]) : 0,
                ServiciosPorDevengar = reader["ServiciosPorDevengar"] != DBNull.Value ? Convert.ToInt64(reader["ServiciosPorDevengar"]) : 0,
                PorcentajeFisico = reader["PorcentajeFisico"] != DBNull.Value ? (decimal)reader["PorcentajeFisico"] : 0
            };
        }
    }
}
