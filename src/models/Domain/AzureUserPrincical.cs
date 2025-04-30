public class AzureUserPrincipal
{
    public string IdentityProvider { get; set; }
    public List<AzureClaim> Claims { get; set; }
}

public class AzureClaim
{
    public string Type { get; set; }
    public string Value { get; set; }
}
