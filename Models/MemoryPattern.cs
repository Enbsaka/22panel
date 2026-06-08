namespace ascendedAuth_v2.Models
{
    public class MemoryPattern
    {
        public string Search { get; set; }
        public string Replace { get; set; }

        public MemoryPattern(string search, string replace)
        {
            Search = search;
            Replace = replace;
        }
    }
}
