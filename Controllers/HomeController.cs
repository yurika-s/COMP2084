using System.Diagnostics;
using System.Security.Claims;
using HouseworkManager.Data;
using HouseworkManager.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;

namespace HouseworkManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["IsBelongToGroup"] = false;
            string loginUserId = User.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (loginUserId == null) {
                return View();
            }
            var belongingGroups = await _context.GroupMembers.Where(g => g.UserID == loginUserId).ToListAsync();
            // check if the login user belongs to at least one group
            if (belongingGroups.Count > 0)
            {
                ViewData["IsBelongToGroup"] = true;
            }
            
            // reffered to this page https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/method-based-query-syntax-examples-filtering
            var task = await _context.Tasks.Include(t => t.Group).Include(t => t.User).Where(t => t.User.Id == loginUserId && t.Done == false).OrderBy(t => t.Deadline).ToListAsync();
            return View(task);
        }

        public IActionResult Tutorial()
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
