FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ET.WebAPI.Api/ET.WebAPI.Api.csproj", "ET.WebAPI.Api/"]
RUN dotnet restore "ET.WebAPI.Api/ET.WebAPI.Api.csproj"
COPY . .
WORKDIR "/src/ET.WebAPI.Api"
RUN dotnet build "ET.WebAPI.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ET.WebAPI.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ET.WebAPI.Api.dll"]
