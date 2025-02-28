
public class Profil
{
    public string Uuid { get; set; }
    public int Ratehour { get; set; }
    public string CV { get; set; }
    public DateTime CV_Date { get; set; }
    public string Job_title { get; set; }


    public Experience Experience_level { get; set; }


    public List<String> Skills { get; set; }

    
    public List<String> Keywords { get; set; }
}
