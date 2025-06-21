using Account_Management.Models;
using Account_Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Account_Management.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class ManageRolesModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DatabaseService _databaseService;

        public ManageRolesModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, DatabaseService databaseService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _databaseService = databaseService;
        }

        [BindProperty]
        public List<string> SelectedRoles { get; set; }

        [BindProperty]
        public List<RolePermission> Permissions { get; set; }

        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public List<IdentityRole> Roles { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            UserId = userId;
            UserEmail = user.Email;
            Roles = _roleManager.Roles.ToList();
            SelectedRoles = (await _userManager.GetRolesAsync(user)).ToList();
            Permissions = _databaseService.GetRolePermissions();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRolesAsync(user, SelectedRoles);

            foreach (var permission in Permissions)
            {
                _databaseService.UpdateRolePermissions(permission);
            }

            return RedirectToPage("Index");
        }
    }
}
