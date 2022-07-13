FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TestNet6/TestNet6.csproj", "TestNet6/"]
RUN dotnet restore "TestNet6/TestNet6.csproj"
COPY . .
WORKDIR "/src/TestNet6"
RUN dotnet build "TestNet6.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestNet6.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestNet6.dll"]
