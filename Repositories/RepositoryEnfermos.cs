using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MvcCoreProcedimientos.Data;
using MvcCoreProcedimientos.Models;

namespace MvcCoreProcedimientos.Repositories;


#region PROCEDURES

// CREATE PROCEDURE SP_ALL_ENFERMOS
//     AS  
// SELECT * FROM ENFERMO
//     GO
//
// CREATE PROCEDURE SP_FIND_ENFERMO
//         (@inscripcion nvarchar(50))
//     as
//     select * from ENFERMO
// where INSCRIPCION=@inscripcion
// go
//
//     create procedure SP_DELETE_ENFERMO
//     (@inscripcion nvarchar(50))
//     as
// delete from ENFERMO WHERE INSCRIPCION=@inscripcion
// go

#endregion
public class RepositoryEnfermos
{

    private EnfermosContext _context;
    public RepositoryEnfermos(EnfermosContext context)
    {
        _context = context;
    }

    public async Task<List<Enfermo>> GetEnfermosAsync()
    {
        //NECESITAMOS UN COMAND, VAMOS A UTILIZAR UN using
        //PARA_TODO
        //EL COMAND, EN SU CREACION, NECESITA DE UNA CADENA DE 
        //CONEXION(OBJETO)
        //EL OBJETO CONNECTION NOS LO OFRECE EF
        //las conexiones se crean a partir del context
        using (DbCommand com=
               _context.Database.GetDbConnection().CreateCommand())
        {
            string sql = "SP_ALL_ENFERMOS";
            com.CommandType = CommandType.StoredProcedure;
            com.CommandText = sql;
            //ABRIMOS LA CONEXION A PARTIR DEL COMAND
            await com.Connection.OpenAsync();
            //EJECUTAMOS NUESTRO READER
            DbDataReader reader = await com.ExecuteReaderAsync();
            List<Enfermo> enfermos = new List<Enfermo>();
            while (await reader.ReadAsync())
            {
                Enfermo enfermo = new Enfermo
                {
                    Inscripcion = reader["INSCRIPCION"].ToString(),
                    Apellido = reader["APELLIDO"].ToString(),
                    Direccion = reader["DIRECCION"].ToString(),
                    FechaNacimiento = DateTime.Parse(reader["FECHA_NAC"].ToString()),
                    Genero = reader["S"].ToString(),
                    Nss = reader["NSS"].ToString(),
                };
                enfermos.Add(enfermo);
                
            }

            await reader.CloseAsync();
            await com.Connection.CloseAsync();
            return enfermos;
        }
        
    }
}