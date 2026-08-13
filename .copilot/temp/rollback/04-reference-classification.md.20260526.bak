# Reference Classification System

## Overview

All article references must be classified with emoji markers to help readers assess source reliability and authority at a glance.

## Classification Markers

| Marker | Category | Trust Level | Use For |
|--------|----------|-------------|---------|
| 📘 | Official | Highest | Official product docs, Microsoft Learn, vendor documentation |
| 📗 | Verified Community | High | Official blogs, recognized experts, peer-reviewed content |
| 📒 | Community | Medium | Tutorials, personal blogs, StackOverflow, community guides |
| 📕 | Unverified | Low/None | Broken links, unknown sources, unverified claims |

## Classification Rules (Domain-Based)

### 📘 Official Sources

**Domain Patterns:**
- `*.microsoft.com` - All Microsoft official domains
- `learn.microsoft.com` - Microsoft Learn documentation
- `docs.github.com` - GitHub official documentation
- `code.visualstudio.com/docs` - VS Code official docs
- `azure.microsoft.com/docs` - Azure documentation
- `*.openai.com/docs` - OpenAI official documentation
- `*.python.org` - Python official documentation
- Official vendor documentation sites

**Examples:**
- Product documentation portals
- Official API references
- Vendor technical specifications
- Official getting started guides

### 📗 Verified Community Sources

**Domain Patterns:**
- `github.blog` - GitHub official blog
- `devblogs.microsoft.com` - Microsoft developer blogs
- Recognized expert personal domains (case-by-case)
- Academic sources (.edu domains with peer review)
- Official company engineering blogs

**Examples:**
- Official blog posts from vendors
- Technical articles by recognized experts
- Academic papers and research
- Well-established technical publication sites

### 📒 Community Sources

**Domain Patterns:**
- `medium.com` - Medium articles
- `dev.to` - Dev.to community posts
- Personal blogs and portfolio sites
- `stackoverflow.com` - Stack Overflow Q&A
- YouTube tutorials and recordings
- GitHub repositories (non-official)
- Community forums

**Examples:**
- Tutorial articles
- Personal blog posts
- Community guides
- Video tutorials
- Code examples and repositories

### 📕 Unverified Sources

**Criteria:**
- Broken or inaccessible links (404, timeout)
- Unknown or suspicious domains
- Content without clear authorship
- Questionable reliability or accuracy
- Paywalled content without verification

**Action Required:**
- Fix or remove broken links before publishing
- Verify and upgrade classification if possible
- Add warning notes if link must remain

## Special Classification Cases

### GitHub Repositories

**Classification depends on ownership:**

| Repository Owner | Classification | Example |
|-----------------|----------------|---------|
| `microsoft/*` | 📘 Official | microsoft/vscode, microsoft/TypeScript |
| `github/*` | 📘 Official | github/copilot-docs |
| `openai/*` | 📘 Official | openai/openai-python |
| Individual/community | 📒 Community | user/cool-project |

### Session Recordings

**Classification depends on host channel:**

| Host | Classification | Example |
|------|----------------|---------|
| Microsoft official channels | 📘 Official | Microsoft Build, Ignite sessions |
| GitHub official | 📘 Official | GitHub Universe talks |
| Community channels | 📒 Community | User-uploaded conference talks |

### Redirected Links

**Use final destination for classification:**

1. Follow redirect to final URL
2. Classify based on final destination domain
3. Prefer using final URL directly in references

**Example:**
- Short link: `bit.ly/abc123` → `learn.microsoft.com/article`
- Classification: 📘 Official (based on final destination)
- Prefer: Use `learn.microsoft.com/article` directly

### Mixed Content/Context-Based

**When domain alone is insufficient:**

**Scenario:** Official tutorial published on community site  
**Example:** Microsoft engineer's tutorial on Medium  
**Classification:** 📗 Verified Community  
**Reason:** Combine author authority + platform context

**Scenario:** Community guide to official product  
**Example:** Unofficial VS Code tutorial  
**Classification:** 📒 Community  
**Reason:** Not from official source, despite covering official product

## Reference Organization

### Grouping Structure

Organize references into sections:

#### 1. Official Documentation (Required if applicable)
Only 📘 sources. Product docs, official guides, specifications.

```markdown
## References

### Official Documentation

**[VS Code Extension API](https://code.visualstudio.com/api)** `[📘 Official]`  
Complete official API reference for VS Code extensions...
```

#### 2. Session Materials (Optional - for event summaries)
Mixed classifications based on host. Recording links, slides, repos.

```markdown
### Session Materials

**[Build 2024 Session Recording](https://youtube.com/microsoft/...)** `[📘 Official]`  
Official session recording covering AI features...

**[Session GitHub Repo](https://github.com/speaker/demo)** `[📒 Community]`  
Demo code from session speaker...
```

