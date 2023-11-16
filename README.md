# Steps under WSL
- start `Docker`
- run `docker compose up`

# Steps under Windows Host
- check if you can access `http://localhost:8080/` - it should be ASP.NET Sample App
- run `dotnet run`
- wait until it will `Fail`
- WSL network is down now (shutdown required to restart)
- check if can access `http://localhost:8080/` - it shouldn't work