using Eduhunt.Applications.Scholarships;
using Eduhunt.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Eduhunt.Infrastructures.Common;
using Microsoft.AspNetCore.Mvc;
using Eduhunt.Applications.Questions;
using Eduhunt.Applications.Surveys;
using Eduhunt.DTOs;

namespace Eduhunt.Pages
{
    public class IndexModel : PageModel
    {
        public IQueryable<Scholarship> NewScholarships = default!;
        public IEnumerable<Scholarship> TopScholarships = default!;
        public IQueryable<Question> AllQuestions = default!;
        public Survey Survey = default!;
        public bool isModalOpen = false;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        [BindProperty]
        public List<string> Answers { get; set; } = new List<string>();

        public IndexModel(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
        {
            _serviceProvider = serviceProvider;
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            var scholarshipService = _serviceProvider.GetService<ScholarshipService>();
            var commonService = _serviceProvider.GetService<CommonService>();
            var questionService = _serviceProvider.GetService<QuestionService>();
            var surveyService = _serviceProvider.GetService<SurveyService>();


            if (scholarshipService != null && commonService != null && questionService != null && surveyService != null)
            {
                var allScholarship = scholarshipService.GetAll();
                Random random = new Random();
                NewScholarships = allScholarship.OrderByDescending(x => x.CreatedAt);
                TopScholarships = allScholarship.ToList().OrderBy(x => x.Budget).Take(4);
                string userID = await commonService.GetUserId();
                if (commonService.IsUserLoggedIn() && await surveyService.GetByUserId(userID) == null)
                {
                    isModalOpen = true;
                    AllQuestions = questionService.GetAllQuestionOption();
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var commonService = _serviceProvider.GetService<CommonService>();
            var surveyService = _serviceProvider.GetService<SurveyService>();
            if (Answers != null && commonService != null && surveyService != null)
            {
                try
                {
                    string userID = await commonService.GetUserId();
                    SurveyAnswerDto newSurveyAnswer = new SurveyAnswerDto()
                    {
                        Title = "Default Title",
                        Description = "Default Description",
                        AnswerIds = Answers,
                        CreateAt = DateTime.Now,
                        UserId = userID,
                    };
                    await surveyService.AddSurvey(newSurveyAnswer);
                } catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            } else
            {
                ModelState.AddModelError(string.Empty, "Error: Not full question answered.");
            }
            return Redirect("/");
        }
    }
}
