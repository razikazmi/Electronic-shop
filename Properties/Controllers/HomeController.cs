using HardwareShop.DataDB;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HardwareShopContext _context;

    public HomeController(ILogger<HomeController> logger, HardwareShopContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Signup()
    {
        return View();
    }

    // Other methods...

    [HttpPost]
    public IActionResult Signup(User user, string confirmPassword)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                _logger.LogError(error.ErrorMessage);
            }
            return View(user);
        }

        if (user.PasswordHash != confirmPassword)
        {
            ModelState.AddModelError("", "Passwords do not match.");
            return View(user);
        }

        user.PasswordHash = HashPassword(user.PasswordHash);
        user.CreatedAt = DateTime.Now;

        _context.Users.Add(user);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult buy()
    {
        return View();
    }


    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
