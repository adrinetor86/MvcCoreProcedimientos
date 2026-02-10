using Microsoft.EntityFrameworkCore;
using MvcCoreProcedimientos.Models;

namespace MvcCoreProcedimientos.Data;

public class EnfermosContext :DbContext
{
    
    public EnfermosContext(DbContextOptions<EnfermosContext>options):base(options){}
    
    public DbSet<Enfermo> Enfermos { get; set; }
    
}