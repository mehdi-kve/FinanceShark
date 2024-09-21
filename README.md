# Finance Shark
 .Net Core Web Api to quickly find relevant financial data 

## Frameworks - Tools - Libraries 

- ASP.NET Core 8.0 and C#
- Entity Framework Core 8.0 as ORM
- JSON Web Tokens (JWT)
- DotNet watcher tools for file changing detection
- OpenApi using Swashbuckle
- Object Mapping


## To run the project

`$ npm install`

`$ dotnet restore`

`$ dotnet user-secrets set ConnectionStrings:Default "<YOUR CONNETION STRING>"`

`$ dotnet ef database update`

`$ dotnet watch run`
