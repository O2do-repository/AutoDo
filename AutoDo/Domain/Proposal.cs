namespace AutoDo.Domain
{
    public class Proposal
    {
        public DateTime SubmissionDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public float? ProposedRate { get; set; }
        public string  AuthorizationLetter { get; set; }
        public float CvO2Do { get; set; }
    }
}
