# API and UI Integration Testing (.NET)

## Create a self-signed certificate
`dotnet dev-certs https -ep cert.pfx -p Test1234!`

## The 5 integration testing steps

1. Setup 
2. Dependency Mocking (API)
3. Execution
4. Assertion
5. Cleanup

## WebUI Integration Testing

If you are getting issues when you're running your tests with that docker-compose.integration.yml file, try changing the line:

- CustomersWebApp_GitHub__ApiBaseUrl=http://localhost:9850

To: 

- CustomersWebApp_GitHub__ApiBaseUrl=http://host.docker.internal:9850

