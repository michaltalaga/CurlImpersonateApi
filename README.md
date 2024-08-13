# Curl Impersonate API

This project provides a Dockerized API service that you can run and access using Swagger for easy interaction with the API.

## How to Run the Docker Container

To pull and run the latest version of the container from Docker Hub, use the following command:

```bash
docker run --pull always --rm -p 28080:8080 -p 28081:8081 --name curl-impersonate-api mikeon/curl-impersonate-api:latest
