# Daily Scrum EmailR

[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)

## 🤔 What is it?

A tool to generate a daily scrum emails and timesheet notes based off tasks stored in Microsoft To Do.

## 📺 Video

https://youtu.be/ZFV-UH5eZ7M

## ✨ Features

- Integration with Microsoft Todo via Graph API
- Generation of Daily Scrum emails
- Generation of Timesheet notes

## 🎉 Getting Started

### .NET

```bash
dotnet run
```

### Docker

Using docker can be helpful to always keep the application running in a container.

```bash
docker compose up -d
```

NOTE: You'll need to run `docker compose build` first to update an existing container

## Usage

Once the website is running you will need to generate an Access Token from Microsoft Graph API.

### Generating a Microsoft Graph Access Token

1. Visit https://developer.microsoft.com/en-us/graph/graph-explorer
2. Ensure you're logged on
3. Execute the `https://graph.microsoft.com/v1.0/me` query
4. Copy the `access_token` value from the response

## Authors

- [@danielmackay](https://www.github.com/danielmackay)

## Tech Stack

- .NET 9
- ASP.NET Core
- Razor Pages
- VSA
- Boostrap
- Docker

[//]: # (## Screenshots)
[//]: # ()
[//]: # (TBC)

## Roadmap

- Single-Sign On Support
- Sending of Emails
