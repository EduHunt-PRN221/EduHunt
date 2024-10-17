using Eduhunt.Applications.ProfileService;
using Eduhunt.Applications.Scholarships;
using Eduhunt.DTOs;
using Eduhunt.Infrastructures.Cloud;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Eduhunt.Pages
{
    public class AddScholarshipModel : PageModel
    {
        [BindProperty]
        public Scholarship Scholarship { get; set; } = new Scholarship();
        private readonly ScholarshipService _scholarshipService;
        private readonly CloudinaryService _cloudinaryService;


        public AddScholarshipModel(ScholarshipService scholarshipService, CloudinaryService cloudinaryService)
        {
            _scholarshipService = scholarshipService;
            _cloudinaryService = cloudinaryService;
        }

        [BindProperty]
        public IFormFile? UploadImage { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //validate address location
            if (Scholarship.Location != null && !Scholarship.Location.Contains(","))
            {
                ModelState.AddModelError("Scholarship.Location", "Location must contain ','");
            }

            //validate URL
            if (Scholarship.Url != null && !Scholarship.Url.StartsWith("http://") && !Scholarship.Url.StartsWith("https://"))
                {
                    ModelState.AddModelError("Scholarship.Url", "URL must start with http:// or https://");
                }


            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Scholarship != null)
            {
                // load image url
                await LoadImageUrl();
                // convert to ScholarshipDto
                var scholarshipDto = new ScholarshipDto
                {
                    Id = Scholarship.Id,
                    Budget = Scholarship.Budget,
                    Level = Scholarship.Level,
                    AuthorId = Scholarship.AuthorId,
                    Description = Scholarship.Description,
                    ImageUrl = Scholarship.ImageUrl,
                    IsApproved = Scholarship.IsApproved,
                    IsInSite = Scholarship.IsInSite,
                    Location = Scholarship.Location,
                    SchoolName = Scholarship.SchoolName,
                    Title = Scholarship.Title,
                    Url = Scholarship.Url

                };
                try
                {
                    await _scholarshipService.PostScholarshipInfo(scholarshipDto);
                    TempData["success"] = "Create Scholarship successfully.";

                }
                catch
                {
                    TempData["error"] = "Create Scholarship failed.";
                }
            }
            return RedirectToPage();
        }

        // load url img from cloudinary to scholarship 
        private async Task LoadImageUrl()
        {
            if (UploadImage != null)
            {
                var imageUrl = await _cloudinaryService.UploadSingleAsync(UploadImage);
                if (imageUrl != null)
                {
                    Scholarship.ImageUrl = imageUrl;
                }
                else
                {
                    Scholarship.ImageUrl = "https://placehold.co/600x400";
                }
            }
            else
            {
                Scholarship.ImageUrl = "https://placehold.co/600x400";
            }
        }
    }
}
