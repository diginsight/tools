---
description: "Input collection schema for tech session analysis generation"
---

# Technical Session Analysis — Input Template

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
e.g., "images/01.001 title.png" or "Not available"}}

**External References:**
{{Optional — additional URLs or documents to enrich the analysis.
e.g., official documentation pages, blog posts, or GitHub repos mentioned in the session.
Leave empty to let the workflow discover references from transcript content.}}

---

## Session Metadata

**Session Title:**
{{Full title of the session (extract from summary or specify).
e.g., "Building Scalable Microservices with Azure Service Fabric"}}

**Session Date:**
{{When the session was recorded.
e.g., "May 21, 2025"}}

**Duration:**
{{Total length of the session.
e.g., "1 hour 30 minutes"}}

**Venue:**
{{Where the session was presented.
e.g., "Microsoft Build 2025", "Internal Tech Talk"}}

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
e.g., "Software architects evaluating microservices patterns"}}

**Concept Depth:**
{{Level of technical detail desired.
e.g., "Deep technical analysis with architecture patterns", "Balanced overview with key insights"}}

**Demo Handling:**
{{How to handle demonstration content in the main flow.
Default: "Brief mention with outcome in main sections; detailed breakdown in appendix"
Options: "Brief + appendix" (default), "Detailed inline", "Minimal mention"}}

**Off-Topic Content:**
{{How to handle tangential discussions.
Default: "Move to appendix with proper context"
Options: "Appendix" (default), "Omit entirely", "Inline with note"}}

---

## Structural Requirements

**Output Filename:**
{{Desired output filename format.
Default logic:
- If session title is in folder path: use "readme.sonnet4.md"
- Otherwise: use "YYYYMMDD-session-title-analysis.md"}}

**TOC Style:**
{{Default: "2-level maximum with emojis for L1 sections and proper nesting"}}

**Timestamp Format:**
{{Default: "HH:MM:SS for start, duration as 'Xm Ys'"}}

**Reference Types:**
{{Default: "Official docs, whitepapers, GitHub repos, related articles"
NOTE: All references classified per `.github/instructions/documentation.instructions.md`}}

---

## Content Focus

**Key Concepts to Emphasize:**
{{Specific topics or themes to highlight (optional).
e.g., "Focus on scalability patterns and failure handling"}}

**Speaker Focus:**
{{Speaker attribution preferences.
Default: "Attribute all major points to speakers with timeframes"}}

**Appendix Organization:**
{{How to structure appendices.
Default: "Separate appendix per demo, one for tangential discussions, one for Q&A"}}

---

## Example: Filled-out template

### Source Materials

**Transcript File:** transcript.txt
**Summary File:** SUMMARY.md
**Title Image:** images/01.001 title.png
**External References:** https://learn.microsoft.com/azure/service-fabric/

### Session Metadata

**Session Title:** Building Scalable Microservices with Azure Service Fabric
**Session Date:** May 21, 2025
**Duration:** 1 hour 30 minutes
**Venue:** Microsoft Build 2025 — Technical Track
**Speakers:** Dr. Sarah Chen (Principal Architect), John Smith (Senior Engineer)
**Session Link:** https://www.youtube.com/watch?v=abc123xyz

### Analysis Preferences

**Target Audience:** Software architects evaluating microservices architectures
**Concept Depth:** Deep technical analysis including architecture patterns
**Demo Handling:** Brief + appendix (default)
**Off-Topic Content:** Appendix (default)

### Structural Requirements

**Output Filename:** readme.sonnet4.md (folder contains session context)
**TOC Style:** 2-level maximum with technical emojis
**Timestamp Format:** HH:MM:SS for start, duration as "Xm Ys"
**Reference Types:** Official docs, architecture whitepapers, GitHub samples

### Content Focus

**Key Concepts:** Scalability patterns, reliability mechanisms, deployment strategies
**Speaker Focus:** Full attribution with timeframes
**Appendix Organization:** Separate appendix per demo + tangential discussions appendix

<!--
---
template_metadata:
  version: "1.0.0"
  last_updated: "2026-03-20"
  created: "2026-03-20"
  consumers:
    - "article-generate-techsession-analysis"
  changelog: "input-techsession-analysis.template.changelog.md"
---
-->
