using Account_Management.Models;
using Account_Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Account_Management.Pages.ChartOfAccounts
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly DatabaseService _databaseService;

        public DeleteModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [BindProperty]
        public Account Account { get; set; }

        public IActionResult OnGet(int id)
        {
            var accounts = _databaseService.GetChartOfAccounts();
            Account = accounts.FirstOrDefault(a => a.AccountId == id);
            if (Account == null)
            {
                return NotFound();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            _databaseService.ManageChartOfAccounts(Account, "DELETE");
            return RedirectToPage("Index");
        }
    }
}
