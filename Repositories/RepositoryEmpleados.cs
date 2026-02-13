using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCoreProcedimientos.Data;
using MvcCoreProcedimientos.Models;

namespace MvcCoreProcedimientos.Repositories;

#region PROCEDURES AND VIEWS

// ALTER VIEW V_EMPLEADOS_DEPARTAMENTOS
//     AS
// SELECT CAST(
//     isnull(
//     ROW_NUMBER() over (order by EMP.APELLIDO),0 ) AS int)
// AS ID,
//     EMP.APELLIDO,EMP.OFICIO,EMP.SALARIO,
// DEPT.DNOMBRE AS DEPARTAMENTO,
//     DEPT.LOC AS LOCALIDAD
//     FROM EMP
//     INNER JOIN DEPT 
// ON EMP.DEPT_NO = DEPT.DEPT_NO
// GO

// CREATE VIEW V_TRABAJADORES
//     AS
// SELECT EMP_NO AS IDTRABAJADOR,
//     APELLIDO,OFICIO,SALARIO
// FROM EMP
// UNION
// SELECT DOCTOR_NO, APELLIDO,ESPECIALIDAD, SALARIO FROM DOCTOR
// UNION
// SELECT EMPLEADO_NO, APELLIDO,FUNCION, SALARIO FROM PLANTILLA
// GO    
//
//     CREATE PROCEDURE SP_TRABAJADORES_OFICIO
//     (@oficio nvarchar(50),
// @personas int out,
// @media int out,
// @suma int out)
//     as
// SELECT * FROM V_TRABAJADORES
// WHERE OFICIO=@oficio
// select @personas=COUNT(IDTRABAJADOR),
//     @media=AVG(SALARIO),
//     @suma=SUM(SALARIO)
// FROM V_TRABAJADORES
// WHERE OFICIO=@oficio
// go

#endregion
public class RepositoryEmpleados
{
    
    private HospitalContext _context;
    
    public RepositoryEmpleados(HospitalContext context)
    {
        _context = context;
    }


    public async Task<List<VistaEmpleado>> GetEmpleadosAync()
    {
        var consulta = from datos in _context.VistaEmpleados
            select datos;
        return await consulta.ToListAsync();
    }

    public async Task<TrabajadoresModel> GetTrabajadoresAsync()
    {
        var consulta = from datos in _context.Trabajadores
            select datos;
        TrabajadoresModel model = new TrabajadoresModel();
        model.Trabajadores=await consulta.ToListAsync();
        model.Personas = await consulta.CountAsync();
        model.SumaSalarial = await consulta.SumAsync(z=>z.Salario);
        model.MediaSalarial = (int) await consulta.AverageAsync(z => z.Salario);
        return model;
    }

    public async Task<List<string>> GetOficiosAsync()
    {
        var consulta = (from datos in _context.Trabajadores
            select datos.Oficio).Distinct();
        return await consulta.ToListAsync();
    }


    public async Task<TrabajadoresModel> GetTrabajadoresModelByOficioAsync(string oficio)
    {
        //YA QUE TENEMOS MODEL, VAMOS A LLAMARLO CON EF
        //LA UNICA DIFERENCIA CUANDO TENEMOS PARAMETROS DE SALIDA 
        //ES INDICAR LA PALABRA OUT EN LA DECLARACION DE LAS VARIABLES
        string sql = "SP_TRABAJADORES_OFICIO @oficio,@personas out, " +
                     "@media out, @suma out";
        
        SqlParameter pamOficio= new SqlParameter("@oficio",oficio);
        SqlParameter pamPersonas = new SqlParameter("@personas", -1);
        pamPersonas.Direction=ParameterDirection.Output;
        
        SqlParameter pamMedia = new SqlParameter("@media", -1);
        pamMedia.Direction=ParameterDirection.Output;
        
        SqlParameter pamSuma = new SqlParameter("@suma", -1);
        pamSuma.Direction=ParameterDirection.Output;
        
       var consulta= _context.Trabajadores.FromSqlRaw(sql,pamOficio,pamPersonas,pamMedia,pamSuma);
       
       TrabajadoresModel model = new TrabajadoresModel();
       //EL ORDEN ES IMPORTANTE, PARA QUE LEA LOS OUT
       model.Trabajadores = await consulta.ToListAsync();
       model.Personas= int.Parse(pamPersonas.Value.ToString());
       model.MediaSalarial= int.Parse(pamMedia.Value.ToString());
       model.SumaSalarial= int.Parse(pamSuma.Value.ToString());
   
       return model;
    }
}