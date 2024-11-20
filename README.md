<div align="center">

<h1><img src="https://github.com/user-attachments/assets/933c8b45-a190-4df6-913e-b7c64ad9938b" width="40" /> AliasVault</h1>

<p align="center">
<a href="https://app.aliasvault.net">Live demo 🚀</a> • <a href="https://aliasvault.net?utm_source=gh-readme">Website 🏠</a> • <a href="#installation">Installation 📦</a>
</p>

<h3 align="center">
Open-source password and alias manager
</h3>

[<img src="https://img.shields.io/github/v/release/lanedirt/AliasVault?include_prereleases&logo=github">](https://github.com/lanedirt/AliasVault/releases)
[<img src="https://img.shields.io/github/actions/workflow/status/lanedirt/AliasVault/docker-compose-build.yml?label=docker-compose%20build">](https://github.com/lanedirt/AliasVault/actions/workflows/docker-compose-build.yml)
[<img src="https://img.shields.io/github/actions/workflow/status/lanedirt/AliasVault/dotnet-unit-tests.yml?label=unit tests">](https://github.com/lanedirt/AliasVault/actions/workflows/dotnet-build-run-tests.yml)
[<img src="https://img.shields.io/github/actions/workflow/status/lanedirt/AliasVault/dotnet-integration-tests.yml?label=integration tests">](https://github.com/lanedirt/AliasVault/actions/workflows/dotnet-build-run-tests.yml)
[<img src="https://img.shields.io/github/actions/workflow/status/lanedirt/AliasVault/dotnet-e2e-client-tests.yml?label=e2e tests">](https://github.com/lanedirt/AliasVault/actions/workflows/dotnet-e2e-client-tests.yml)
[<img src="https://img.shields.io/sonar/coverage/lanedirt_AliasVault?server=https%3A%2F%2Fsonarcloud.io&label=test code coverage">](https://sonarcloud.io/summary/new_code?id=lanedirt_AliasVault)
[<img src="https://img.shields.io/sonar/quality_gate/lanedirt_AliasVault?server=https%3A%2F%2Fsonarcloud.io&label=sonarcloud&logo=sonarcloud">](https://sonarcloud.io/summary/new_code?id=lanedirt_AliasVault)
</div>

AliasVault is an open-source password and alias manager built with C# ASP.NET technology. AliasVault can be self-hosted on your own server with Docker, providing a secure and private solution for managing your online identities and passwords.

### What makes AliasVault unique:
- **Zero-knowledge architecture**: All data is end-to-end encrypted on the client and stored in encrypted state on the server. Your master password never leaves your device and the server never has access to your data.
- **Built-in email server**: AliasVault includes its own email server that allows you to generate virtual email addresses for each alias. Emails sent to these addresses are instantly visible in the AliasVault app.
- **Alias generation**: Generate aliases and assign them to a website, allowing you to use different email addresses and usernames for each website. Keeping your online identities separate and secure, making it harder for bad actors to link your accounts.
- **Open-source**: The source code is available on GitHub and can be self-hosted on your own server.

> Note: AliasVault is currently in active development and some features may not yet have been (fully) implemented. If you run into any issues, please create an issue on GitHub.

## Live demo
A live demo of the app is available at the official website at [app.aliasvault.net](https://app.aliasvault.net) (up-to-date with `main` branch). You can create a free account to try it out yourself.

<img width="700" alt="Screenshot of AliasVault" src="docs/img/screenshot.png">

## Installation

Choose one of the following installation methods:

### Option 1: Quick Install (Pre-built Images)

This method uses pre-built Docker images and works on minimal hardware specifications:
- Linux (Ubuntu or RHEL based distros recommended)
- 512MB RAM
- 1 vCPU
- At least 16GB disk space
- Docker installed

```bash
# Download install script
curl -o install.sh https://raw.githubusercontent.com/lanedirt/AliasVault/main/install.sh

# Make install script executable and run it. This will create the .env file, pull the Docker images, and start the AliasVault containers.
chmod +x install.sh
./install.sh install
```

### Option 2: Build from Source

Building from source requires more resources:
- Minimum 2GB RAM (more RAM will speed up build time)
- At least 1 vCPU
- 40GB+ disk space (for dependencies and build artifacts)
- Docker installed
- Git installed

```bash
# Clone the repository
git clone https://github.com/lanedirt/AliasVault.git
cd AliasVault

# Make build script executable and run it. This will create the .env file, build the Docker images from source, and start the AliasVault containers.
chmod +x install.sh
./install.sh build
```

Note: If you do not wish to run the script, you can set up the environment variables and build the Docker image and containers manually instead. See the [manual setup instructions](docs/install/1-manually-setup-docker.md) for more information.

### Post-Installation

The install script will output the URL where the app is available. By default this is:
- Client: https://localhost
- Admin portal: https://localhost/admin

> Note: If you want to change the default AliasVault ports you can do so in the `docker-compose.yml` file for the `nginx` (reverse-proxy) container.

#### First Time Setup Notes:
- When building from source for the first time, it may take several minutes for Docker to download and compile all dependencies. Subsequent builds will be faster.
- A SQLite database file will be created in `./database/AliasServerDb.sqlite`. This file will store all (encrypted) password vaults. It should be kept secure and not shared.

#### Useful Commands:
- To reset the admin password: `./install.sh reset-password`
- To uninstall AliasVault: `./install.sh uninstall`
  This will remove all containers, images, and volumes related to AliasVault while keeping configuration files intact for future reinstallation.
- If something goes wrong you can run the install script in verbose mode to get more information: `./install.sh [command] --verbose`

## Security Architecture
AliasVault takes security seriously and implements various measures to protect your data:

- All sensitive user data is encrypted end-to-end using industry-standard encryption algorithms. This includes the complete vault contents and all received emails.
- Your master password never leaves your device.
- Zero-knowledge architecture ensures the server never has access to your unencrypted data

For detailed information about our encryption implementation and security architecture, see the following documents:
- [SECURITY.md](SECURITY.md)
- [Security Architecture Diagram](docs/security-architecture.md)

## Tech stack / credits
The following technologies, frameworks and libraries are used in this project:

- [C#](https://docs.microsoft.com/en-us/dotnet/csharp/) - A simple, modern, object-oriented, and type-safe programming language.
- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) - An open-source framework for building modern, cloud-based, internet-connected applications.
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - A lightweight, extensible, open-source and cross-platform version of the popular Entity Framework data access technology.
- [Blazor WASM](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) - A framework for building interactive web UIs using C# instead of JavaScript. It's a single-page app framework that runs in the browser via WebAssembly.
- [Playwright](https://playwright.dev/) - A Node.js library to automate Chromium, Firefox and WebKit with a single API. Used for end-to-end testing.
- [Docker](https://www.docker.com/) - A platform for building, sharing, and running containerized applications.
- [SQLite](https://www.sqlite.org/index.html) - A C-language library that implements a small, fast, self-contained, high-reliability, full-featured, SQL database engine.
- [Tailwind CSS](https://tailwindcss.com/) - A utility-first CSS framework for rapidly building custom designs.
- [Flowbite](https://flowbite.com/) - A free and open-source UI component library based on Tailwind CSS.
- [Konscious.Security.Cryptography](https://github.com/kmaragon/Konscious.Security.Cryptography) - A .NET library that implements Argon2id, a memory-hard password hashing algorithm.
- [SRP.net](https://github.com/secure-remote-password/srp.net) - SRP6a Secure Remote Password protocol for secure password authentication.
- [SmtpServer](https://github.com/cosullivan/SmtpServer) - A SMTP server library for .NET that is used for the virtual email address feature.
- [MimeKit](https://github.com/jstedfast/MimeKit) - A .NET MIME creation and parser library used for the virtual email address feature.
