using Microsoft.AspNetCore.Mvc;

namespace TalentAgency.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
