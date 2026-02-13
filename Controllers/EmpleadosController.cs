using Microsoft.AspNetCore.Mvc;
using MvcCoreProcedimientos.Models;
using MvcCoreProcedimientos.Repositories;

namespace MvcCoreProcedimientos.Controllers;

public class EmpleadosController : Controller
{
    
    private RepositoryEmpleados _repo;

    public EmpleadosController(RepositoryEmpleados repo)
    {
        _repo = repo;
    }
    
    public async Task<IActionResult> Index()
    {
        List<VistaEmpleado> empleados = await _repo.GetEmpleadosAync();
        return View(empleados);
    }
}