FROM mcr.microsoft.com/devcontainers/dotnet:1-8.0-bookworm

COPY mssql/installSQLtools.sh installSQLtools.sh
RUN bash ./installSQLtools.sh \
     && apt-get clean -y && rm -rf /var/lib/apt/lists/* /tmp/library-scripts

RUN dotnet dev-certs https --trust