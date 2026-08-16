---
mode: agent
description: Runs .NET tests and generates XPlat Code Coverage for a specific project.
---

# Run tests and coverage

Run the .NET tests in this repository and generate a code coverage report, including an HTML version for visual inspection.

## Instructions

1. Use the test project path provided by the user.
2. If no path is provided, default to:
   - tests/NetAspireServer.Application.Tests.csproj
3. Run the command:
   - dotnet test <project-path> --collect:"XPlat Code Coverage" --results-directory TestResults
4. After the test run, generate an HTML coverage report from the Cobertura XML with ReportGenerator using:
   - reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:Html
5. Show:
   - the summary of the executed tests
   - whether the coverage report was generated
   - the path of the generated XML coverage file
   - the path of the generated HTML coverage report
6. If tests fail, stop and summarize the errors clearly.

## Expected behavior

- Work from the repository root.
- Use coverage collection explicitly.
- Generate both the raw XML and the HTML report.
- If the project does not exist, report it and suggest the correct path.
