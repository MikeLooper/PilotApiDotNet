# Add Security

Create a plan to implement security on all domain endpoints of this API, excepting the Healthcheck and About endpoints (which are System endpoints).  This security will include authentication and authorization.

## Cross-Cutting

This implementation will use the standard security patterns and practices for the current programming language.

## Authentication

Use the Keycloak IDP (Open source, self-hosted on local Docker).
- URLs for Keycloak are:
	- Production: http://local-keycloak:8080
	- Development: http://localhost:55001
- Keycloak access:
	- Realm: local-realm
	- Client ID: local-client

This implementation will include an active flag, that will do the following:
- When active: failure to authenticate/authorize will prevent access to the covered endpoints.
- When inactive: failure to authenticate/authorize will still allow access to the covered endpoints, but the response will include a `Warning` header that will tell the caller that they did not succeed during authentication.

Security settings (URLs, active flag, etc.) should be part of the application configuration.
The text `Keycloak` will not be used as any part of the configuration settings, to avoid vendor lock-in.

Use JWT (JSON Web Tokens) and OAuth2.

## Authorization

Include Role-Based Access, as described below:
- Roles and accesses:
	| Role | Description | Endpoint Methods |
	| --- | --- | --- |
	| read_only_role | Can only read data | GET+ HEAD + OPTIONS + QUERY + TRACE |
	| read_write_role | Can read and update data | GET+ HEAD + OPTIONS + QUERY + PATCH + POST + PUT + TRACE |
	| admin_role | All data maintenance | DELETE + GET+ HEAD + OPTIONS + QUERY + PATCH + POST + PUT + TRACE |
- Users and roles:
	- Create a repository object that will mock the reading of a database table, but will be hard-coded for now. The mock table will be called `UserRoles`.
	- Return roles, when asked about a user, according to the following table:
		| UserId | Role |
		| --- | --- |
		| reader_user | read_only_role |
		| working_user | read_write_role |
		| working_admin | admin_role |
- The security token will include user roles and scopes and client attributes.
- The Keycloak token also includes the following custom attributes:
	- realm_access.roles <-- an array of default user roles
	- resource_access.<area>.roles<-- an array of special user roles.  An example area would be: `account`.
- Enrich the context User so authentication and authorization code can see role, claim, and scope data from the security token.

## Centralization

Create a `SecurityHelper` class, and place the new security related logic within it.

## Additional

Logging: log all successful authentications and failed attempts.
- Passwords must be redacted, if present in any log entry.
- Read the Authorization header value, extract the JWT - then add the JWT to what is being logged.

Update the README with a usage section that demonstrates how the security would be used when calling an endpoint.

The word `Keycloak` should not be used as part of any class, variable, or configuration name; To minimizes vendor lock-in.

## Testing

Unit tests will be generated to cover all new logic, and will be executed and validated to ensure they pass successfully.

## Final

Save the resulting plan in a Markdown file under the `docs` directory.
