using Microsoft.AspNetCore.Mvc;

namespace TalentAgency.Controllers
{
    public class TalentDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
