#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update \
    && apt-get install -y tzdata libgdiplus fontconfig fonts-liberation \
    && ln -fs /usr/share/zoneinfo/Asia/Ho_Chi_Minh /etc/localtime \
    && dpkg-reconfigure -f noninteractive tzdata \
    && fc-cache -f -v \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 5000
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["saca_api/SACA_API.csproj", "saca_api/"]
COPY ["saca_common/SACA_Common.csproj", "saca_common/"]
COPY ["saca_infra/SACA_Infra.csproj", "saca_infra/"]
COPY ["saca_service/SACA_Service.csproj", "saca_service/"]
RUN dotnet restore "saca_api/SACA_API.csproj"

# Copying all source files
COPY saca_api/ saca_api/
COPY saca_common/ saca_common/
COPY saca_infra/ saca_infra/
COPY saca_service/ saca_service/

WORKDIR "/src/saca_api"
RUN dotnet build "SACA_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SACA_API.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SACA_API.dll"]