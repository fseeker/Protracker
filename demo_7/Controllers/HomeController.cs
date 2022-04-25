using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using demo_7.Models;
using Microsoft.EntityFrameworkCore;

namespace demo_7.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ProTrackerDbContext _context;

        public HomeController(ILogger<HomeController> logger, ProTrackerDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {                       

            List<Tasks> todo = _context.Tasks.Include(x => x.CreatedBy)
                                             .Include(x => x.InvolvedEmployees).ThenInclude(x => x.Employee)
                                             .Where(x => x.Status.Contains("In Progress")).ToList();
            ViewBag.todo = todo;

            List<Tasks> urgent = _context.Tasks.Include(x => x.CreatedBy)
                                               .Include(x => x.InvolvedEmployees).ThenInclude(x => x.Employee)
                                               .Where(x => x.Status.Contains("Urgent")).ToList();
            ViewBag.urgent = urgent;

            List<Tasks> complete = _context.Tasks.Include(x => x.CreatedBy)
                                                 .Include(x => x.InvolvedEmployees).ThenInclude(x => x.Employee)
                                                 .Where(x => x.Status.Contains("Complete")).ToList();
            ViewBag.complete = complete;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
