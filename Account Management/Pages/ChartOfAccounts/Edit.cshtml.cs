using Account_Management.Models;
using Account_Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Account_Management.Pages.ChartOfAccounts
{
    [Authorize(Roles = "Admin,Accountant")]
    public class EditModel : PageModel
    {
        private readonly DatabaseService _databaseService;

        public EditModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [BindProperty]
        public Account Account { get; set; }

        public List<Account> Accounts { get; set; }

        public IActionResult OnGet(int id)
        {
            var accounts = _databaseService.GetChartOfAccounts();
            Account = accounts.FirstOrDefault(a => a.AccountId == id);
            if (Account == null)
            {
                return NotFound();
            }
            Accounts = accounts.Where(a => a.AccountId != id).ToList();
            return Page();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                _databaseService.ManageChartOfAccounts(Account, "UPDATE");
                return RedirectToPage("Index");
            }
            Accounts = _databaseService.GetChartOfAccounts();
            return Page();
        }
    }
}
