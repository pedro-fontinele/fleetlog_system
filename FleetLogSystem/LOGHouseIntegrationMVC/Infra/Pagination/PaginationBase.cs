using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Infra.Pagination
{
    public class PaginationBase<T> 
    {
        public List<T> Data { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRegisters { get; set; }
        public int FirstRegisterInActualPage { get; set; }
        public int LastRegisterInActualPage { get; set; }


    }
}
