ARG MODE=production

# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /source

# copy csproj and restore
COPY ./Nov.Caps.Int.D365 .

RUN dotnet restore -r linux-musl-x64
RUN dotnet publish -c release -o /app -r linux-musl-x64 --self-contained true --no-restore /p:PublishTrimmed=true /p:PublishReadyToRun=true

# final stage/image
FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1-alpine
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=$MODE

# Capitalize the env var
RUN export ASPNETCORE_ENVIRONMENT=$(echo $ASPNETCORE_ENVIRONMENT | awk '{print toupper(substr($0,0,1))tolower(substr($0,2))}')

ENTRYPOINT ["./Nov.Caps.Int.D365.Server"]
