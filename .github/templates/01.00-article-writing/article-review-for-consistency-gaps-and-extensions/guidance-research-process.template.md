---
description: "Research process guidance for article review workflows"
---

# Article Review - Research & Gap Discovery Process

Detailed research methodology for Phase 3 of `article-review-for-consistency-gaps-and-extensions.prompt.md`.

---

## Step 1: Identify core article topics (from cached content)

- Extract main topics from headings and sections
- Note versions mentioned (VS Code 1.x, Visual Studio 17.x, etc.)
- Identify technologies/products covered
- Consider user priorities from Phase 1

---

## Step 2: Topic expansion - Discover adjacent & emerging topics

For each core topic, systematically discover related topics not currently in article:

### A. Workspace context mining

- Use `semantic_search` with core topic keywords
- Query examples: "[core topic] best practices", "[technology] customization patterns"
- Extract topics/concepts from discovered articles that current article doesn't cover
- Note: Workspace articles often contain emerging practices not yet in official docs

### B. Official documentation exploration

- Fetch main documentation index/TOC pages for article's domain
  - VS Code Copilot: `https://code.visualstudio.com/docs/copilot/`
  - GitHub Copilot: `https://docs.github.com/copilot/`
  - Relevant product documentation sites
- Identify sections in TOC not covered in article
- Map hierarchical relationships (parent topics, sibling topics, child topics)

### C. Release notes & changelog mining

- Fetch release notes for last 6-12 months
- Extract new features, capabilities, breaking changes
- Identify deprecations and migrations
- Focus on features related to article's core topics

### D. Community & ecosystem trends

- Search GitHub for curated lists: "awesome-[topic]", "[topic]-examples"
- Use `github_repo` to analyze popular repositories' patterns
- Identify community best practices not yet formalized in docs
- Extract integration patterns (MCP servers, custom tools, orchestration)

### E. Cross-reference & deduplicate

- Merge all discovered topics, remove duplicates
- Group by relationship to core topics

### F. Alternative solutions & approaches (Goal 4.2)

- For each core technology/approach in article, identify alternatives
- Search patterns: "[core topic] vs [alternative]", "[core topic] alternatives"
- Document: Trade-offs, use cases, migration considerations
- Classification: Direct alternatives vs complementary tools

---

## Step 3: Build comprehensive URL discovery list

Build URL list for **core + adjacent topics** (deduplicate before fetching):
- Official documentation, release notes, official blogs
- Community resources, workspace instruction files

---

## Step 4: Fetch all URLs in parallel

Batch process ~15-30 URLs for performance.

---

## Step 5: Compare article vs fetched sources

Analyze THREE gap categories:

| Category | Focus |
|----------|-------|
| **A. Accuracy gaps** | Article claims vs source facts, deprecated features |
| **B. Completeness gaps** | Missing details, new features, breaking changes |
| **C. New topic gaps** | Adjacent topics not mentioned, emerging patterns |

---

## Gap Type Classification

| Gap Type | Impact | Examples |
|----------|--------|----------|
| **Correctness** | Breaking | Deprecated API shown as current, wrong version numbers |
| **Completeness** | High | Core feature missing, major use case not covered |
| **Adjacent Topic** | Medium-High | Related concept not covered (context engineering, agent orchestration) |
| **Currency** | Medium | Outdated best practices, old UI screenshots |
| **Enhancement** | Low | Additional examples, advanced tips |

## Dual-Priority Weighting

Combine editorial priorities with user priorities:

- **Editorial Priority:** Correctness=Critical, Completeness=High, Currency=Medium, Enhancement=Low
- **User Priority Boost:** Gap in user-specified section/topic = +2 levels, user-selected text = +1 level
- **Final Priority:** max(Editorial Priority, User Priority Boost)

**Key Principle:** User priorities influence *where we invest extra effort*, but editorial judgment determines *what must be fixed for accuracy*.

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "article-review-for-consistency-gaps-and-extensions"
  changelog: "guidance-research-process.template.changelog.md"
---
-->
