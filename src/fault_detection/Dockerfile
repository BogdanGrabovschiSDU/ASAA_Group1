# Use a Golang base image
FROM golang:1.23.2

# Set the working directory
WORKDIR /app

COPY logConfig.json ./

# Copy the go.mod and go.sum files to the working directory
COPY go.mod go.sum ./

# Download dependencies
RUN go mod download

# Copy the rest of the application source code
COPY *.go ./

# Copy the wait-for-it.sh script
COPY wait-for-it.sh /app/wait-for-it.sh
RUN chmod +x /app/wait-for-it.sh

RUN mkdir -p /app/logs

RUN mkdir /app/Protos

COPY Protos/ /app/Protos


# Build the Go app
RUN go build -o fault_detection

# Command to run the executable
CMD ["./wait-for-it.sh", "rabbitmq:5672", "--", "./fault_detection"]
