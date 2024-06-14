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
    public class RepositorioDocuments : IRepositorioDocuments
    {
        private readonly string _connectionString;

        public RepositorioDocuments(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection"); ;
        }

        public async Task<ActaEntregaRecepcion> getDatosActa(int cedula, int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getDatosActaER", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new ActaEntregaRecepcion();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if (servicio == 3)
                                {
                                    response = MapToValueMensajeria(reader);
                                }
                                else
                                {
                                    response = MapToValue(reader);
                                }
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

        private ActaEntregaRecepcion MapToValue(SqlDataReader reader)
        {
            return new ActaEntregaRecepcion
            {
                Id = (int)reader["Id"],
                Mes = reader["Mes"].ToString(),
                Folio = reader["Folio"].ToString(),
                Anio = (int)reader["Anio"],
                EncabezadoInmueble = reader["EncabezadoInmueble"] != DBNull.Value ? reader["EncabezadoInmueble"].ToString():"",
                TipoServicio = reader["TipoServicio"] != DBNull.Value ? reader["TipoServicio"].ToString():"",
                InmuebleEvaluado = reader["InmuebleEvaluado"] != DBNull.Value ? reader["InmuebleEvaluado"].ToString():"",
                Direccion = reader["Direccion"] != DBNull.Value ? reader["Direccion"].ToString():"",
                Estado = reader["Estado"] != DBNull.Value ? reader["Estado"].ToString():"",
                Administrador = reader["Administrador"] != DBNull.Value ? reader["Administrador"].ToString():"",
                PuestoAutoriza = reader["PuestoAutoriza"] != DBNull.Value ? reader["PuestoAutoriza"].ToString():"",
                Elaboro = reader["Elaboro"] != DBNull.Value ? reader["Elaboro"].ToString():"",
                Reviso = reader["Reviso"] != DBNull.Value ? reader["Reviso"].ToString():"",
                Folios = reader["Folios"] != DBNull.Value ? reader["Folios"].ToString():"",
                FechasTimbrado = reader["FechasTimbrado"] != DBNull.Value ? reader["FechasTimbrado"].ToString():"",
                Cantidad = reader["Cantidad"] != DBNull.Value ? reader["Cantidad"].ToString():"",
                Total = reader["Total"] != DBNull.Value ? reader["Total"].ToString():"",
                FoliosNC = reader["FoliosNC"] != DBNull.Value ? reader["FoliosNC"].ToString():"",
                FechasTimbradoNC = reader["FechasTimbradoNC"] != DBNull.Value ? reader["FechasTimbradoNC"].ToString():"",
                CantidadNC = reader["CantidadNC"] != DBNull.Value ? reader["CantidadNC"].ToString():"",
                TotalNC = reader["TotalNC"] != DBNull.Value ? reader["TotalNC"].ToString():"",
                TotalServicios = reader["TotalServicios"] != DBNull.Value ? reader["TotalServicios"].ToString():"",
                FechaInicial = reader["FechaInicial"] != DBNull.Value ? Convert.ToDateTime(reader["FechaInicial"]) : Convert.ToDateTime("1990-01-01T00:00:00"),
                FechaFinal = reader["FechaFinal"] != DBNull.Value ? Convert.ToDateTime(reader["FechaFinal"]) : Convert.ToDateTime("1990-01-01T00:00:00"),
                ConceptosComedor = reader["ConceptosComedor"] != DBNull.Value ? reader["ConceptosComedor"].ToString() : "",
            };
        }

        private ActaEntregaRecepcion MapToValueMensajeria(SqlDataReader reader)
        {
            return new ActaEntregaRecepcion
            {
                Id = (int)reader["Id"],
                Mes = reader["Mes"].ToString(),
                Folio = reader["Folio"].ToString(),
                Anio = (int)reader["Anio"],
                EncabezadoInmueble = reader["EncabezadoInmueble"].ToString(),
                InmuebleEvaluado = reader["InmuebleEvaluado"].ToString(),
                Direccion = reader["Direccion"].ToString(),
                Estado = reader["Estado"].ToString(),
                Administrador = reader["Administrador"].ToString(),
                PuestoAutoriza = reader["PuestoAutoriza"].ToString(),
                Elaboro = reader["Elaboro"].ToString(),
                Reviso = reader["Reviso"].ToString(),
                Folios = reader["Folios"].ToString(),
                FechasTimbrado = reader["FechasTimbrado"].ToString(),
                Cantidad = reader["Cantidad"].ToString(),
                Total = reader["Total"].ToString(),
                FoliosNC = reader["FoliosNC"].ToString(),
                FechasTimbradoNC = reader["FechasTimbradoNC"].ToString(),
                CantidadNC = reader["CantidadNC"].ToString(),
                TotalNC = reader["TotalNC"].ToString(),
                TipoInmueble = reader["TipoInmueble"].ToString(),
            };
        }
    }
}
