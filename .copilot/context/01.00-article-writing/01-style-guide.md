---
title: "Style guide — quantitative targets and reference tables"
description: "Quantitative writing metrics, enforcement targets, and quick-reference lookup tables that complement the auto-loaded article-writing.instructions.md"
domain: "article-writing"
goal: "Provide measurable readability targets, audience calibration rules, and replacement tables so that article quality can be validated quantitatively — not just by subjective judgment"
scope:
  covers:
    - "Readability metrics and targets (Flesch, Flesch-Kincaid, sentence length, active voice)"
    - "Audience calibration (beginner/intermediate/advanced passage examples and rules)"
    - "Quick-reference replacement tables (wordy→crisp, UI verbs, phrasal verbs, bias-free terms)"
    - "Global-readiness checklist"
    - "Visual documentation guidance (diagrams, screenshots, annotation limits)"
    - "Procedure writing rules (summary — canonical rules in article-writing.instructions.md)"
  excludes:
    - "Writing voice and tone rules (see article-writing.instructions.md)"
    - "Article structure and Diátaxis patterns (see 03-article-creation-rules.md)"
    - "Validation pass/fail thresholds (see 02-validation-criteria.md)"
boundaries:
  - "MUST NOT duplicate rules already in article-writing.instructions.md — provide quantitative complements only"
  - "MUST NOT define validation pass/fail thresholds — those belong in 02-validation-criteria.md"
  - "Procedure writing section MUST remain a summary pointer to canonical rules in article-writing.instructions.md"
rationales:
  - "Separating quantitative targets from qualitative writing rules keeps article-writing.instructions.md focused on voice and structure while this file provides measurable enforcement criteria"
  - "Replacement tables are lookup data — keeping them in a context file (loaded on-demand) avoids bloating the always-loaded instruction file"
  - "Audience calibration was added because readability targets alone don't account for audience expertise level — Flesch 55 is fine for advanced content but too hard for beginners"
---

# Style Guide — Quantitative Targets and Reference Tables

**Purpose**: Quantitative writing metrics, enforcement targets, and quick-reference lookup tables that complement the auto-loaded `article-writing.instructions.md`.

**Referenced by**:
- `.github/prompts/01.00-article-writing/*.prompt.md` (all article writing prompts)
- `.github/instructions/article-writing.instructions.md` (references this file)

---

## ⚠️ Instruction Layering

This context file COMPLEMENTS (does not duplicate) auto-loaded instructions:

- `article-writing.instructions.md` — Voice, structure, formatting, Diátaxis, accessibility rules
- `documentation.instructions.md` — Base structure, reference classification, dual metadata

**This file provides:** quantitative targets, formula references, replacement tables, visual documentation guidance, and global-readiness rules sourced from the `03.00-tech/40.00-technical-writing/` article series.

---

## 📊 Readability Metrics and Targets

### Primary Metrics

| Metric | Target Range | Use Case |
|--------|-------------|----------|
| Flesch Reading Ease | 50–70 | Primary readability indicator |
| Flesch-Kincaid Grade | 8–10 (general), 11–12 (advanced) | Grade-level targeting |
| Active Voice % | 80–90% | Voice consistency enforcement |
| Avg Sentence Length | 15–25 words | Sentence complexity check |
| Paragraph Length | 3–5 sentences | Visual density check |

### Secondary Metrics (Deeper Analysis)

| Metric | Target Range | Best For |
|--------|-------------|----------|
| Gunning Fog Index | 10–14 | Business/technical audience |
| Coleman-Liau Index | 9–11 | Automated CI/CD (character-based, no syllable counting) |
| SMOG Index | 10–13 | Accuracy for healthcare/academic content |
| Dale-Chall Score | 7.0–8.0 | Vocabulary difficulty detection |
| ARI (Automated Readability) | 9–11 | Fast automated pipeline checks |

**CI/CD recommendation:** Use Coleman-Liau or ARI — they're character-based (no syllable counting needed), making them fast and deterministic for automated pipelines.

### Sentence Type Distribution

| Type | Target % | Description |
|------|----------|-------------|
| Simple | 40–50% | Single independent clause |
| Compound | 20–30% | Two independent clauses joined |
| Complex | 20–30% | Independent + dependent clause |
| Compound-complex | <10% | Minimize for readability |

---

## 🎯 Audience Calibration

The same concept reads differently depending on audience level. Use these examples to calibrate your writing for the target reader.

**Concept: "Progressive disclosure"**

| Audience | Example passage | Characteristics |
|----------|----------------|------------------|
| **Beginner** | "Don't overwhelm readers with everything at once. Start with the basics they need right now, then add more detail as they're ready. Think of it like teaching someone to drive—you don't explain engine mechanics on day one." | Analogy-driven, no jargon, short sentences, conversational |
| **Intermediate** | "Progressive disclosure layers content from essential to advanced. Lead with the surface level (what most users need), provide detail on demand (configuration options, parameters), and reserve expert content (edge cases, internals) for those who seek it." | Term defined on first use, 3-layer model named, concrete categories |
| **Advanced** | "Apply the 3-layer progressive disclosure model: surface (TL;DR, quick-start commands), detail (parameters, configuration, variations), and expert (implementation internals, performance tuning). Map each layer to your Diátaxis type—tutorials expose surface → detail; references expose all three simultaneously." | Assumes prior knowledge, cross-references frameworks, prescriptive |

**Calibration rules by audience:**

| Audience | Jargon handling | Analogies | Flesch target |
|----------|----------------|-----------|---------------|
| **Beginner** | Every technical term marked with `<mark>` and explained in context | Encouraged—bridge to familiar concepts | 60–70 |
| **Intermediate** | Common terms used freely; new terms marked and explained | Sparingly—only for genuinely complex ideas | 50–65 |
| **Advanced** | Domain expertise assumed; only novel jargon marked on first use | Rarely—readers prefer precision over analogy | 45–60 |

