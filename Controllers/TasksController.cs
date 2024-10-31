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
            foreach (var item in belongingGroups)
            {
                groupIds.Add(item.GroupID);
            }
            var groups = await _context.Groups.Where(g => groupIds.Contains(g.GroupID)).ToArrayAsync();


            Dictionary<string, dynamic> groupTasks = new Dictionary<string, dynamic>();

            foreach (var item in groups)
            {
                var tasks = _context.Tasks.Where(t => t.GroupID == item.GroupID);
                groupTasks.Add(item.Name, tasks);
            }
            ViewData["Tasks"] = groupTasks;

            return View();
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Group)
                .Include(t => t.user)
                .FirstOrDefaultAsync(m => m.TaskID == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            ViewData["GroupID"] = new SelectList(_context.Groups, "GroupID", "Name");
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskID,Name,Description,Deadline,Done,GroupID,UserID")] Models.Task task)
        {
            //if (ModelState.IsValid)
            //{
                _context.Add((Models.Task)task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            //}
            ViewData["GroupID"] = new SelectList(_context.Groups, "GroupID", "Name", task.GroupID);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", task.UserID);
            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskID,Name,Description,Deadline,Done,GroupID,UserID")] Models.Task task)
        {
            if (id != task.TaskID)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
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
                return RedirectToAction(nameof(Index));
            //}
            ViewData["GroupID"] = new SelectList(_context.Groups, "GroupID", "Name", task.GroupID);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", task.UserID);
            return View(task);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Group)
                .Include(t => t.user)
                .FirstOrDefaultAsync(m => m.TaskID == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskID == id);
        }
    }
}
