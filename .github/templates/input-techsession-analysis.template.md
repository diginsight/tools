# Technical Session Analysis - Input Template

Use this template to provide structured input for the `article-generate-techsession-analysis.prompt.md` workflow.

---

## Source Materials

**Transcript File:**
{{Name of the transcript file in the active folder.
e.g., "transcript.txt" (default) or "session-recording-transcript.txt"}}

**Summary File:**
{{Name of the summary file with key points.
e.g., "SUMMARY.md" (default) or "session-summary.md"}}

**Title Image:**
{{Path to title slide image (should be in summary file).
e.g., "images/01.001 title.png" or "Not available - please provide"}}

---

## Session Metadata

**Session Title:**
{{Full title of the session (extract from summary or specify).
e.g., "Building Scalable Microservices with Azure Service Fabric"}}

**Session Date:**
{{When the session was recorded.
e.g., "August 15, 2025"}}

**Duration:**
{{Total length of the session.
e.g., "1 hour 30 minutes" or "90 minutes"}}

**Venue:**
{{Where the session was presented.
e.g., "Microsoft Build 2025", "Internal Tech Talk", "Azure Webinar Series"}}

**Speakers:**
{{List of speakers (extract from summary/transcript or specify).
e.g., "Dr. Sarah Chen (Principal Architect), John Smith (Senior Engineer)"}}

**Session Link:**
{{URL to recording if available.
e.g., "https://www.youtube.com/watch?v=xyz123" or "Internal SharePoint link"}}

---

## Analysis Preferences

**Target Audience:**
{{Who will read this analysis?
e.g., "Software architects evaluating microservices patterns", "Developers new to Service Fabric"}}

**Concept Depth:**
{{Level of technical detail desired.
e.g., "Deep technical analysis with architecture patterns", "Balanced overview with key insights"}}

**Demo Handling:**
{{How to handle demonstration content.
e.g., "Move all demo steps to Appendix A with screenshots", "Create separate appendix per demo"}}

**Off-Topic Content:**
{{How to handle tangential discussions.
e.g., "Move to 'Additional Discussions' appendix", "Only move clearly unrelated content"}}

---

## Structural Requirements

**Output Filename:**
{{Desired output filename format.
Default logic:
- If session title is in folder path: use "readme.sonnet4.md"
- Otherwise: use "YYYYMMDD-session-title-analysis.md"}}

**TOC Style:**
{{Table of contents preferences.
Default: "2-level maximum with emojis for L1 sections and proper nesting"}}

**TOC Emoji Themes:**
{{Preferred emoji style for top-level sections (optional).
e.g., "Technical themes (🏗️ Architecture, ⚡ Performance, 🛡️ Security)", "Auto-select based on content"}}

**Timestamp Format:**
{{Preferred time format.
Default: "HH:MM:SS for start, duration as 'Xm Ys'"}}

**Reference Types:**
{{Types of references to include.
Default: "Official docs, whitepapers, GitHub repos, related articles"
NOTE: All references classified per `.github/instructions/documentation.instructions.md` (📘📗📒)}}

**Reference Detail Level:**
{{How much explanation for each reference.
Default: "2-4 sentences explaining content and relevance"}}

---

## Content Focus

**Key Concepts to Emphasize:**
{{Specific topics or themes to highlight (optional).
e.g., "Focus on scalability patterns and failure handling", "All concepts equally"}}

**Speaker Focus:**
{{Speaker attribution preferences.
e.g., "Attribute all major points to speakers", "Only attribute key insights"}}

**Appendix Organization:**
{{How to structure appendices.
e.g., "Separate appendix per demo, one for off-topic content", "Group all supplementary content"}}

---

## Example: Filled-Out Template

### Source Materials

**Transcript File:** transcript.txt
**Summary File:** SUMMARY.md
**Title Image:** images/01.001 title.png

### Session Metadata

**Session Title:** Building Scalable Microservices with Azure Service Fabric
**Session Date:** May 21, 2025
**Duration:** 1 hour 30 minutes
**Venue:** Microsoft Build 2025 - Technical Track
**Speakers:** Dr. Sarah Chen (Principal Architect), John Smith (Senior Engineer)
**Session Link:** https://www.youtube.com/watch?v=abc123xyz

### Analysis Preferences

**Target Audience:** Software architects evaluating microservices architectures
**Concept Depth:** Deep technical analysis including architecture patterns
**Demo Handling:** Create separate appendices for each demo with cross-references
**Off-Topic Content:** Move to "Additional Discussions" appendix

### Structural Requirements

**Output Filename:** readme.sonnet4.md (folder contains session context)
**TOC Style:** 2-level maximum with technical emojis
**Timestamp Format:** HH:MM:SS for start, duration as "Xm Ys"
**Reference Types:** Official docs, architecture whitepapers, GitHub samples
**Reference Detail Level:** 2-4 sentences per reference

### Content Focus

**Key Concepts:** Scalability patterns, reliability mechanisms, deployment strategies
**Speaker Focus:** Full attribution with timeframes
**Appendix Organization:** Separate appendix per demo + Additional Discussions

<!--
template_metadata:
  version: "1.0.0"
  last_updated: "2026-06-12"
-->
