using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HouseworkManager.Data;
using HouseworkManager.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HouseworkManager.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            string loginUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var belongingGroups = await _context.GroupMembers.Where(g => g.UserID == loginUserId).ToListAsync();
            var groupIds = new List<int>();
            Dictionary<string, dynamic> groupTasks = new Dictionary<string, dynamic>();
            ViewData["Tasks"] = groupTasks;
            ViewData["IsShow"] = true;

            if (belongingGroups.Count == 0)
            {
                ViewData["IsShow"] = false;
            }
            else
            {
                foreach (var item in belongingGroups)
                {
                    groupIds.Add(item.GroupID);
                }
                var groups = await _context.Groups.Where(g => groupIds.Contains(g.GroupID)).ToListAsync();

                foreach (var item in groups)
                {
                    var tasks = await _context.Tasks.Include(t=>t.User).Where(t => t.GroupID == item.GroupID).OrderBy(t=>t.Deadline).ToListAsync();
                    groupTasks.Add(item.Name, tasks);
                }
                ViewData["Tasks"] = groupTasks;
            }

            return View();
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id, int? from)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Group)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TaskID == id);
            if (task == null)
            {
                return NotFound();
            }

            // let it back to previous page depending on the value
            ViewData["PreviousPage"] = from ?? 0;
            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create(int? from)
        {
            string loginUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var belongingGroups = _context.GroupMembers.Where(g => g.UserID == loginUserId).ToList();
            var groupIds = new List<int>();
            var userIds = new List<string>();
            ViewData["GroupID"] = new SelectList(_context.Groups, "GroupID", "Name");
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["loginUserId"] = loginUserId;
            // let it back to previous page depending on the value
            ViewData["PreviousPage"] = from ?? 0;

            if (belongingGroups.Count != 0)
            {
                foreach (var item in belongingGroups)
                {
                    groupIds.Add(item.GroupID);
                    var groupMembers = _context.GroupMembers.Where(g => g.GroupID == item.GroupID).ToList();
                    foreach(var member in groupMembers)
                    {
                        // in case the member id doesn't exist in the list 
                        if (!userIds.Contains(member.UserID))
                        {
                            userIds.Add(member.UserID);
                        }
                    }
                }
                // filter select options that only groups the login user belongs to and users who belong to the filtered groups 
                var groups = _context.Groups.Where(g => groupIds.Contains(g.GroupID));
                var users =  _context.Users.Where(u => userIds.Contains(u.Id));
                ViewData["GroupID"] = new SelectList(groups, "GroupID", "Name");
                ViewData["UserID"] = new SelectList(users, "Id", "Id");
            }
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskID,Name,Description,Deadline,Done,GroupID,UserID")] Models.Task task, int? from)
        {
            _context.Add((Models.Task)task);
            await _context.SaveChangesAsync();

            if (from == 1)
            {
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Index",
                });
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id, int? from)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            ViewData["GroupID"] = new SelectList(_context.Groups, "GroupID", "Name", task.GroupID);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", task.UserID);
            // let it back to previous page depending on the value
            ViewData["PreviousPage"] = from ?? 0;

            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskID,Name,Description,Deadline,Done,GroupID,UserID")] Models.Task task, int? from)
        {
            if (id != task.TaskID)
            {
                return NotFound();
            }

            try
            {
                _context.Update(task);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(task.TaskID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            if (from == 1) {
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Index",
                });
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id, int? from)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Group)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TaskID == id);
            if (task == null)
            {
                return NotFound();
            }

            // let it back to previous page depending on the value
            ViewData["PreviousPage"] = from ?? 0;
            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int? from)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
            }

            await _context.SaveChangesAsync();
            if (from == 1)
            {
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Index",
                });
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskID == id);
        }
    }
}
