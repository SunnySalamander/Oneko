using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;


namespace Oneko_WebApp.Pages
{
    public class IndexModel : PageModel
    {   
        //ILogger is an interface in ASP.NET Core that provides a standardized way to write log messages
        private readonly ILogger<IndexModel> _logger;

        //Service for making HTTP calls
        private readonly IHttpClientFactory _httpClientFactory;
        //Service for reading secrets
        private readonly IConfiguration _configuration;
        
        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        //TicketForm Class
        public class TicketForm
        {
            [Required(ErrorMessage = "First name is required.")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Last name is required")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "This is a required field")]
            public string Help { get; set; } = string.Empty;
        }

        [BindProperty]
        public TicketForm Input { get; set; } = new TicketForm();
        public void OnGet()
        {
            //not getting anything
        }

        public async Task<IActionResult> OnPostAsync()
        {
            
            // Check if the form data is valid based on the annotations in the TicketForm class.
            if (!ModelState.IsValid)
            {
                // If not valid, return the same page to show validation errors.
                return Page();
            }

            // --- Start: Call Power Automate Flow ---

            // 1. Securely get the URL from your configuration (secrets.json or Azure settings)
            var paURL = _configuration["PowerAutomate:Url"];
            if (string.IsNullOrEmpty(paURL))
            {
                _logger.LogError("Power Automate URL is not configured.");
                // Add a user-friendly error message
                TempData["ErrorMessage"] = "There was a configuration error. Please try again later.";
                return Page();
            }

            // 2. Create an HttpClient to make the web request
            var client = _httpClientFactory.CreateClient();

            // 3. Serialize your form data into a JSON string
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(Input),
                Encoding.UTF8,
                "application/json"
            );

            // 4. Send the POST request and wait for the response
            var response = await client.PostAsync(paURL, jsonContent);

            // 5. Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully sent data to Power Automate.");
                TempData["SuccessMessage"] = "Thank you! Your submission has been received.";
            }
            else
            {
                _logger.LogError("Failed to send data to Power Automate. Status: {StatusCode}", response.StatusCode);
                TempData["ErrorMessage"] = "Sorry, there was a problem sending your request. Please try again.";
            }

            // --- End: Call Power Automate Flow ---

            // Reset the form model and redirect to the same page
            ModelState.Clear();
            Input = new TicketForm();


            return RedirectToPage("./Index");
        }


    }
}
