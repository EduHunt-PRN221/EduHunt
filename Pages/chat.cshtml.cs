using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Eduhunt.Pages
{
    public class chatModel : PageModel
    {
        public string UserEmail { get; set; }
        public string ConvertedUserEmail { get; set; }

        // Method to convert email to Firebase-friendly format
        public string ConvertEmailToString(string email)
        {
            if (string.IsNullOrEmpty(email)) return email;
            return email.Replace("@", "__AT__").Replace(".", "__DOT__");
        }

        // Method to restore the email from the converted format
        public string RestoreEmail(string convertedEmail)
        {
            if (string.IsNullOrEmpty(convertedEmail)) return convertedEmail;
            return convertedEmail.Replace("__AT__", "@").Replace("__DOT__", ".");
        }

        public void OnGet()
        {
            // Lấy email từ Cookie
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
    }
}