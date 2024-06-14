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
    public class RepositorioEntregablesContrato : IRepositorioEntregablesContrato
    {
        private readonly string _connectionString;

        public RepositorioEntregablesContrato(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<List<EntregablesContrato>> GetEntregablesCS(int contrato)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getEntregablesCS", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        var response = new List<EntregablesContrato>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueEntregablesContratos(reader));
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

        public async Task<EntregablesContrato> GetEntregableCsById(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getEntregableCsById", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        var response = new EntregablesContrato();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueEntregablesContratos(reader);
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

        public async Task<int> InsertarActualizarContrato(EntregablesContrato entregables)
        {
            DateTime date = DateTime.Now;
            string date_str = date.ToString("yyyyMMddHHmmss");
            int id = 0;
            string saveFile = "Ok";

            if (entregables.Archivo != null) { 
                if (entregables.Id != 0)
                {
                    int isDeleted = await eliminaArchivo(entregables);
                }

                saveFile = await guardaArchivo(entregables.Archivo, entregables.ContratoId + "", date_str);
            }

            try
            {
                if (saveFile.Equals("Ok"))
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertarActualizarEntregableContrato", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@id", entregables.Id)).Direction = System.Data.ParameterDirection.Output;
                            if (entregables.Id != 0)
                                cmd.Parameters.Add(new SqlParameter("@ids", entregables.Id));

                            cmd.Parameters.Add(new SqlParameter("@contratoId", entregables.ContratoId));
                            cmd.Parameters.Add(new SqlParameter("@descripcion", entregables.Descripcion));
                            cmd.Parameters.Add(new SqlParameter("@tipoContrato", entregables.TipoContrato));
                            cmd.Parameters.Add(new SqlParameter("@tipo", entregables.Tipo));
                            if (entregables.Archivo != null)
                            {
                                cmd.Parameters.Add(new SqlParameter("@archivo", (date_str + "_" + entregables.Archivo.FileName)));
                                cmd.Parameters.Add(new SqlParameter("@tamanio", entregables.Archivo.Length));
                            }
                            cmd.Parameters.Add(new SqlParameter("@comentarios", entregables.Comentarios));
                            cmd.Parameters.Add(new SqlParameter("@inicioPeriodo", entregables.InicioPeriodo));
                            cmd.Parameters.Add(new SqlParameter("@finPeriodo", entregables.FinPeriodo));
                            cmd.Parameters.Add(new SqlParameter("@montoGarantia", entregables.MontoGarantia));
                            cmd.Parameters.Add(new SqlParameter("@fechaProgramada", entregables.FechaProgramada.ToString("MM/dd/yyyy")));
                            cmd.Parameters.Add(new SqlParameter("@fechaEntrega", entregables.FechaEntrega.ToString("MM/dd/yyyy")));

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
                return 0;
            }
        }

        public async Task<string> guardaArchivo(IFormFile archivo, string folio, string date)
        {
            long size = archivo.Length;
            string folderCedula = folio;

            string newPath = Directory.GetCurrentDirectory() + "\\ObligacionesPS\\Contrato_" + folderCedula;
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
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaEntregableContrato", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", entregable.Id));
                        cmd.Parameters.Add(new SqlParameter("@archivo", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        string archivo = (cmd.Parameters["@archivo"].Value).ToString();
                        string newPath = Directory.GetCurrentDirectory() + "\\Entregables\\Contrato_" + entregable.ContratoId + "\\" + archivo;
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

        private EntregablesContrato MapToValueEntregablesContratos(SqlDataReader reader)
        {
            return new EntregablesContrato {
                Id = (int)reader["Id"],
                ContratoId = (int)reader["ContratoId"],
                Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : "",
                MontoGarantia = reader["MontoGarantia"] != DBNull.Value ? (decimal)reader["MontoGarantia"] : 0,
                MontoPenalizacion = reader["MontoPenalizacion"] != DBNull.Value ? (decimal)reader["MontoPenalizacion"] : 0,
                Tipo = reader["Tipo"] != DBNull.Value ? reader["Tipo"].ToString() : "",
                TipoContrato = reader["TipoContrato"] != DBNull.Value ? reader["TipoContrato"].ToString() : "",
                InicioPeriodo = reader["InicioPeriodo"] != DBNull.Value ? Convert.ToDateTime(reader["InicioPeriodo"].ToString()):Convert.ToDateTime("0000-00-00"),
                FinPeriodo = reader["FinPeriodo"] != DBNull.Value ?Convert.ToDateTime(reader["FinPeriodo"].ToString()): Convert.ToDateTime("0000-00-00"),
                FechaProgramada = reader["FechaProgramada"] != DBNull.Value ? Convert.ToDateTime(reader["FechaProgramada"].ToString()): Convert.ToDateTime("0000-00-00"),
                FechaEntrega = reader["FechaEntrega"] != DBNull.Value ?Convert.ToDateTime(reader["FechaEntrega"].ToString()): Convert.ToDateTime("0000-00-00"),
                Comentarios = reader["Comentarios"] != DBNull.Value ? reader["Comentarios"].ToString():"",
                NombreArchivo = reader["Archivo"].ToString(),
                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"].ToString()),
            };
        }
    }
}
