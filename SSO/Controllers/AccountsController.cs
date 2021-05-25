using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSO.Models;
using SSO.Utils;

namespace SSO.Controllers
{
    public class AccountsController : Controller
    {
        private readonly AppDBContext _context;

        public AccountsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Accounts
        public IActionResult Index()
        {
            // get session
            var sessionID = HttpContext.Session.GetString("session_id");

            if (sessionID != null)
            {
                // check session ID stills available
                var session = _context.Sessions.SingleOrDefault(p => p.SessionID == sessionID);
                if (session != null)
                {
                    return View(session);
                }

                return RedirectToAction("SessionTimeout");
            }

            // redirect to login page
            return View("Login");
        }


        public IActionResult Login()
        {
            return View();
        }

        [Route("login")]
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                var account = _context.Accounts.Where(p => p.Username == username && p.Password == password).SingleOrDefault();

                if (account != null)
                {
                    // delete all previous sessions
                    var preSessions = _context.Sessions.Where(p => p.AccountID == account.ID).ToList();
                    _context.Sessions.RemoveRange(preSessions);

                    var rand = new RandomHelper();
                    // generate new session id and store to db
                    var sessionID = rand.RandomString(6, true);
                    _context.Sessions.Add(new Session { SessionID = sessionID, AccountID = account.ID });
                    _context.SaveChanges();

                    // create session
                    HttpContext.Session.SetString("session_id", sessionID);

                    return RedirectToAction("Index");
                }

                return View("Login");
            }
            else
            {
                ViewBag.error = "Invalid Account";
                return View("Login");
            }
        }

        public IActionResult SessionTimeout()
        {
            HttpContext.Session.Remove("session_id");
            return View();
        }
    }
}
