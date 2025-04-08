public interface IConsultantService{
    List<Consultant> GetAllConsultants();
    Consultant AddConsultant(Consultant consultant);
    Consultant GetConsultantById(Guid consultantUuid);
    void DeleteConsultant(Guid consultantUuid);
}