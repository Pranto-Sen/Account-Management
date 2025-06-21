using Account_Management.Models;
using Account_Management.Services;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Account_Management.Pages.Vouchers
{
    [Authorize(Roles = "Admin,Accountant,Viewer")]
    public class IndexModel : PageModel
    {
        private readonly DatabaseService _databaseService;

        public IndexModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public List<Voucher> Vouchers { get; set; }

        public void OnGet()
        {
            Vouchers = _databaseService.GetVouchers();
        }

        public IActionResult OnPostExport()
        {
            var vouchers = _databaseService.GetVouchers();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Vouchers");
                worksheet.Cell(1, 1).Value = "Voucher ID";
                worksheet.Cell(1, 2).Value = "Voucher Type";
                worksheet.Cell(1, 3).Value = "Voucher Date";
                worksheet.Cell(1, 4).Value = "Reference No";

                int row = 2;
                foreach (var voucher in vouchers)
                {
                    worksheet.Cell(row, 1).Value = voucher.VoucherId;
                    worksheet.Cell(row, 2).Value = voucher.VoucherType;
                    worksheet.Cell(row, 3).Value = voucher.VoucherDate.ToShortDateString();
                    worksheet.Cell(row, 4).Value = voucher.ReferenceNo;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Vouchers.xlsx");
                }
            }
        }
    }
}
