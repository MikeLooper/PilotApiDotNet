<a id="readme-top"></a>
<!-- PROJECT SHIELDS -->
<!--
*** This file is using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![project_license][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]



<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/MikeLooper/repo_name">
    <img src="images/logo.png" alt="Logo" width="660" height="350">
  </a>

<h3 align="center">Pilot API</h3>

  <p align="center">
    A proof of concept API to explore best-practices and new ideas
    <br />
    <a href="https://github.com/MikeLooper/repo_name"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/MikeLooper/repo_name">View Demo</a>
    &middot;
    <a href="https://github.com/MikeLooper/repo_name/issues/new?labels=bug&template=bug-report---.md">Report Bug</a>
    &middot;
    <a href="https://github.com/MikeLooper/repo_name/issues/new?labels=enhancement&template=feature-request---.md">Request Feature</a>
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#copilot-customization">Copilot Customization</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

A proof of concept API to explore best-practices and new ideas

<p align="right">(<a href="#readme-top">back to top</a>)</p>


### Built With

* [![Bruno][bruno-badge]][bruno-url]
* [![C#][csharp-badge]][csharp-url]
* [![GitHub Copilot][githubcopilot-badge]][githubcopilot-url]
* [![Microsoft SQL Server][mssql-badge]][mssql-url]
* [![OpenAPI][openapi-badge]][openapi-url]
* [![Postgres][postgres-badge]][postgres-url]
* [![Swagger][swagger-badge]][swagger-url]
* [![Visual Studio][visualstudio-badge]][visualstudio-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started

This application depends on Docker for databases and an API deployment location.

Docker install and setup details can be found here: [Local Docker](https://github.com/MikeLooper/Docker)

The design of this application was based upon the OpenAPI specification, found in the shared\PilotSharedSource directory (which is a submodule of [PilotSharedSource](https://github.com/MikeLooper/PilotSharedSource)).

### Prerequisites

- [Visual Studio 2026](https://visualstudio.microsoft.com/vs/)

### Installation

1. Clone the repo (including submodules)
   ```
    git clone --recurse-submodules https://github.com/MikeLooper/PilotApiDotNet.git
   ```
2. If the repository was cloned without submodules, initialize them:
    ```
    git submodule update --init --recursive
    ```

3. Open the .sln file in Visual Studio.

4. Press F5 to build and run the application.

### Submodule Management

This repository includes `PilotSharedSource` as a Git submodule at `shared/PilotSharedSource/`.

To pull the latest submodule changes from its tracked branch:

```
git submodule update --remote --recursive shared/PilotSharedSource
```

After updating, commit the changed submodule pointer in this repository:

```
git add shared/PilotSharedSource .gitmodules
git commit -m "Update PilotSharedSource submodule"
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Local Development

When Executing locally, a User Secrets file will be needed to provide local connection details.

This User Secrets file will look like the following:
```json
{
    "Application": {
        "DataConnections": [
            {
                "Active": true,
                "ConnectTimeout": 30,
                "DataSourceName": "NorthWind_SQL",
                "Host": "localhost",
                "Password": "<DevUser password for SQL Server>",
                "Port": 1433,
                "UserName": "DevUser"
            },
            {
                "Active": false,
                "ConnectTimeout": 30,
                "DataSourceName": "NorthWind_Pgs",
                "Host": "localhost",
                "Password": "<DevUser password for PostgreSQL>",
                "Port": 5432,
                "UserName": "DevUser"
            }
        ],
        "OpenTelemetry": {
            "Server": "localhost",
            "Port": 4318
        }
    }
}

```

The User Secrets file is located at `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`.

### Configuration

The application configuration is broken into three files:
- appsettings.json
- appsettings.Development.json
- appsettings.Production.json

This separation simplifies changing which data connection is active for a specific deployment and provides separated locations for sensitive values.

The data source values for Development are included in the local development config file, which can be found in the Web project:
- appsettings.Development.json

The data source values for Production are included in the docker-deploy config files, as noted here:
- SQL Server:
    - ..\docker\SqlServer\appsettings.Production.json
- PostgreSQL:
    - ..\docker\PostgreSQL\appsettings.Production.json

#### Example configurations

##### appsettings.json

```json
{
    "Application": {
        "DataSources": [
            {
                "Active": true,
                "DataSourceName": "NorthWind_SQL",
                "DataSource": "NorthWind",
                "DataSourceType": "SqlServer",
                "Schema": "dbo"
            },
            {
                "Active": true,
                "DataSourceName": "NorthWind_Pgs",
                "DataSource": "northwind",
                "DataSourceType": "PostgreSQL",
                "Schema": "pilot"
            }
        ],
        "Security": {
            "Active": true,
            "BaseUrl": "http://local-keycloak:8080",
            "Realm": "local-realm",
            "ClientId": "local-client",
            "RequireHttpsMetadata": false,
            "ClockSkewSeconds": 60
        },
        "OpenApi": {
            "Title": "PilotApiDotNet",
            "Contact": {
                "Email": "MikelLooper@gmail.com",
                "Name": "Michael Looper",
                "URL": "https://github.com/MikeLooper/PilotApiDotNet"
            },
            "Description": "A proof of concept API to explore best-practices and new ideas (.NET/C#)",
            "License": "MIT",
            "Summary": "Proof of concept API",
            "Version": "0.1.1"
        },
        "OpenTelemetry": {
            "Server": "otel-collector",
            "Port": 4318
        }
    },
    "Serilog": {
        "MinimumLevel": {
            "Default": "Information",
            "Override": {
                "Microsoft": "Warning",
                "Microsoft.AspNetCore.Hosting.Diagnostics": "Error",
                "Microsoft.Hosting.Lifetime": "Information",
                "System": "Warning"
            }
        },
        "WriteTo": [
            { "Name": "Console" },
            {
                "Name": "File",
                "Args": {
                    "path": "logs/log-.json",
                    "rollingInterval": "Day",
                    "rollOnFileSizeLimit": true,
                    "fileSizeLimitBytes": 104857600,
                    "retainedFileCountLimit": 14,
                    "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
                }
            }
        ],
        "Enrich": [
            "FromLogContext",
            "WithMachineName",
            "WithProcessId",
            "WithThreadId",
            "WithExceptionDetails"
        ]
    },
    "AllowedHosts": "*"
}
```

##### appsettings_dataconnections.json

```json
{
    "Application": {
        "DataConnections": [
            {
                "Active": true,
                "ConnectTimeout": 30,
                "DataSourceName": "NorthWind_SQL",
                "Host": "local_mssql",
                "Password": "<DevUser password>",
                "Port": 1433,
                "UserName": "DevUser"
            },
            {
                "Active": false,
                "ConnectTimeout": 30,
                "DataSourceName": "NorthWind_Pgs",
                "Host": "local_postgres",
                "Password": "<DevUser password>",
                "Port": 5432,
                "UserName": "DevUser"
            }
        ]
    }
}
```

The configuration that controls the application logic is in the "Application" section.

#### DataConnections

An array of data connections settings.

##### Active

Is the current section of settings active?  Available options: true, false.

Within the DataConnections section, only one setting group can be active at one time.

##### ConnectTimeout

The number of seconds for the data source timeout.

##### DataSourceName

The name of the current data source section.

This name will match A DataSources.DataSourceName setting.

##### Host

The name of the host for the data source.

When running locally for development, this value would typically be "localhost".

When deployed to a Docker container, this value would typically be the name of the data source container.

Examples: "local_mssql", "local_postgres"

##### Password

The password for the user for the data source.

##### Port

The active port for the target data source.

##### UserName

The name of the user for the data source.

#### DataSources

The available data sources (such as a database).

##### Active

Is the current section of settings active?  Available options: true, false.

##### DataSourceName

The name of the current data source section.

This name will match A DataConnections.DataSourceName setting.

##### DataSource

The name of the data source, such as a database name.

##### DataSourceType

The type of data source.
Available values: "SqlServer", "PostgreSQL"

##### Schema

The schema where the target tables would be found.

#### Security

The settings that control authentication (JWT/OAuth2) and role-based authorization against Security. See [Calling a Secured Endpoint](#calling-a-secured-endpoint) for usage details.

##### Active

Is the current section of settings active?  Available options: true, false.

When true, a missing/invalid token or an insufficient role for the requested HTTP verb returns a 401/403 response.

When false, the same failures do not block the request — it proceeds to the controller action, but the response includes a `Warning` header describing what failed. This is intended for staged rollout or incident response, not for production use.

##### BaseUrl

The base URL of the Security server. Example (local development): "http://localhost:55001". Example (Docker deployment): "http://local-keycloak:8080".

##### Realm

The Security realm. Example: "local-realm".

##### ClientId

The Security client Id. Example: "local-client".

##### RequireHttpsMetadata

Whether HTTPS is required when retrieving Security's metadata/signing keys. Should be true in production; false is typical for local development against an HTTP Security instance.

##### ClockSkewSeconds

The clock skew tolerance, in seconds, applied when validating token expiry. Defaults to 60 (tighter than the framework default of 300), so a token isn't rejected purely due to minor clock drift between the API host and Security.

#### OpenApi

The settings that control how the OpenAPI specification is define within the application.

These values will typically appear in an API UI or in an extracted OpenAPI specification.

##### Title

The API title.

##### Contact

A section of settings regarding who the point of contact is for this application.

###### Email

The email address of the contact person.

###### Name

The name of the contact person.

###### URL

A web address relating to the contact person.

##### Description

A description of this API.

##### License

The license relating to the source code for this application.

##### Summary

A summary of this API.

##### Version

The application version.

#### OpenTelemetry

The settings that control how OpenTelemetry (OTEL) is configured within the application.

##### Server

The OpenTelemetry server address.

##### Port

The OpenTelemetry server port.


### Troubleshooting

#### Port Tracing

When troubleshooting OTEL, you can check for port usages, at the command line:
```
netstat -ano | findstr 4318
```

When working correctly, this will result in something similar to the following:
```
   Proto  Local Address          Foreign Address        State           PID
   ...
   TCP    0.0.0.0:4318           0.0.0.0:0              LISTENING       25384
   TCP    [::]:4318              [::]:0                 LISTENING       25384
   TCP    [::1]:4318             [::]:0                 LISTENING       46888
   TCP    [::1]:4318             [::1]:59028            TIME_WAIT       0
   ...
```

The, start the command (via Win+R) the `resmon.exe` application.  Locate the PID, from the port listing, on the PID column.

Examples fro the above:
| PID | Application |
| --- | ----------- |
| 25384 | Docker Desktop Backend |
| 46888 | Windows Subsystem for Linux |

## OpenTelemetry

The application exports traces, metrics, and logs via OpenTelemetry (OTEL) auto-instrumentation.

Telemetry is sent over OTLP/gRPC to a local OpenTelemetry Collector, which routes it to a Grafana LGTM stack (Tempo for traces, Mimir for metrics, Loki for logs):

```
PilotApiDotNet --OTLP/gRPC--> otel-collector --> Tempo / Mimir / Loki --> Grafana
```

View traces, metrics, and logs in Grafana at `http://localhost:3000`.

### Querying Logs in Grafana

Application logs land in Loki. In Grafana, open **Explore**, select the **Loki** datasource, and query using [LogQL](https://grafana.com/docs/loki/latest/query/), filtering on these labels:

| Label | Example value | Purpose |
|---|---|---|
| `service_name` | `PilotApiDotNet` | Scope to this API |
| `deployment_environment` | `development` / `production` | Filter by environment (from `OTEL_DEPLOYMENT_ENVIRONMENT`) |
| `detected_level` / `severity_text` | `info`, `warn`, `error` | Filter by log level |
| `code_namespace` | `PilotApi.Shared.Helpers.SecurityHelper` | Filter by originating Java class |
| `host_name` | *(machine name)* | Filter by the host the API ran on |

Example queries:

```logql
# All PilotApiDotNet logs
{service_name="PilotApiDotNet"}

# Errors and warnings only
{service_name="PilotApiDotNet", detected_level=~"error|warn"}

# Logs from a specific class
{service_name="PilotApiDotNet", code_namespace="PilotApi.Shared.Helpers.SecurityHelper"}

# Production logs mentioning a specific route
{service_name="PilotApiDotNet", deployment_environment="production"} |= "/v1/categories"
```

Each log entry also carries `trace_id`/`span_id` (when logged within a traced request), so you can jump from a log line directly to its trace in Tempo.

## Deployment

This application will be deployed to a Docker container.
Deploy instructions can be found in the [Docker README](..\docker\README.md).

Once deployed to Docker, the application will be accessible at `https://localhost:55551/...`.

For Exmaple, the Categories endpoint for GetAll will be accessible at `http://localhost:55551/categories/get-all`.

<!-- USAGE EXAMPLES -->
## Usage

The API contract for this project is defined in the OpenAPI specification file located at `docs/openapi.yaml`.
You can use this file to generate client code or to explore the API using tools like Swagger UI or Bruno.

To get a visual representation of the API, you can use the Swagger editor by navigating to `https://editor.swagger.io/`.

You can also interact with the API using the Swagger UI by navigating to `https://localhost:5001/swagger` after running the application locally with Visual Studio.

### Calling a Secured Endpoint

All endpoints are secured with a Security-issued JWT (OAuth2), except `/healthcheck` and `/about`, which always remain open.

#### Obtaining a token

For local development, obtain an access token directly from Security using the Resource Owner Password grant:

```
curl -X POST "http://localhost:55001/realms/local-realm/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=local-client" \
  -d "username=working_admin_user" \
  -d "password=<the user's password>"
```

> Note: the Resource Owner Password grant sends a user's password directly from the calling client, which is convenient for local development and scripting, but should not be used by real/production clients — those should use the Authorization Code flow with PKCE instead.

> **Prerequisite:** the `local-client` client in Keycloak must have **Direct access grants** enabled (Keycloak Admin Console → realm `local-realm` → Clients → `local-client` → Settings/Capability config). Without it, Keycloak rejects every request from this grant type with `400 {"error":"unauthorized_client","error_description":"Client not allowed for direct access grants"}` — before it even checks the username/password — and neither this `curl` command nor the Bruno collection below can obtain a token.

#### Calling an endpoint

Include the returned `access_token` as a bearer token on the request:

```
curl -H "Authorization: Bearer <access_token>" "http://localhost:55551/v1/categories/get-all"
```

#### Roles

Roles are not read from Security token role claims. They are looked up (via a `UserRoles` repository) using the token's `preferred_username` claim:

| UserId | Role | Allowed HTTP Methods |
| --- | --- | --- |
| reader_user | ReadOnly | GET |
| working_user | ReadWrite | GET, POST, PUT |
| working_admin_user | Admin | GET, POST, PUT, DELETE |

A request using a verb outside of the assigned role's allowed methods is treated as an authorization failure.

#### Response behavior

- **401 Unauthorized** — the request had no token, or the token was missing/invalid/expired, and `Security.Active` is `true`.
- **403 Forbidden** — the request was authenticated, but the resolved role does not permit the requested HTTP verb, and `Security.Active` is `true`.
- **`Security.Active` = `false`** — either of the failures above no longer blocks the request; it proceeds to the controller action, but the response includes a `Warning` header describing what failed (e.g. "Token expired.", "Missing or invalid bearer token.", or the specific role/verb mismatch). This is intended for staged rollout or incident response, not for production use.

#### Token expiry and refresh

This API only validates bearer tokens; it does not issue or refresh them (it is a Security resource server, not a token issuer). Token expiry is enforced on every request, with a small clock-skew tolerance (`ClockSkewSeconds`, default 60 seconds) so a token isn't rejected purely due to minor clock drift between the API host and Security.

When a request fails with `401` because the access token expired, the client must call Security's token endpoint again with `grant_type=refresh_token` (using the `refresh_token` value returned alongside the original `access_token`) to obtain a new `access_token`, then retry the original request:

```
curl -X POST "http://localhost:55001/realms/local-realm/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=refresh_token" \
  -d "client_id=local-client" \
  -d "refresh_token=<refresh_token from the original token response>"
```

#### Bruno collection

The Bruno collection (`test/Bruno/PilotApiDotNet`) automates the token flow described above instead of requiring the manual `curl` steps. It has the same prerequisite as the `curl` flow above — the `local-client` Keycloak client must have **Direct access grants** enabled, or every "Get Access Token" attempt fails with a generic "No access token received" error in the Bruno UI (the underlying `400 unauthorized_client` from Keycloak isn't surfaced).

Authentication is configured once, as an OAuth2 "Password Credentials" auth block on the collection root (`test/Bruno/PilotApiDotNet/opencollection.yml`). Every folder and request in the collection defaults to `auth: inherit`, and Bruno resolves that by walking up the tree to the nearest folder with a concrete (non-`inherit`, non-`none`) auth override, falling all the way back to the collection root if none is found — so this single config is automatically picked up by `v1` today, and by any future `v2` folder added as a sibling, with nothing further to configure.

Bruno's inheritance only stops at a folder when that folder or one of its own requests carries an explicit, concrete auth override — a folder set to `none` is itself skipped when resolving its descendants, exactly like `inherit` is. Because of that, the three requests under `System` (`About_NoDetails`, `About_WithDetails`, `HealthCheck`) each carry no `auth` entry of their own (equivalent to `none`) directly on the request, not on the `System` folder — that is what actually stops them from picking up the collection-level token. Any new request added under `System` must do the same (omit `auth`, or explicitly set it, rather than leaving the inherited default) to stay token-free.

Bruno fetches an access token from Security's token endpoint (`{{IdpHostBaseUrl}}/realms/{{IdpHostRealm}}/protocol/openid-connect/token`) the first time a request under `v1` is sent, caches it, adds it to the request as `Authorization: Bearer <access_token>`, and automatically refreshes it (using the `refresh_token` grant) once it expires. No manual token copy/paste is required.

The collection root also sets two headers (`Accept: application/json`, `ApiVersion: 1`) that apply to every request, `System` included — headers merge cumulatively from the collection root down through each folder to the request, so there is no need to repeat them anywhere else in the tree.

> **Note:** `test/Bruno/opencollection.yml` (one directory above the collection) is not part of this collection and has no effect on it. Bruno collections are self-contained — everything (auth, headers, variables) is scoped to the single directory containing the collection's own `opencollection.yml` (here, `test/Bruno/PilotApiDotNet`); nothing cascades in from a parent directory. The only cross-collection concept Bruno has is a *workspace*, which requires a file literally named `workspace.yml` and, even then, only shares named global environments — never headers or auth. Define any collection-wide request headers/auth directly in `test/Bruno/PilotApiDotNet/opencollection.yml`, not in the outer file.
>
> Also: if this collection is open in the Bruno app while these files are edited by hand, saving anything from Bruno's UI rewrites the whole file from Bruno's in-memory copy, silently reverting the on-disk changes. Close the collection in Bruno (or reload it) after editing these files externally, and re-open/reload it before making further changes in the app.

##### Collection variables

The token request is built from variables defined for the collection:

| Variable | Purpose |
| --- | --- |
| `IdpHostBaseUrl` | Base URL of the Security server, matching `Application.Security.BaseUrl`. |
| `IdpHostRealm` | The Security realm, matching `Application.Security.Realm`. |
| `IdpHostClientId` | The Security client Id, matching `Application.Security.ClientId`. |
| `IdpHostUsername` | The user to authenticate as. Defaults to `working_admin_user` (Admin role), so every request in the collection — including `Add`, `Update`, and `Delete` — works without editing anything. |

##### Environment variables

The token request also includes a variable defined for the general environment:

| Variable | Purpose |
| --- | --- |
| `IDP_HOST_PASSWORD` | The password for `IdpHostUsername`. |

The above value is stored in a `.env` file inside the collection root, `test/Bruno/PilotApiDotNet/.env` (Bruno only looks for `.env` directly alongside the collection's `opencollection.yml`, not in a parent folder). This file must be created manually before opening Bruno:
1. Copy `test/Bruno/PilotApiDotNet/.env.example` to `test/Bruno/PilotApiDotNet/.env`.
2. Change the `<host-password>` value to the current password for the `IdpHostUsername` user.

`.env` is git-ignored; `.env.example` is intentionally excluded from that ignore rule so it stays committed as the template for other developers.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ROADMAP -->
## Roadmap

- [x] API (.NET) version of the API (this code)
- [x] Java version of the API
- [x] Deploy API to Docker
- [x] Angular Frontend User Interface (UI) to consume APIs in Docker

See the [open issues](https://github.com/MikeLooper/PilotApi/issues) for a full list of proposed features (and known issues).

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Top contributors:

<a href="https://github.com/MikeLooper/PilotApi/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=MikeLooper/PilotApi" alt="contrib.rocks image" />
</a>



<!-- COPILOT CUSTOMIZATION -->
## Copilot Customization

This repository uses two Copilot instruction layers:

- Repository-wide guidance: `.github/copilot-instructions.md`
- Unit-test-specific guidance: `.github/instructions/unit-tests.instructions.md`

How to use them:

- The repository-wide file is intentionally minimal and applies across all work.
- The unit-test file is scoped to `test/**/*.cs` and applies to unit test creation and maintenance tasks.
- For unit test work, follow the NUnit and test-structure rules in the scoped instruction file.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTACT -->
## Contact

Michael Looper - MikelLooper@gmail.com

Project Link: [https://github.com/MikeLooper/PilotApi](https://github.com/MikeLooper/PilotApi)

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

* [TBD]()
* [TBD]()
* [TBD]()

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/MikeLooper/PilotApi.svg?style=for-the-badge
[contributors-url]: https://github.com/MikeLooper/PilotApi/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/MikeLooper/PilotApi.svg?style=for-the-badge
[forks-url]: https://github.com/MikeLooper/PilotApi/network/members
[stars-shield]: https://img.shields.io/github/stars/MikeLooper/PilotApi.svg?style=for-the-badge
[stars-url]: https://github.com/MikeLooper/PilotApi/stargazers
[issues-shield]: https://img.shields.io/github/issues/MikeLooper/PilotApi.svg?style=for-the-badge
[issues-url]: https://github.com/MikeLooper/PilotApi/issues
[license-shield]: https://img.shields.io/github/license/MikeLooper/PilotApi.svg?style=for-the-badge
[license-url]: https://github.com/MikeLooper/PilotApi/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/michaellooper
[product-screenshot]: images/screenshot.png
<!-- Shields.io badges. You can a comprehensive list with many more badges at: https://github.com/inttter/md-badges -->
[bruno-badge]: https://img.shields.io/badge/Bruno-F4AA41?logo=Bruno&logoColor=black
[bruno-url]: https://www.usebruno.com/
[csharp-badge]: https://custom-icon-badges.demolab.com/badge/C%23-%23239120.svg?logo=cshrp&logoColor=white
[csharp-url]: https://learn.microsoft.com/en-us/dotnet/csharp/
[githubcopilot-badge]: https://img.shields.io/badge/GitHub%20Copilot-000?logo=githubcopilot&logoColor=fff
[githubcopilot-url]: https://github.com/copilot
[mssql-badge]: https://custom-icon-badges.demolab.com/badge/Microsoft%20SQL%20Server-CC2927?logo=mssqlserver-white&logoColor=white
[mssql-url]: https://www.microsoft.com/en-us/sql-server
[openapi-badge]: https://img.shields.io/badge/OpenAPI-6BA539?logo=openapiinitiative&logoColor=white
[openapi-url]: https://www.openapis.org/
[postgres-badge]: https://img.shields.io/badge/Postgres-%23316192.svg?logo=postgresql&logoColor=white
[postgres-url]: https://www.postgresql.org/
[swagger-badge]: https://img.shields.io/badge/Swagger-85EA2D?logo=swagger&logoColor=173647
[swagger-url]: https://swagger.io/
[visualstudio-badge]: https://custom-icon-badges.demolab.com/badge/Visual%20Studio-5C2D91.svg?&logo=visualstudio&logoColor=white
[visualstudio-url]: https://visualstudio.microsoft.com/
