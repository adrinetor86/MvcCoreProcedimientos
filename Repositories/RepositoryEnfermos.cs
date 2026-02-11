using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
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
// CREATE PROCEDURE SP_INSERT_ENFERMO
//     (@apellido as nvarchar(50),@direccion as nvarchar(100),@fecha_nac as datetime,
//         @genero as nvarchar(50),@nss as nvarchar(50))
//     as
//     declare @inscripcion nvarchar(50);
//     
// set @inscripcion=(select Max(INSCRIPCION) from ENFERMO)+1;
//     
// print @inscripcion
//
// INSERT INTO ENFERMO values (@inscripcion,@apellido,@direccion,@fecha_nac,@genero,@nss)
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

    public async Task<Enfermo> FindEnfermoAsync(string inscripcion)
    {
        //PARA LLAMAR A UN PROCEDIMIENTO QUE CONTIENE PARAMETROS 
        //LA LLAMADA SE REALIZA MEDIANTE EL NOMBRE DEL PROCEDURE
        //Y CADA PARAMETRO A CONTINUACION EN LA DECLARACION
        //DEL SQL: SP_PROCEDURE @PAM1, @PAM2

        string sql = "SP_FIND_ENFERMO @inscripcion";

        SqlParameter pamIns = new SqlParameter("@inscripcion",inscripcion);
        //SI LOS DATOS QUE DEVUELVE EL PROCEDURE ESTAN MAPEADOS CON UN MODEL
        //PODEMOS UTILIZAR EL METODO 
        //FromSqlRaw PARA RECUPERAR DIRECTAMENTE EL MODEL/S
        //NO PODEMOS CONSULTAR Y EXTRAER A LA VEZ CON LINQ, SE DEBE
        //REALIZAR SIEMPRE EN DOS PASOS
        
        var consulta= await _context.Enfermos.FromSqlRaw(sql,pamIns).ToListAsync();
        //DEBEMOS UTILIZAR AsEnumerable() PARA EXTRAER LOS DATOS
        Enfermo enfermo= consulta.FirstOrDefault();
        return enfermo;
    }


    public async Task DeleteEnfermoAsync(string inscripcion)
    {
        string sql = "SP_DELETE_ENFERMO";

        SqlParameter pamIns = new SqlParameter("@inscripcion", inscripcion);

        using(DbCommand com=
          _context.Database.GetDbConnection().CreateCommand())
        {
            com.CommandType = CommandType.StoredProcedure;
            com.CommandText = sql;
            com.Parameters.Add(pamIns);
            await com.Connection.OpenAsync();
            await com.ExecuteNonQueryAsync();
            await com.Connection.CloseAsync();

            com.Parameters.Clear();

        }
    }


    public async Task DeleteEnfermoRawAsync(string inscripcion)
    {
        string sql="SP_DELETE_ENFERMO @inscripcion";
        SqlParameter pamIns= 
            new SqlParameter("@inscripcion", inscripcion);
        
        await _context.Database.ExecuteSqlRawAsync(sql, pamIns);
    }


    public async Task CreateEnfermoAsync
        (string apellido,string direccion,
         DateTime fecha_nac,string genero,string nss)
    {
        string sql = "SP_INSERT_ENFERMO @apellido,@direccion,@fecha_nac,@genero,@nss";
        
        
        SqlParameter pamApe=new SqlParameter("@apellido",apellido);
        SqlParameter pamDireccion=new SqlParameter("@direccion",direccion);
        SqlParameter pamFechaNac=new SqlParameter("@fecha_nac",fecha_nac);
        SqlParameter pamGenero=new SqlParameter("@genero",genero);
        SqlParameter pamNss=new SqlParameter("@nss",nss);
        
        await _context.Database.ExecuteSqlRawAsync(sql,
            [pamApe,pamDireccion,pamFechaNac,pamGenero,pamNss]);
              
    }
    
}