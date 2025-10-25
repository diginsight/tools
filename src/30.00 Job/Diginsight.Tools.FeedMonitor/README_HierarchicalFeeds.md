# Hierarchical Feed Support

## Overview

The Feed Monitor now supports hierarchical feed structures where the same item can appear in multiple feeds (e.g., a global feed and specialized category feeds). The system tracks which feeds expose each item and stores this information for analysis.

## Key Features

### 1. **Multi-Feed Source Tracking**
Every feed item now tracks:
- **FeedSources**: List of feed URIs where the item was found
- **FeedSourceDetails**: Detailed metadata about each feed source including:
  - Feed URI and title
  - Category path in the hierarchy
  - First discovered and last seen timestamps
  - Feed-specific categories

### 2. **Deduplication**
Items are uniquely identified by their `Guid`. When the same item appears in multiple feeds:
- It's stored only once in the database
- The feed source metadata is merged/updated
- All feed sources are tracked in the item

### 3. **Feed Hierarchy Configuration**

Configure feeds with hierarchy information in `appsettings.json`:

```json
{
  "FeedMonitor": {
    "Feeds": [
      {
        "Uri": "https://azure.microsoft.com/en-us/blog/feed",
        "CategoryPath": "global",
   "ParentFeedUri": null,
        "IsPrimarySource": true
     },
      {
        "Uri": "https://azure.microsoft.com/en-us/blog/content-type/announcements/feed",
  "CategoryPath": "announcements",
 "ParentFeedUri": "https://azure.microsoft.com/en-us/blog/feed",
   "IsPrimarySource": false
      }
    ]
  }
}
```

#### Configuration Properties

| Property | Type | Description |
|----------|------|-------------|
| `Uri` | string | The feed URL to monitor |
| `CategoryPath` | string? | Category path for hierarchical organization (e.g., "announcements", "updates/security") |
| `ParentFeedUri` | string? | URI of the parent feed if this is a child feed |
| `IsPrimarySource` | bool | Whether this feed is the canonical/primary source for its items (default: true) |

## Data Model

### FeedItemBase Properties

```csharp
public abstract class FeedItemBase: EntityBase
{
    // ...existing properties...

    /// <summary>
 /// List of feed URIs where this item was found
    /// </summary>
    public List<string> FeedSources { get; set; } = new List<string>();

    /// <summary>
    /// Metadata about each feed source
    /// Key: Feed URI, Value: Feed metadata
    /// </summary>
    public Dictionary<string, FeedSourceMetadata> FeedSourceDetails { get; set; } 
        = new Dictionary<string, FeedSourceMetadata>();
}
```

### FeedSourceMetadata

```csharp
public class FeedSourceMetadata
{
    public string FeedUri { get; set; }
    public string FeedTitle { get; set; }
    public string FeedId { get; set; }
    public string CategoryPath { get; set; }
    public DateTimeOffset FirstDiscovered { get; set; }
    public DateTimeOffset LastSeen { get; set; }
    public List<string> FeedCategories { get; set; }
}
```

## Example Use Cases

### 1. Azure Blog Hierarchy

```json
{
  "Feeds": [
    {
      "Uri": "https://azure.microsoft.com/en-us/blog/feed",
      "CategoryPath": "global",
      "IsPrimarySource": true
    },
    {
      "Uri": "https://azure.microsoft.com/en-us/blog/content-type/announcements/feed",
      "CategoryPath": "announcements",
      "ParentFeedUri": "https://azure.microsoft.com/en-us/blog/feed"
    },
    {
      "Uri": "https://azure.microsoft.com/en-us/blog/content-type/updates/feed",
      "CategoryPath": "updates",
     "ParentFeedUri": "https://azure.microsoft.com/en-us/blog/feed"
    }
  ]
}
```

**Result**: 
- An announcement item appears in both the global feed and announcements feed
- It's stored once with:
  - `FeedSources`: `["https://azure.microsoft.com/en-us/blog/feed", "https://azure.microsoft.com/en-us/blog/content-type/announcements/feed"]`
  - `FeedSourceDetails`: Contains metadata for both feeds with their category paths

### 2. Product-Specific Feeds

```json
{
  "Feeds": [
    {
"Uri": "https://devblogs.microsoft.com/feed/",
      "CategoryPath": "global",
      "IsPrimarySource": true
    },
    {
      "Uri": "https://devblogs.microsoft.com/dotnet/feed/",
      "CategoryPath": "dotnet",
      "ParentFeedUri": "https://devblogs.microsoft.com/feed/"
    },
    {
      "Uri": "https://devblogs.microsoft.com/azure-sdk/feed/",
      "CategoryPath": "azure-sdk",
      "ParentFeedUri": "https://devblogs.microsoft.com/feed/"
    }
]
}
```

## Storage Implementation

### CosmosDB
- Items are uniquely identified by `Guid` (id) and `PartitionKey`
- When an existing item is encountered from a different feed:
  - The existing document is read
  - `FeedSources` array is merged
  - `FeedSourceDetails` dictionary is merged/updated
  - Document is upserted with combined metadata

### Table Storage
- Items stored with `FeedSources` as semicolon-separated string
- `FeedSourceDetailsJson` contains serialized JSON of the metadata dictionary
- Upsert mode ensures updates to existing items

### Blob Storage
- Each item stored as a folder with:
  - `metadata.json` - Contains feed source tracking
  - `content.md` - Full content
  - `description.md` - Summary/description
- Metadata includes `FeedSources` and `FeedSourceDetails`

## Query Examples

### Find Items from Multiple Feeds

**CosmosDB Query**:
```sql
SELECT c.Guid, c.Title, c.FeedSources, c.FeedSourceDetails
FROM c
WHERE ARRAY_LENGTH(c.FeedSources) > 1
```

### Find Items by Category Path

**CosmosDB Query**:
```sql
SELECT c.Guid, c.Title, c.FeedSourceDetails
FROM c
JOIN f IN c.FeedSourceDetails
WHERE f.CategoryPath = "announcements"
```

### Track Feed Coverage

```sql
SELECT 
    c.Guid,
    c.Title,
    ARRAY_LENGTH(c.FeedSources) as FeedCount,
    c.FeedSources
FROM c
ORDER BY ARRAY_LENGTH(c.FeedSources) DESC
```

## Benefits

1. **Comprehensive Tracking**: Know exactly which feeds expose each item
2. **Deduplication**: Store each unique item only once, regardless of how many feeds it appears in
3. **Category Analysis**: Understand item categorization across different feeds
4. **Timeline Tracking**: See when items first appeared and were last seen in each feed
5. **Feed Relationships**: Map hierarchical feed structures
6. **Storage Efficiency**: Avoid duplicate storage of the same content

## Best Practices

1. **Mark Primary Sources**: Set `IsPrimarySource: true` for canonical feeds
2. **Use Descriptive Paths**: Make `CategoryPath` meaningful (e.g., "product/announcements/security")
3. **Parent-Child Relationships**: Always set `ParentFeedUri` for child feeds
4. **Monitor Feed Coverage**: Regularly check which items appear in multiple feeds
5. **Retention Policies**: Use `FeedSourceDetails.LastSeen` for cleanup decisions

## Migration from Previous Version

Existing items without feed source tracking will automatically get the tracking fields populated when re-processed:
- `FeedSources` will contain the current feed URI
- `FeedSourceDetails` will be initialized with current feed metadata
- Subsequent runs will add additional feeds as they're discovered

No manual migration is required - the system handles it transparently.
