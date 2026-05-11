using Microsoft.AspNetCore.Mvc;

namespace IndPubBack.Controllers;

public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}