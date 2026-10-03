---
mode: agent
description: Ensures implementation work is not considered complete unless test coverage stays at or above 90%.
---

# Require 90% test coverage

Use this instruction for any code implementation, fix, refactor, or feature work.

## Mandatory rules

1. Do not consider the implementation complete unless the relevant test project is run with coverage collection enabled.
2. Run tests with:
   - dotnet test <project-path> --collect:"XPlat Code Coverage" --results-directory TestResults --settings coverlet.runsettings
3. If no path is provided, default to:
   - tests/NetAspireServer.Application.Tests.csproj
4. After the run, inspect the generated Cobertura XML and verify the coverage is at least 90%.
5. If the coverage is below 90%, do not finish the task. Add or fix tests until the threshold is met.
6. For human-readable validation, generate an HTML report with:
   - reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:Html
7. Report the final coverage percentage, the XML file path, and the HTML report path in the summary.
8. If the code changes affect a single class or feature, prefer a focused test run, but still require the minimum threshold for the affected scope.
9. If coverage cannot be raised to 90%, explain the blocker and keep the implementation in a non-completed state.

## Expected behavior

- Every implementation must include validation through automated tests.
- Coverage is a completion gate, not an optional extra.
- Aim for stable coverage above 90%, not just a passing test run.
- If tests fail, stop and summarize the errors clearly before proposing a fix.
