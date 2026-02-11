using Microsoft.AspNetCore.Mvc;
using MvcCoreProcedimientos.Models;
using MvcCoreProcedimientos.Repositories;

namespace MvcCoreProcedimientos.Controllers;

public class DoctoresController : Controller
{
    
    private RepositoryDoctores _repo;

    public DoctoresController(RepositoryDoctores repo)
    {
        _repo = repo;
    }
    
    // GET
    public async Task<IActionResult> Index()
    {
        ViewData["ESPECIALIADES"] = await _repo.GetEspecialidadesAsync();
        List<Doctor> doctores = await _repo.GetDoctorsAsync();
        
        return View(doctores);
        
    } 
    
    [HttpPost]
    public async Task<IActionResult> Index(string especialidad,int salario,string accion)
    {
        ViewData["ESPECIALIADES"] = await _repo.GetEspecialidadesAsync();
        
        if (accion.ToLower() == "incremento1")
        {
            await _repo.UpdateDoctorAsync(especialidad,salario);    
        }
        else if(accion.ToLower() == "incremento2")
        {
            await _repo.UpdateDoctorEFAsync(especialidad,salario);
        }
        
        List<Doctor> doctores = await _repo.GetDoctoresByEspecialiadAsync(especialidad);

        
        return View(doctores);
    }
 }
