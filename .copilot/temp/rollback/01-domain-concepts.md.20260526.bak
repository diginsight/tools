# Domain Concepts for Learning Documentation Site

This document defines core concepts, principles, and terminology used throughout this learning and documentation repository.

## Repository Purpose

This is a **personal learning and development documentation site** focused on:
- Technical skill development
- Knowledge capture and retention
- Best practices documentation and development
- Idea exploration and development

## Core Concepts

### Content Categories

The Learning Hub uses a **7-category taxonomy** extending the Diátaxis framework. Each category addresses a specific user question:

| Category | Purpose | User Question |
|----------|---------|---------------|
| **Overview** | First-touch orientation | "What is this? Why should I care?" |
| **Getting Started** | First success path | "How do I begin?" |
| **Concepts** | Mental model building | "How does this work?" |
| **How-to** | Task accomplishment and practice development | "How do I accomplish X effectively?" |
| **Analysis** | Strategic evaluation and strategy development | "What approach should we use?" |
| **Reference** | Authoritative lookup | "What are the exact specifications?" |
| **Resources** | Supporting materials | "Where can I learn more?" |

**Authoritative source:** See `06.00-idea/learning-hub/02-documentation-taxonomy/01-learning-hub-documentation-taxonomy.md`

#### How-to Subcategories

The How-to category is divided into four subcategories:

| Subcategory | Focus | Title Pattern |
|-------------|-------|---------------|
| **Task Guides** | Accomplish specific goals | "How to [do X]" |
| **Patterns & Practices** | Reusable solutions to recurring problems | "How to [structure/organize X]" |
| **Techniques** | Specific approaches for optimal results | "How to [optimize/improve X]" |
| **Methodology** | Frameworks for complex tasks | "How to [orchestrate/approach X]" |

#### Analysis Subcategories

Analysis content addresses strategic questions:

| Subcategory | Focus | Example |
|-------------|-------|--------|
| **Technology Radar** | Adoption decisions | ADOPT/TRIAL/ASSESS/HOLD classification |
| **Comparative Analysis** | Alternatives evaluation | Framework comparison tables |
| **Strategy Development** | Approach formulation | Model selection strategy |
| **Trend Analysis** | Future direction assessment | Ecosystem maturity assessment |

### Technology Radar Framework

The Technology Radar classifies technologies into four adoption rings:

| Rating | Definition | Implication |
|--------|------------|-------------|
| **ADOPT** | Proven in production, recommended for new projects | Default choice for relevant use cases |
| **TRIAL** | Worth pursuing, ready for evaluation in real projects | Allocate resources for pilot |
| **ASSESS** | Worth exploring, understand impact | Research and prototyping only |
| **HOLD** | Proceed with caution, consider alternatives | Avoid for new work, plan migration |

### Progressive Depth Model

Content supports different depths of engagement:

```
Overview (5-min) → Getting Started (30-60 min) → Concepts (layered) → Analysis (deep)
```

**Concepts Layer Structure:**
- **Layer 1: Core** — Fundamental terms and definitions
- **Layer 2: Architecture** — System components and their roles
- **Layer 3: Advanced** — Edge cases and limitations

### Quality Dimensions

**Accuracy**: Information is factually correct and verified against authoritative sources  
**Currency**: Content is up-to-date with latest versions, standards, and best practices  
**Clarity**: Ideas are expressed clearly for the target audience  
**Completeness**: Coverage is comprehensive without unnecessary gaps  
**Consistency**: Terminology, style, and structure are uniform across content

### Validation Framework

**Grammar Validation**: Spelling, grammar, punctuation correctness  
**Readability Validation**: Appropriate complexity level, flow, and structure  
**Structure Validation**: Required sections present, proper formatting, template compliance  
**Logic Validation**: Ideas flow logically, concepts properly connected  
**Fact-Checking**: Claims verified against authoritative sources  
**Gap Analysis**: Missing information identified  
**Understandability**: Content appropriate for target audience  

### Metadata System

**Article Metadata**: Core information about content (title, author, dates, status)  
**Validation History**: Records of quality checks with timestamps and outcomes  
**Cross-References**: Links between related content pieces  
**Analytics**: Metrics about content (word count, reading time, engagement)  

### Content Lifecycle

```
Draft → In-Review → Published → [Updates/Revisions] → Archived
```

