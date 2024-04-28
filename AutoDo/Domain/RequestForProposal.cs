namespace AutoDo.Domain
{
    public class RequestForProposal
    {
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime Deadline { get; set; }
        public string Workplace { get; set; }
        public string Description { get; set; }
        public string Experience { get; set; }
        public string Skills { get; set; }
        public string URL { get; set; }
        public string Relevance { get; set; }
        public int? Priority { get; set; }
    }
}
