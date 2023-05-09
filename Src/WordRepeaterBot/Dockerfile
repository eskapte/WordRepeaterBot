#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Src/WordRepeaterBot/WordRepeaterBot.csproj", "Src/WordRepeaterBot/"]
COPY ["Src/WordRepeaterBot.Application/WordRepeaterBot.Application.csproj", "Src/WordRepeaterBot.Application/"]
COPY ["Src/WordRepeaterBot.DataAccess/WordRepeaterBot.DataAccess.csproj", "Src/WordRepeaterBot.DataAccess/"]
COPY ["/etc/letsencrypt/live/atweek.fun/fullchain.pem", "Certs/"]
RUN dotnet restore "Src/WordRepeaterBot/WordRepeaterBot.csproj"
COPY . .
WORKDIR "/src/Src/WordRepeaterBot"
RUN dotnet build "WordRepeaterBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WordRepeaterBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WordRepeaterBot.dll"]