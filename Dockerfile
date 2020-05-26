FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY ./ShoppingCart.Core/*.csproj  ./ShoppingCart.Core/
COPY ./ShoppingCart.Domain/*.csproj  ./ShoppingCart.Domain/
COPY ./ShoppingCart.Console/*.csproj  ./ShoppingCart.Console/
COPY ./ShoppingCart.Tests/*.csproj  ./ShoppingCart.Tests/

RUN dotnet restore

# Copy everything else and build
RUN find -type d -name bin -prune -exec rm -rf {} \; && find -type d -name obj -prune -exec rm -rf {} \;
COPY ShoppingCart.Core/. ./ShoppingCart.Core/
COPY ShoppingCart.Domain/. ./ShoppingCart.Domain/
COPY ShoppingCart.Console/. ./ShoppingCart.Console/
COPY ShoppingCart.Tests/. ./ShoppingCart.Tests/
WORKDIR /app/ShoppingCart.Console
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/ShoppingCart.Console/out .
ENTRYPOINT ["dotnet", "ShoppingCart.Console.dll"]