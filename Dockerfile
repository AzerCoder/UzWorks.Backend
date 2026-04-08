FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first for better layer caching
COPY ["UzWorks.sln", "./"]
COPY ["UzWorks/UzWorks.API.csproj", "UzWorks/"]
COPY ["UzWorks.BL/UzWorks.BL.csproj", "UzWorks.BL/"]
COPY ["UzWorks.Core/UzWorks.Core.csproj", "UzWorks.Core/"]
COPY ["UzWorks.Identy/UzWorks.Identity.csproj", "UzWorks.Identy/"]
COPY ["UzWorks.Infrastructure/UzWorks.Infrastructure.csproj", "UzWorks.Infrastructure/"]
COPY ["UzWorks.Persistence/UzWorks.Persistence.csproj", "UzWorks.Persistence/"]

RUN dotnet restore "UzWorks/UzWorks.API.csproj"

# Copy the full repository and publish
COPY . .
RUN dotnet publish "UzWorks/UzWorks.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 10000

COPY --from=build /app/publish .
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT:-10000} dotnet UzWorks.API.dll"]
