using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HotelManagementSystem.Context;
using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SHA256 _sha256;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
            _sha256 = SHA256.Create();

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            // Check if passwords match
            if (model.PasswordHash != Request.Form["ConfirmPassword"])
            {
                ModelState.AddModelError("", "Passwords do not match");
                ViewBag.ErrorMessage = "Passwords do not match.";

                return View();
            }

            // Check if the user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username || u.Email == model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "User already exists with the same username or email");
                ViewBag.ErrorMessage = "User already exists with the same username or email.";
                return View();
            }

            // Hash the password using SHA256
            var passwordHash = HashPassword(model.PasswordHash);

            // Create the user object
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = passwordHash,
                Role = "User"
            };

            // Save the user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Redirect to login page or dashboard
            return RedirectToAction("Login", "Auth");
        }
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }

        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User model)
        {

            var user = _context.Users.FirstOrDefault(u => u.Username == model.Email || u.Email == model.Email);

            if (user != null)
            {
                // Check password hash
                using (var sha256 = SHA256.Create())
                {
                    var passwordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(model.PasswordHash)));

                    if (passwordHash == user.PasswordHash)
                    {
                        // Log in user (Set session or cookie)
                        // Example: store user ID in session
                        HttpContext.Session.SetInt32("UserId", user.Id);
                        HttpContext.Session.SetString("Role", user.Role);
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Role, user.Role) // Add role claim
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        // Sign the user in
                        await HttpContext.SignInAsync("Cookies", claimsPrincipal);

                        // Redirect to home page or dashboard
                        if (user.Role == "Admin")
                            return RedirectToAction("Dashboard", "Admin");
                        else if (user.Role == "User")
                            return RedirectToAction("Dashboard", "User");
                    }
                    else
                    {
                        // Invalid password
                        ViewBag.ErrorMessage = "Invalid username or password.";
                        return View();
                    }
                }
            }
            else
            {
                // User not found
                ViewBag.ErrorMessage = "User not found.";
                return View();
            }


            return View();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            string enteredPasswordHash = HashPassword(enteredPassword);
            return enteredPasswordHash == storedPasswordHash;
        }


        public async Task<ActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Optional: clear session
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Auth"); // or your login page
        }

    }
}
