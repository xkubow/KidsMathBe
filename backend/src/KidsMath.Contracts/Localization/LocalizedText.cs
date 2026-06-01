namespace KidsMath.Contracts.Localization;

public record LocalizedText(string Cs, string En)
{
    public string For(string lang) => lang.StartsWith("en", StringComparison.OrdinalIgnoreCase) ? En : Cs;
}
