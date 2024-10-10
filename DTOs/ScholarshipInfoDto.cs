namespace Eduhunt.DTOs
{
    public class ScholarshipInfoDto
    {
        public required string Id { get; set; }

        public string Title { get; set; } = default!;

        public string Budget { get; set; } = default!;

        public string Location { get; set; } = default!;

        public string School_name { get; set; } = default!;

        public string Level { get; set; } = default!;

        public string Url { get; set; } = default!;
    }
}
