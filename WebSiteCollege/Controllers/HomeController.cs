using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using WebSiteCollege.Identity;
using WebSiteCollege.Models;
using OfficeOpenXml;
using System.IO;

namespace WebSiteCollege.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _dbContext;

        public HomeController( IConfiguration config, ApplicationDbContext dbContext)
        {
            _config = config;
            _dbContext = dbContext;
            
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("Data Source=localhost;Initial Catalog=COLLEGE;Integrated Security=True;Trust Server Certificate=True"));
            }
        }

        public async Task<IActionResult> AdminDashboardAsync()
        {
            //var students = await _dbContext.Students.ToListAsync();
            //return View("AdminDashboard", students);
            return View();
        }

        public IActionResult ExportToExcel()
        {
            // Get your data from the database or any other source
            var data = _dbContext.Students.ToList();

            // Create a new Excel package
            using (var package = new ExcelPackage())
            {
                // Add a new worksheet
                var worksheet = package.Workbook.Worksheets.Add("Students");

                // Set the header row
                worksheet.Cells["A1"].Value = "Student ID";
                worksheet.Cells["B1"].Value = "User Name";
                worksheet.Cells["C1"].Value = "User Type";
                worksheet.Cells["D1"].Value = "Login";
                worksheet.Cells["E1"].Value = "Email";

                // Populate data starting from the second row
                for (int i = 0; i < data.Count; i++)
                {
                    var row = i + 2;
                    worksheet.Cells[$"A{row}"].Value = data[i].StudentID;
                    worksheet.Cells[$"B{row}"].Value = data[i].UserName;
                    worksheet.Cells[$"C{row}"].Value = data[i].UserType;
                    worksheet.Cells[$"D{row}"].Value = data[i].Login;
                    worksheet.Cells[$"E{row}"].Value = data[i].Email;
                }

                // Set content type and file name
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "Students.xlsx";

                // Save the Excel package to a MemoryStream
                using (var memoryStream = new MemoryStream())
                {
                    package.SaveAs(memoryStream);
                    memoryStream.Position = 0;

                    // Return the Excel file
                    return File(memoryStream.ToArray(), contentType, fileName);
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminDashboardAsync(StudentModel student)
        {
            if (ModelState.IsValid)
            {

                _dbContext.Students.Add(student);
                await _dbContext.SaveChangesAsync();

                // Redirect to a different action or view after successful save
                return RedirectToAction("AdminDashboard");
            }

            return View("AdminDashboard", student);
        }


        public IActionResult StudentDashboard()
        {
            return View();
        }

 
        public IActionResult TeacherDashboard()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var user = await _dbContext.ApplicationUsers.SingleOrDefaultAsync(u => u.Login == model.Login);

                if (user != null && user.Password == model.Password) 
                {
                    // Получите роли пользователя
                    var roles = await _dbContext.ApplicationUsers
                        .Where(ur => ur.Login == user.Login)
                        .Join(_dbContext.ApplicationUsers, ur => ur.UserType, r => r.UserType, (ur, r) => r.UserType)
                        .ToListAsync();

                    // Перенаправление в зависимости от ролей
                    if (roles.Contains("Administrator"))
                        return RedirectToAction("AdminDashboard", "Home");

                    if (roles.Contains("Student"))
                        return RedirectToAction("StudentDashboard", "Home");

                    if (roles.Contains("Teacher"))
                        return RedirectToAction("TeacherDashboard", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt");
            }
            return View("Privacy");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
