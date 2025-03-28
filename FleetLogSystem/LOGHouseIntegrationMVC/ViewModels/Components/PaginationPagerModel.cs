namespace LOGHouseSystem.ViewModels.Components
{
    public class PaginationPagerModel
    {
        public int PageCount { get; set; }

        public int Page { get; set; }

        public string FormToSubmit { get; set; }

        public string InputIdForPage { get; set; }

        public bool ShowSearch { get; set; } = true;
    }
}
