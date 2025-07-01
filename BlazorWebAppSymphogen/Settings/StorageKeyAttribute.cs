namespace BlazorWebAppSymphogen.Settings;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class StorageKeyAttribute(string key) : Attribute
{
    public string Key { get; } = key;
}
