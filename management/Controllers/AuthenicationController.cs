using management.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace management.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private ILogger<AuthController> _logger;
        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet("login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // Redirect user to Google authentication page
        [HttpGet("login-google")]
        public IActionResult LoginWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Auth", null, Request.Scheme);
            _logger.LogInformation("Redirect URI: {RedirectUri}", redirectUrl);  // Log the redirect URI

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirectUrl);
            var challengeUrl = Url.Action("Challenge", "Auth", properties);  // Log the full challenge URL
            _logger.LogInformation("Challenge URL: {ChallengeUrl}", challengeUrl);

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Handle Google authentication response
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            // Retrieve the external login info from Google
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                // Handle the case where external login info is not available
                return RedirectToAction("LoginFailed");
            }

            // Extract the email from the external login info
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            // Check if the user already exists in the database
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Create a new user if one doesn't exist
                user = new ApplicationUser
                {
                    UserName = email?.Split('@')[0], // Use email as the username
                    Email = email,
                    EmailConfirmed = true // Mark email as confirmed since it's verified by Google
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    // Handle the case where user creation fails
                    return BadRequest("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                // Add the external login info to the user
                await _userManager.AddLoginAsync(user, info);
            }

            // Sign in the user
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Redirect to the home page or another desired page
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");  // Redirect to AuthController's Login action
        }

        // Handle failed login attempt
        [HttpGet("loginfailed")]
        public IActionResult LoginFailed()
        {
            ViewData["Error"] = "Google login failed. Please try again.";
            return View("Login");  // Ensure you have a Login view to display the error message
        }
    }
}