📖 **Jargon introduction rules (mark → explain → teach):** `.copilot/context/01.00-article-writing/03-article-creation-rules.md` → Jargon and New Terms

---

## 📝 Quick-Reference Replacement Tables

### Wordy → Crisp Replacements

| Instead of... | Use... |
|---------------|--------|
| in order to | to |
| due to the fact that | because |
| at this point in time | now |
| in the event that | if |
| for the purpose of | to, for |
| has the ability to | can |
| it is important to note that | note: |
| a large number of | many |
| in the majority of cases | usually |
| on a regular basis | regularly |
| it is necessary to | must |
| make changes to | change |
| it should be noted that | (delete — just state the fact) |

### UI Verb Replacements (Microsoft Mandate)

| Don't Use | Use Instead | Context |
|-----------|-------------|---------|
| Click | Select | All UI interactions |
| Tap | Select | All UI interactions |
| Click on | Select | Remove "on" |
| Check (checkbox) | Select the checkbox | Checkbox controls |
| Uncheck | Clear the checkbox | Checkbox controls |
| Right-click | Right-click or long-press | Context menus |

### Phrasal Verb → Single Verb (Global Readiness)

| Phrasal Verb | Replacement |
|-------------|-------------|
| set up | configure |
| carry out | perform |
| figure out | determine |
| get rid of | remove |
| come up with | create |
| put together | assemble |
| look into | investigate |
| run into | encounter |
| go through | review |
| point out | identify |
| turn on / turn off | enable / disable |
| fill out | complete |

### Bias-Free Term Replacements

| Problematic | Replacement |
|------------|-------------|
| master/slave | primary/replica, main/subordinate |
| whitelist/blacklist | allowlist/denylist |
| sanity check | validation check, confidence check |
| dummy (value) | placeholder, sample |
| grandfathered | legacy, exempt |
| native (feature) | built-in |
| man-hours | person-hours, staff-hours |

---

## 🌐 Global-Readiness Checklist

Before publishing content for international audiences:

- [ ] Include articles (a, an, the) — don't drop for brevity
- [ ] Include relative pronouns (that, which, who) — don't omit
- [ ] Use SVO sentence order (Subject-Verb-Object)
- [ ] Replace phrasal verbs with single-word equivalents (see table above)
- [ ] Avoid idioms — use plain alternatives
- [ ] Spell out month names in dates (January 5, 2026 — never 1/5/2026)
- [ ] Specify currency codes (USD, not just $)
- [ ] Include time zones for scheduled events
- [ ] Clarify ambiguous words: "once" (one time? after?), "since" (because? from that time?), "while" (during? although?)
- [ ] Use diverse example names (not only Western names/addresses)

---

## 🎨 Visual Documentation Guidance

### Visual Budget by Diátaxis Type

| Content Type | Visual Budget | Visual Preferred When... |
|-------------|--------------|--------------------------|
| Tutorial | 3–6 diagrams | Showing step progression |
| How-to | 1–3 diagrams | Illustrating workflows |
| Reference | 0–2 diagrams | Showing API relationships |
| Explanation | 2–5 diagrams | Comparing concepts, architecture |

### Diagramming Standards

- **Default tool**: Mermaid (native rendering in GitHub and Quarto)
- **Architecture**: Use C4 model levels (Context → Container → Component → Code)
- **Pairing**: Always lead text → show visual → explain visual → detail in text
- **Colors**: Maximum 3–5 colors per diagram; never rely on color alone
- **Alt text**: Short description for simple diagrams; detailed description below for complex ones
- **Screenshot naming**: `feature-name-step-N-vX.Y.png` (tracks product version changes)

---

## 📋 Procedure Writing Rules

📖 **Canonical rules (7-rule list):** `.github/instructions/article-writing.instructions.md` → Procedures (Step-by-Step Instructions)

This section is intentionally a pointer — the detailed procedure rules live in the auto-loaded Tier 1 instructions to avoid duplication.

---

## References

- **External:** [Microsoft Writing Style Guide](https://learn.microsoft.com/en-us/style-guide/)
- **External:** [Diátaxis Framework](https://diataxis.fr/)
- **External:** [Google Developer Style Guide](https://developers.google.com/style)
- **Internal:** `.github/instructions/article-writing.instructions.md` (auto-loaded writing rules)
- **Internal:** `.github/instructions/documentation.instructions.md` (auto-loaded base rules)
- **Internal:** `03.00-tech/40.00-technical-writing/` (source article series — 18 articles)

---

## Version History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 2.2.0 | 2026-03-01 | Added "Audience Calibration" section with example passages at beginner/intermediate/advanced levels, calibration rules table (jargon handling, analogies, Flesch targets), and cross-reference to Tier 2 jargon rules. Source: Gap 1 from context file audit. | System |
| 2.1.0 | 2026-03-01 | Replaced "Procedure Writing Quick Rules" section with pointer to canonical rules in `article-writing.instructions.md` (eliminates duplication). Source: Recommendation C from coverage analysis. | System |
| 2.0.0 | 2026-02-28 | Complete rewrite: eliminated duplication with auto-loaded article-writing.instructions.md; added quantitative metrics (7 readability formulas, sentence type distribution, active voice targets); added 4 replacement tables (wordy→crisp, UI verbs, phrasal verbs, bias-free terms); added global-readiness checklist, visual documentation guidance, procedure rules. Source: 40.00-technical-writing series (articles 00-12 + MWSG 00-04) | System |
| 1.0.0 | 2025-12-26 | Initial version | System |

<!--
context_metadata:
  version: "2.2.0"
  last_updated: "2026-04-26"
-->
