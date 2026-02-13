using Microsoft.AspNetCore.Mvc;
using MvcCoreProcedimientos.Models;
using MvcCoreProcedimientos.Repositories;

namespace MvcCoreProcedimientos.Controllers;

public class TrabajadoresController : Controller
{
    private RepositoryEmpleados _repo;

    public TrabajadoresController(RepositoryEmpleados repo)
    {
        _repo = repo;
    }
        
    public async Task<IActionResult> Index()
    {
        TrabajadoresModel model=await _repo.GetTrabajadoresAsync();
        ViewData["OFICIOS"] =  await _repo.GetOficiosAsync();
        return View(model);
    }  
    [HttpPost]
    public async Task<IActionResult> Index(string oficio)
    {
        TrabajadoresModel model=await _repo.GetTrabajadoresModelByOficioAsync(oficio);
        ViewData["OFICIOS"] =  await _repo.GetOficiosAsync();
        return View(model);
    }
}