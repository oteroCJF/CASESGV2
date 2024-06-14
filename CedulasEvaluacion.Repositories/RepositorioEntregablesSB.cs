using CedulasEvaluacion.Entities.Models;
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
    public class RepositorioEntregablesSB : IRepositorioEntregablesSB
    {
        private readonly string _connectionString;

        public RepositorioEntregablesSB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<List<Entregables>> GetEntregablesSB(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getEntregablesSB", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        var response = new List<Entregables>();
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

        public async Task<int> insertaEntregableSB(Entregables entregables)
        {
            DateTime date = DateTime.Now;
            string date_str = date.ToString("yyyyMMddHHmmss");
            int id = 0;
            string saveFile = "";

            if (entregables.Archivo != null)
            {
                saveFile = await guardaArchivo(entregables.Archivo, entregables.NombreServicio, date_str, entregables.Mes);
            }
            try
            {
                if (saveFile.Equals("Ok") || entregables.Archivo == null)
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertaEntregableSB", sql))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@id", entregables.Id)).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(new SqlParameter("@servicio", entregables.ServicioBasico));
                            cmd.Parameters.Add(new SqlParameter("@tipo", entregables.Tipo));
                            if (entregables.Archivo != null)
                            {
                                cmd.Parameters.Add(new SqlParameter("@archivo", (date_str + "_" + entregables.Archivo.FileName)));
                            }
                            cmd.Parameters.Add(new SqlParameter("@comentarios", entregables.Comentarios));

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

        public async Task<int> actualizaEntregableSB(Entregables entregables)
        {
            DateTime date = DateTime.Now;
            string date_str = date.ToString("yyyyMMddHHmmss");
            int id = 0;
            string saveFile = "";
            
            if (entregables.Id != 0 && entregables.Archivo != null)
            {
                int isDeleted = await eliminaEntregable(entregables);
            }

            if (entregables.Archivo != null)
            {
                saveFile = await guardaArchivo(entregables.Archivo, entregables.NombreServicio, date_str, entregables.Mes);
            }
            try
            {
                if (saveFile.Equals("Ok") || entregables.Archivo == null)
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_actualizaEntregableSB", sql))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@id", entregables.Id));
                            cmd.Parameters.Add(new SqlParameter("@servicio", entregables.ServicioBasico));
                            cmd.Parameters.Add(new SqlParameter("@tipo", entregables.Tipo));
                            if (entregables.Archivo != null)
                            {
                                cmd.Parameters.Add(new SqlParameter("@archivo", (date_str + "_" + entregables.Archivo.FileName)));
                            }
                            cmd.Parameters.Add(new SqlParameter("@comentarios", entregables.Comentarios));

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

        public async Task<string> guardaArchivo(IFormFile archivo, string servicio, string date, string mes)
        {
            long size = archivo.Length;
            string folderCedula = servicio;

            string newPath = Directory.GetCurrentDirectory() + "\\Entregables SB\\" + servicio + "\\" + mes;
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

        public async Task<int> eliminaEntregable(Entregables sb)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaEntregableSB", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", sb.Id));
                        cmd.Parameters.Add(new SqlParameter("@archivo", System.Data.SqlDbType.VarChar, 500)).Direction = System.Data.ParameterDirection.Output;
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        string archivo = (cmd.Parameters["@archivo"].Value).ToString();
                        string newPath = Directory.GetCurrentDirectory() + "\\Entregables SB\\" + sb.NombreServicio + "\\" + sb.Mes + "\\" + archivo;
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

        private Entregables MapToValue(SqlDataReader reader)
        {
            return new Entregables
            {
                Id = (int)reader["Id"],
                ServicioBasico = (int)reader["SBasicoId"],
                Tipo = reader["Tipo"].ToString(),
                NombreArchivo = reader["Archivo"].ToString(),
                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"].ToString()),
                FechaActualizacion = reader["FechaActualizacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaActualizacion"].ToString()) : Convert.ToDateTime("01/01/1990"),
                Comentarios = reader["Comentarios"].ToString(),
            };
        }
    }
}
