---
name: issue-generate-analysis
description: Generate analysis from current conversation
agent: agent
model: claude-opus-4.6
tools: ['codebase', 'fetch']
argument-hint: 'topic="Your Article Topic" outline="key points to cover"'
---

# Generate analysis from current conversation

## Goal
Generate a comprehensive issue markdown document using the enhanced structure defined in the file `ISSUE Template.md`.
The issue markdown document should be created in the folder `/src/docs/90. Issues` and named as `YYYYMMDD - [Issue Title].md`.

## Instructions

### 1. Analyze Current Conversation
Analyze the current conversation and identify the following information:
- **IssueTitle**: A concise, descriptive title about the issue identified in the current conversation
- **DatePrefix**: The current date in the format `YYYYMMDD` (e.g., `20251107`)
- **Author**: The current user name (e.g., "Dario Airoldi")
- **Severity**: Assess the severity level (Low/Medium/High/Critical)
- **Component**: Identify the affected component or project
- **Framework**: Determine the target framework version

### 2. Read and Understand Template Structure
Read the template file located at:
`.github/copilot/templates/ISSUE Template.md`

Understand the enhanced structure including:
- **Header with metadata** (Date, Author, Status, Severity, Component, Framework)
- **Table of Contents** with emoji navigation
- **Comprehensive sections** with detailed subsections
- **Modern formatting** with tables, code blocks, and checklists

### 3. Create New Issue Document
Create a new issue document in the folder:
`/src/docs/90. Issues`

Name the document as:
`<DatePrefix> - <IssueTitle>.md`

### 4. Fill Content from Conversation Analysis
Analyze the current debugging conversation and fill ALL sections of the issue report:

#### Required Sections to Complete:
- **📝 DESCRIPTION**: Brief description, error messages, and impact points
- **🔍 CONTEXT INFORMATION**: Environment details, exception details, call stack, variable values
- **🔬 ANALYSIS**: Root cause analysis, impact assessment, affected workflows
- **🔄 REPRODUCTION STEPS**: Step-by-step reproduction and affected code locations
- **✅ SOLUTION IMPLEMENTED**: Fix overview, code changes, solution features (if solution was discussed)
- **📚 ADDITIONAL INFORMATION**: Testing recommendations, migration considerations, performance impact
- **✔️ RESOLUTION STATUS**: Current status, verification checklist, follow-up actions
- **🎓 LESSONS LEARNED**: What went wrong/right, improvements for future
- **📎 APPENDIX**: Additional reference materials and examples

#### Content Guidelines:
- Use **emojis** in section headers for visual appeal
- Include **comprehensive tables** for structured data
- Add **code snippets** with proper syntax highlighting
- Use **checkboxes** for actionable items
- Include **links and references** where applicable
- Maintain **professional technical writing** style

### 5. Quality Assurance
Ensure the generated document:
- ✅ Follows the exact template structure
- ✅ Includes Table of Contents with proper anchor links
- ✅ Contains all emoji headers as specified
- ✅ Has comprehensive content in each section
- ✅ Uses consistent formatting throughout
- ✅ Includes actionable follow-up items
- ✅ Provides clear reproduction steps
- ✅ Documents lessons learned for future prevention

<!--
prompt_metadata:
  version: "1.0.0"
  last_updated: "2026-06-12"
-->
