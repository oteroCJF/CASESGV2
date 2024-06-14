using CedulasEvaluacion.Entities.Models;
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
    public class RepositorioAreas : IRepositorioAreas
    {
        private readonly string _connectionString;

        public RepositorioAreas(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection"); ;
        }

        private SqlConnection conexion()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<Areas> getAreasById(int area)
        {
            Areas areas = null;
            SqlConnection sqlConexion = conexion();
            SqlCommand Comm = null;
            SqlDataReader reader = null;
            try
            {
                sqlConexion.Open();
                Comm = sqlConexion.CreateCommand();
                Comm.CommandText = "dbo.sp_getAreaById";
                Comm.CommandType = CommandType.StoredProcedure;
                Comm.Parameters.Add("@id", SqlDbType.Int).Value = area;

                reader = await Comm.ExecuteReaderAsync();

                while (reader.Read())
                {
                    areas = new Areas();
                    areas.Id = Convert.ToInt32(reader["Id"].ToString());
                    areas.cveArea = Convert.ToInt32(reader["ClaveArea"].ToString());
                    areas.cve_adscripcion = reader["ClaveAdscripcion"].ToString();
                    areas.ClaveInmueble = Convert.ToInt32(reader["ClaveInmueble"].ToString());
                    areas.nom_area = reader["Nombre"].ToString();
                    areas.nom_edo = reader["Estado"].ToString();
                }
            }
            catch (Exception ex)
            {
                areas = new Areas();
                areas.nom_area = ex.Message;
            }
            return areas;
        }

        public async Task<int> insertaArea(Areas area)
        {
            SqlConnection sqlConexion = conexion();
            SqlCommand Comm = null;
            try
            {
                sqlConexion.Open();
                Comm = sqlConexion.CreateCommand();
                Comm.CommandText = "dbo.sp_insertaArea";
                Comm.CommandType = CommandType.StoredProcedure;
                Comm.Parameters.Add("@claveArea", SqlDbType.Int).Value = area.cveArea;
                Comm.Parameters.Add("@claveAdsc", SqlDbType.VarChar, 50).Value = area.cve_adscripcion;
                Comm.Parameters.Add("@nombre", SqlDbType.VarChar, 256).Value = area.nom_area;
                Comm.Parameters.Add("@estado", SqlDbType.VarChar, 60).Value = area.nom_edo;
                Comm.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;

                int i = await Comm.ExecuteNonQueryAsync();
                if (i > 0)
                {
                    return i;   
                }
                else if (Convert.ToInt32(Comm.Parameters["@id"].Value) != 0)
                {
                    return Convert.ToInt32(Comm.Parameters["@id"].Value); ;
                }


            }
            catch(Exception ex)
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
    }
}
