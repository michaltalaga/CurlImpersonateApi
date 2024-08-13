# Curl Impersonate API

This project provides a Dockerized API service built on top of [curl-impersonate](https://github.com/lwthiker/curl-impersonate). It allows you to interact with `curl-impersonate` via a RESTful API, making it easier to leverage its capabilities programmatically. The API service is containerized for ease of deployment and access.

## How to Run the Docker Container

To pull and run the latest version of the container from Docker Hub, use the following command:

```bash
docker run --pull always --rm -p 28080:8080 --name curl-impersonate-api mikeon/curl-impersonate-api:latest
```

```bash
docker run --pull always -d -p 28080:8080 --name curl-impersonate-api mikeon/curl-impersonate-api:latest
```
