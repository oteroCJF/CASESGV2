using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.Models;
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
    public class RepositorioEvaluacionServicios : IRepositorioEvaluacionServicios
    {
        private readonly string _connectionString;

        public RepositorioEvaluacionServicios(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<int> VerificaCedula(CedulaEvaluacion cedula)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_buscaCedulaRegistrada", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", cedula.Id)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@servicioId", cedula.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@anio", cedula.Anio));
                        cmd.Parameters.Add(new SqlParameter("@mes", cedula.Mes));
                        cmd.Parameters.Add(new SqlParameter("@inmueble", cedula.InmuebleId));
                        if(cedula.TipoServicio != null && !cedula.TipoServicio.Equals(""))
                            cmd.Parameters.Add(new SqlParameter("@tipoServicio", cedula.TipoServicio));
                        if (!cedula.FechaInicial.ToString("yyyy-MM-dd").Equals("0001-01-01"))
                            cmd.Parameters.Add(new SqlParameter("@fechaInicial", cedula.FechaInicial));
                        if (!cedula.FechaFinal.ToString("yyyy-MM-dd").Equals("0001-01-01"))
                            cmd.Parameters.Add(new SqlParameter("@fechaFinal", cedula.FechaFinal));
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                id = Convert.ToInt32(reader["Id"].ToString());
                            }
                            return id;
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

        //Posible metodo de sustitución a los servicios
        public async Task<List<VCedulas>> GetCedulasEvaluacion(ModelsIndex cedula, int usuario)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCedulasEvaluacionFiltro", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicioId", cedula.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@anio", cedula.Anio));
                        cmd.Parameters.Add(new SqlParameter("@mes", cedula.Mes));
                        cmd.Parameters.Add(new SqlParameter("@estatus", cedula.Estatus));
                        cmd.Parameters.Add(new SqlParameter("@inmueble", cedula.InmuebleId));
                        cmd.Parameters.Add(new SqlParameter("@administracion", cedula.AdministracionId));
                        cmd.Parameters.Add(new SqlParameter("@iDestino", cedula.InmuebleDestinoId));
                        if(!cedula.FechaInicial.ToString("dd/MM/yyyy").Equals("01/01/0001"))
                            cmd.Parameters.Add(new SqlParameter("@fechaInicial", cedula.FechaInicial.ToString("yyyy-MM-dd")));
                        if (!cedula.FechaFinal.ToString("dd/MM/yyyy").Equals("01/01/0001"))
                            cmd.Parameters.Add(new SqlParameter("@fechaFinal", cedula.FechaFinal.ToString("yyyy-MM-dd")));
                        cmd.Parameters.Add(new SqlParameter("@usuario", usuario));
                        var response = new List<VCedulas>();
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
        public async Task<int> insertaCedula(CedulaEvaluacion cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaCedulaEvaluacion", sql))
                    {
                        string folio = await GetFolioCedula(cedula.ServicioId);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", cedula.Id)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@servicioId", cedula.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@usuario", cedula.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@inmueble", cedula.InmuebleId));
                        if(cedula.InmuebleDestinoId != 0)
                            cmd.Parameters.Add(new SqlParameter("@inmuebleDestino", cedula.InmuebleDestinoId));
                        cmd.Parameters.Add(new SqlParameter("@folio", generaFolio(cedula.Anio,cedula.InmuebleId, cedula.Mes, await GetFolioCedula(cedula.ServicioId),cedula.InmuebleDestinoId)));
                        cmd.Parameters.Add(new SqlParameter("@mes", cedula.Mes));
                        cmd.Parameters.Add(new SqlParameter("@anio", cedula.Anio));
                        if (!cedula.FechaInicial.ToString("yyyy-MM-dd").Equals("0001-01-01"))
                            cmd.Parameters.Add(new SqlParameter("@fechaInicial", cedula.FechaInicial));
                        if (!cedula.FechaFinal.ToString("yyyy-MM-dd").Equals("0001-01-01"))
                            cmd.Parameters.Add(new SqlParameter("@fechaFinal", cedula.FechaFinal));
                        if (cedula.TipoServicio != null)
                            cmd.Parameters.Add(new SqlParameter("@tipoServicio", cedula.TipoServicio));

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
        public async Task<string> GetFolioCedula(int servicio)
        {
            string folio = "";
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getFolioCedula", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                folio = reader["Valor"].ToString();
                            }
                        }
                    }
                }
                return folio;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
        public async Task<CedulaEvaluacion> CedulaById(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCedulaEvaluacionById", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        CedulaEvaluacion response = null;
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueCedula(reader);
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
        public async Task<int> GuardaRespuestas(List<RespuestasEncuesta> respuestasEncuestas)
        {
            int id = 0;
            try
            {
                foreach (var r in respuestasEncuestas)
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertaActualizaRespuestasEvaluacion", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(new SqlParameter("@cedula", r.CedulaEvaluacionId));
                            cmd.Parameters.Add(new SqlParameter("@pregunta", r.Pregunta));
                            cmd.Parameters.Add(new SqlParameter("@respuesta", r.Respuesta));
                            if (r.Detalles != null)
                                cmd.Parameters.Add(new SqlParameter("@detalles", r.Detalles));

                            await sql.OpenAsync();
                            int i = await cmd.ExecuteNonQueryAsync();
                            if (i > 0)
                            {
                                id = Convert.ToInt32(cmd.Parameters["@id"].Value);
                            }
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
        public async Task<List<RespuestasEncuesta>> obtieneRespuestas(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getRespuestasEvaluacion", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        var response = new List<RespuestasEncuesta>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueRespuestas(reader));
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
        public async Task<int> enviaRespuestas(int servicio, int cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_calculaEvaluacionServicio", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicioId", servicio));
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));

                        await sql.OpenAsync();

                        await cmd.ExecuteNonQueryAsync();
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
        public async Task<int> apruebaRechazaCedula(CedulaEvaluacion cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_autorizarRechazarCedula", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", cedula.Id));
                        cmd.Parameters.Add(new SqlParameter("@estatus", cedula.Estatus));
                        cmd.Parameters.Add(new SqlParameter("@servicioId", cedula.ServicioId));

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
        public async Task<int> capturaHistorial(HistorialCedulas historialCedulas)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaHistorialCedulas", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", historialCedulas.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@cedula", historialCedulas.CedulaId));
                        cmd.Parameters.Add(new SqlParameter("@usuario", historialCedulas.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@estatus", historialCedulas.Estatus));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", historialCedulas.Comentarios));

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
        }//
        public async Task<List<HistorialCedulas>> getHistorial(object id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getHistorialCedula", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedula", id));
                        var response = new List<HistorialCedulas>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueHistorial(reader));
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
        }//
        public async Task<int> EliminaCedula(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminarCedulaLimpieza", sql))
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
        public int CuentaCedulasUrgentes(List<VCedulasEvaluacion> cedulas)
        {
            int i = 0;
            foreach (var ced in cedulas)
            {
                if (ced.Prioridad.Equals("Urgente"))
                {
                    i++;
                }
            }

            return i;
        }

        public async Task<string> getPlantillaCorreo(int cedula)
        {
            try
            {
                string html = "";
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getPlantillaCorreo", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                html = reader["html"].ToString();
                            }
                        }

                        return html;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
        
        public async Task<string> getPlantillaCorreoSNC(int cedula)
        {
            try
            {
                string html = "";
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getPlantillaSolicitudNC", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                html = reader["html"].ToString();
                            }
                        }

                        return html;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }
        public async Task<string> getPlantillaCorreoSNCSD(int cedula)
        {
            try
            {
                string html = "";
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getPlantillaSolicitudNCSD", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                html = reader["html"].ToString();
                            }
                        }

                        return html;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        /*************** Obtiene Penalizaciones *******************/
        public async Task<List<Penalizaciones>> GetPenalizacionesByCedula(int cedula, int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getPenasDeductivas", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<Penalizaciones>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValuePenalizaciones(reader));
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
        public async Task<decimal> SumaDeductivas(int cedula, int servicio)
        {
            decimal totalDeductivas = 0;
            List<Penalizaciones> deductivas = await GetPenalizacionesByCedula(cedula, servicio);
            for (var i = 0; i < deductivas.Count; i++)
            {
                if (deductivas[i].TipoDeduccion.Equals("Deductiva"))
                {
                    totalDeductivas = totalDeductivas + deductivas[i].MontoPenalizacion;
                }
            }
            return totalDeductivas;
        }
        public async Task<decimal> SumaPenalizaciones(int cedula, int servicio)
        {
            decimal totalPenas= 0;
            List<Penalizaciones> penalizaciones = await GetPenalizacionesByCedula(cedula, servicio);
            for (var i = 0; i < penalizaciones.Count; i++)
            {
                if (penalizaciones[i].TipoDeduccion.Equals("Penalizacion"))
                {
                    totalPenas = totalPenas + penalizaciones[i].MontoPenalizacion;
                }
            }
            return totalPenas;
        }
        
        public async Task<int> SolicitudNC(CedulaEvaluacion cedula)
        {
            int success = -1; 
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_solicitudNotaCredito", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula.Id));
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        success = 1;
                        return success;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }

        public async Task<int> autorizarRechazarSNC(HistorialNotasCredito cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_autorizarRechazarSNC", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", cedula.Id));
                        cmd.Parameters.Add(new SqlParameter("@estatus", cedula.Estatus));

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

        public async Task<int> AutorizarRechazoCedula(CedulaEvaluacion cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_autorizarRechazoCedula", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", cedula.Id));
                        cmd.Parameters.Add(new SqlParameter("@usuario", cedula.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@servicio", cedula.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@estatus", cedula.Estatus));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", cedula.Comentarios));

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

        public async Task<int> SolicitudRechazoCedula(CedulaEvaluacion cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_solicitudRechazoCedula", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", cedula.Id));
                        cmd.Parameters.Add(new SqlParameter("@usuario", cedula.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@servicio", cedula.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@estatus", cedula.Estatus));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", cedula.Comentarios));

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

        public async Task<List<HistorialNotasCredito>> HistorialNotasCredito(int cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getHistorialNC", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
                        var response = new List<HistorialNotasCredito>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueHistorialNC(reader));
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

        private HistorialNotasCredito MapToValueHistorialNC(SqlDataReader reader)
        {
           return new HistorialNotasCredito
           {
               Id = (int)reader["Id"],
               CedulaEvaluacionId = (int)reader["CedulaEvaluacionId"],
               FechaEmision = reader["FechaEmision"] != DBNull.Value ? Convert.ToDateTime(reader["FechaEmision"]) : Convert.ToDateTime("1990-01-01T00:00:00"),
               FechaVencimiento = reader["FechaVencimiento"] != DBNull.Value ? Convert.ToDateTime(reader["FechaVencimiento"]) : Convert.ToDateTime("1990-01-01T00:00:00"),
               MontoDupe = reader["MontoDupe"] != DBNull.Value ? (decimal)reader["MontoDupe"] : 0,
               Vencio = reader["Vencio"] != DBNull.Value ? (bool)reader["Vencio"] : false,
               Comentarios = reader["Comentarios"] != DBNull.Value ? reader["Comentarios"].ToString() : "",
               Estatus = reader["Estatus"] != DBNull.Value ? reader["Estatus"].ToString() : "",
           };
        }


        /*************** Fin de obtiener Penalizaciones *******************/

        /*Identifica la Pregunta de Capacitación en el Servicio de Limpieza y Fumigacíón*/
        public async Task<bool> GetPreguntaCapacitacion(int cedula)
        {
            bool id = false;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getPreguntaCapacitacion", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
                        var response = new List<VCedulasEvaluacion>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                id = reader.HasRows;
                            }
                        }
                    }
                }
                return id;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
        }
        /*Fin Identifica la Pregunta de Capacitación en el Servicio de Limpieza y Fumigacíón*/

        /*Identifica la Cédula está en Enviado a DAS o Rechazada*/
        public async Task<bool> GetEstatusCedula(int cedula)
        {
            bool success = false;
            string estatus = "";
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getEstatusCedula", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@estatus", SqlDbType.VarChar,50)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
                        var response = new List<VCedulasEvaluacion>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            estatus = cmd.Parameters["@estatus"].Value+"";
                            if (estatus.Equals("En Proceso") || estatus.Equals("Rechazada"))
                            {
                                success = !success;
                            }
                        }
                    }
                }
                return success;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
        }
        /*Fin Identifica la Pregunta de Capacitación en el Servicio de Limpieza y Fumigacíón*/
        private VCedulas MapToValue(SqlDataReader reader)
        {
            return new VCedulas()
            {
                Id = (int)reader["Id"],
                ServicioId = (int)reader["ServicioId"],
                InmuebleId = (int)reader["InmuebleId"],
                TipoServicio = reader["TipoServicio"] != DBNull.Value ? reader["TipoServicio"].ToString():"",
                Abreviacion = reader["Abreviacion"] != DBNull.Value ? reader["Abreviacion"].ToString():"",
                Folio = reader["Folio"].ToString(),
                Nombre = reader["Nombre"].ToString(),
                Destino = reader["Destino"].ToString(),
                Mes = reader["Mes"].ToString(),
                Anio = (int)reader["Anio"],
                CedulaValidada = reader["CedulaValidada"] != DBNull.Value ? (bool)reader["CedulaValidada"] : false,
                MemoValidado = reader["MemoValidado"] != DBNull.Value ? (bool) reader["MemoValidado"] : false,
                ActaFirmada = reader["ActaFirmada"] != DBNull.Value ? (bool) reader["ActaFirmada"] : false,
                Servicio = reader["Servicio"].ToString(),
                Estatus = reader["Estatus"].ToString(),
                Calificacion = reader["Calificacion"] != DBNull.Value ? (decimal)reader["Calificacion"] : 0,
                Fondo = reader["Fondo"].ToString(),
                Icono = reader["Icono"].ToString(),
                FechaInicial = reader["FechaInicial"] != DBNull.Value ? Convert.ToDateTime(reader["FechaInicial"]) : Convert.ToDateTime("1990-01-01T00:00:00"),
                FechaFinal = reader["FechaFinal"] != DBNull.Value ? Convert.ToDateTime(reader["FechaFinal"]) : Convert.ToDateTime("1990-01-01T00:00:00"),
                FechaActualizacion = reader["FechaActualizacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaActualizacion"]): Convert.ToDateTime(reader["FechaCreacion"])
            };
        }
        private CedulaEvaluacion MapToValueCedula(SqlDataReader reader)
        {
            return new CedulaEvaluacion()
            {
                Id = (int)reader["Id"],
                ServicioId = (int)reader["ServicioId"],
                UsuarioId = (int)reader["UsuarioId"],
                InmuebleId = (int)reader["InmuebleId"],
                InmuebleDestinoId = reader["InmuebleDestinoId"] != DBNull.Value ? (int)reader["InmuebleDestinoId"]:0,
                TipoServicio = reader["TipoServicio"] != DBNull.Value ? reader["TipoServicio"].ToString() : "",
                Maniobras = reader["Maniobras"] != DBNull.Value ? (int)reader["Maniobras"] :0,
                Folio = reader["Folio"].ToString(),
                Icono = reader["Icono"].ToString(),
                Fondo = reader["Fondo"].ToString(),
                Mes = reader["Mes"].ToString(),
                Anio = (int)reader["Anio"],
                Alcance = reader["Alcance"] != DBNull.Value ? (bool)reader["Alcance"] : false,
                Calificacion = reader["Calificacion"] != DBNull.Value ? (decimal)reader["Calificacion"] : 5,
                PenaCalificacion = reader["PenaCalificacion"] != DBNull.Value ? (decimal)reader["PenaCalificacion"] : 0,
                FechaInicial = reader["FechaInicial"] != DBNull.Value ? (DateTime)reader["FechaInicial"] : Convert.ToDateTime("01/01/1990"+""),
                FechaFinal = reader["FechaFinal"] != DBNull.Value ? (DateTime)reader["FechaFinal"] : Convert.ToDateTime("01/01/1990" + ""),
                Estatus = reader["Estatus"].ToString(),
                FechaCreacion = (DateTime)reader["FechaCreacion"],
                FechaActualizacion = reader["FechaActualizacion"] != DBNull.Value ? (DateTime)reader["FechaActualizacion"] : (DateTime)reader["FechaCreacion"]
            };
        }
        private RespuestasEncuesta MapToValueRespuestas(SqlDataReader reader)
        {
            return new RespuestasEncuesta
            {
                CedulaEvaluacionId = (int)reader["CedulaEvaluacionId"],
                Pregunta = (int)reader["Pregunta"],
                Respuesta = Convert.ToBoolean(reader["Respuesta"]),
                Detalles = reader["Detalles"] != DBNull.Value ? reader["Detalles"].ToString(): "",
                Penalizable = reader["Penalizable"] != DBNull.Value ? Convert.ToBoolean(reader["Penalizable"]) : false,
                MontoPenalizacion = reader["MontoPenalizacion"] != DBNull.Value ? Convert.ToDecimal(reader["MontoPenalizacion"]) : 0
            };
        }
        private HistorialCedulas MapToValueHistorial(SqlDataReader reader)
        {
            return new HistorialCedulas
            {
                Servicio = reader["Servicio"].ToString(),
                CedulaId = (int)reader["CedulaId"],
                UsuarioId = (int)reader["UsuarioId"],
                Estatus = reader["Estatus"].ToString(),
                Comentarios = reader["Comentarios"].ToString(),
                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"])
            };
        }        
        private Penalizaciones MapToValuePenalizaciones(SqlDataReader reader)
        {
            return new Penalizaciones
            {
                Id = (int)reader["Id"],
                CedulaEvaluacionId = (int)reader["CedulaEvaluacionId"],
                Pregunta = (int)reader["Pregunta"],
                PreguntaReal = DBNull.Value != reader["PreguntaReal"] ? (int) reader["PreguntaReal"]:0,
                Penalizable = DBNull.Value != reader["Penalizable"] ? (bool)reader["Penalizable"]:false,
                Abreviacion = DBNull.Value != reader["Abreviacion"] ? reader["Abreviacion"].ToString() : "",
                TipoDeduccion = DBNull.Value != reader["TipoDeduccion"] ? reader["TipoDeduccion"].ToString() : "",
                Icono = DBNull.Value != reader["Icono"] ? reader["Icono"].ToString() : "",
                Valor = DBNull.Value != reader["Valor"] ? reader["Valor"].ToString() : "",
                MontoPenalizacion = DBNull.Value != reader["MontoPenalizacion"] ? (decimal)reader["MontoPenalizacion"] : 0,                
            };
        }
        private string generaFolio(int date, int inmuebleId, string mes, string servicio, int destino)
        {
            string inmueble = "";
            string inmuebleD = "";
            if (destino != 0)
            {
                inmueble = inmuebleId <= 9 ? "-O0" + inmuebleId: "-O"+inmuebleId+"";
                inmuebleD = destino <= 9 ? "-D0" + destino : "-D" + destino + "";
                return servicio + inmueble + inmuebleD + "-" + date + convertirMes(mes);
            }
            else
            {
                return inmuebleId <= 9 ? servicio + "-0" + inmuebleId + "-" + date + convertirMes(mes) : servicio + "-" + inmuebleId + "-" + date + convertirMes(mes);
            }            
        }
        private string convertirMes(string mes)
        {
            string[] meses = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            for (var i = 0; i < meses.Length; i++)
            {
                if (meses[i] == mes)
                    return (i + 1) > 9 ? (i + 1) + "" : "0" + (i + 1);
            }
            return "";
        }
    }
}
