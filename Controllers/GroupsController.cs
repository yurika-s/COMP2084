﻿using System;
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
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Groups
        public async Task<IActionResult> Index()
        {
            // reffered to the page https://community.auth0.com/t/getting-currently-logged-user-in-web-api/6810/9
            string loginUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            // retrieve groups that the login user created
            var adminstratingGroups = await _context.Groups.Where(g => g.AdministratorID == loginUserId).ToListAsync();
            ViewData["AdminstratingGropus"] = adminstratingGroups;


            var belongingGroups = await _context.GroupMembers.Where(g => g.UserID == loginUserId).ToListAsync();
            var groupIds = new List<int>();
            foreach (var item in belongingGroups)
            {
                groupIds.Add(item.GroupID);
            }
            // filter group that user belongs to but did't create
            var groups = await _context.Groups.Where(g => groupIds.Contains(g.GroupID)).Where(g=> g.AdministratorID != loginUserId).ToListAsync();

            return View(groups);
        }

        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .FirstOrDefaultAsync(m => m.GroupID == id);
            if (@group == null)
            {
                return NotFound();
            }

            var members = await _context.GroupMembers.Include(g => g.Group).Include(g => g.User).Where(g => g.GroupID == id).ToListAsync();
            var adminUser = await _context.Users.FirstOrDefaultAsync(g => g.Id == @group.AdministratorID);
            ViewData["GroupMembers"] = members;
            ViewData["Administrator"] = adminUser;

            return View(@group);
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            string loginUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            ViewData["AdministratorID"] = loginUserId;

            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GroupID,Name,Description,AdministratorID")] Group @group)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@group);
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GroupID,Name,Description,AdministratorID")] Group @group)
        {
            if (id != @group.GroupID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(@group.GroupID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@group);
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .FirstOrDefaultAsync(m => m.GroupID == id);
            if (@group == null)
            {
                return NotFound();
            }
            var adminUser = await _context.Users.FirstOrDefaultAsync(g => g.Id == @group.AdministratorID);
            ViewData["Administrator"] = adminUser;


            return View(@group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @group = await _context.Groups.FindAsync(id);
            if (@group != null)
            {
                _context.Groups.Remove(@group);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.GroupID == id);
        }
    }
}
