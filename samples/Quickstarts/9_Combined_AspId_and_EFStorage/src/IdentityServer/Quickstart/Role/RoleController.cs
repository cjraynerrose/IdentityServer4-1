using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Managers;
using IdentityServer.Quickstart.Role;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Quickstart.User
{
    [SecurityHeaders]
    [Authorize]
    [Route("Manage/Role")]
    public class RoleController : Controller
    {
        private readonly TimekeepingUserManager _userManager;
        private readonly TimekeepingRoleManager _roleManager;

        public RoleController(IServiceProvider serviceProvider)
        {
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            //Get roles
            return View();
        }

        [HttpPost("Create")]
        public IActionResult Create(RoleInputModel model)
        {
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            return View(role);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            return View(role);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(string id, RoleInputModel model)
        {

            return View(nameof(Details), new { id });
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            return View(role);
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(string id, RoleViewModel model)
        {
            return View(nameof(Index));
        }



    }
}