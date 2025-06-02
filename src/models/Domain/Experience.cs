using System.Runtime.Serialization;

public enum Experience
{
    [EnumMember(Value = "Unspecified")]
    Unspecified,
    [EnumMember(Value = "Junior")]
    Junior,

    [EnumMember(Value = "Medior")]
    Medior,

    [EnumMember(Value = "Senior")]
    Senior
}
    