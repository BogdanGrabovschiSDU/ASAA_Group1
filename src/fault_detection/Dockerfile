# Use the appropriate Go version as per go.mod requirements
FROM golang:1.23.2

# Set the Current Working Directory inside the container
WORKDIR /app

# Copy go.mod file and download dependencies (omit go.sum if it's not present)
COPY go.mod ./
RUN go mod download

# Copy the rest of the application source code
COPY *.go ./

# Build the Go app
RUN go build -o fault_detection

# Use an unprivileged user to run the application
USER 1000

# Command to run the executable
CMD ["/app/fault_detection"]
