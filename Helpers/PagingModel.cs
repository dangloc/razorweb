using System;

namespace DBL.Helpers 
{
    public class PagingModel {
        public int currentPage { get;set; }
        public int CountPages { get; set; }

        public Func<int?, string> GenerateUrl { get; set; }
    }
}