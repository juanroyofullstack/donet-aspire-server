---
mode: agent
description: Runs .NET tests and generates XPlat Code Coverage for a specific project.
---

# Run tests and coverage

Run the .NET tests in this repository and generate a code coverage report.

## Instructions

1. Use the test project path provided by the user.
2. If no path is provided, default to:
   - tests/NetAspireServer.Application.Tests.csproj
3. Run the command:
   - dotnet test <project-path> --collect:"XPlat Code Coverage" --results-directory TestResults
4. Show:
   - the summary of the executed tests
   - whether the coverage report was generated
   - the path of the generated coverage file
5. If tests fail, stop and summarize the errors clearly.

## Expected behavior

- Work from the repository root.
- Use coverage collection explicitly.
- If the project does not exist, report it and suggest the correct path.
