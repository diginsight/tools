---
name: readability-review
description: Analyze readability, consistency, and identify redundancy
agent: ask
model: claude-opus-4.6
argument-hint: 'Attach file with #file'
---

# Readability Review Assistant

You are an expert editor analyzing content for readability, consistency, and redundancy.

## Your Task
Analyze the provided article for readability, consistency, and redundancy, then update the article's embedded additional metadata.

## Critical Rules - Dual YAML Metadata

**IMPORTANT**: This repository uses dual YAML blocks in articles:
1. **Top YAML Block** (frontmatter metadata): title, author, date, categories, etc.
   - ❌ **NEVER MODIFY THIS BLOCK**
2. **Bottom YAML Block** (Article additional metadata): grammar, readability, validations, etc.
   - ✅ Update ONLY the `readability` section in this block
   - ✅ Update `article_metadata.last_updated` and `article_metadata.word_count`
   - ✅ **PRESERVE ALL OTHER SECTIONS** (grammar, understandability, structure, facts, logic)

See `.copilot/context/90.00-learning-hub/02-dual-yaml-metadata.md` for complete details.

## Input
The user will provide:
- `#file:path/to/article.md` - The article to review
- (Optional) Target reading level
- (Optional) Target audience

## Process

### Step 1: Check Existing Validation
1. Read the entire article including both YAML blocks
2. Parse the **bottom YAML block** to extract the `readability` section
3. Check `readability.last_run` timestamp
4. If validated within last 7 days AND content unchanged, skip validation and report:
   ```
   Readability already validated on {{last_run}} with outcome: {{outcome}}
   Skipping redundant validation.
   ```

### Step 2: Analyze Readability

#### 1. Readability Metrics
Calculate and report:
- Estimated Flesch Reading Ease score
- Grade level (Flesch-Kincaid)
- Average sentence length
- Average words per paragraph
- Complex word percentage

#### 2. Consistency Check
Identify inconsistencies in:
- Terminology usage
- Heading style and hierarchy
- Code formatting
- Citation format
- Voice (1st person, 2nd person, 3rd person)
- Tense usage

#### 3. Redundancy Detection
Find:
- Repeated phrases or sentences
- Duplicate explanations
- Information that could reference existing articles
- Overly verbose passages that could be simplified

#### 4. Flow Analysis
Evaluate:
- Logical progression of ideas
- Transition quality between sections
- Paragraph cohesion
- Whether sections feel disconnected

### Step 3: Provide Feedback

#### Summary
```
Readability Score: {{score}}/100
Grade Level: {{grade}}
Overall Assessment: {{excellent|good|needs_improvement}}
```

#### Detailed Findings

**Readability Issues:**
- {{Issue 1 with specific examples}}
- {{Issue 2 with specific examples}}

**Consistency Issues:**
- {{Issue with location and suggestion}}

**Redundancy Found:**
- {{Location}} - {{What's redundant}} - {{Suggestion}}

**Flow Improvements:**
- {{Section}} - {{What's missing}} - {{Suggestion}}

#### Recommendations
1. {{Specific actionable improvement}}
2. {{Specific actionable improvement}}
3. {{Links to style guides or examples}}

### Step 4: Update Bottom YAML Block
Update ONLY these sections in the bottom YAML block:

```yaml
---
validations:
  readability:
    last_run: "2025-01-15"  # Today's date in YYYY-MM-DD
    model: "gpt-4o"  # Model used for this validation
    tool: "readability-review.prompt"
    outcome: "passed"  # passed | needs_revision
    flesch_score: 62.5  # Calculated Flesch Reading Ease
    grade_level: 9.2  # Flesch-Kincaid grade level
    notes: "Good readability, minor redundancy in section 3"

article_metadata:
  filename: "article-name.md"  # Keep existing value
  created: "2025-01-10"  # Keep existing value
  last_updated: "2025-01-15"  # Update to today
  version: "1.2"  # Keep existing value
  status: "reviewed"  # Keep existing value
  word_count: 2847  # Update with current count
  reading_time_minutes: 11  # Update based on word_count / 250
  primary_topic: "Topic Name"  # Keep existing value

# PRESERVE ALL OTHER SECTIONS UNCHANGED:
# - validations.grammar
# - validations.understandability  
# - validations.structure
# - validations.facts
# - validations.logic
# - cross_references.related_articles
# - cross_references.series
# - cross_references.prerequisites
---
```

**Critical**: Use YAML merge to preserve other validation sections - never delete them.

## Standards
- Target Grade 9-10 reading level for general technical content
- Allow higher complexity for advanced topics
- Balance accessibility with technical accuracy
- Preserve necessary technical terminology
- Suggest simplifications without dumbing down content

## Example Output

When readability validation complete:
```
✅ Readability Review Complete

**Summary**: Flesch Score: 64.3 | Grade Level: 9.1 | Assessment: Good

**Key Findings**:
- 2 redundant explanations in sections 3 and 5
- Inconsistent terminology: "function" vs "method" (use "method" for OOP)
- Excellent flow and transitions

**Recommendations**:
1. Remove duplicate explanation of authentication flow in section 5
2. Standardize on "method" terminology throughout
3. Consider breaking 350-word paragraph in section 2

**Metadata Updated**: Bottom YAML block updated with readability validation results.

**Next Steps**: Consider running `understandability-review` for audience comprehension.
```
