using Eduhunt.Data;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenAI.Chat;
using System.ClientModel;
using Eduhunt.DTOs;
using EDUHUNT_BE.Models;
using System.Text;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Eduhunt.Infrastructures.Repositories;
using Microsoft.AspNetCore.Hosting.Server;
using System;

public class Url
{
    public string URL { get; set; }
    public int ParentPage { get; set; }
    public string CssSelector { get; set; }
}

public class ScholarshipRepository : IScholarship
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _apiKey;

    public ScholarshipRepository(ApplicationDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _apiKey = configuration["ChatGPT:Key"];
    }

    public async Task<List<Scholarship>> GetRecommendedScholarships(string userId)
    {
            return new List<Scholarship>();
    }

    private async Task<List<Scholarship>> RankScholarshipsWithChatGPT(SurveyDto userSurvey, IEnumerable<Scholarship> allScholarships)
    {
        const int batchSize = 50;
        var batches = allScholarships.Select((s, i) => new { Scholarship = s, Index = i })
                                     .GroupBy(x => x.Index / batchSize)
                                     .Select(g => g.Select(x => x.Scholarship).ToList())
                                     .ToList();

        var resultList = new ConcurrentBag<List<Scholarship>>();

        await Parallel.ForEachAsync(batches, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, async (batch, token) =>
        {
            var result = await ProcessBatch(userSurvey, batch);
            resultList.Add(result);
        });

        return resultList.SelectMany(x => x).ToList();
    }

    private async Task<List<Scholarship>> ProcessBatch(SurveyDto userSurvey, List<Scholarship> scholarshipBatch)
    {

        string prompt = PreparePromptForChatGPT(userSurvey, scholarshipBatch);
        ChatClient client = new(model: "gpt-4-1106-preview", new ApiKeyCredential(_apiKey));
        ChatCompletion completion = await client.CompleteChatAsync(prompt);
        string response = completion.ToString();
        var result = ParseChatGPTResponse(response, scholarshipBatch);

        return result;
    }

    private string PreparePromptForChatGPT(SurveyDto userSurvey, List<Scholarship> scholarships)
    {
        StringBuilder promptBuilder = new StringBuilder();
        promptBuilder.AppendLine("Based on the following user survey and list of scholarships, please rank the scholarships from most suitable to least suitable. Return the result as a comma-separated list of scholarship IDs in order of suitability.");

        promptBuilder.AppendLine("\nUser Survey:");
        foreach (var answer in userSurvey.SurveyAnswers)
        {
            promptBuilder.AppendLine($"- {answer.Question}: {answer.Answer}");
        }

        promptBuilder.AppendLine("\nScholarships:");
        foreach (var scholarship in scholarships)
        {
            promptBuilder.AppendLine($"- ID: {scholarship.Id}");
            promptBuilder.AppendLine($"  Title: {scholarship.Title}");
            promptBuilder.AppendLine($"  Budget: {scholarship.Budget}");
            promptBuilder.AppendLine($"  Location: {scholarship.Location}");
            promptBuilder.AppendLine($"  School: {scholarship.SchoolName}");
            promptBuilder.AppendLine($"  Level: {scholarship.Level}");
            promptBuilder.AppendLine();
        }

        return promptBuilder.ToString();
    }

    private List<Scholarship> ParseChatGPTResponse(string response, List<Scholarship> scholarshipBatch)
    {
        var scholarshipIds = response.Split(',').Select(id => id.Trim());
        var scholarshipDict = scholarshipBatch.ToDictionary(s => s.Id.ToString());
        return scholarshipIds
            .Where(id => scholarshipDict.ContainsKey(id))
            .Select(id => scholarshipDict[id])
            .ToList();
    }

    public async Task<List<ScholarshipDto>> GetScholarships()
    {
        List<Url> scholarshipLinks = new List<Url>
        {
            new Url { URL = "https://www.scholarshipportal.com/", ParentPage = 1, CssSelector = "main" },
            new Url { URL = "https://www.rmit.edu.vn/study-at-rmit/scholarships/international-student-scholarships", ParentPage = 1, CssSelector = ".body-gridcontent.responsivegrid" },
            new Url { URL = "https://www.rmit.edu.vn/study-at-rmit/scholarships/future-undergraduate-student-scholarships", ParentPage = 1, CssSelector = ".body-gridcontent.responsivegrid" }
        };

        using (var driver = new ChromeDriver())
        {
            SearchGoogle(driver);
            var scholarships = await Scrapingdata(driver, scholarshipLinks);

            // Add logic to save scholarships to the database if needed
            // ...

            return scholarships;
        }
    }






    public Task AddScholarship(ScholarshipDto scholarship)
    {
        // Implement logic to add scholarship to your data source (e.g., database)
        // ...

        // Return a completed task since there's no asynchronous operation
        return Task.CompletedTask;
    }

    private List<ScholarshipDto> ParseHtml(string html)
    {
        // Implement logic to parse the HTML and extract scholarship information
        // ...

        // For illustration purposes, return an empty list
        return new List<ScholarshipDto>();
    }

    private void SearchGoogle(ChromeDriver driver)
    {
        try
        {
            driver.Navigate().GoToUrl("https://www.google.com/");
            IWebElement textarea = driver.FindElement(By.ClassName("gLFyf"));
            //fill in textarea that " scholarshipportal "
            textarea.SendKeys("scholarshipportal");
            //enter
            textarea.SendKeys(Keys.Enter);
            //wait for 2 seconds
            Thread.Sleep(2000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private async Task<List<ScholarshipDto>> Scrapingdata(ChromeDriver driver, List<Url> scholarshipLinks)
    {
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--ignore-certificate-errors-spki-list");
        chromeOptions.AddArgument("--ignore-certificate-errors");

        List<ScholarshipDto> scholarships = new List<ScholarshipDto>();
        try
        {
            List<string> scholarshipDivs = new List<string>();

            foreach (Url url in scholarshipLinks)
            {

            }

            foreach (Url urls in scholarshipLinks)
            {
                scholarshipDivs = (await ProcessData(driver, urls.URL, urls.ParentPage, urls.CssSelector));
                scholarships.AddRange(await ScrapingProcessedData(driver, scholarshipDivs, urls.CssSelector));
            }

            // For illustration purposes, return an empty list
            return scholarships;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return new List<ScholarshipDto>();
        }
        finally
        {
            // This will only close the main window, not the additional tabs
            driver.Quit();
        }
    }

    private string Content(string completions, string title)
    {
        var index = completions.IndexOf(title);
        var mainIndex = completions.IndexOf(':', index) + 1;
        var length = completions.IndexOf(';', index) - mainIndex;

        return completions.Substring(mainIndex, length);
    }

    private async Task<List<ScholarshipDto>> ScrapingProcessedData(ChromeDriver driver, List<string> scholarshipDivs, string css)
    {
        List<ScholarshipDto> scholarships = new List<ScholarshipDto>();
        var count = 0;
        foreach (var scholarshipDiv in scholarshipDivs)
        {
            var lastDivs = scholarshipDivs.Last();
            var scholarshipUrl = scholarshipDiv;

            ((IJavaScriptExecutor)driver).ExecuteScript($"window.open('{scholarshipUrl}', '_blank');");

            driver.SwitchTo().Window(driver.WindowHandles[^1]);

            var mainElement = driver.FindElement(By.CssSelector(css));
            Console.WriteLine($"css: {css} {scholarshipUrl}");

            count++;
            if (count == 3 && lastDivs != scholarshipDiv)
            {
                await Task.Delay(60000);
                count = 0;
            }

            driver.Close();

            driver.SwitchTo().Window(driver.WindowHandles[0]);
        }
        return scholarships;
    }

    private async Task<List<string>> ProcessData(ChromeDriver driver, string urls, int numberOfParentPage, string css)
    {
        List<string> scholarshipDivs = new List<string>();
        string scholarshipUrls = urls;

        driver.Navigate().GoToUrl(scholarshipUrls);

        await Task.Delay(1000);

        // Find all div elements with class "flex md:w-1/3"

        Uri uri = new Uri(scholarshipUrls);
        string domain = uri.Host;
        string completions = "";

        string html = driver.FindElement(By.CssSelector(css)).GetAttribute("outerHTML");

        while (true)
        {
            try
            {
                string prompt = "I'm a software developer." +
                $" I need you to get the data from a scholarship page by reading the html code of the page: {html}." +
                " Please read those pages contain the list of links to scholarship." +
                $" Get for me all the url of the scholarship (add the domain {domain}) contained in this page." +
                " Return it as a string follow the format: https://url1, https://url2,...." +
                " Not answer anything else" +
                " so that I can easily to manipulate the information.";

                /*var openAI = new OpenAIAPI("");
                var chat = openAI.Chat.CreateConversation();
                chat.Model = OpenAI_API.Models.Model.GPT4_Turbo;
                chat.AppendUserInput(prompt);
                completions = await chat.GetResponseFromChatbotAsync();*/

                ChatClient client = new(model: "gpt-4-1106-preview", new ApiKeyCredential(_apiKey));
                ChatCompletion completion = client.CompleteChat(prompt);
                completions = completion.ToString();
                break;
            }
            catch
            {
                await Task.Delay(60000);
            }
        }

        string[] temp = completions.Split(", ");
        foreach (string scholarship in temp)
        {
            scholarshipDivs.Add(scholarship);
        }

        return scholarshipDivs;
    }
}

