
# Microsoft debian dotnet image for dotnet core 3.1
FROM mcr.microsoft.com/dotnet/sdk:3.1
WORKDIR /mnt
# Copy the things
COPY ./src .
COPY ./exchangeapi.csproj .
RUN dotnet restore
# build
RUN dotnet build

# Install ntlm requirement
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
     && apt-get -y install --no-install-recommends gss-ntlmssp

# Bind to 0.0.0.0 so it works
ENV ASPNETCORE_URLS=http://0.0.0.0:5000
CMD ["./bin/netcoreapp3.1/exchangeapi"]

# TODO(Patrick) - Figure out alpine image