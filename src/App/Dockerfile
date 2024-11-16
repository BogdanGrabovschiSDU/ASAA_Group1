FROM mcr.microsoft.com/dotnet/sdk:8.0

# Set the working directory to /app
WORKDIR /app

# Copy the project files into the container
COPY *.csproj ./
COPY *.cs /app/
RUN mkdir /app/Protos
COPY Protos/* /app/Protos/
# Restore NuGet packages
RUN dotnet restore

COPY log4net.config ./
RUN mkdir -p /app/logged

RUN ls
# Build the project
RUN dotnet build

# Expose the port
EXPOSE 8080

# Run the application
CMD ["dotnet", "run"]
