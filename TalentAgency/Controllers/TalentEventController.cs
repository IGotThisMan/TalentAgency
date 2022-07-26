using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TalentAgency.Data;
using TalentAgency.Models;

namespace TalentAgency.Controllers
{
    public class TalentEventController : Controller
    {
        private readonly TalentAgencyNonIdentityContext _context;
        private readonly TalentAgencyContext _idcontext;

        public TalentEventController(TalentAgencyNonIdentityContext context, TalentAgencyContext identitycontext)
        {
            _context = context;
            _idcontext = identitycontext;
        }


        public async Task<IActionResult> Index(string msg = "")
        {
            ViewBag.msg = msg;
            return View(await _context.Event.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply([Bind("Apply_id,Event_name,Status,Introduction,date_created")] Apply @apply)
        {
            string message = "";
            string email = HttpContext.Session.GetString("_email");
            var @event = await _context.Apply.FirstOrDefaultAsync(m => m.email == email && m.Event_name == apply.Event_name);
            if (@event != null)
            {
                message = "You have already applied for " + apply.Event_name + ", please check your application status";
                return RedirectToAction("Index", "TalentEvent", new { msg = message });
            }
            if (ModelState.IsValid)
            {
                @apply.Apply_id = Guid.NewGuid().ToString();
                @apply.email = email;
                @apply.Status = "Pending";
                @apply.Date_created = DateTime.Now;
                _context.Apply.Add(@apply);
                await _context.SaveChangesAsync();

            }
            

            message = "You have successfully applied for " + apply.Event_name + ", please check your application status";
            
            return RedirectToAction("Index", "TalentEvent", new { msg = message });
        }

        public async Task<IActionResult> DetailsAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.Event_name == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }
        
    }
}
