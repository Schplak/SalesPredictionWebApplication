using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesPredictionWebApplication.Data;
using SalesPredictionWebApplication.Models;
using System.Globalization;

namespace SalesPredictionWebApplication.Controllers
{
    public class SalesDataController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IConfiguration configuration;
        public SalesDataController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            configuration = config;
        }

        // GET: SalesData
        [HttpGet]
        public async Task<IActionResult> Index()
        {
              return _context.SalesData != null ?
                          View(await _context.SalesData.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.SalesData' is null.");
        }

        // GET: SalesData/Details/5
        [HttpGet]
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
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // GET: SalesData/Predict
        [HttpGet]
        public IActionResult Predict()
        {
            return View();
        }

        // GET: SalesData/Import
        [HttpGet]
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
            if (ModelState.IsValid && _context.SalesData != null)
            {
                //Get the sales data and dates in the db
                List<SalesDataModel> dbSalesData = await _context.SalesData.ToListAsync();
                List<DateTime> currentDbSalesDataDates = dbSalesData.Select(d => d.Date).ToList();

                int salesDataSuccessCount = 0;

                foreach (IFormFile file in csvFiles.Files)
                {
                    var (ResultMessages, SuccessCount) = AddSalesDataToDatabase(file, currentDbSalesDataDates);

                    csvFiles.CSVFileErrors.AddRange(ResultMessages.ToList());
                    salesDataSuccessCount += SuccessCount;
                }

                csvFiles.CSVFileSuccessMessage = salesDataSuccessCount > 0
                    ? $"{salesDataSuccessCount} sales data date(s) added successfully."
                    : "No sales data dates added.";

                await _context.SaveChangesAsync();
            }
            return View(csvFiles);
        }

        // GET: SalesData/Predict/2023-12-31
        // Gets SalesData then predicts future sales data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Predict([Bind("Date")] PredictionModel prediction)
        {
            if (prediction == null || _context.SalesData == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                List<SalesDataModel> salesData = new();

                string historyConfig = configuration.GetSection("AppSettings").GetSection("History").Value;
                if (int.TryParse(historyConfig, out int daysPredictionHistory) && prediction.Date.HasValue)
                {
                    prediction.DaysOfHistory = daysPredictionHistory;
                    DateTime predictionFromDate = prediction.Date.Value.AddDays(-daysPredictionHistory);
                    salesData = await _context.SalesData.Where(data => data.Date >= predictionFromDate)
                                                        .OrderBy(data => data.Date)
                                                        .ToListAsync();

                    if (salesData != null && salesData.Count > 0)
                    {
                        //calculate predicted sales based on sales data
                        prediction.PredictedSales = PredictSales(salesData, prediction.Date.Value);
                        prediction.NumberOfHistoricalSales = salesData.Count;
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            return View(prediction);
        }

        // POST: SalesData/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Amount")] SalesDataModel salesData)
        {
            if (ModelState.IsValid && _context.SalesData != null)
            {
                //Get the sales data in the db
                List<SalesDataModel> dbSalesData = await _context.SalesData.ToListAsync();
                List<DateTime> dbSalesDataDates = dbSalesData.Select(d => d.Date).ToList();

                if (!dbSalesDataDates.Contains(salesData.Date))
                {
                    _context.Add(salesData);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //clear the sales data
                    salesData.Date = DateTime.MinValue;
                    salesData.Amount = 0;
                }
            }
            return View(salesData);
        }

        // GET: SalesData/Edit/5
        [HttpGet]
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Amount")] SalesDataModel salesData)
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
        [HttpGet]
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

        private IEnumerable<SalesDataModel> GetSalesDataFromCSV(IFormFile csvFile)
        {
            var allSalesData = new List<SalesDataModel>();
            List<string> entries = ConvertCSVFileToText(csvFile).ToList();

            foreach (string entry in entries)
            {
                SalesDataModel? salesData = GetSalesDataFromText(entry);

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

        private SalesDataModel? GetSalesDataFromText(string text)
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
                    var result = new SalesDataModel()
                    {
                        Date = date,
                        Amount = amount,
                    };
                    return result;
                }
            }
            return null;
        }

        private (IEnumerable<string> ResultMessages, int SuccessCount) AddSalesDataToDatabase(IFormFile file, List<DateTime> salesDatesInDatabase)
        {
            List<SalesDataModel> salesDataToAdd = GetSalesDataFromCSV(file).ToList();
            List<string> resultMessages = new();
            List<DateTime> addedSalesDataDates = new();
            int salesDataSuccessCount = 0;

            if (salesDataToAdd.Count > 0)
            {
                foreach (SalesDataModel salesData in salesDataToAdd)
                {
                    if (salesData.Amount > 0f)
                    {
                        if (salesData.Date <= DateTime.Now)
                        {
                            //check to make sure we are not adding a date that already exists in this file
                            if (!addedSalesDataDates.Contains(salesData.Date))
                            {
                                //check to make sure we are not adding a date that already exists in the db
                                if (!salesDatesInDatabase.Contains(salesData.Date))
                                {
                                    _context.Add(salesData);
                                    addedSalesDataDates.Add(salesData.Date);
                                    salesDataSuccessCount++;
                                }
                                else
                                {
                                    resultMessages.Add($"Error found in '{file.FileName}': The database already contains sales data for the date {salesData.Date:dd/MM/yyyy}. Sales data  of ${salesData.Amount} ignored.");
                                }
                            }
                            else
                            {
                                resultMessages.Add($"Error found in '{file.FileName}': The file already contains sales data for the date {salesData.Date:dd/MM/yyyy}. Sales data  of ${salesData.Amount} ignored.");
                            }
                        }
                        else
                        {
                            resultMessages.Add($"Error found in '{file.FileName}': The sales data date {salesData.Date:dd/MM/yyyy} cannot be in the future. Sales data  of ${salesData.Amount} ignored.");
                        }
                    }
                    else
                    {
                        resultMessages.Add($"Error found in '{file.FileName}': The sales data for the date {salesData.Date:dd/MM/yyyy} is negative. Sales data  of ${salesData.Amount} ignored.");
                    }
                }
            }
            else
            {
                resultMessages.Add($"Error found in '{file.FileName}': The sales data could not be read from the file. File '{file.FileName}' ignored.");
            }

            return (resultMessages, salesDataSuccessCount);
        }

        private float PredictSales(List<SalesDataModel> salesData, DateTime predictionDate)
        {
            // Prediction calculation - Historical Forecasting using the formula:
            // Previous Sales + Velocity Average (the rate at which sales increase over time) = Predicted Sales

            float predictedSales = 0f;
            int numberOfDaysOfSales = salesData.Count;

            //we need more than one sale to determine sales velocity
            if (numberOfDaysOfSales > 1)
            {
                float salesVelocityAverage = 0f;
                SalesDataModel? previousSales = null;

                foreach (SalesDataModel sales in salesData)
                {
                    if (previousSales != null)
                    {
                        //calculate rate of change
                        salesVelocityAverage += sales.Amount - previousSales.Amount;
                    }
                    previousSales = sales;
                }

                salesVelocityAverage /= (numberOfDaysOfSales - 1);

                //get the number of days since the last actual sales data. Since the list is ordered, it will be the last element
                int numberOfDaysSinceLastSalesData = (predictionDate - salesData.Last().Date).Days;

                predictedSales = (previousSales?.Amount + (salesVelocityAverage * numberOfDaysSinceLastSalesData)) ?? 0f;
            }
            return predictedSales;
        }

    }
}
