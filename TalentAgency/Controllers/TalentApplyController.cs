using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TalentAgency.Data;

namespace TalentAgency.Controllers
{
    public class TalentApplyController : Controller
    {
        private readonly TalentAgencyNonIdentityContext _context;

        public TalentApplyController(TalentAgencyNonIdentityContext context)
        {
            _context = context;
        }

        public IActionResult Index(string msg = "")
        {
            string email = HttpContext.Session.GetString("_email");
            var item = _context.Apply.FromSqlRaw("Select Apply_id,a.email,a.Event_name,Status,Introduction,a.Date_created from Apply a inner join Event e on e.Event_name = a.Event_name where a.email = '" + email + "'").ToList();
            ViewBag.msg = msg;
            return View(item);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apply = await _context.Apply
                .FirstOrDefaultAsync(m => m.Apply_id == id);
            if (apply == null)
            {
                return NotFound();
            }

            return View(apply);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            string message = "";
            var applycheck = await _context.Apply
                .FirstOrDefaultAsync(m => m.Apply_id == id && m.Status == "Accepted" || m.Status == "Rejected");
            if (applycheck != null)
            {
                message = "This applicant is already being accept or reject, you can only cancel an applicant that is still pending";
                return RedirectToAction("Index", "TalentApply", new { msg = message });
            }
            var apply = await _context.Apply.FindAsync(id);
            _context.Apply.Remove(apply);
            await _context.SaveChangesAsync(); 
            message = "You have cancel the applicant of events " + apply.Event_name;
            return RedirectToAction("Index", "TalentApply", new { msg = message });
        }
    }
}
