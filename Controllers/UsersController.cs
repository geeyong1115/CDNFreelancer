using CDNFreelancer.Data;
using CDNFreelancer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CDNFreelancer.Controllers
{
    public class UsersController : Controller
    {
        public async Task<IActionResult> Index(int page = 1, int pageSize = 3)
        {
            PagedResult<User> pagedResult = new PagedResult<User>
            {
                Results = new List<User>() // Initialize Results as an empty list
            };
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync($"https://localhost:7165/api/usersapi?page={page}&pageSize={pageSize}"))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            var jsonObject = JsonDocument.Parse(apiResponse).RootElement;
                            pagedResult.TotalCount = jsonObject.GetProperty("totalCount").GetInt32();

                            if (jsonObject.TryGetProperty("results", out var results))
                            {
                                foreach (var userJson in results.EnumerateArray())
                                {
                                    var user = new User
                                    {
                                        Id = userJson.GetProperty("id").GetInt32(),
                                        Username = userJson.GetProperty("username").GetString(),
                                        Email = userJson.GetProperty("email").GetString(),
                                        PhoneNumber = userJson.GetProperty("phoneNumber").GetString(),
                                        Skillsets = userJson.GetProperty("skillsets").GetString(),
                                        Hobby = userJson.GetProperty("hobby").GetString()
                                    };

                                    pagedResult.Results.Add(user);
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "An error occurred while retrieving the users. Please try again later.");
                            }
                        }
                    }

                    ViewBag.CurrentPage = page;
                    ViewBag.TotalPages = (int)Math.Ceiling(pagedResult.TotalCount / (double)pageSize);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
            }

            return View(pagedResult.Results);
        }


        // Initial Edit Page, call Find API
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            User user = new User();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync($"https://localhost:7165/api/usersapi/{id}"))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            user = JsonSerializer.Deserialize<User>(apiResponse);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "An error occurred while retrieving the user. Please try again later.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An unexpected error occurred: {ex.Message}");
            }
            return View(user);
        }


        //Call Edit API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    var content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PutAsync($"https://localhost:7165/api/usersapi/{id}", content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            return NotFound();
                        }
                        else
                        {
                            return StatusCode((int)response.StatusCode);
                        }
                    }
                }
            }
            return View(user);
        }

        public async Task<IActionResult> Delete(int id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync($"https://localhost:7165/api/usersapi/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return RedirectToAction("Index");
        }

        //Initial Create Page
        public IActionResult Create()
        {
            return View();
        }

        //Call Create API
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid) // Checks if the model is valid
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync("https://localhost:7165/api/usersapi", content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            var addedUser = JsonSerializer.Deserialize<User>(apiResponse);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "An error occurred while creating the user. Please try again.");
                        }
                    }
                }
            }

            return View(user); 
        }

    }
}

