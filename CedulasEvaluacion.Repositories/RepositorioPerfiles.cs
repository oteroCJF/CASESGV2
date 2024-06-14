using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Repositories
{
    public class RepositorioPerfiles : IRepositorioPerfiles
    {
        private readonly string _connectionString;

        public RepositorioPerfiles(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection"); ;
        }

        public async Task<List<Perfiles>> getPerfiles()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getPerfiles", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var response = new List<Perfiles>();
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

        //insertamos el perfil
        public async Task<int> insertarPerfil(Perfiles perfiles)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertarPerfil", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id",SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@nombre", perfiles.Nombre));
                        await sql.OpenAsync();
                        int i = await cmd.ExecuteNonQueryAsync();

                        int id = Convert.ToInt32(cmd.Parameters["@id"].Value);


                        return i == 1 ? id : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }

        //actualizamos el perfil
        public async Task<int> actualizaPerfil(Perfiles perfiles)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaPerfil", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", perfiles.Id));
                        cmd.Parameters.Add(new SqlParameter("@nombre", perfiles.Nombre));
                        await sql.OpenAsync();
                        int i = await cmd.ExecuteNonQueryAsync();

                        return i == 1 ? i : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }

        //obtenemos el permiso para el modulo correspondiente
        public async Task<int> getPermiso(int user, string modulo, string operacion)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getPermisoModulo", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@modulo",modulo));
                        cmd.Parameters.Add(new SqlParameter("@user", user));
                        cmd.Parameters.Add(new SqlParameter("@operacion", operacion));
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                return 1;
                            }
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

        public async Task<List<Perfiles>> getPerfilesByUser(int user)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getPerfilesUsuario", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@user", user));
                        var response = new List<Perfiles>();

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

        public async Task<int> eliminaPerfilByUser(int id, int user)
        {
            try
            {

                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaPerfilByUser", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@usuario", user));
                        cmd.Parameters.Add(new SqlParameter("@perfil", id));
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        await sql.CloseAsync();
                        return 1;
                    }

                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        //obtenemos el perfil
        public async Task<Perfiles> getPerfilById(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getPerfil", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        var response = new Perfiles();
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

        private Perfiles MapToValue(SqlDataReader reader)
        {
            return new Perfiles { 
                Id = (int)reader["Id"],
                Nombre = reader["Nombre"].ToString()
            };
        }

    }
}
