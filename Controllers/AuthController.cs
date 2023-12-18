using EventsClient.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace EventsClient.Controllers
{
    public class AuthController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7161/api/Auth");
        const string LOGIN_ENDPOINT = "Login";

        private readonly HttpClient _client;

        public AuthController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        public IActionResult Register()
        {
            User user = new User();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"{_client.BaseAddress}/register", content);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                if (responseData.ContainsKey("status") && responseData["status"].ToString() == "success")
                {
                    ViewBag.RegisterSuccessMessage = responseData["message"].ToString();
                    return RedirectToAction("Index", "Auth");
                }
                else
                {
                    // Registration failed logic
                    ViewBag.RegisterError = responseData["message"].ToString();
                }
            }
            else
            {
                // Handle HTTP error status codes if needed
                ViewBag.RegisterError = "Registration failed. Please try again later.";
            }

            return View("Index", user);
        }

        public IActionResult Index()
        {
            User user = new User();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Index(User user)
        {
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"{_client.BaseAddress}/{LOGIN_ENDPOINT}", content);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                if (responseData.ContainsKey("status") && responseData["status"].ToString() == "success")
                {
                    if (responseData.ContainsKey("data"))
                    {
                        var token = responseData["data"].ToString();
                        if (!string.IsNullOrEmpty(token))
                        {
                            HttpContext.Session.SetString("SessionAuth", token);
                            HttpContext.Session.SetString("SessionUsername", user.Username);
                            var returnUrl = HttpContext.Session.GetString("returnUrl");

                            if (returnUrl == null)
                            {
                                return RedirectToAction("Index", "Home");
                            }
                            return Redirect(returnUrl);
                        }
                    }
                }

                ViewBag.LoginError = "The username or password you've entered is incorrect";
            }
            return View(user);
        }


        public async Task<IActionResult> LogOut()
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.Remove("SessionAuth");
                return RedirectToAction("Index", "Auth");
            }
            return View();
        }
    }
}
