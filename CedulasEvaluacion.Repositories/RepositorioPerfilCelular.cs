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
    public class RepositorioPerfilCelular: IRepositorioPerfilCelular
    {
        private readonly string _connectionString;

        public RepositorioPerfilCelular(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        private SqlConnection conexion()
        {
            return new SqlConnection(_connectionString);
        }

        //obtenemos las cedulas de limpieza
        public async Task<List<PerfilesCelular>> GetPerfilesCelular()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getPerfilesCelular", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var response = new List<PerfilesCelular>();
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

        public async Task<PerfilesCelular> GetPerfilCelularById(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getPerfilCelularById", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        var response = new PerfilesCelular();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValue(reader);
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

        private PerfilesCelular MapToValue(SqlDataReader reader)
        {
            return new PerfilesCelular {
                Id = (int)reader["Id"],
                Nombre = reader["Nombre"].ToString(),
                TipoPerfil = reader["TipoPerfil"].ToString(),
                CostoMensual = (decimal)reader["CostoMensual"],
                CostoMensualIVA = (decimal)reader["CostoMensualIVA"],
            };
        }
    }
}
