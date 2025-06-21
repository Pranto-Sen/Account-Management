using Account_Management.Models;
using Account_Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Account_Management.Pages.ChartOfAccounts
{
    [Authorize(Roles = "Admin,Accountant")]
    public class CreateModel : PageModel
    {
        private readonly DatabaseService _databaseService;

        public CreateModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [BindProperty]
        public Account Account { get; set; }

        public List<Account> Accounts { get; set; }

        public void OnGet()
        {
            Accounts = _databaseService.GetChartOfAccounts();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                _databaseService.ManageChartOfAccounts(Account, "INSERT");
                return RedirectToPage("Index");
            }
            Accounts = _databaseService.GetChartOfAccounts();
            return Page();
        }
    }
}