#### 3. Community Resources (Required if applicable)
📗 and 📒 sources. Blogs, tutorials, community content.

```markdown
### Community Resources

**[Comprehensive Guide to Agents](https://dev.to/author/guide)** `[📒 Community]`  
Community tutorial with practical examples...
```

#### 4. Product-Specific (Optional - for multi-product articles)
When article covers multiple products, group by product.

```markdown
### VS Code Resources

**[VS Code Docs](https://code.visualstudio.com/docs)** `[📘 Official]`  
Official VS Code documentation...

### Visual Studio Resources

**[Visual Studio Docs](https://docs.microsoft.com/visualstudio)** `[📘 Official]`  
Official Visual Studio documentation...
```

### Ordering Within Groups

**Priority order:**
1. Most comprehensive/authoritative first
2. Official before community within mixed groups
3. Most relevant to article topic
4. Most recent (for time-sensitive content)

## Reference Format

### Required Elements

Each reference must include:

1. **Full title** (clickable link text)
2. **URL** (full, not shortened)
3. **Classification marker** in backticks
4. **Description** (2-4 sentences)

### Format Template

```markdown
**[Full Article/Resource Title](https://full-url.com/path)** `[📘 Official]`  
First sentence: What the resource covers. Second sentence: Why it's valuable or what makes it unique. Third sentence: When/why readers should reference it. Optional fourth sentence: Additional context or specific use cases.
```

### Description Guidelines

**First sentence:** Summarize what content covers  
**Second sentence:** Explain value/unique aspects  
**Third sentence:** When to use this reference  
**Fourth sentence (optional):** Additional context

**Example:**
```markdown
**[GitHub Copilot Chat API](https://docs.github.com/copilot/building-copilot-extensions/building-a-copilot-agent-for-your-copilot-extension)** `[📘 Official]`  
Complete official documentation for building Copilot chat agents including API endpoints, authentication, and message handling. Provides detailed examples and best practices from the GitHub team. Essential reference when implementing custom Copilot extensions or chat participants. Includes troubleshooting section and migration guides for existing extensions.
```

## Classification Examples by Type

### Official Documentation Examples

```markdown
**[VS Code Release Notes](https://code.visualstudio.com/updates/)** `[📘 Official]`  
Official Visual Studio Code monthly release notes with complete changelog including all features, bug fixes, and breaking changes. Essential reading for understanding new capabilities and staying current with VS Code evolution.

**[Azure Functions Documentation](https://learn.microsoft.com/azure/azure-functions/)** `[📘 Official]`  
Comprehensive official Microsoft documentation covering Azure Functions development, deployment, and best practices. Includes quickstarts, tutorials, and detailed API references for all supported languages.

**[Python Official Tutorial](https://docs.python.org/3/tutorial/)** `[📘 Official]`  
Official Python language tutorial from the Python Software Foundation covering core concepts and standard library usage. Authoritative source for Python language features and recommended patterns.
```

### Verified Community Examples

```markdown
**[How to Write Great AGENTS.md](https://github.blog/ai-and-ml/github-copilot/how-to-write-a-great-agents-md-lessons-from-over-2500-repositories/)** `[📗 Verified Community]`  
Official GitHub blog post analyzing patterns from 2,500+ repositories to identify best practices for agent file creation. Provides data-driven insights into what makes agent files effective and actionable recommendations based on real-world usage patterns.

**[The Missing Semester of Your CS Education](https://missing.csail.mit.edu/)** `[📗 Verified Community]`  
MIT course materials covering practical command-line tools and development workflows often missing from traditional CS curricula. Highly regarded academic resource with exercises and detailed explanations of shell scripting, version control, and debugging tools.
```

### Community Resources Examples

```markdown
**[Building Custom VS Code Agents Tutorial](https://dev.to/author/custom-agents)** `[📒 Community]`  
Community tutorial walking through custom agent creation with practical examples and code samples. Useful for developers getting started with agent customization beyond official documentation. Includes common pitfalls and troubleshooting tips from author's experience.

**[Stack Overflow: Copilot Agent Best Practices](https://stackoverflow.com/questions/12345/copilot-agents)** `[📒 Community]`  
Community discussion thread addressing common questions and sharing experiences with Copilot agent development. Multiple perspectives and real-world solutions to implementation challenges.
```

### Session Materials Examples

```markdown
### Session Materials

**[AI Dev Days 2024: Unified Agent Architecture](https://youtube.com/microsoft/ai-dev-days-2024)** `[📘 Official]`  
Full session recording from Microsoft AI Dev Days covering unified agent architecture across VS Code and GitHub Copilot. Includes all demos, architectural diagrams, and Q&A segments referenced in this article.

**[Session Demo Repository](https://github.com/microsoft/ai-dev-days-demos)** `[📘 Official]`  
Official demo code and samples from the AI Dev Days session showing agent implementation patterns. Contains working examples of custom agents, chat participants, and tool integration.

**[Speaker's Extended Tutorial](https://medium.com/@speaker/extended-tutorial)** `[📒 Community]`  
Extended tutorial by session speaker diving deeper into topics covered in the conference talk. Provides additional context and advanced patterns not covered in the timed session format.
```

