public interface IEnterpriseService
{
    List<Enterprise> GetAllEnterprises();
    Enterprise AddEnterprise(Enterprise enterprise);
    void DeleteEnterprise(Guid enterpriseUuid);
}
