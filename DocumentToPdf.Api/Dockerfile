FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy everything (includes NuGet.config + .nuget-local)
COPY . .

# restore using NuGet.config (local feed included)
RUN dotnet restore DocumentToPdf.Api/DocumentToPdf.Api.csproj
RUN dotnet publish DocumentToPdf.Api/DocumentToPdf.Api.csproj -c Release -o /out --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet","DocumentToPdf.Api.dll"]