using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCoreProcedimientos.Data;
using MvcCoreProcedimientos.Models;

namespace MvcCoreProcedimientos.Repositories;

#region PROCEDURES

// CREATE PROCEDURE SP_GET_ESPECIALIDADES
//     AS
// SELECT DISTINCT ESPECIALIDAD FROM DOCTOR 
//     GO
//
// ALTER PROCEDURE SP_UPDATE_DOCTOR
//     (@especialidad nvarchar(50),@salario int)
// AS
//     UPDATE DOCTOR SET SALARIO+=@salario
// WHERE ESPECIALIDAD=@especialidad
// GO
//
//     CREATE PROCEDURE SP_DOCTORES_ESPECIALIDAD(@especialidad nvarchar(50))
// AS
//     SELECT * FROM DOCTOR WHERE ESPECIALIDAD=@especialidad
// GO


#endregion

public class RepositoryDoctores
{
    private EnfermosContext _context;

    public RepositoryDoctores(EnfermosContext context)
    {
        _context = context;
    }


    public async Task<List<string>> GetEspecialidadesAsync()
    {

        using (DbCommand com = _context.Database.GetDbConnection().CreateCommand())
        {
            string sql = "SP_GET_ESPECIALIDADES";

            com.CommandType = CommandType.StoredProcedure;
            com.CommandText=sql;
            await com.Connection.OpenAsync();
            List<string> especialidades = new List<string>();
            DbDataReader reader = await com.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                string espe = reader["ESPECIALIDAD"].ToString();
                especialidades.Add(espe);
                
            }
            await com.Connection.CloseAsync();
            await reader.CloseAsync();
            return especialidades;
        }
    }

    public async Task<List<Doctor>> GetDoctorsAsync()
    {

        using (DbCommand com = _context.Database.GetDbConnection().CreateCommand())
        {
            var consulta=from datos in _context.Doctores
                         select datos;

            if (consulta.Count() == 0)
            {
                return null;
            }
            else
            {
                List<Doctor> doctores = new List<Doctor>();
                foreach (var row in consulta)
                {
                    Doctor doctor = new Doctor
                    {
                        HospitalCod = row.HospitalCod,
                        DoctorNo = row.DoctorNo,
                        Apellido = row.Apellido,
                        Especialidad = row.Especialidad,
                        Salario = row.Salario,
                    };
                    doctores.Add(doctor);
                }
                return doctores;
            }
        }
    }


    public async Task<List<Doctor>> GetDoctoresByEspecialiadAsync(string especialidad)
    {
             string sql = "SP_DOCTORES_ESPECIALIDAD @especialidad";
             SqlParameter pamEspe=new  SqlParameter("@especialidad",especialidad);

             var consulta= await _context.Doctores.FromSqlRaw(sql,pamEspe).ToListAsync();
             
             return consulta;
    }

    public async Task UpdateDoctorAsync(string especialidad,int salario)
    {
        
        string sql="SP_UPDATE_DOCTOR @especialidad,@salario";

        SqlParameter pamEspecialidad=new SqlParameter("@especialidad",especialidad);
        SqlParameter pamSalario=new SqlParameter("@salario",salario);
        
         await _context.Database.ExecuteSqlRawAsync(sql, pamEspecialidad,pamSalario);
        
        
    } 
    
    public async Task UpdateDoctorEFAsync(string especialidad,int salario)
    {
        //MILAGRO QUE FUNCIONE, PARA ESTOS CASOS MEJOR HACER
        //CONSULTA AL CONTEXT EN VEZ DE USAR EL PROCEDURE
         List<Doctor> doctores = await GetDoctoresByEspecialiadAsync(especialidad);
         
         foreach (var doctor in doctores)
         {
             doctor.Salario += salario;    
         }
         
         await _context.SaveChangesAsync();  
        
    }
}