**Draft**: Initial creation, not yet validated  
**In-Review**: Validations in progress, being refined  
**Published**: Approved, validated, available to readers  
**Archived**: Outdated or superseded but kept for reference  

### Learning Paths

**Prerequisites**: Foundational knowledge needed before a topic  
**Core Content**: Main learning material  
**Advanced Topics**: Next-level concepts building on foundations  
**Related Topics**: Parallel subjects at similar complexity

### Automation Philosophy

**Validation Caching**: Avoid redundant AI validations when content unchanged  
**Prompt-Driven**: Use reusable prompts for consistent automation  
**Template-Based**: Enforce structure through templates  
**Metadata-Tracked**: Record all operations and outcomes  

## Content Organization

### File Naming Convention

**Always use kebab-case** (lowercase with hyphens) for article filenames:

```
✅ Good: configure-azure-key-vault.md
✅ Good: how-to-setup-mcp-server.md
✅ Good: 20251224-vscode-release-notes.md

❌ Bad: Configure Azure Key Vault.md
❌ Bad: HowTo_Setup_MCP_Server.md
❌ Bad: configureAzureKeyVault.md
```

**Rationale**: When Quarto compiles Markdown to HTML, the filename becomes the URL path. Kebab-case produces readable, SEO-friendly URLs:

- `docs/configure-azure-key-vault.html` → Clean and scannable
- `docs/Configure%20Azure%20Key%20Vault.html` → URL-encoded spaces are ugly

**Rules**:
- Use lowercase letters only
- Replace spaces with hyphens (`-`)
- Avoid underscores, camelCase, or PascalCase
- Date prefixes use format `YYYYMMDD-` (no spaces)
- Keep names concise but descriptive

### Folder Structure

**📖 Complete guidance:** See `06-folder-organization-and-navigation.md`

**Quick reference:**
- **Subject Folders** (`XX.YY name/`): Numeric prefixes for ordering
- **Date-Prefixed Folders** (`YYYYMMDD topic/`): Time-sensitive content
- **Series**: Related articles grouped under common theme

### Navigation Patterns

**📖 Sidebar menu rules:** See `06-folder-organization-and-navigation.md`

**Quick reference:**
- Table of contents in long articles
- Series navigation (prev/next)
- Prerequisite and related topic links

## Audience Considerations

### Skill Levels

**Beginner**: New to the topic, needs fundamentals
**Intermediate**: Has basics, building deeper understanding
**Advanced**: Experienced, seeking optimization or edge cases

### Reading Goals

- **Learning**: Acquiring new knowledge or skills
- **Reference**: Looking up specific information
- **Troubleshooting**: Solving a specific problem
- **Exploration**: Discovering new topics

## Quality Standards

### Citation Requirements

- All factual claims must have authoritative sources
- References section required if sources cited
- External links must be to credible, maintained resources
- Version information specified for technology documentation

### Code Standards

- Code examples must be tested and functional
- Syntax highlighting specified
- Comments explain non-obvious logic
- Examples show realistic usage
- Error handling included where relevant

### Accessibility Standards

- Semantic Markdown structure
- Descriptive link text
- Alt text for images
- Proper heading hierarchy
- Clear, concise language

## Continuous Improvement

### Review Cycles

- Grammar/readability: Re-validate if content changes
- Facts: Re-verify quarterly or when technologies update
- Structure: Check with template updates
- Gaps: Revisit when related content added

### Deprecation Strategy

- Mark outdated content clearly
- Provide replacement links
- Keep archived for historical reference
- Update cross-references

## Tools and Automation

### Prompt Files

Located in `.github/prompts/`, these automate common tasks:
- Content creation
- Quality validation
- Gap analysis
- Metadata management

### Templates

Located in `.github/templates/`, these enforce structure:
- Article templates
- HowTo templates
- Tutorial templates
- Metadata schemas

### Scripts

Located in `.copilot/scripts/`, these handle programmatic tasks:
- Metadata validation
- Stale content detection
- Series index generation
- Citation extraction

## Success Metrics

### Content Quality

- Validation pass rates
- Source verification coverage
- Template compliance
- Readability scores

### Coverage

- Topic breadth
- Depth of treatment
- Gap filling over time
- Series completion

### Utility

- Content referenced by other articles
- External reference links
- Practical applicability
- Real-world examples
