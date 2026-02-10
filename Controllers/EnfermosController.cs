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
}