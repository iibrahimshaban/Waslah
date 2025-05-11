namespace Waslah.Helppers;

public static class EmailBodyBuilder
{
    public static string GenerateEmailBody(string templateName, Dictionary<string, string> templateModel)
    {
        var TemplatePath = $"{Directory.GetCurrentDirectory()}/Templates/{templateName}.html";

        var streamReader = new StreamReader(TemplatePath);
        var body = streamReader.ReadToEnd();
        streamReader.Close();

        foreach (var item in templateModel)
            body = body.Replace(item.Key, item.Value);

        return body;
    }
}
