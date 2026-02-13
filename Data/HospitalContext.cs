using Microsoft.EntityFrameworkCore;
using MvcCoreProcedimientos.Models;

namespace MvcCoreProcedimientos.Data;

public class HospitalContext :DbContext
{
    
    public HospitalContext(DbContextOptions<HospitalContext>options):base(options){}
    
    public DbSet<VistaEmpleado>  VistaEmpleados { get; set; }
    
    public DbSet<Trabajador>  Trabajadores { get; set; }
    
    public DbSet<Enfermo> Enfermos { get; set; }
    
    public DbSet<Doctor> Doctores { get; set; }
 
}