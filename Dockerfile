FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 5050

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /source
COPY ./ParsAptekaRu.csproj /source/ParsAptekaRu/ParsAptekaRu.csproj
RUN dotnet restore "./ParsAptekaRu/ParsAptekaRu.csproj"
COPY . ./ParsAptekaRu
WORKDIR /source/ParsAptekaRu
RUN dotnet build "./ParsAptekaRu.csproj" -c ${BUILD_CONFIGURATION} -o ./app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ParsAptekaRu.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "ParsAptekaRu.dll" ]