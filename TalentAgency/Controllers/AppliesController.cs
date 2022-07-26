using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TalentAgency.Data;
using TalentAgency.Models;

namespace TalentAgency.Controllers
{
    public class AppliesController : Controller
    {
        private readonly TalentAgencyNonIdentityContext _context;
        private readonly TalentAgencyContext _profcontext;

        public AppliesController(TalentAgencyNonIdentityContext context, TalentAgencyContext profcontext)
        {
            _context = context;
            _profcontext = profcontext;
        }

        // GET: Applies
        public IActionResult Index(string msg = "")
        {
            var email = HttpContext.Session.GetString("_email");
            var item = _context.Apply.FromSqlRaw("Select Apply_id,a.email,a.Event_name,Status,Introduction,a.Date_created from Apply a inner join Event e on " +
                "e.Event_name = a.Event_name where e.email = '" + email + "'").ToList();
            ViewBag.msg = msg;
            return View(item);
        }

        // GET: Applies/Details/5
        public async Task<IActionResult> Details(string id)
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
      
        // GET: Applies/Accept/
        public async Task<IActionResult> Accept(string id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(string id, Apply apply)
        {
            if (id != apply.Apply_id)
            {
                return NotFound();
            }

            if (apply.Status == "Accept")
            {
                //viewbag message it
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    apply.Status = "Accepted";
                    _context.Update(apply);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplyExists(apply.Apply_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                string message = "";

                message = "The candidate has been accepted for "+apply.Event_name;

                return RedirectToAction("Index", "Applies", new { msg = message });
            }
            return View(apply);
        }

        // GET: Applies/Reject/
        public async Task<IActionResult> Reject(string id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(string id, Apply apply)
        {
            if (id != apply.Apply_id)
            {
                return NotFound();
            }

            if (apply.Status == "Reject")
            {
                //viewbag message it
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    apply.Status = "Rejected";
                    _context.Update(apply);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplyExists(apply.Apply_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                string message = "";

                message = "The candidate has been accepted for " + apply.Event_name;

                return RedirectToAction("Index", "Applies", new { msg = message });
            }

            return View(apply);
        }

        public async Task<IActionResult> ShowProfile(string prof)
        {
            if (prof == null)
            {
                return NotFound();
            }

            var profile = await _profcontext.Users
                .FirstOrDefaultAsync(m => m.Email == prof);
            if (prof == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        // GET: Applies/Delete/5
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

        // POST: Applies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var apply = await _context.Apply.FindAsync(id);
            _context.Apply.Remove(apply);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplyExists(string id)
        {
            return _context.Apply.Any(e => e.Apply_id == id);
        }
    }
}
