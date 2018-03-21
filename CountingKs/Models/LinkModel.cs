namespace CountingKs.Models
{
    public class LinkModel
    {
        public string Href { get; set; }
        public string Relation { get; set; }
        public string Method { get; set; }
        public bool IsTemplated { get; set; }
    }
}