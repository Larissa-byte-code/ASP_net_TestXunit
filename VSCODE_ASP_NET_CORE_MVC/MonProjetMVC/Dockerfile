# 🛠️ Étape 1 : Build (compilation du projet)
# On utilise l'image officielle .NET SDK pour compiler ton projet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# On définit le dossier de travail dans le conteneur
WORKDIR /src

# On copie tout ton projet dans le conteneur
COPY . .

# On restaure les dépendances NuGet (packages nécessaires)
RUN dotnet restore MonProjetMVC.csproj

# On compile et publie ton projet en mode Release dans le dossier /app
RUN dotnet publish MonProjetMVC.csproj -c Release -o /app


# 🛠️ Étape 2 : Runtime (exécution du projet)
# On utilise l'image officielle .NET Runtime (plus légère, juste pour exécuter ton app)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# On définit le dossier de travail pour l'exécution
WORKDIR /app

# On copie les fichiers compilés depuis l'étape build
COPY --from=build /app .

# On définit la commande de démarrage → lance ton projet
ENTRYPOINT ["dotnet", "MonProjetMVC.dll"]
