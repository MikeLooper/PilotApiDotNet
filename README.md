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
* [![Docker][docker-badge]][docker-url]
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

### Prerequisites

- [Visual Studio 2026](https://visualstudio.microsoft.com/vs/)

### Installation

1. Clone the repo
   ```
   git clone https://github.com/MikeLooper/PilotApi.git
   ```
2. Open the .sln file in Visual Studio.

3. Press F5 to build and run the application.

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
        ]
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
		}
	},
	"Name": "OpenTelemetry",
	"Args": {
		"endpoint": "http://localhost:4317",
		"protocol": "Grpc",
		"resourceAttributes": {
			"service.name": "serilog-demo-api"
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

### Deployment

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

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ROADMAP -->
## Roadmap

- [x] API (.NET)
- [ ] Angular Frontend User Interface
- [ ] Java Version of the API

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
[docker-badge]: https://img.shields.io/badge/Docker-2496ED?logo=docker&logoColor=fff
[docker-url]: https://www.docker.com/
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
