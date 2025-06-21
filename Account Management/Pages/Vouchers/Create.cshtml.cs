using Account_Management.Models;
using Account_Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Account_Management.Pages.Vouchers
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
        public Voucher Voucher { get; set; }

        public List<Account> Accounts { get; set; }

        public void OnGet()
        {
            Voucher = new Voucher { VoucherDate = DateTime.Today, Entries = new List<VoucherEntry> { new VoucherEntry() } };
            Accounts = _databaseService.GetChartOfAccounts();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid && Voucher.Entries.Any())
            {
                _databaseService.SaveVoucher(Voucher);
                return RedirectToPage("Index");
            }
            Accounts = _databaseService.GetChartOfAccounts();
            return Page();
        }
    }

}
