# Contributing to PowerHouse Desktop

Thank you for considering contributing to PowerHouse Desktop!

## Development Setup

1. Clone the repository
2. Open `PowerHouse_desktop.sln` in Visual Studio 2022
3. Restore NuGet packages
4. Copy `PowerHouseMonitor/App.config.example` to `PowerHouseMonitor/App.config`
5. Configure your local database connection

## Coding Standards

- Follow Microsoft C# Coding Conventions
- Use meaningful names for variables, methods, and classes
- Add XML documentation comments for public APIs
- Keep methods under 50 lines when possible
- Write self-documenting code

## Pull Request Process

1. Create a feature branch from `main`
2. Make your changes
3. Ensure the solution builds without errors
4. Test your changes thoroughly
5. Update documentation if needed
6. Submit a pull request with a clear description

## Commit Messages

- Use present tense ("Add feature" not "Added feature")
- Use imperative mood ("Move cursor to..." not "Moves cursor to...")
- Limit first line to 72 characters
- Reference issues and pull requests when relevant

## Testing

- Test database connectivity with various connection strings
- Verify path resolution in different directory structures
- Test scheduled task scenarios
- Validate error handling and logging

## Code Review Guidelines

### What We Look For

- Code follows established patterns in the codebase
- Changes are well-tested
- Documentation is updated
- No unnecessary dependencies added
- Security best practices followed

### Review Process

1. Automated checks must pass
2. At least one maintainer approval required
3. All comments must be resolved
4. Branch must be up to date with main

## Reporting Bugs

When reporting bugs, please include:

- Operating system and version
- .NET version
- SQL Server version
- Steps to reproduce
- Expected behavior
- Actual behavior
- Error messages or logs

## Suggesting Features

Feature requests should include:

- Clear description of the feature
- Use case and benefits
- Potential implementation approach
- Any breaking changes

## Development Workflow

### Branch Naming

- `feature/short-description` - New features
- `bugfix/short-description` - Bug fixes
- `hotfix/short-description` - Critical production fixes
- `docs/short-description` - Documentation only

### Before Submitting

- [ ] Code builds without warnings
- [ ] All tests pass
- [ ] New code has appropriate tests
- [ ] Documentation updated
- [ ] Commit messages are clear
- [ ] No sensitive data in commits

## Questions?

[Add contact information or discussion forum link]
