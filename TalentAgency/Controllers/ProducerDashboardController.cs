using Microsoft.AspNetCore.Mvc;

namespace TalentAgency.Controllers
{
    public class ProducerDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
