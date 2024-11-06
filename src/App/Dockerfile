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

RUN ls
# Build the project
RUN dotnet build

# Expose the port
EXPOSE 80

# Run the application
CMD ["dotnet", "run"]
