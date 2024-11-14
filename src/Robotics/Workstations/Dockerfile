# syntax=docker/dockerfile:1

# Set the base image to rust:1.70.0-slim-bullseye
FROM rust:1.70.0-slim-bullseye AS build

# Set the working directory to /app
WORKDIR /app

# Copy the Cargo.toml file into the container
COPY Cargo.toml .

# Copy the src directory into the container
RUN mkdir src

COPY src src


# Build the Rust application, specifying release mode

RUN cargo build --release

# Create a new, smaller image for running the application
FROM debian:stable-slim AS run

# Set the working directory to /app
WORKDIR /app

COPY log4rs.yaml ./

RUN mkdir -p /app/log

# Copy the built binary from the build stage
COPY --from=build /app/target/release/workstations .

# Run the web server
CMD ["./workstations"]