## Quality Checklist

Before publishing, verify all references meet these criteria:

### Classification
- [ ] All URLs have emoji classification markers (📘/📗/📒/📕)
- [ ] Classifications match domain patterns correctly
- [ ] Special cases (repos, recordings, redirects) classified appropriately
- [ ] Mixed content uses context-based classification
- [ ] No broken links remain (all 📕 fixed or removed)

### Organization
- [ ] References grouped by category (Official / Session Materials / Community / Product-Specific)
- [ ] Groups ordered from most to least authoritative
- [ ] Within groups, most comprehensive sources listed first
- [ ] Official sources (`📘`) precede community sources in mixed groups

### Format
- [ ] Each reference has full title as link text
- [ ] Full URLs used (not shortened links)
- [ ] Each reference has 2-4 sentence description
- [ ] Descriptions explain WHAT content covers
- [ ] Descriptions explain WHY it's valuable
- [ ] Descriptions explain WHEN to use it

### Completeness
- [ ] All references mentioned in article text are included
- [ ] All code examples cite source references
- [ ] All claims/facts have supporting references
- [ ] Related articles are cross-referenced
- [ ] Prerequisites link to required background material

### Accuracy
- [ ] URLs tested and accessible
- [ ] Titles match actual page titles
- [ ] Descriptions accurately reflect content
- [ ] Classifications verified by checking actual domains
- [ ] Update dates noted for time-sensitive content

## Common Mistakes to Avoid

### ❌ Don't Do This

**Missing classification markers:**
```markdown
**[Title](url)**  
Description...
```

**Insufficient descriptions:**
```markdown
**[Title](url)** `[📘 Official]`  
Good resource.
```

**Wrong classification:**
```markdown
**[Community Blog Post](https://medium.com/...)** `[📘 Official]`
```

**Broken links not fixed:**
```markdown
**[Old Article](https://broken-link.com/404)** `[📕 Unverified]`  
Great article about topic... (Link is broken - should remove or fix)
```

### ✅ Do This Instead

**Proper classification and format:**
```markdown
**[GitHub Copilot Documentation](https://docs.github.com/copilot/)** `[📘 Official]`  
Comprehensive official GitHub Copilot documentation covering setup, customization, and all features. Essential reference for understanding Copilot capabilities and configuration options. Regularly updated with new features and best practices.
```

**Good descriptions:**
```markdown
**[Introduction to Prompt Engineering](https://learn.microsoft.com/ai/prompt-engineering)** `[📘 Official]`  
Microsoft Learn guide to effective prompt engineering covering core principles, patterns, and anti-patterns. Provides practical examples for various AI scenarios including chat, completion, and code generation. Recommended starting point for understanding how to craft effective prompts.
```

**Proper community classification:**
```markdown
**[My Experience with Copilot Agents](https://medium.com/@author/copilot-agents)** `[📒 Community]`  
Personal blog post sharing practical experiences and lessons learned building custom Copilot agents. Includes real-world examples, common pitfalls, and workarounds not found in official documentation.
```

## Migration Guide

### Updating Existing Articles

**Step 1:** Audit current references
- List all references in article
- Note current format/classification (if any)

**Step 2:** Classify each reference
- Identify domain
- Apply classification rules
- Note special cases

**Step 3:** Reformat references
- Use proper template
- Add 2-4 sentence descriptions
- Include classification markers

**Step 4:** Organize and group
- Create appropriate sections
- Order by authority/relevance
- Verify all are included

**Step 5:** Quality check
- Run through complete checklist
- Test all links
- Verify descriptions are accurate

### Example: Before and After

**Before:**
```markdown
## References

- https://code.visualstudio.com/docs
- https://dev.to/author/article
- https://github.com/user/repo
```

**After:**
```markdown
## References

### Official Documentation

**[VS Code Documentation](https://code.visualstudio.com/docs)** `[📘 Official]`  
Complete official Visual Studio Code documentation covering all editor features, extensions, and customization options. The authoritative source for VS Code usage, troubleshooting, and best practices.

### Community Resources

**[Building VS Code Extensions Tutorial](https://dev.to/author/article)** `[📒 Community]`  
Community tutorial providing step-by-step guide to creating custom VS Code extensions with practical examples. Useful for developers new to extension development wanting a beginner-friendly introduction.

**[Extension Example Repository](https://github.com/user/repo)** `[📒 Community]`  
Community repository demonstrating extension patterns discussed in the tutorial. Contains working code examples and helpful inline comments explaining implementation details.
```
