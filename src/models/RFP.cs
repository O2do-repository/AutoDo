namespace models{
    public class RFP
    {
        public DateTime DeadlineDate { get; set; }
        public string DescriptionBrut { get; set; }
        public string ExperienceLevel { get; set; }
        public string RfpPriority { get; set; }
        public DateTime PublicationDate { get; set; }
        public List<string> Skills { get; set; }
        public string JobTitle { get; set; }
        public string RfpUrl { get; set; }
        public string Workplace { get; set; }
        public string RfpId { get; set; }
    }
}


