FROM mcr.microsoft.com/dotnet/core/sdk:3.0-bionic

WORKDIR /opt
COPY . .
RUN ls -ls
WORKDIR /opt/src

RUN sed -i 's/\\/\//g' Binaron.Serializer.sln
RUN dotnet test -c Release -v n
RUN dotnet publish -c Release

WORKDIR /opt/src/Binaron.Serializer/bin/Release
RUN cp *.nupkg /
