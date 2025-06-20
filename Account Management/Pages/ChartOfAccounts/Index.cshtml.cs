using Account_Management.Models;
using Account_Management.Services;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Account_Management.Pages.ChartOfAccounts
{
    [Authorize(Roles = "Admin,Accountant")]
    public class IndexModel : PageModel
    {
        private readonly DatabaseService _databaseService;

        public IndexModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public List<AccountNode> AccountTree { get; set; }

        public class AccountNode
        {
            public Account Account { get; set; }
            public List<AccountNode> Children { get; set; } = new List<AccountNode>();
        }

        public void OnGet()
        {
            var accounts = _databaseService.GetChartOfAccounts();
            AccountTree = BuildAccountTree(accounts, null);
        }

        private List<AccountNode> BuildAccountTree(List<Account> accounts, int? parentId)
        {
            return accounts
                .Where(a => a.ParentAccountId == parentId)
                .Select(a => new AccountNode
                {
                    Account = a,
                    Children = BuildAccountTree(accounts, a.AccountId)
                })
                .ToList();
        }

        public IActionResult OnPostExport()
        {
            var accounts = _databaseService.GetChartOfAccounts();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("ChartOfAccounts");
                worksheet.Cell(1, 1).Value = "Account ID";
                worksheet.Cell(1, 2).Value = "Account Name";
                worksheet.Cell(1, 3).Value = "Account Type";
                worksheet.Cell(1, 4).Value = "Parent Account ID";
                worksheet.Cell(1, 5).Value = "Is Active";

                int row = 2;
                foreach (var account in accounts)
                {
                    worksheet.Cell(row, 1).Value = account.AccountId;
                    worksheet.Cell(row, 2).Value = account.AccountName;
                    worksheet.Cell(row, 3).Value = account.AccountType;
                    worksheet.Cell(row, 4).Value = account.ParentAccountId?.ToString() ?? "";
                    worksheet.Cell(row, 5).Value = account.IsActive;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ChartOfAccounts.xlsx");
                }
            }
        }
    }
}
