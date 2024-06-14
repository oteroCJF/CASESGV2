using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using CedulasEvaluacion.Entities.Login;
using System.Xml;
using Newtonsoft.Json;
using CedulasEvaluacion.Entities.MCedula;

namespace CedulasEvaluacion.Repositories
{
    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly string _connectionString;
        private readonly IRepositorioAreas vRepositorioAreas;

        public RepositorioUsuarios(IConfiguration configuration, IRepositorioAreas iRepositorioAreas)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
            this.vRepositorioAreas = iRepositorioAreas ?? throw new ArgumentNullException(nameof(iRepositorioAreas));
        }

        private SqlConnection conexion()
        {
            return new SqlConnection(_connectionString);
        }

        //Método para Obtener los Usuarios
        public async Task<List<Usuarios>> getUsuarios()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_getUsuarios", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    var response = new List<Usuarios>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(MapToValueUsuarios(reader));
                        }
                    }

                    return response;
                }
            }
        }

        //Método para Obtener un usuario especifico
        public async Task<Usuarios> getUserById(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getUserById", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        Usuarios response = null;
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueUsuarios(reader);
                            }
                        }
                        return response;
                    }
                }
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        //Inserta usuario en el sistema
        public async Task<int> insertaUsuario(string datosUsuario, string password)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(datosUsuario);

            XmlNodeList list_dg = doc.SelectNodes("//DatosGenerales");
            XmlNodeList list_areas = doc.SelectNodes("//Áreas");

            string jsonDatosGen = JsonConvert.SerializeXmlNode(list_dg[0], Newtonsoft.Json.Formatting.None, true);
            string jsonAreas = JsonConvert.SerializeXmlNode(list_areas[0], Newtonsoft.Json.Formatting.None, true);

            Usuarios user = JsonConvert.DeserializeObject<Usuarios>(jsonDatosGen);
            Areas area = JsonConvert.DeserializeObject<Areas>(jsonAreas);

            //Definimos la variable con el ID del area que se registrara...
            int areaID = await vRepositorioAreas.insertaArea(area);

            SqlConnection sqlConexion = conexion();
            SqlCommand Comm = null;
            int respuesta = 0;
            DatosUsuario usuario = new DatosUsuario();
            try
            {
                sqlConexion.Open();
                Comm = sqlConexion.CreateCommand();
                Comm.CommandText = "dbo.sp_insertaUsuario";
                Comm.CommandType = CommandType.StoredProcedure;
                Comm.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                Comm.Parameters.Add("@areaId", SqlDbType.Int).Value = areaID;
                Comm.Parameters.Add("@expediente", SqlDbType.Int).Value = user.exp;
                Comm.Parameters.Add("@usuario", SqlDbType.VarChar, 10).Value = user.cve_usuario;
                Comm.Parameters.Add("@password", SqlDbType.VarChar, 20).Value = password;
                Comm.Parameters.Add("@nombre", SqlDbType.VarChar, 50).Value = user.nombre_emp;
                Comm.Parameters.Add("@apellidoPat", SqlDbType.VarChar, 50).Value = user.paterno_emp;
                Comm.Parameters.Add("@apellidoMat", SqlDbType.VarChar, 50).Value = user.materno_emp;
                Comm.Parameters.Add("@rfc", SqlDbType.VarChar, 14).Value = user.rfc_emp;
                Comm.Parameters.Add("@curp", SqlDbType.VarChar, 20).Value = user.curp_emp;
                Comm.Parameters.Add("@cvePuesto", SqlDbType.VarChar, 15).Value = user.cve_puesto;
                Comm.Parameters.Add("@puesto", SqlDbType.VarChar, 256).Value = user.nom_pue;
                if (!String.IsNullOrEmpty(user.correo_electronico))
                    Comm.Parameters.Add("@email", SqlDbType.VarChar, 256).Value = user.correo_electronico;
                else
                    Comm.Parameters.Add("@email", SqlDbType.VarChar, 256).Value = "N/A";

                Comm.Parameters.Add("@nomCat", SqlDbType.VarChar, 20).Value = user.nom_cat;
                Comm.Parameters.Add("@intentos", SqlDbType.Int).Value = user.intentos;
                Comm.Parameters.Add("@vigenciaPass", SqlDbType.DateTime).Value = user.fch_vig_pswd;

                respuesta = await Comm.ExecuteNonQueryAsync();
                if (respuesta > 0)
                {
                    return respuesta;
                }


            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            finally
            {
                Comm.Dispose();
                sqlConexion.Close();
                sqlConexion.Dispose();
            }

            return 0;
        }

        //Obtenemos los inmuebles por usuaio
        public async Task<List<InmueblesUsuarios>> getInmueblesUsuario(int user)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_getInmueblesUsuario", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@user", user));
                    var response = new List<InmueblesUsuarios>();
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

        public async Task<int> insertaAdminByUser(List<InmueblesUsuarios> inmueblesUsuarios)
        {
            try
            {

                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    foreach (var inmUsr in inmueblesUsuarios)
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertaAdminByUser", sql))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@usuario", inmUsr.UsuarioId));
                            cmd.Parameters.Add(new SqlParameter("@administracion", inmUsr.AdministracionId));
                            await sql.OpenAsync();
                            await cmd.ExecuteNonQueryAsync();
                            await sql.CloseAsync();
                        }
                    }
                        return 1;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<int> asignaPerfil(List<PerfilesUsuario> perfilesUsuarios)
        {
            try
            {

                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    foreach (var pu in perfilesUsuarios)
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertaPerfilByUser", sql))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@usuario", pu.UsuarioId));
                            cmd.Parameters.Add(new SqlParameter("@perfil", pu.PerfilId));
                            await sql.OpenAsync();
                            await cmd.ExecuteNonQueryAsync();
                            await sql.CloseAsync();
                        }
                    }
                    return 1;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<int> EliminaAdminByUser(int usuario, int administracion)
        {
            try
            {

                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaAdminByUser", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@usuario", usuario));
                        cmd.Parameters.Add(new SqlParameter("@administracion", administracion));
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        await sql.CloseAsync();
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

        public async Task<int> actualizaCorreoElectronico(Usuarios usuarios)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaEmail", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", usuarios.Id));
                        cmd.Parameters.Add(new SqlParameter("@email", usuarios.correo_electronico));
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

        public async Task<List<DestinatariosCorreo>> getDestinatariosCorreo(int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getDestinatarioCorreo", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<DestinatariosCorreo>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueDestinatarios(reader));
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

        private DestinatariosCorreo MapToValueDestinatarios(SqlDataReader reader)
        {
            return new DestinatariosCorreo {
                ServicioId = reader["ServicioId"] != DBNull.Value ? (int)reader["ServicioId"] : 0,
                UsuarioId = reader["UsuarioId"]  != DBNull.Value ? (int)reader["UsuarioId"] : 0,
                Usuario = reader["Usuario"] != DBNull.Value ? reader["Usuario"].ToString() : "",
                CorreoElectronico = reader["CorreoElectronico"] != DBNull.Value ? reader["CorreoElectronico"].ToString() : "",
                Servicio = reader["Servicio"] != DBNull.Value ? reader["Servicio"].ToString() : "",                
            };
        }

        private InmueblesUsuarios MapToValue(SqlDataReader reader)
        {
            return new InmueblesUsuarios()
            {
                Clave = (int)reader["Clave"],
                AdministracionId = (int) reader["AdministracionId"],
                Tipo = reader["Tipo"] != DBNull.Value ? (int) reader["Tipo"]:0,
                Inmueble = reader["Nombre"].ToString(),
                Direccion = reader["Direccion"].ToString(),
            };
        }
        private Usuarios MapToValueUsuarios(SqlDataReader reader)
        {
            return new Usuarios()
            {
                Id = (int)reader["ID"],
                AreaId = (int)reader["AreaId"],
                exp = (int)reader["Expediente"],
                cve_usuario = reader["Usuario"].ToString(),
                nom_cat = reader["NomCat"].ToString(),
                nombre_emp = reader["NombreEmp"].ToString(),
                paterno_emp = reader["PaternoEmp"].ToString(),
                materno_emp = reader["MaternoEmp"].ToString(),
                cve_puesto = reader["ClavePuesto"].ToString(),
                nom_pue = reader["Puesto"].ToString(),
                rfc_emp = reader["RFC"].ToString(),
                curp_emp = reader["CURP"].ToString(),
                correo_electronico = reader["CorreoElectronico"].ToString()
            };
        }
    }
}
