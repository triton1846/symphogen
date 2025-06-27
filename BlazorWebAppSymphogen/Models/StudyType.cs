namespace BlazorWebAppSymphogen.Models;

[Serializable]
public class StudyType : ICloneable
{
    public required string Key { get; set; }
    public required string InputType { get; set; }

    public object Clone()
    {
        return new StudyType
        {
            Key = Key,
            InputType = InputType
        };
    }
}
