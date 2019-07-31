using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer.Managers;
using IdentityServer.Quickstart.Role;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityServer.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityServer.Quickstart.User
{
    [SecurityHeaders]
    [AutoValidateAntiforgeryToken]
    [Authorize(Roles = "Administrator")]
    public class UserController : Controller
    {
        private readonly TimekeepingUserManager _userManager;
        private readonly TimekeepingRoleManager _roleManager;
        private readonly ILogger<UserController> _logger;

        public UserController(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetService(typeof(ILogger<UserController>)) as ILogger<UserController>;
            _userManager = serviceProvider.GetService(typeof(TimekeepingUserManager)) as TimekeepingUserManager;
            _roleManager = serviceProvider.GetService(typeof(TimekeepingRoleManager)) as TimekeepingRoleManager;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var userModels = Mapper.Map<List<UserViewModel>>(users);
            userModels = await _userManager.AddRolesToModelAsync(userModels);
            

            return View(userModels);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            var roles = _roleManager.Roles.ToList();
            var roleModels = Mapper.Map<List<RoleViewModel>>(roles);
            ViewBag.roles = roleModels;

            return View();
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(UserInputModel model)
        {
            var user = Mapper.Map<TimekeepingUser>(model);

            var createResult = await _userManager.CreateAsync(user);
            if(!createResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, createResult.Errors.FirstOrDefault().Description);
                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, model.RoleName);
            if(!roleResult.Succeeded)
            {
                var message = $"The user was created, but an error occurred when assigning a role.\r\n" +
                    $"{createResult.Errors.FirstOrDefault().Description}";
                _logger.LogError(message, model);
                ModelState.AddModelError(string.Empty, message);
            }

            return RedirectToAction(nameof(Details), new { id = user.Id });
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            var userModel = Mapper.Map<UserViewModel>(user);
            userModel = await _userManager.AddRolesToModelAsync(userModel);

            return View(userModel);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var userModel = Mapper.Map<UserInputModel>(user);
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            if(!string.IsNullOrEmpty(userRole))
            {
                userModel.RoleName = userRole;
            }

            var roles = _roleManager.Roles.ToList();
            var roleModels = Mapper.Map<List<RoleViewModel>>(roles);
            ViewBag.roles = roleModels;

            return View(userModel);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(UserInputModel model, string id)
        {
            if (id != model.Id)
            { 
                return BadRequest();
            }

            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var updatedData = Mapper.Map<TimekeepingUser>(model);
            var user = await _userManager.FindByIdAsync(id);
            user.Update(updatedData);

            var userResult = await _userManager.UpdateAsync(user);
            if(!userResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, string.Join(',', userResult.Errors));
                return View(model);
            }

            var role = await _roleManager.FindByNameAsync(model.RoleName);

            var sameRole = await _userManager.IsInRoleAsync(user, role.Name);
            if (sameRole)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
            if(!removeResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, removeResult.Errors.First().Description);
                return View(model);
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, role.Name);
            if(!addRoleResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, addRoleResult.Errors.First().Description);
                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var userModel = Mapper.Map<UserInputModel>(user);
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            if (!string.IsNullOrEmpty(userRole))
            {
                userModel.RoleName = userRole;
            }

            return View(userModel);
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(string id, UserViewModel model)
        {
            if(id != model.Id)
            {
                return BadRequest();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(id);

            var result = await _userManager.DeleteAsync(user);

            if(!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, string.Join(',', result.Errors));
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    
    }
}