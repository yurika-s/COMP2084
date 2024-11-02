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
using Microsoft.CodeAnalysis;

namespace HouseworkManager.Controllers
{
    [Authorize]
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
            var applicationDbContext = _context.GroupMembers.Include(g => g.Group).Include(g => g.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: GroupMembers/Create
        public IActionResult Create(int? groupId)
        {
            ViewData["GroupID"] = new SelectList(_context.Groups, "GroupID", "Name");
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["DetailsGroupID"] = groupId;

            return View();
        }

        // POST: GroupMembers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GroupMemberID,GroupID,UserID")] GroupMember groupMember)
        {
            _context.Add(groupMember);
            await _context.SaveChangesAsync();
            int belongingGroupId = groupMember.GroupID;

            ViewData["GroupID"] = new SelectList(_context.Groups, "GroupID", "Name", belongingGroupId);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", groupMember.UserID);

            return RedirectToRoute(new
            {
                controller = "Groups",
                action = "Details",
                id = belongingGroupId
            });
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
            return RedirectToRoute(new
            {
                controller = "Groups",
                action = "Details",
                id = groupMember.GroupID
            });
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
                .Include(g => g.User)
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
            if (groupMember == null)
            {
                return NotFound();
            }
            _context.GroupMembers.Remove(groupMember);
            await _context.SaveChangesAsync();
            return RedirectToRoute(new
            {
                controller = "Groups",
                action = "Details",
                id = groupMember.GroupID
            });
        }

        private bool GroupMemberExists(int id)
        {
            return _context.GroupMembers.Any(e => e.GroupMemberID == id);
        }
    }
}
