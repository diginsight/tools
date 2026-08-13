# Reference Entry Template

Use this format for all references in article Reference sections.

## Standard Format

```markdown
**[Title](url)** `[📘 Official]`  
Description (2-4 sentences): what it covers, why valuable, when to use it.
```

## Classification Markers

| Marker | Type | When to Use |
|--------|------|-------------|
| `[📘 Official]` | Official | Microsoft domains, GitHub docs, official product sites |
| `[📗 Verified Community]` | Verified | GitHub blog, DevBlogs, academic sources, reputable tech sites |
| `[📒 Community]` | Community | Medium, Dev.to, personal blogs, tutorials |
| `[📕 Unverified]` | Unverified | Broken links, unknown sources (fix before publishing) |

## Domain Classification Guide

### 📘 Official Sources

- `*.microsoft.com`
- `learn.microsoft.com`
- `docs.github.com`
- `code.visualstudio.com`
- Official product documentation sites

### 📗 Verified Community Sources

- `github.blog`
- `devblogs.microsoft.com`
- `azure.microsoft.com/blog`
- Academic papers (arxiv.org, IEEE, ACM)
- Major tech company engineering blogs

### 📒 Community Sources

- `medium.com`
- `dev.to`
- `hashnode.com`
- Personal developer blogs
- YouTube tutorials
- Stack Overflow answers

### 📕 Unverified Sources

- Broken or dead links
- Unknown domains
- Outdated documentation
- Sites with security warnings

## Examples

### Official Documentation

```markdown
**[VS Code: Agent Skills](https://code.visualstudio.com/docs/copilot/customization/agent-skills)** `[📘 Official]`  
Microsoft's official documentation for Agent Skills in VS Code. Covers the complete skill structure, YAML frontmatter requirements, and resource organization. Essential reference for understanding how VS Code processes skills.
```

### Verified Community

```markdown
**[GitHub Blog: Copilot Best Practices](https://github.blog/developer-skills/github/how-to-use-github-copilot/)** `[📗 Verified Community]`  
Official GitHub blog post on maximizing Copilot effectiveness through context management. Provides insights on how context impacts suggestion quality and explains key principles for instruction design.
```

### Community Resource

```markdown
**[Building Custom Copilot Agents](https://dev.to/author/custom-copilot-agents)** `[📒 Community]`  
Developer tutorial walking through agent creation step-by-step. Includes practical examples and common pitfalls. Good for beginners but verify patterns against official docs.
```

## Organization Guidelines

Group references by category:

```markdown
## References

### Official Documentation

[Official sources here]

### Community Resources  

[Verified and community sources here]

### Related Articles

[Internal article links here]
```

Order within each group:
1. By authority (most authoritative first)
2. By relevance to article topic
3. Alphabetically if equal weight
