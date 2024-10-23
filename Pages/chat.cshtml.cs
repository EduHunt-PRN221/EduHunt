using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Eduhunt.Pages
{
    public class chatModel : PageModel
    {
        public string UserEmail { get; set; }
        public string ConvertedUserEmail { get; set; }

        // Converts the email to a Firebase-friendly format
        public string ConvertEmailToString(string email)
        {
            return string.IsNullOrEmpty(email) ? email : email.Replace("@", "__AT__").Replace(".", "__DOT__");
        }

        // Restores the original email from the Firebase-friendly format
        public string RestoreEmail(string convertedEmail)
        {
            return string.IsNullOrEmpty(convertedEmail) ? convertedEmail : convertedEmail.Replace("__AT__", "@").Replace("__DOT__", ".");
        }

        // Get method for the chat page
        public void OnGet()
        {
            // Get email from Cookie
            if (Request.Cookies.ContainsKey("email"))
            {
                UserEmail = Request.Cookies["email"];
            }
            else
            {
                UserEmail = "clone@gmail.com"; // Default email
            }

            // Convert email to Firebase-friendly format
            ConvertedUserEmail = ConvertEmailToString(UserEmail);
        }

        // AJAX method to get restored emails
        public IActionResult OnGetRestoredEmails()
        {
            // Simulated data; in real use, fetch the emails from the database
            var convertedEmails = new List<string> { "user__AT__example__DOT__com", "test__AT__example__DOT__com" };
            var restoredEmails = new List<string>();

            // Restore emails
            foreach (var email in convertedEmails)
            {
                restoredEmails.Add(RestoreEmail(email));
            }

            // Return as JSON
            return new JsonResult(restoredEmails);
        }
    }
}
