using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.MIncidencias;
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
    public class RepositorioValidaciones : IRepositorioValidaciones
    {
        private readonly string _connectionString;

        public RepositorioValidaciones(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<bool> ObtienePeriodoComedor(CedulaEvaluacion cedula)
        {
            bool c = false;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getPeriodoComedor", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@anio", cedula.Anio));
                        cmd.Parameters.Add(new SqlParameter("@mes", cedula.Mes));
                        cmd.Parameters.Add(new SqlParameter("@fechaInicial", cedula.FechaInicial));
                        cmd.Parameters.Add(new SqlParameter("@fechaFinal", cedula.FechaFinal));
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                c = true;
                            }
                        }

                        return c;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
        }

        public async Task<bool> GetAccionesEntregables(string entregable, string estatus, int servicio)
        {
            bool c = false;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getAccionesEntregables", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@entregable", entregable));
                        cmd.Parameters.Add(new SqlParameter("@estatus", estatus));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                c = (int)reader["Existe"] != 0 ? true: false;
                            }
                        }

                        return c;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
        }

        public async Task<bool> GetValidacionIncidenciaComedor(IncidenciasComedor comedor)
        {
            bool c = false;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getFechaIncidenciaComedor", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", comedor.CedulaComedorId));
                        cmd.Parameters.Add(new SqlParameter("@tipo", comedor.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@incumplimiento", comedor.Incumplimiento));
                        cmd.Parameters.Add(new SqlParameter("@fechaIncidencia", comedor.FechaIncidencia));

                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                c = (int)reader["Existe"] != 0 ? true : false;
                            }
                        }

                        return c;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
        }

    }
}
