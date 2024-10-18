using Eduhunt.Applications.Roadmaps;
using Eduhunt.Applications.ApplicactionUsers;
using Eduhunt.Applications.ProfileService;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Eduhunt.Pages
{
    public class RoadmapModel : PageModel
    {
        private readonly RoadmapService _roadmapService;
        private readonly ProfileService _profileService;
        private readonly ApplicationUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoadmapModel(
            RoadmapService roadmapService,
            ProfileService profileService,
            ApplicationUserService userService,
            IHttpContextAccessor httpContextAccessor)
        {
            _roadmapService = roadmapService;
            _profileService = profileService;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public List<Roadmap> Roadmaps { get; set; } = new List<Roadmap>();

        public List<ApplicationUser> Mentors { get; set; } = new List<ApplicationUser>();

        [BindProperty(SupportsGet = true)]
        public string? SelectedMentorId { get; set; }

        public bool IsMentor { get; set; }
        public bool CanAddStep { get; set; }

        public async Task OnGetAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext!;
            var idToken = httpContext.Request.Cookies["IdToken"];
            if (string.IsNullOrEmpty(idToken))
            {
                LocalRedirect("/Identity/Login");
                return;
            }

            var userEmail = _profileService.GetEmailFromToken(idToken);
            IsMentor = await _userService.IsMentorAsync(userEmail);

            var userId = _userService.GetUserId(userEmail);

            if (IsMentor)
            {
                Roadmaps = await _roadmapService.GetAll()
                    .Where(x => x.ApplicationUserId == userId && x.IsNotDeleted)
                    .ToListAsync();

                if(Roadmaps.Count == 0)
                {
                    CanAddStep = true;
                }
                else
                {
                    CanAddStep = false;
                }
            }
            else
            {
                Mentors = await _userService.GetMentorsAsync();
                Roadmaps = await _roadmapService.GetAll()
                    .Where(x => x.IsNotDeleted)
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext!;
            var idToken = httpContext.Request.Cookies["IdToken"];
            if (string.IsNullOrEmpty(idToken))
            {
                return RedirectToPage("Login");
            }

            var userEmail = _profileService.GetEmailFromToken(idToken);
            IsMentor = await _userService.IsMentorAsync(userEmail);

            var userId = _userService.GetUserId(userEmail);

            if (IsMentor)
            {
                if (!ModelState.IsValid)
                {
                    CanAddStep = true;
                    return Page();
                }

                // Remove existing roadmap steps for this user
                var existingSteps = await _roadmapService.GetAll()
                    .Where(x => x.ApplicationUserId == userId && x.IsNotDeleted)
                    .ToListAsync();

                foreach (var step in existingSteps)
                {
                    step.IsNotDeleted = false;
                    await _roadmapService.UpdateAsync(step);
                }

                // Add new roadmap steps
                foreach (var step in Roadmaps)
                {
                    step.ApplicationUserId = userId;
                    step.IsNotDeleted = true;
                    await _roadmapService.AddAsync(step);
                }

                TempData["success"] = "Roadmap đã được lưu thành công.";

                return LocalRedirect("/Roadmap");
            }
            else
            {
                return Forbid();
            }
        }
    }
}