using CedulasEvaluacion.Entities.MAvisos;
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
    public class RepositorioAvisosCASESG: IRepositorioAvisosCASESG
    {
        private readonly string _connectionString;

        public RepositorioAvisosCASESG(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }
        public async Task<List<AvisosCASESG>> GetAvisosCASESG()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getAvisosCASESG", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var response = new List<AvisosCASESG>();
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
        public async Task<AvisosCASESG> GetAvisosCASESGById(int aviso)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getAvisosCASESGById", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", aviso));
                        var response = new AvisosCASESG();
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
        public async Task<List<VAvisos>> GetAvisosCASESGByUsuario(int usuario)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getAvisosCASESGByUsuario", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@usuario", usuario));
                        var response = new List<VAvisos>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueUsuario(reader));
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
        public async Task<List<AvisosPerfil>> GetPerfilesByAviso(int aviso)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getPerfilesByAviso", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@aviso", aviso));
                        var response = new List<AvisosPerfil>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValuePerfiles(reader));
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
        public async Task<UsuariosAvisos> GetAvisosVisibles(int usuario)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getAvisosVisiblesByUser", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@usuario", usuario));
                        var response = new UsuariosAvisos();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueVisibles(reader);
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
        public async Task<int> insertaAvisoCASESG(AvisosCASESG aviso)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaAvisoCASESG", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", aviso.Id)).Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@titulo", aviso.Titulo));
                        cmd.Parameters.Add(new SqlParameter("@activo", aviso.Activo));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", aviso.Comentarios));
                        cmd.Parameters.Add(new SqlParameter("@fechaInicio", aviso.FechaInicio));
                        cmd.Parameters.Add(new SqlParameter("@fechaFin", aviso.FechaFin));

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
        public async Task<int> actualizaAvisoCASESG(AvisosCASESG aviso)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaAvisoCASESG", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", aviso.Id));
                        cmd.Parameters.Add(new SqlParameter("@titulo", aviso.Titulo));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", aviso.Comentarios));
                        cmd.Parameters.Add(new SqlParameter("@fechaInicio", aviso.FechaInicio));
                        cmd.Parameters.Add(new SqlParameter("@fechaFin", aviso.FechaFin));
                        cmd.Parameters.Add(new SqlParameter("@activo", aviso.Activo));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        return aviso.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }
        public async Task<int> insertaAvisosPerfil(List<AvisosPerfil> avisosP,int aviso)
        {
            await eliminaPerfilesAviso(aviso);
            try
            {
                if (avisosP != null)
                {
                    foreach (var a in avisosP)
                    {
                        using (SqlConnection sql = new SqlConnection(_connectionString))
                        {
                            using (SqlCommand cmd = new SqlCommand("sp_insertaAvisosPerfil", sql))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add(new SqlParameter("@perfil", a.PerfilId));
                                cmd.Parameters.Add(new SqlParameter("@aviso", aviso));

                                await sql.OpenAsync();
                                int i = await cmd.ExecuteNonQueryAsync();
                                if (i != 1)
                                {
                                    return aviso;
                                }
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
        public async Task<int> insertaAvisosUsuario(UsuariosAvisos usuarioA)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaIncidenciasAnalisis", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@usuario", usuarioA.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@visible", usuarioA.Visible));

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
        public async Task<int> insertaUsuarioVisible(UsuariosAvisos avisosU)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaUsuarioVisible", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@usuario", avisosU.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@visible", avisosU.Visible));

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
        public async Task<int> eliminaAviso(AvisosCASESG aviso)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaAvisoCASESG", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", aviso.Id));

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
        public async Task<int> eliminaUsuarioVisible(UsuariosAvisos avisosU)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaUsuarioVisible", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@usuario", avisosU.UsuarioId));

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
        public async Task<int> eliminaPerfilesAviso(int aviso)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaPerfilesByAviso", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@aviso", aviso));
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
        public async Task<int> eliminaPerfilAviso(int aviso, int perfil)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaPerfilByAviso", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@aviso", aviso));
                        cmd.Parameters.Add(new SqlParameter("@perfil", perfil));
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
        private AvisosCASESG MapToValue(SqlDataReader reader)
        {
            return new AvisosCASESG
            {
                Id = (int)reader["Id"],
                Titulo = reader["Titulo"].ToString(),
                Comentarios = reader["Comentarios"] != DBNull.Value ? reader["Comentarios"].ToString() : "",
                FechaInicio = reader["FechaInicio"] != DBNull.Value ? Convert.ToDateTime(reader["FechaInicio"]) : DateTime.Now,
                FechaFin = reader["FechaFin"] != DBNull.Value ? Convert.ToDateTime(reader["FechaFin"]) : DateTime.Now,
                Activo = reader["Activo"] != DBNull.Value ? (bool)reader["Activo"] : false
            };
        }
        private VAvisos MapToValueUsuario(SqlDataReader reader)
        {
            return new VAvisos
            {
                Id = (int)reader["Id"],
                Titulo = reader["Titulo"].ToString(),
                Comentarios = reader["Comentarios"] != DBNull.Value ? reader["Comentarios"].ToString() : "",
                FechaInicio = reader["FechaInicio"] != DBNull.Value ? Convert.ToDateTime(reader["FechaInicio"]) : DateTime.Now,
                FechaFin = reader["FechaFin"] != DBNull.Value ? Convert.ToDateTime(reader["FechaFin"]) : DateTime.Now,
                Activo = reader["Activo"] != DBNull.Value ? (bool)reader["Activo"] : false,
            };
        }
        private AvisosPerfil MapToValuePerfiles(SqlDataReader reader)
        {
            return new AvisosPerfil
            {
                AvisoId = (int)reader["AvisoId"],
                PerfilId = (int)reader["PerfilId"]
            };
        }        
        private UsuariosAvisos MapToValueVisibles(SqlDataReader reader)
        {
            return new UsuariosAvisos
            {
                UsuarioId = (int)reader["UsuarioId"],
                Visible = (bool)reader["Visible"]
            };
        }
    }
}
