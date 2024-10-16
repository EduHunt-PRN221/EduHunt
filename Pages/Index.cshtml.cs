using Eduhunt.Applications.Scholarships;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Eduhunt.Pages
{
    public class IndexModel : PageModel
    {
        public IQueryable<Scholarship> NewScholarships = default!;
        public IEnumerable<Scholarship> TopScholarships = default!;
        private readonly IServiceProvider _serviceProvider;

        public IndexModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void OnGet()
        {
            var scholarship = _serviceProvider.GetService<ScholarshipService>();
            if (scholarship != null)
            {
                var allScholarship = scholarship.GetAll();
                NewScholarships = allScholarship.OrderByDescending(x => x.CreatedAt);
                Random random = new Random();
                TopScholarships = allScholarship.ToList().OrderBy(x => x.Budget)
                                .Take(4);
            }
        }
    }
}
