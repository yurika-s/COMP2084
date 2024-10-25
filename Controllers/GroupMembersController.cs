using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HouseworkManager.Data;
using HouseworkManager.Models;

namespace HouseworkManager.Controllers
{
    public class GroupMembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GroupMembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GroupMembers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.GroupMembers.Include(g => g.Group).Include(g => g.user);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: GroupMembers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupMember = await _context.GroupMembers
                .Include(g => g.Group)
                .Include(g => g.user)
                .FirstOrDefaultAsync(m => m.GroupMemberID == id);
            if (groupMember == null)
            {
                return NotFound();
            }

            return View(groupMember);
        }

        // GET: GroupMembers/Create
        public IActionResult Create()
        {
            ViewData["GroupID"] = new SelectList(_context.Groups, "GroupID", "Name");
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: GroupMembers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GroupMemberID,GroupID,UserID")] GroupMember groupMember)
        {
            //if (ModelState.IsValid)
            //{
                _context.Add(groupMember);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            //}
            ViewData["GroupID"] = new SelectList(_context.Groups, "GroupID", "Name", groupMember.GroupID);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", groupMember.UserID);
            return View(groupMember);
        }

        // GET: GroupMembers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupMember = await _context.GroupMembers.FindAsync(id);
            if (groupMember == null)
            {
                return NotFound();
            }
            ViewData["GroupID"] = new SelectList(_context.Groups, "GroupID", "Name", groupMember.GroupID);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", groupMember.UserID);
            return View(groupMember);
        }

        // POST: GroupMembers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GroupMemberID,GroupID,UserID")] GroupMember groupMember)
        {
            if (id != groupMember.GroupMemberID)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {
                    _context.Update(groupMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupMemberExists(groupMember.GroupMemberID))
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
            ViewData["GroupID"] = new SelectList(_context.Groups, "GroupID", "Name", groupMember.GroupID);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", groupMember.UserID);
            return View(groupMember);
        }

        // GET: GroupMembers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupMember = await _context.GroupMembers
                .Include(g => g.Group)
                .Include(g => g.user)
                .FirstOrDefaultAsync(m => m.GroupMemberID == id);
            if (groupMember == null)
            {
                return NotFound();
            }

            return View(groupMember);
        }

        // POST: GroupMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var groupMember = await _context.GroupMembers.FindAsync(id);
            if (groupMember != null)
            {
                _context.GroupMembers.Remove(groupMember);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupMemberExists(int id)
        {
            return _context.GroupMembers.Any(e => e.GroupMemberID == id);
        }
    }
}
