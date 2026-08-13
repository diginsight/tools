# Repository Structure Guide

This document describes the folder structure and organization of this learning documentation repository.

**📖 Context folder mapping:** See [.copilot/context/00.00-prompt-engineering/00.00-context-structure-index.md](../.copilot/context/00.00-prompt-engineering/00.00-context-structure-index.md) for context file sources and generation.

---

## 📁 Repository Folder Structure

### .github/ — GitHub Copilot Configuration

Repository-level configuration for GitHub Copilot customization.

| Folder/File | Purpose |
|-------------|---------|
| `copilot-instructions.md` | Global instructions applied to all Copilot interactions |
| `instructions/` | Path-specific instruction files (auto-loaded by `applyTo` patterns) |
| `prompts/` | Reusable prompt files for common workflows |
| `templates/` | Content templates for articles, how-tos, and metadata |
| `agents/` | Custom agent definitions for specialized tasks |
| `skills/` | Agent skill definitions (SKILL.md files) |

### .copilot/ — Copilot Context and Tools

AI context library and automation tools.

| Folder | Purpose |
|--------|---------|
| `context/` | Rich context files organized by domain (numbered for priority) |
| `mcp-servers/` | MCP server executables (IQPilot) |
| `scripts/` | PowerShell automation scripts |
| `bin/` | (Deprecated — use mcp-servers/) |

**📖 Context folder details:** [.copilot/context/00.00-prompt-engineering/00.00-context-structure-index.md](../.copilot/context/00.00-prompt-engineering/00.00-context-structure-index.md)

### src/ — Source Code

| Folder | Purpose |
|--------|---------|
| `IQPilot/` | C# MCP Server for content quality tools (16 tools) |
| `MetadataWatcher/` | C# file watcher for metadata synchronization |

### .vscode/ — VS Code Configuration

| Folder/File | Purpose |
|-------------|---------|
| `extensions/metadata-watcher/` | TypeScript VS Code extension |
| `tasks.json` | Build and automation tasks |
| `launch.json` | Debug configurations |
| `settings.json` | Workspace settings |

### Content Folders (Numbered for Ordering)

| Folder | Purpose |
|--------|---------|
| `01.00-news/` | VS Code releases, tool updates (date-prefixed: YYYYMMDD) |
| `02.00-events/` | Conference notes (Build, Ignite) with session summaries |
| `03.00-tech/` | Technical articles organized by topic |
| `04.00-howto/` | Step-by-step guides and tutorials |
| `05.00-issues/` | Problem solving and troubleshooting notes |
| `06.00-idea/` | Project concepts (IQPilot, LearnHub) |
| `07.00 projects/` | Active project documentation |
| `90.00-travel/` | Personal travel and event planning |

---

## 🔧 Prompt Files

### Content Creation
- **`/article-writing`**: Generate article drafts from topic/outline
  - Use with templates for structured content
  - Includes research via semantic search

### Quality Validation
- **`/grammar-review`**: Check spelling, grammar, punctuation
- **`/readability-review`**: Analyze clarity, consistency, redundancy
- **`/understandability-review`**: Evaluate audience comprehension
- **`/logic-analysis`**: Verify logical flow and concept connections
- **`/structure-validation`**: Ensure template compliance
- **`/fact-checking`**: Verify accuracy against authoritative sources
- **`/gap-analysis`**: Identify missing information

### Content Discovery
- **`/correlated-topics`**: Find related subjects and learning paths
- **`/series-validation`**: Check consistency across article series

### Metadata Management
- **`/metadata-init`**: Create metadata file for new articles
- **`/metadata-update`**: Update metadata after validations
- **`/publish-ready`**: Comprehensive pre-publish checklist

## 📋 Templates

### Article Types
- **`article.template.md`**: General explanatory content
- **`howto.template.md`**: Step-by-step procedures
- **`tutorial.template.md`**: Hands-on learning with exercises

### Documentation
- **`recording-summary.template.md`**: Meeting/presentation summaries
- **`recording-analysis.template.md`**: Deep analysis of recordings
- **`issue.template.md`**: Bug/improvement reports

### Metadata
- **`metadata-template.yml`**: Schema for article metadata

## �️ Automation Scripts

Located in `.copilot/scripts/`:

| Script | Purpose |
|--------|---------|
| `validate-metadata.ps1` | Validate all metadata files |
| `check-stale-validations.ps1` | Find articles with outdated checks |
| `build-iqpilot.ps1` | Build IQPilot MCP server |

---

## 📝 Instructions Files

### Global Instructions

**`.github/copilot-instructions.md`**: Applied to all Copilot interactions
- Repository architecture overview
- Content organization
- Key workflows and conventions

### Path-Specific Instructions

**`.github/instructions/`**: Auto-loaded based on `applyTo` patterns

| File | Applies To | Purpose |
|------|------------|---------|
| `documentation.instructions.md` | `*.md` (content) | Base markdown rules |
| `article-writing.instructions.md` | `*.md` (content) | Comprehensive writing guidance |
| `pe-prompts.instructions.md` | `.github/prompts/**/*.md` | Prompt file creation rules |
| `pe-agents.instructions.md` | `.github/agents/**/*.agent.md` | Agent file creation rules |
| `pe-context-files.instructions.md` | `.copilot/context/**/*.md` | Context file creation rules |
| `pe-skills.instructions.md` | `.github/skills/**/SKILL.md` | Skill file creation rules |

---

## 📚 Reference Documentation

### GitHub Copilot Features

| Feature | Location | Purpose |
|---------|----------|---------|
| Prompt files | `.github/prompts/*.prompt.md` | Reusable workflows |
| Instructions | `.github/instructions/*.instructions.md` | Path-specific guidance |
| Agents | `.github/agents/*.agent.md` | Specialized assistants |
| Skills | `.github/skills/*/SKILL.md` | Agent capabilities |
| Templates | `.github/templates/*.md` | Content scaffolds |
| Context | `.copilot/context/**/*.md` | Shared reference docs |

### Supported IDEs

| IDE | Support Level |
|-----|---------------|
| VS Code | All features supported |
| Visual Studio 2022 (17.10+) | Prompt and instruction files |
| GitHub Copilot CLI | Agent mode prompts |

---

*Last updated: 2026-01-24*  
*Version: 2.0*
