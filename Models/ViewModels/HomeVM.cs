using EventsClients.Models;

namespace EventsClient.Models.ViewModels
{
    public class HomeVM
    {
        public IQueryable<EvHeader> Data { get; set; }

        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string Term { get; set; }

        public IEnumerable<EvHeader> Events { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
