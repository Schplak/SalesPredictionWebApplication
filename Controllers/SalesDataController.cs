using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesPredictionWebApplication.Data;
using SalesPredictionWebApplication.Models;
using static System.Net.Mime.MediaTypeNames;

namespace SalesPredictionWebApplication.Controllers
{
    public class SalesDataController : Controller
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public DateTime PredictionDate { get; set; }

        public SalesDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SalesData
        public async Task<IActionResult> Index()
        {
              return _context.SalesData != null ?
                          View(await _context.SalesData.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.SalesData'  is null.");
        }

        // GET: SalesData/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SalesData == null)
            {
                return NotFound();
            }

            var salesData = await _context.SalesData
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salesData == null)
            {
                return NotFound();
            }

            return View(salesData);
        }

        // GET: SalesData/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: SalesData/Predict
        // Gets SalesData then predicts future sales data
        public IActionResult Predict()
        {
            return View();
        }

        // GET: SalesData/Import
        public IActionResult Import()
        {
            return View();
        }

        // POST: SalesData/Import
        //Imports a CSV file containing one or more SalesData records
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import([Bind("Files")] CSVFilesModel csvFiles)
        {
            var test = Request.Form.Files;

            if (ModelState.IsValid)
            {
                foreach (IFormFile file in csvFiles.Files)
                {
                    List<SalesData> allSalesData = GetSalesDataFromCSV(file).ToList();

                    foreach (SalesData salesData in allSalesData)
                    {
                        _context.Add(salesData);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: SalesData/Predict/5
        public async Task<IActionResult> Predict(DateTime? predictionDate)
        {
            if (predictionDate == null || _context.SalesData == null)
            {
                return NotFound();
            }

            //First get all applicable sales data
            DateTime dateFrom = DateTime.Now.AddDays(-14);
            List<SalesData> salesData = await _context.SalesData.Where(data => data.Date > dateFrom).ToListAsync();
            if (salesData == null || salesData.Count == 0)
            {
                return NotFound();
            }
            return View(salesData);
        }

        // POST: SalesData/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Amount")] SalesData salesData)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salesData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salesData);
        }

        // GET: SalesData/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SalesData == null)
            {
                return NotFound();
            }

            var salesData = await _context.SalesData.FindAsync(id);
            if (salesData == null)
            {
                return NotFound();
            }
            return View(salesData);
        }

        // POST: SalesData/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Amount")] SalesData salesData)
        {
            if (id != salesData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesDataExists(salesData.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(salesData);
        }

        // GET: SalesData/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SalesData == null)
            {
                return NotFound();
            }

            var salesData = await _context.SalesData
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salesData == null)
            {
                return NotFound();
            }

            return View(salesData);
        }

        // POST: SalesData/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SalesData == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SalesData'  is null.");
            }
            var salesData = await _context.SalesData.FindAsync(id);
            if (salesData != null)
            {
                _context.SalesData.Remove(salesData);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesDataExists(int id)
        {
          return (_context.SalesData?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private IEnumerable<SalesData> GetSalesDataFromCSV(IFormFile csvFile)
        {
            var allSalesData = new List<SalesData>();
            List<string> entries = ConvertCSVFileToText(csvFile).ToList();

            foreach (string entry in entries)
            {
                SalesData? salesData = GetSalesDataFromText(entry);

                if (salesData != null)
                {
                    allSalesData.Add(salesData);
                }
            }
            return allSalesData;
        }

        private IEnumerable<string> ConvertCSVFileToText(IFormFile file)
        {
            var result = new List<string>();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                {
                    string? line = reader.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        result.Add(line);
                    }
                }
            }
            return result;
        }

        private SalesData? GetSalesDataFromText(string text)
        {
            string dateFormat = "yyyy-MM-dd";
            IFormatProvider provider = CultureInfo.InvariantCulture;
            DateTimeStyles styles = DateTimeStyles.None;

            string[] textParts = text.Split(',');
            if (textParts != null && textParts.Length == 2)
            {
                if (DateTime.TryParseExact(textParts[0], dateFormat, provider, styles, out DateTime date)
                    && float.TryParse(textParts[1], out float amount))
                {
                    var result = new SalesData()
                    {
                        Date = date,
                        Amount = amount,
                    };
                    return result;
                }
            }
            return null;
        }
    }
}
