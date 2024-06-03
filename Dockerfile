# Use the official Microsoft Windows Server Core image as the base
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["iuca.Web/iuca.Web.csproj", "./"]
RUN dotnet restore "./iuca.Web.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "iuca.Web/iuca.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "iuca.Web/iuca.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY entrypoint.sh /app/entrypoint.sh

ENV PATH="${PATH}:/root/.dotnet/tools"

RUN chmod +x /app/entrypoint.sh
ENTRYPOINT ["/app/entrypoint.sh"]
ENTRYPOINT ["dotnet", "iuca.Web.dll"]
