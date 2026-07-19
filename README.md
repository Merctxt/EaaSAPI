# EaaSAPI - Email as a Service API

A lightweight, production-ready REST API built with ASP.NET Core 9.0 that enables sending emails through any SMTP server. Clients provide their own SMTP credentials and message content at the time of the request, making the API fully agnostic to mail providers.


## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Running Locally](#running-locally)
  - [Running with Docker](#running-with-docker)
- [Configuration](#configuration)
- [API Reference](#api-reference)
  - [Send Email](#post-apiv1smtpsend)
- [Rate Limiting](#rate-limiting)
- [Project Structure](#project-structure)
- [Development](#development)
- [License](#license)


## Overview

EaaSAPI (Email as a Service API) provides a single, simple HTTP endpoint to send emails. Instead of integrating with a specific email provider's SDK, you call this API with your SMTP server details, sender and recipient information, and the email content. The API handles the SMTP connection, authentication, and message delivery, returning a clear success or error response.

This design decouples email sending logic from your application and makes it easy to centralize, audit, or queue email operations.



## Features

- Single REST endpoint for sending emails via SMTP
- Supports SSL/TLS (port 465) and STARTTLS (port 587)
- HTML email body support
- Multiple recipients via comma-separated addresses
- Per-IP rate limiting (30 requests per 60 minutes)
- Reverse proxy support through forwarded headers
- Interactive API documentation via Scalar UI
- Docker and .NET SDK container support
- Comprehensive error handling and descriptive error messages



## Technology Stack

| Component            | Technology                                     |
|----------------------|------------------------------------------------|
| Runtime              | .NET 9.0                                       |
| Framework            | ASP.NET Core 9.0                                |
| Email Handling       | MailKit 4.17.0 / MimeKit                       |
| API Documentation    | Scalar.AspNetCore 2.16.15 / OpenAPI            |
| Containerization     | Docker (multi-stage build)                     |
| Rate Limiting        | ASP.NET Core built-in rate limiter             |

---

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- (Optional) [Docker](https://www.docker.com/products/docker-desktop/)

### Running Locally

1. Clone the repository:

   ```bash
   git clone <repository-url>
   cd EaaSAPI
   ```

2. Restore dependencies and run the application:

   ```bash
   dotnet restore
   dotnet run --project EaaSAPI
   ```

   The API will be available at `http://localhost:5057` (or `https://localhost:7196` with the HTTPS profile).

3. Open the interactive API documentation:

   ```
   http://localhost:5057/scalar/v1
   ```

### Running with Docker

A multi-stage `Dockerfile` is included for containerized deployment:

```bash
docker build -t eaasapi .
docker run -d -p 5000:5000 --name eaasapi eaasapi
```

The API will be available at `http://localhost:5000`.



## Configuration

All application settings are defined in `appsettings.json` and `appsettings.Development.json`.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

| Setting          | Description                                           | Default     |
|------------------|-------------------------------------------------------|-------------|
| `Logging`        | Configures the logging levels for the application.    | Information |
| `AllowedHosts`   | CORS-allowed hosts. Use `*` to allow all hosts.       | `*`         |

### Environment Variables (Docker)

| Variable                | Description                          | Default           |
|-------------------------|--------------------------------------|-------------------|
| `ASPNETCORE_URLS`       | URL the application binds to.        | `http://+:5000`   |
| `ASPNETCORE_ENVIRONMENT`| Runtime environment.                 | `Production`      |

### Reverse Proxy

When deployed behind a reverse proxy (Nginx, Traefik, Azure Front Door, etc.), the API respects `X-Forwarded-For`, `X-Forwarded-Proto`, and `X-Forwarded-Host` headers. Known networks and proxies are cleared by default, so the API trusts all forwarding sources.



## API Reference

### `POST /api/v1/smtp/send`

Sends an email through the specified SMTP server.

#### Request Body

```json
{
  "smtpConfig": {
    "host": "smtp.example.com",
    "port": 587,
    "secure": false,
    "user": "user@example.com",
    "pass": "your-password"
  },
  "email": {
    "from": "Sender Name <sender@example.com>",
    "to": "recipient1@example.com, recipient2@example.com",
    "subject": "Hello from EaaSAPI",
    "html": "<h1>Hello!</h1><p>This is a test email.</p>"
  }
}
```

#### Fields

| Field                    | Type    | Required | Description                                       |
|--------------------------|---------|----------|---------------------------------------------------|
| `smtpConfig.host`        | string  | Yes      | SMTP server hostname or IP address.               |
| `smtpConfig.port`        | integer | No       | SMTP port (default: `587`). Range: 1-65535.      |
| `smtpConfig.secure`      | boolean | No       | If `true`, uses SSL/TLS (port 465). If `false`, uses STARTTLS (default: `false`). |
| `smtpConfig.user`        | string  | Yes      | SMTP authentication username.                     |
| `smtpConfig.pass`        | string  | Yes      | SMTP authentication password.                     |
| `email.from`             | string  | Yes      | Sender address (e.g., `Name <email@domain.com>`).|
| `email.to`               | string  | Yes      | Recipient(s) separated by commas.                 |
| `email.subject`          | string  | Yes      | Email subject line.                               |
| `email.html`             | string  | Yes      | HTML body of the email.                          |

#### Success Response

```json
{
  "success": true,
  "message": "Email enviado com sucesso."
}
```

#### Error Responses

| Status Code | Condition                                          |
|-------------|----------------------------------------------------|
| 400         | Invalid sender/recipient addresses or authentication failure. |
| 429         | Rate limit exceeded (30 requests per 60 minutes).  |
| 503         | Unable to connect to the specified SMTP server.    |
| 500         | Internal error during email sending.               |

---

## Rate Limiting

The API implements a fixed-window rate limiter to prevent abuse:

- **Limit:** 30 requests per IP address
- **Window:** 60 minutes
- **Queue:** 0 (excess requests are immediately rejected)
- **Response code on rejection:** `429 Too Many Requests`

Rate limiting is enabled in all environments except Development.



## Project Structure

```
EaaSAPI/
  ├── Controllers/
  │   └── SmtpController.cs       # API endpoint for sending emails
  ├── Models/
  │   └── Smtp.cs                 # Request models (SmtpConfig, EmailMessage, SmtpSendRequest)
  ├── Properties/
  │   └── launchSettings.json     # Launch profiles (HTTP, HTTPS, Container)
  ├── appsettings.json            # Production configuration
  ├── appsettings.Development.json# Development configuration
  ├── EaaSAPI.csproj              # Project file with dependencies
  ├── EaaSAPI.http                # HTTP request samples for testing
  └── Program.cs                  # Application entry point and middleware setup
Dockerfile                        # Multi-stage Docker build
LICENSE                           # License information
README.md                         # This file
```



## Development

### Key Dependencies

- **[MailKit](https://github.com/jstedfast/MailKit)** - Cross-platform mail client library for .NET
- **[Scalar.AspNetCore](https://github.com/scalar/scalar)** - Interactive API reference UI for OpenAPI specifications
- **[Microsoft.AspNetCore.OpenApi](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/overview)** - OpenAPI document generation

### Launch Profiles

| Profile              | URL                                          | Description                              |
|----------------------|----------------------------------------------|------------------------------------------|
| `http`               | `http://localhost:5057`                      | HTTP only, Development environment.      |
| `https`              | `https://localhost:7196` / `http://localhost:5057` | HTTP + HTTPS, Development environment.   |
| `Container (.NET SDK)`| Dynamic ports (8080/8081)                   | .NET SDK container, HTTPS enabled.       |

### API Documentation

When the application is running, visit `/scalar/v1` to access the interactive API reference:

- **Local:** `http://localhost:5057/scalar/v1`
- **Docker:** `http://localhost:5000/scalar/v1`



## License

This project is licensed under the **CC0 1.0 Universal** license - see the [LICENSE](LICENSE) file for details. You can copy, modify, distribute, and perform the work, even for commercial purposes, all without asking permission.
