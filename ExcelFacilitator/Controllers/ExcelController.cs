using ExcelFacilitator.Models.Domain;
using ExcelFacilitator.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.Diagnostics;
using System.Globalization;
using EFCore.BulkExtensions;
using ExcelFacilitator.Models.Data;

namespace ExcelFacilitator.Controllers
{
    public class ExcelController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ExcelController> _logger;

        public ExcelController(ApplicationDbContext context, ILogger<ExcelController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> GetAnalytics(string type, bool useGlobal, ExcelFilterViewModel filters)
        {
            var query = _context.ExcelRecords.AsQueryable();

            if (!useGlobal)
            {
                query = ApplyFilters(query, filters);
            }

            var limited = await query.Take(10000).ToListAsync();
            _logger.LogInformation("Query count before grouping: {Count}, Type: {Type}", limited.Count, type);

            var result = GenerateAnalytics(limited, type);

            return PartialView("_AnalyticsTable", result);
        }

        [HttpGet]
        public IActionResult Upload() => View();

        [HttpPost]
        public async Task<IActionResult> Upload(ExcelUploadViewModel model)
        {
            if (ModelState.IsValid && model.ExcelFile != null)
            {
                var extension = Path.GetExtension(model.ExcelFile.FileName)?.ToLower();
                if (extension != ".xlsx")
                {
                    ViewBag.Message = "Only .xlsx files are supported.";
                    return View(model);
                }

                try
                {
                    ExcelPackage.License.SetNonCommercialPersonal("Zafar");

                    var fileName = Path.GetFileNameWithoutExtension(model.ExcelFile.FileName);
                    var (records, parseTime, insertTime) = await ParseExcelFileAsync(model.ExcelFile, fileName);

                    ViewBag.Message = $"Uploaded {records.Count} records from {fileName} successfully." +
                                      $"Parse Time: {parseTime} ms" +
                                      $"Bulk Insert Time: {insertTime} ms";

                    return View();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while uploading Excel file.");
                    ViewBag.Message = "Failed to upload file.";
                    return View(model);
                }
            }

            ViewBag.Message = "❌ Invalid file or input.";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetUniqueValues(string column, string term)
        {
            IQueryable<string> query = column switch
            {
                "SellerTin" => _context.ExcelRecords.Select(x => x.SellerTin),
                "BuyerTin" => _context.ExcelRecords.Select(x => x.BuyerTin),
                "ItemCatalogCode" => _context.ExcelRecords.Select(x => x.ItemCatalogCode),
                "SourceFile" => _context.ExcelRecords.Select(x => x.SourceFile),
                _ => Enumerable.Empty<string>().AsQueryable()
            };

            if (!string.IsNullOrWhiteSpace(term))
                query = query.Where(x => x != null && x.StartsWith(term));

            var results = await query
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .OrderBy(x => x)
                .Take(30)
                .ToListAsync();

            return Json(results);
        }

        [HttpGet]
        public IActionResult Filter() => View(new ExcelFilterViewModel());

        [HttpPost]
        public async Task<IActionResult> Filter(ExcelFilterViewModel filters, string sortColumn, string sortDirection)
        {
            var query = ApplyFilters(_context.ExcelRecords.AsQueryable(), filters);

            // Handle Top/Bottom limits
            if (!string.IsNullOrEmpty(filters.TopCriteria))
            {
                query = filters.TopCriteria switch
                {
                    "TopQuantity" => query.OrderByDescending(x => x.Quantity),
                    "BottomQuantity" => query.OrderBy(x => x.Quantity),
                    "TopTotalPrice" => query.OrderByDescending(x => x.TotalWithVat),
                    "BottomTotalPrice" => query.OrderBy(x => x.TotalWithVat),
                    "MostRecentContractDate" => query.OrderByDescending(x => x.ContractDate),
                    "OldestContractDate" => query.OrderBy(x => x.ContractDate),
                    _ => query
                };
            }
            else if (!string.IsNullOrEmpty(sortColumn))
            {
                bool asc = sortDirection?.ToLower() == "asc";
                query = sortColumn switch
                {
                    "ContractNo" => asc ? query.OrderBy(x => x.ContractNo) : query.OrderByDescending(x => x.ContractNo),
                    "ContractDate" => asc ? query.OrderBy(x => x.ContractDate) : query.OrderByDescending(x => x.ContractDate),
                    "InvoiceNo" => asc ? query.OrderBy(x => x.InvoiceNo) : query.OrderByDescending(x => x.InvoiceNo),
                    "InvoiceDate" => asc ? query.OrderBy(x => x.InvoiceDate) : query.OrderByDescending(x => x.InvoiceDate),
                    "SellerTin" => asc ? query.OrderBy(x => x.SellerTin) : query.OrderByDescending(x => x.SellerTin),
                    "BuyerTin" => asc ? query.OrderBy(x => x.BuyerTin) : query.OrderByDescending(x => x.BuyerTin),
                    "ItemName" => asc ? query.OrderBy(x => x.ItemName) : query.OrderByDescending(x => x.ItemName),
                    "ItemCatalogCode" => asc ? query.OrderBy(x => x.ItemCatalogCode) : query.OrderByDescending(x => x.ItemCatalogCode),
                    "Unit" => asc ? query.OrderBy(x => x.Unit) : query.OrderByDescending(x => x.Unit),
                    "Quantity" => asc ? query.OrderBy(x => x.Quantity) : query.OrderByDescending(x => x.Quantity),
                    "TotalWithVat" => asc ? query.OrderBy(x => x.TotalWithVat) : query.OrderByDescending(x => x.TotalWithVat),
                    "DocumentDate" => asc ? query.OrderBy(x => x.DocumentDate) : query.OrderByDescending(x => x.DocumentDate),
                    "SourceFile" => asc ? query.OrderBy(x => x.SourceFile) : query.OrderByDescending(x => x.SourceFile),
                    _ => query
                };
            }

            var rowLimit = filters.NumberOfRows ?? 10000;
            var results = await query.Take(Math.Min(rowLimit, 10000)).ToListAsync();

            ViewBag.SortColumn = sortColumn;
            ViewBag.SortDirection = sortDirection;

            return PartialView("~/Views/Excel/_ExcelTable.cshtml", results);
        }

        // ----------------- Helper Methods --------------------

        private static IQueryable<ExcelRecord> ApplyFilters(IQueryable<ExcelRecord> query, ExcelFilterViewModel filters)
        {
            if (filters.SellerTin?.Any() == true)
                query = query.Where(x => filters.SellerTin.Contains(x.SellerTin));

            if (filters.BuyerTin?.Any() == true)
                query = query.Where(x => filters.BuyerTin.Contains(x.BuyerTin));

            if (filters.ItemCatalogCode?.Any() == true)
                query = query.Where(x => filters.ItemCatalogCode.Contains(x.ItemCatalogCode));

            if (filters.SourceFile?.Any() == true)
                query = query.Where(x => filters.SourceFile.Contains(x.SourceFile));

            if (filters.ContractDateFrom.HasValue)
            {
                var from = DateTime.SpecifyKind(filters.ContractDateFrom.Value, DateTimeKind.Utc);
                query = query.Where(x => x.ContractDate >= from);
            }

            if (filters.ContractDateTo.HasValue)
            {
                var to = DateTime.SpecifyKind(filters.ContractDateTo.Value, DateTimeKind.Utc);
                query = query.Where(x => x.ContractDate <= to);
            }

            if (filters.DocumentDateFrom.HasValue)
            {
                var from = DateTime.SpecifyKind(filters.DocumentDateFrom.Value, DateTimeKind.Utc);
                query = query.Where(x => x.DocumentDate >= from);
            }

            if (filters.DocumentDateTo.HasValue)
            {
                var to = DateTime.SpecifyKind(filters.DocumentDateTo.Value, DateTimeKind.Utc);
                query = query.Where(x => x.DocumentDate <= to);
            }

            if (filters.QuantityFrom.HasValue)
                query = query.Where(x => x.Quantity >= filters.QuantityFrom.Value);

            if (filters.QuantityTo.HasValue)
                query = query.Where(x => x.Quantity <= filters.QuantityTo.Value);

            if (filters.PriceFrom.HasValue)
                query = query.Where(x => x.Price >= filters.PriceFrom.Value);

            if (filters.PriceTo.HasValue)
                query = query.Where(x => x.Price <= filters.PriceTo.Value);

            if (filters.PriceWithVatFrom.HasValue)
                query = query.Where(x => x.PriceWithVat >= filters.PriceWithVatFrom.Value);

            if (filters.PriceWithVatTo.HasValue)
                query = query.Where(x => x.PriceWithVat <= filters.PriceWithVatTo.Value);

            if (filters.TotalWithVatFrom.HasValue)
                query = query.Where(x => x.TotalWithVat >= filters.TotalWithVatFrom.Value);

            if (filters.TotalWithVatTo.HasValue)
                query = query.Where(x => x.TotalWithVat <= filters.TotalWithVatTo.Value);

            return query;
        }

        private List<AnalyticsResult> GenerateAnalytics(List<ExcelRecord> records, string type)
        {
            IEnumerable<IGrouping<string, ExcelRecord>> grouped;
            decimal total;
            Func<ExcelRecord, decimal> valueSelector;
            Func<ExcelRecord, string> keySelector;

            switch (type)
            {
                case "TopSoldItems":
                case "LeastSoldItems":
                    keySelector = x => x.ItemCatalogCode;
                    valueSelector = x => x.Quantity;
                    break;
                case "TopValuableItems":
                case "LeastValuableItems":
                    keySelector = x => x.ItemCatalogCode;
                    valueSelector = x => x.TotalWithVat;
                    break;
                case "TopBuyers":
                case "LeastBuyers":
                    keySelector = x => x.BuyerTin;
                    valueSelector = x => x.TotalWithVat;
                    break;
                case "TopSellers":
                case "LeastSellers":
                    keySelector = x => x.SellerTin;
                    valueSelector = x => x.TotalWithVat;
                    break;
                default:
                    return new List<AnalyticsResult>();
            }

            grouped = records.GroupBy(keySelector);
            total = records.Sum(valueSelector);

            var result = grouped
                .Select(g => new AnalyticsResult
                {
                    Key = g.Key,
                    Value = g.Sum(valueSelector),
                    Percentage = total > 0 ? Math.Round(g.Sum(valueSelector) * 100m / total, 2) : 0
                });

            return type.StartsWith("Top")
                ? result.OrderByDescending(r => r.Value).Take(30).ToList()
                : result.OrderBy(r => r.Value).Take(30).ToList();
        }

        private async Task<(List<ExcelRecord> Records, long ParseTime, long InsertTime)> ParseExcelFileAsync(IFormFile file, string sourceFileName)
        {
            var parseWatch = Stopwatch.StartNew();
            var records = new List<ExcelRecord>();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets.First();
            var rowCount = worksheet.Dimension.Rows;
            var culture = CultureInfo.InvariantCulture;

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    DateTime.TryParseExact(worksheet.Cells[row, 3].Text, "dd.MM.yyyy", culture, DateTimeStyles.None, out var contractDate);
                    contractDate = DateTime.SpecifyKind(contractDate, DateTimeKind.Utc);

                    DateTime.TryParseExact(worksheet.Cells[row, 5].Text, "dd.MM.yyyy", culture, DateTimeStyles.None, out var invoiceDate);
                    invoiceDate = DateTime.SpecifyKind(invoiceDate, DateTimeKind.Utc);

                    DateTime.TryParseExact(worksheet.Cells[row, 19].Text, "dd.MM.yyyy", culture, DateTimeStyles.None, out var documentDate);
                    documentDate = DateTime.SpecifyKind(documentDate, DateTimeKind.Utc);

                    decimal.TryParse(worksheet.Cells[row, 15].Text.Replace(" ", ""), NumberStyles.Any, culture, out var quantity);
                    decimal.TryParse(worksheet.Cells[row, 16].Text.Replace(" ", ""), NumberStyles.Any, culture, out var price);
                    decimal.TryParse(worksheet.Cells[row, 17].Text.Replace(" ", ""), NumberStyles.Any, culture, out var priceWithVat);
                    decimal.TryParse(worksheet.Cells[row, 18].Text.Replace(" ", ""), NumberStyles.Any, culture, out var totalWithVat);

                    records.Add(new ExcelRecord
                    {
                        Status = worksheet.Cells[row, 1].Text,
                        ContractNo = worksheet.Cells[row, 2].Text,
                        ContractDate = contractDate,
                        InvoiceNo = worksheet.Cells[row, 4].Text,
                        InvoiceDate = invoiceDate,
                        SellerTin = worksheet.Cells[row, 6].Text,
                        SellerName = worksheet.Cells[row, 7].Text,
                        BuyerTin = worksheet.Cells[row, 8].Text,
                        CommitentTin = worksheet.Cells[row, 9].Text,
                        CommitentName = worksheet.Cells[row, 10].Text,
                        CommitentVatNo = worksheet.Cells[row, 11].Text,
                        ItemName = worksheet.Cells[row, 12].Text,
                        ItemCatalogCode = worksheet.Cells[row, 13].Text,
                        Unit = worksheet.Cells[row, 14].Text,
                        Quantity = quantity,
                        Price = price,
                        PriceWithVat = priceWithVat,
                        TotalWithVat = totalWithVat,
                        DocumentDate = documentDate,
                        SourceFile = sourceFileName
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Row {Row} failed to parse: {Message}", row, ex.Message);
                }
            }

            parseWatch.Stop();

            var insertWatch = Stopwatch.StartNew();
            await _context.BulkInsertAsync(records);
            insertWatch.Stop();

            return (records, parseWatch.ElapsedMilliseconds, insertWatch.ElapsedMilliseconds);
        }
    }
}