# https://github.com/microsoft/vscode-dev-containers/tree/v0.191.0/containers/dotnetcore/.devcontainer/base.Dockerfile

ARG VARIANT="3.1"
FROM mcr.microsoft.com/vscode/devcontainers/dotnetcore:0-${VARIANT}

# [Optional] Uncomment this section to install additional OS packages.
 RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
     && apt-get -y install --no-install-recommends gss-ntlmssp
