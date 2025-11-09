FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 5017

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["DentalBooking.BookingService.csproj", "./"]
RUN dotnet restore "./DentalBooking.BookingService.csproj"
COPY . .
RUN dotnet build "./DentalBooking.BookingService.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "./DentalBooking.BookingService.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN apt-get update && apt-get install -y unzip curl && \
    mkdir -p /remote_debugger && \
    curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /remote_debugger

ENTRYPOINT ["dotnet", "DentalBooking.BookingService.dll"]
