namespace DiginsightCopilotApi.Configuration;

public class AzureDevopsOptions
{
    public string OrgName { get; set; }
    public string PAT { get; set; }
    public string Project { get; set; }
    public string Repository { get; set; }
    public string Branch { get; set; }
    public string Environment { get; set; }
    public string BuildNumber { get; set; }
    public string BuildID { get; set; }
    public string SiteName { get; set; }
}

