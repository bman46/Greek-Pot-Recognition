#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Greek Pot Recognition/Greek Pot Recognition.csproj", "Greek Pot Recognition/"]
RUN dotnet restore "Greek Pot Recognition/Greek Pot Recognition.csproj"
COPY . .
WORKDIR "/src/Greek Pot Recognition"
RUN dotnet build "Greek Pot Recognition.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Greek Pot Recognition.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final

# ENV Vars:
ENV MongoDBConnectionString=changeme
ENV ENDPOINT=changeme
ENV KEY=changeme
ENV PROJECTID=changeme
ENV PROJECTNAME=changeme

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Greek Pot Recognition.dll"]
