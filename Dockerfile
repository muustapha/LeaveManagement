# -------------------
# Stage 1: Build
# -------------------
    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src
    
    # Copie du fichier csproj et restauration des dépendances
    COPY *.csproj ./
    RUN dotnet restore
    
    # Copie du reste du code et publication
    COPY . ./
    RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false
    
    # -------------------
    # Stage 2: Runtime
    # -------------------
    FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
    WORKDIR /app
    
    # Exposer le port HTTP
    EXPOSE 80
    ENV ASPNETCORE_URLS=http://+:80
    
    # Copie des artifacts publiés
    COPY --from=build /app/publish .
    
    # Point d'entrée
    ENTRYPOINT ["dotnet", "LeaveManagement.dll"]
    