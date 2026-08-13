# Technical Session Summary — Input Template

Use this template to provide structured input for the `article-generate-techsession-summary.prompt.md` workflow.

---

## Source Materials

**Summary File:**
{{Name of the summary file with session metadata.
e.g., "SUMMARY.md" (default) or "session-notes.md"}}

**Transcript File:**
{{Name of the transcript file.
e.g., "transcript.txt" (default) or "session-transcript.txt"}}

**Title Image:**
{{Path to title slide image (usually in summary file).
e.g., "images/01.001 title.png" or "Not available"}}

**External References:**
{{Optional — additional URLs or documents to enrich the summary.
e.g., official documentation pages, blog posts, GitHub repos mentioned in the session.
Leave empty to let the workflow discover references from transcript content.}}

---

## Output Preferences

**Output Filename:**
{{Desired output filename.
Default logic:
- If session title is in folder path: use "summary.md"
- Otherwise: use "YYYYMMDD-session-title.md"}}

**Focus Level:**
{{Level of detail for the summary.
Options: "Balanced" (default), "Concise — key takeaways only", "Detailed — comprehensive coverage"}}

**Demo Handling:**
{{How to handle demonstration content.
Options: "Brief summary with outcomes" (default), "Minimal mention", "Detailed step references"}}

---

## Expected Input Content

### What to look for in the summary file (e.g., SUMMARY.md)

- **Metadata**: Session date, speakers with titles, duration, venue, recording links
- **Title slide**: Image path (e.g., `![Title](<images/01.001 title.png>)`)
- **Key topics**: Main discussion points and themes
- **Resources**: Links and references mentioned

### What to look for in the transcript file (e.g., transcript.txt)

- **Timestamps**: `[HH:MM:SS]` or `[MM:SS]` format
- **Speaker attributions**: "Speaker Name:", "Moderator:", etc.
- **Topic transitions**: Changes in discussion subject
- **Demonstrations**: When speakers show tools, code, or examples
- **Q&A sections**: Questions and answers
- **External links or resources**: URLs, product names, documentation references mentioned verbally

---

## Example: Standard session in descriptive folder

### Source Materials

**Summary File:** SUMMARY.md
**Transcript File:** transcript.txt
**Title Image:** images/01.001 title.png

### Output Preferences

**Output Filename:** summary.md (folder "BRK226 Boost Development" contains session context)
**Focus Level:** Balanced
**Demo Handling:** Brief summary with outcomes

---

## Example: Generic folder with custom files

### Source Materials

**Summary File:** session-notes.md
**Transcript File:** session-transcript.txt
**Title Image:** Not available
**External References:** https://learn.microsoft.com/azure/some-service

### Output Preferences

**Output Filename:** 20251010-practical-patterns-intelligent-agents.md
**Focus Level:** Concise — key takeaways only
**Demo Handling:** Minimal mention

<!--
template_metadata:
  version: "1.0.0"
  last_updated: "2026-06-12"
-->
