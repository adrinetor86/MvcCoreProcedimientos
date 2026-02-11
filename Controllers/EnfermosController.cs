using Microsoft.AspNetCore.Mvc;
using MvcCoreProcedimientos.Models;
using MvcCoreProcedimientos.Repositories;

namespace MvcCoreProcedimientos.Controllers;

public class EnfermosController : Controller
{

    private RepositoryEnfermos _repo;

    public EnfermosController(RepositoryEnfermos repo)
    {
        _repo = repo;
    }
    // GET
    
    public async Task<IActionResult> Index()
    {
        List<Enfermo> enfermos = await _repo.GetEnfermosAsync();
        return View(enfermos);
    }

    public async Task<IActionResult> Details(string inscripcion)
    {
        Enfermo enfermo= await _repo.FindEnfermoAsync(inscripcion);
        return View(enfermo);
    }

    public async Task<IActionResult> Delete(string inscripcion)
    {
        await _repo.DeleteEnfermoAsync(inscripcion);

        return RedirectToAction("Index");
    }  
    public async Task<IActionResult> DeleteRaw(string inscripcion)
    {
        await _repo.DeleteEnfermoRawAsync(inscripcion);

        return RedirectToAction("Index");
    }
    
    public async Task<IActionResult> Create(){
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create
        (Enfermo enf){
     
        await _repo.CreateEnfermoAsync(enf.Apellido, enf.Direccion, enf.FechaNacimiento, enf.Genero, enf.Nss);
        return RedirectToAction("Index");
    }
}