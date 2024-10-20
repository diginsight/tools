namespace DiginsightCopilotApi.Models;

public class Approver
{
    public string displayName { get; set; }
    public string id { get; set; }
    public string uniqueName { get; set; }
    public string imageUrl { get; set; }
    public string descriptor { get; set; }
}

public class CreatedBy
{
    public string displayName { get; set; }
    public string id { get; set; }
    public string uniqueName { get; set; }
    public string descriptor { get; set; }
}

public class DefinitionRef
{
    public string id { get; set; }
}

public class Links
{
    public Self self { get; set; }
}

public class ModifiedBy
{
    public string displayName { get; set; }
    public string id { get; set; }
    public string uniqueName { get; set; }
    public string descriptor { get; set; }
}

public class Resource
{
    public string type { get; set; }
    public string id { get; set; }
    public string name { get; set; }
}

public class Root
{
    public int count { get; set; }
    public List<Value> value { get; set; }
}

public class Self
{
    public string href { get; set; }
}

public class Settings
{
    public List<Approver> approvers { get; set; }
    public string executionOrder { get; set; }
    public DefinitionRef definitionRef { get; set; }
    public int minRequiredApprovers { get; set; }
    public string instructions { get; set; }
    public List<object> blockedApprovers { get; set; }
}

public class Type
{
    public string id { get; set; }
    public string name { get; set; }
}

public class Value
{
    public Settings settings { get; set; }
    public CreatedBy createdBy { get; set; }
    public DateTime createdOn { get; set; }
    public ModifiedBy modifiedBy { get; set; }
    public DateTime modifiedOn { get; set; }
    public int timeout { get; set; }
    public Links _links { get; set; }
    public int id { get; set; }
    public int version { get; set; }
    public Type type { get; set; }
    public string url { get; set; }
    public Resource resource { get; set; }
}

public class ChangeParam
{
    public string Id { get; set; }
    public string Author { get; set; }
    public string AuthorId { get; set; }
    public string AuthorUniqueName { get; set; }
    public string DisplayUriAbsolutePath { get; set; }
    public string DisplayUriAbsoluteUri { get; set; }
    public string DisplayUriOriginalString { get; set; }
    public string Location { get; set; }
    public string Message { get; set; }
    public DateTime? Timestamp { get; set; }
}

public class WorkItemParam
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string AcceptanceCriteria { get; set; }
}
public class HttpRequestHeader
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class AssemblyMetadata
{
    public string Name { get; set; }
    public string Value { get; set; }
}

