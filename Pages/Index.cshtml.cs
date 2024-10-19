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

        public void OnGet(string SchoolName, string Country, string City, string Major, string Level, string Budget)
        {
            var scholarship = _serviceProvider.GetService<ScholarshipService>();
            if (scholarship != null)
            {
                var allScholarship = scholarship.GetAll();
                NewScholarships = allScholarship.OrderByDescending(x => x.CreatedAt);
                Random random = new Random();
                TopScholarships = allScholarship.ToList().OrderBy(x => x.Budget)
                                .Take(4);
                //search
                if (NewScholarships != null)
                {
                    if (!string.IsNullOrEmpty(SchoolName))
                    {
                        NewScholarships = NewScholarships.Where(x => x.SchoolName != null && x.SchoolName.ToLower().Contains(SchoolName.ToLower()));
                    }

                    if (!string.IsNullOrEmpty(Country) || !string.IsNullOrEmpty(City))
                    {
                        NewScholarships = NewScholarships.Where(x => x.Location != null && (x.Location.ToLower().Contains(Country.ToLower()) || x.Location.ToLower().Contains(City.ToLower())));
                    }

                    if (!string.IsNullOrEmpty(Level))
                    {
                        NewScholarships = NewScholarships.Where(x => x.Level != null && x.Level.Contains(Level.ToLower()));
                    }

                    if (!string.IsNullOrEmpty(Budget))
                    {
                        NewScholarships = NewScholarships.Where(x => x.Budget != null && x.Budget.Contains(Budget.ToLower()));
                    }
                }
            }
        }
    }
}
