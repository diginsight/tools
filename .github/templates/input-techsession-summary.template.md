# Technical Session Summary - Input Template

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

---

## Output Preferences

**Output Filename:**
{{Desired output filename.
Default logic:
- If session title is in folder path: use "summary.md"
- Otherwise: use "YYYYMMDD-session-title.md"}}

**Focus Level:**
{{Level of detail for the summary.
Options: "Balanced" (default), "Concise - key takeaways only", "Detailed - comprehensive coverage"}}

**Demo Handling:**
{{How to handle demonstration content.
Options: "Brief summary with outcomes" (default), "Minimal mention", "Detailed step references"}}

---

## Expected Input Content

### What to Look for in SUMMARY.md:
- **Metadata**: Session date, speakers with titles, duration, venue, recording links
- **Title slide**: Image path (e.g., `![Title](<images/01.001 title.png>)`)
- **Key topics**: Main discussion points and themes
- **Resources**: Links and references mentioned

### What to Look for in transcript.txt:
- **Timestamps**: `[HH:MM:SS]` or `[MM:SS]` format
- **Speaker attributions**: "Speaker Name:", "Moderator:", etc.
- **Topic transitions**: Changes in discussion subject
- **Demonstrations**: When speakers show tools, code, or examples
- **Q&A sections**: Questions and answers

---

## Example: Standard Session in Descriptive Folder

### Source Materials
**Summary File:** SUMMARY.md
**Transcript File:** transcript.txt
**Title Image:** images/01.001 title.png

### Output Preferences
**Output Filename:** summary.md (folder "BRK226 Boost Development" contains session context)
**Focus Level:** Balanced
**Demo Handling:** Brief summary with outcomes

---

## Example: Generic Folder with Custom Files

### Source Materials
**Summary File:** session-notes.md
**Transcript File:** session-transcript.txt
**Title Image:** Not available

### Output Preferences
**Output Filename:** 20251010-practical-patterns-intelligent-agents.md
**Focus Level:** Concise - key takeaways only
**Demo Handling:** Minimal mention

<!--
template_metadata:
  version: "1.0.0"
  last_updated: "2026-06-12"
-->
