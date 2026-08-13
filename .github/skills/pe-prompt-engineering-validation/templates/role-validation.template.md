# Role Validation Template

## Role Under Test

> **Role statement:** [Paste the role/identity being validated]  
> **Target goal:** [The goal this role serves]

## Test 1: Authority

**Question:** Can this role make the necessary judgments for the goal?

| Judgment Required | Role Has Authority? | Evidence |
|------------------|--------------------:|----------|
| [e.g., "Evaluate grammar correctness"] | ✅ / ❌ | [Why/why not] |
| [e.g., "Accept or reject code changes"] | ✅ / ❌ | [Why/why not] |
| [e.g., "Define architecture decisions"] | ✅ / ❌ | [Why/why not] |

**Authority Result:** ✅ PASS / ❌ FAIL — [Summary]

## Test 2: Expertise

**Question:** Does this role imply the required domain knowledge?

| Knowledge Required | Role Implies It? | Evidence |
|-------------------|----------------:|----------|
| [e.g., "Markdown formatting rules"] | ✅ / ❌ | [Why/why not] |
| [e.g., "C# async patterns"] | ✅ / ❌ | [Why/why not] |
| [e.g., "Azure deployment"] | ✅ / ❌ | [Why/why not] |

**Expertise Result:** ✅ PASS / ❌ FAIL — [Summary]

## Test 3: Specificity

**Question:** Is this role concrete enough to guide behavior?

| Criterion | Assessment |
|-----------|-----------|
| **Generic test**: Could "helpful assistant" produce same output? | Yes → ❌ FAIL / No → ✅ |
| **Substitution test**: Replace role with different specialist — does output change? | No → ❌ FAIL / Yes → ✅ |
| **Domain signal**: Does role name contain domain-specific terminology? | No → ⚠️ / Yes → ✅ |

**Specificity Result:** ✅ PASS / ❌ FAIL — [Summary]

## Overall Assessment

| Test | Result | Notes |
|------|--------|-------|
| Authority | ✅ / ❌ | |
| Expertise | ✅ / ❌ | |
| Specificity | ✅ / ❌ | |

**Verdict:** ✅ Role is valid / ❌ Role needs revision

## Recommended Fix (if needed)

> **Original role:** [Original]  
> **Improved role:** [Improved version with specific domain qualifiers]  
> **Rationale:** [What was added and why]
