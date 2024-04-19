FROM mcr.microsoft.com/dotnet/sdk:5.0

VOLUME /mod
WORKDIR /mod

ENTRYPOINT ["dotnet"]
