FROM mcr.microsoft.com/dotnet/sdk:5.0-focal

RUN mkdir /artifacts/
WORKDIR /opt
COPY . .
RUN ls -ls
WORKDIR /opt/src

RUN sed -i 's/\\/\//g' Binaron.Serializer.sln
RUN dotnet test -c Release -v n
RUN dotnet publish -c Release

WORKDIR /opt/src/Binaron.Serializer/bin/Release
RUN cp *.snupkg /artifacts/
RUN cp *.nupkg /artifacts/
