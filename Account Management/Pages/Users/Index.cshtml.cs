using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Account_Management.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public List<UserViewModel> Users { get; set; }

        public class UserViewModel
        {
            public string Id { get; set; }
            public string Email { get; set; }
            public List<string> Roles { get; set; }
        }

        public async Task OnGetAsync()
        {
            Users = new List<UserViewModel>();
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Users.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }
        }
    }
}
