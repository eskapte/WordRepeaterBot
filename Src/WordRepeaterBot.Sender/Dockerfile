#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Src/WordRepeaterBot.Sender/WordRepeaterBot.Sender.csproj", "Src/WordRepeaterBot.Sender/"]
COPY ["Src/WordRepeaterBot.DataAccess/WordRepeaterBot.DataAccess.csproj", "Src/WordRepeaterBot.DataAccess/"]
RUN dotnet restore "Src/WordRepeaterBot.Sender/WordRepeaterBot.Sender.csproj"
COPY . .
WORKDIR "/src/Src/WordRepeaterBot.Sender"
RUN dotnet build "WordRepeaterBot.Sender.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WordRepeaterBot.Sender.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD tail -f /dev/null