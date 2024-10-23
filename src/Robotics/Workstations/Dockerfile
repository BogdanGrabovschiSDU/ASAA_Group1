# syntax=docker/dockerfile:1

# Set the base image to rust:1.70.0-slim-bullseye
FROM rust:1.70.0-slim-bullseye AS build

# Set the working directory to /app
WORKDIR /app

# Copy the Cargo.toml file into the container
COPY Cargo.toml .

# Copy the src directory into the container
COPY src .

# Build the Rust application
RUN cargo build --release

# Expose the port for the web server
EXPOSE 6969

# Run the web server
CMD ["cargo", "run", "--release"]
