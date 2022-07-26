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
    public class EventsController : Controller
    {
        private readonly TalentAgencyNonIdentityContext _context;

        public EventsController(TalentAgencyNonIdentityContext context)
        {
            _context = context;
        }

        // GET: Events
        public IActionResult Index(string msg = "")
        {
            var email = HttpContext.Session.GetString("_email");
            var item = _context.Event.FromSqlRaw("Select * from Event where email = '" + email + "'").ToList();
            ViewBag.msg = msg;
            return View(item);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.Event_id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Event_id,Event_name,email,description,date_created")] Event @event)
        {
            var email = HttpContext.Session.GetString("_email");
            if (ModelState.IsValid)
            {
                @event.Event_id = Guid.NewGuid().ToString();
                @event.email = email;
                @event.date_created = DateTime.Now;
                _context.Add(@event);
                await _context.SaveChangesAsync();
                string message = "";
                message = "Event " + @event.Event_name + " has been created";
                return RedirectToAction("Index", "Events", new { msg = message });
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Event_id,Event_name,email,description,date_created")] Event @event)
        {
            if (id != @event.Event_id)
            {
                return NotFound();
            }
            var email = HttpContext.Session.GetString("_email");
            if (ModelState.IsValid)
            {
                try
                {
                    @event.email = email;
                    @event.date_created = DateTime.Now;
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Event_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                string message = "";
                message = "Event " + @event.Event_name + " has sucessfully edited";
                return RedirectToAction("Index", "Events", new { msg = message });
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.Event_id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var @event = await _context.Event.FindAsync(id);
            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();
            string message = "";
            message = "Event " + @event.Event_name + " has been deleted";
            return RedirectToAction("Index", "Events", new { msg = message });
        }

        private bool EventExists(string id)
        {
            return _context.Event.Any(e => e.Event_id == id);
        }
    }
}
