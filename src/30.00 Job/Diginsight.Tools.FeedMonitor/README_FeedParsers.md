# Feed Parser Base Classes

This document describes the refactored feed parser architecture that provides a unified interface for parsing both RSS and Atom feeds.

## Architecture

### Base Classes

- **`FeedParserBase`**: Abstract base class that defines the interface for all feed parsers
- **`FeedChannelBase`**: Base model class for feed channels
- **`FeedItemBase`**: Base model class for feed items

### Concrete Implementations

- **`RSSFeedParser`**: Handles RSS 2.0 feeds with iTunes extensions
- **`AtomFeedParser`**: Handles Atom feeds (RFC 4287)

### Factory Classes

- **`IFeedParserFactory`**: Interface for feed parser factory service
- **`FeedParserFactory`**: Implementation that provides factory methods for creating parsers and parsing feeds

## Dependency Injection Setup

### Service Registration

```csharp
// In Program.cs or Startup.cs
services.AddFeedParsing();

// Or register manually:
services.AddSingleton<IFeedParserFactory, FeedParserFactory>();
```

### Constructor Injection

```csharp
public class MyService
{
    private readonly IFeedParserFactory feedParserFactory;

    public MyService(IFeedParserFactory feedParserFactory)
    {
        this.feedParserFactory = feedParserFactory;
    }

    public async Task ProcessFeedAsync(string feedUrl)
    {
        using var httpClient = new HttpClient();
        var xmlContent = await httpClient.GetStringAsync(feedUrl);
        
        // Use the injected factory
        var feed = feedParserFactory.ParseFeed(xmlContent);
        
        // Process the feed...
    }
}
```

## Usage Examples

### Approach 1: Direct Factory Usage (Recommended with DI)

```csharp
// Inject IFeedParserFactory in your constructor
public class FeedService
{
    private readonly IFeedParserFactory feedParserFactory;
    
    public FeedService(IFeedParserFactory feedParserFactory)
    {
        this.feedParserFactory = feedParserFactory;
    }
    
    public FeedChannelBase ParseFeed(string xmlContent)
    {
        // One-step parsing using factory
        return feedParserFactory.ParseFeed(xmlContent);
    }
}
```

### Approach 2: Two-Step Parser Creation

```csharp
// Get parser first, then parse
var parser = feedParserFactory.GetParser(xmlContent);
var feed = parser.ParseFeed(xmlContent);

// Or detect type first
var feedType = feedParserFactory.DetectFeedType(xmlContent);
var parser = feedParserFactory.CreateParser(feedType);
var feed = parser.ParseFeed(xmlContent);
```

### Approach 3: Legacy Direct Parser Instantiation

```csharp
// Still supported for backward compatibility, but not recommended
var doc = XDocument.Parse(xmlContent);
bool isRss = doc.Root.Name.LocalName.Equals("rss", StringComparison.OrdinalIgnoreCase);
bool isAtom = doc.Root.Name.LocalName.Equals("feed", StringComparison.OrdinalIgnoreCase);

if (!isRss && !isAtom) 
{
    throw new InvalidOperationException("Unknown feed format. Expected RSS or Atom.");
}

var feedParser = isRss ? (FeedParserBase)new RSSFeedParser() : new AtomFeedParser();
var feed = feedParser.ParseFeed(xmlContent);
```

## Working with Different Feed Types

```csharp
public class FeedMonitorService
{
    private readonly IFeedParserFactory feedParserFactory;
    
    public FeedMonitorService(IFeedParserFactory feedParserFactory)
    {
        this.feedParserFactory = feedParserFactory;
    }
    
    public async Task ProcessFeedAsync(string feedUrl)
    {
        var feed = feedParserFactory.ParseFeed(xmlContent);

        // Type-specific operations
        if (feed is RSSFeedChannel rssFeed)
        {
            Console.WriteLine($"RSS Feed: {rssFeed.Title}");
            Console.WriteLine($"WebSub Hub: {rssFeed.WebSubHub ?? "Not available"}");
            Console.WriteLine($"Managing Editor: {rssFeed.ManagingEditor}");
        }
        else if (feed is AtomFeedChannel atomFeed)
        {
            Console.WriteLine($"Atom Feed: {atomFeed.Title}");
            Console.WriteLine($"WebSub Hub: {atomFeed.WebSubHub ?? "Not available"}");
            Console.WriteLine($"Last Updated: {atomFeed.LastUpdated}");
            Console.WriteLine($"Authors: {string.Join(", ", atomFeed.Authors.Select(a => a.Name))}");
        }
    }
}
```

## Benefits of the New Architecture

1. **Dependency Injection**: Factory is registered as a singleton and injected where needed
2. **Testability**: Easy to mock `IFeedParserFactory` for unit tests
3. **Polymorphism**: Work with feeds through a common interface
4. **Extensibility**: Easy to add new feed formats by implementing `FeedParserBase`
5. **Type Safety**: Strong typing with specific models for RSS and Atom
6. **Consistent API**: Same methods work for both RSS and Atom feeds
7. **Service Lifetime Management**: Proper singleton lifetime for the factory

## Parser Capabilities

### RSS Parser Features
- RSS 2.0 specification support
- iTunes podcast extensions
- WebSub (PubSubHubbub) support
- Media enclosures
- Categories and metadata

### Atom Parser Features
- Atom 1.0 (RFC 4287) support
- Multiple authors and contributors
- Rich link relationships
- Content types (text, HTML, XHTML)
- WebSub support

## Migration Guide

### From Static Methods to DI-Based Factory

#### Old Approach (Static)
```csharp
// Old static method calls
var rssFeed = RSSFeedParser.ParseRSS(xmlContent);
var atomFeed = AtomFeedParser.ParseAtom(xmlContent);
```

#### New Approach (DI-Based)
```csharp
// Register services
services.AddFeedParsing();

// Inject and use
public MyService(IFeedParserFactory factory)
{
    this.factory = factory;
}

// Use in methods
var feed = factory.ParseFeed(xmlContent);
```

### Service Registration Options

```csharp
// Option 1: Use extension method (Recommended)
services.AddFeedParsing();

// Option 2: Manual registration
services.AddSingleton<IFeedParserFactory, FeedParserFactory>();
```

The factory is registered as a singleton to ensure efficient reuse across the application lifetime, as it's stateless and thread-safe.