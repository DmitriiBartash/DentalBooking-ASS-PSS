# Stage 1: Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["DentalBooking.Gateway.csproj", "."]
RUN dotnet restore "./DentalBooking.Gateway.csproj"
COPY . .
RUN dotnet build "./DentalBooking.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./DentalBooking.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Install Visual Studio Debugger (vsdbg)
RUN apt-get update && apt-get install -y curl unzip && \
    mkdir -p /remote_debugger && \
    curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /remote_debugger

# Stage 4: Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /remote_debugger /remote_debugger
ENTRYPOINT ["dotnet", "DentalBooking.Gateway.dll"]
