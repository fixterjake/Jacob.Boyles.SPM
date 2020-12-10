# Simple Project Management

### CPSC 498 Capstone Project by Jacob Boyles


Master branch build status: [![Build Status](https://dev.azure.com/fixterjake0899/Capstone/_apis/build/status/fixterjake.SimpleProjectManagement?branchName=master)](https://dev.azure.com/fixterjake0899/Capstone/_build/latest?definitionId=11&branchName=master)  
Develop branch build status: [![Build Status](https://dev.azure.com/fixterjake0899/Capstone/_apis/build/status/fixterjake.SimpleProjectManagement?branchName=develop)](https://dev.azure.com/fixterjake0899/Capstone/_build/latest?definitionId=11&branchName=develop)  

## Project Overview

A simple web application for use in project management.

This application will loosely follow the scrum philosophy of software development.
Starting from creating sprints, adding items to the sprint, and tasks to individual items.
It will also include permissions for adding members to sprints, items, and tasks, giving those members permissions to edit based on roles.

## Technologies

Base Web Framework: [ASP.Net Core](https://dotnet.microsoft.com/apps/aspnet).  
Database Framework: [Entity Framework](https://docs.microsoft.com/en-us/ef/).  
User Management: [ASP.Net Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-3.1&tabs=visual-studio).  
Relational Database: [MySQL Server](https://dev.mysql.com/downloads/mysql/).

## Requirements

- Windows or Linux
- Web server of your choice (Must support proxy)
- MySQL
- Dotnet 5 SDK
- Entity framework Dotnet CLI tool
- AWS credentials, find out more [here](https://docs.aws.amazon.com/cli/latest/userguide/cli-configure-files.html)

## Deployment
1. Clone the repository from github
2. Create a new file `appsettings.json` and copy the json from appsettings.example.json
3. Fill out all the fields in `appsettings.json`
4. Change into the project directory and run `dotnet-ef database update` (This will use the connection string from `appsettings.json`)
5. Run `dotnet publish`, and copy the output to your location of choice
6. Setup your web server as a proxy to `localhost:5100`
7. Perform first time setup, and you're done!'