# HbysApi Project

## Overview
This project is designed using the **Clean Architecture** principles to ensure scalability, maintainability, and testability. The project is structured into multiple layers, each with a specific responsibility.

### Layers
1. **Domain**: Contains core business logic and entities. Example: `IEntity`, `BaseEntity`, `Contact`.
2. **Application**: Contains service interfaces, DTOs, and application-specific logic. Example: `IService`, `BaseService`, `ContactDto`.
3. **Infrastructure**: Handles database access and external dependencies. Example: `Repository` for Oracle database.
4. **WebAPI**: The entry point of the application. Handles HTTP requests and responses. Example: `Program.cs` with endpoints like `/api/sample`.

### Workflow
1. **Request**: A client sends an HTTP request to the WebAPI layer.
2. **Service Call**: The WebAPI layer calls the Application layer services.
3. **Business Logic**: The Application layer interacts with the Domain layer for business rules.
4. **Data Access**: The Application layer uses the Infrastructure layer to fetch or persist data.
5. **Response**: The WebAPI layer returns the response to the client.

### Redis TLS Cache Flow (Summary)
- When an API request arrives, a secure TLS connection to Redis is established.
- If the requested data is in the Redis cache, it is returned quickly from the cache.
- If not in the cache, the data is fetched from the database and written to Redis.
- Subsequent requests for the same data are served quickly from the cache.
- All data transmission is encrypted and secure via TLS.

---

## How to Run
1. Restore dependencies:
   ```bash
   dotnet restore
   ```
2. Build the project:
   ```bash
   dotnet build
   ```
3. Run the WebAPI:
   ```bash
   dotnet run --project src/HbysApi.WebApi/HbysApi.WebApi.csproj
   ```

---

## Notes
- Ensure `appsettings.json` and `appsettings.Development.json` are configured correctly.
- Sensitive data like connection strings are excluded from Git using `.gitignore`.

---

# Redis TLS Integration

This project demonstrates secure Redis connection with TLS using StackExchange.Redis. All certificate generation, Redis configuration, and .NET client integration steps are included for both Windows and Docker environments. Sensitive data is kept out of version control and only example values are shown.

---

# Redis TLS Integration Guide

## What Was Done?
- Redis server was configured to use TLS with client authentication.
- All certificates (CA, server, client) were generated on the server and stored under `D:\DockerConfig\Certs`.
- Redis was started with a custom configuration file enabling TLS, client authentication, and password protection.
- Only the required certificates (ca.crt, client.pfx) were copied to the .NET project for secure connection.
- Sensitive information (passwords, certificate paths) is kept out of version control.

## 1. Certificate Generation (on Redis server)
All certificate operations are performed in `D:\DockerConfig\Certs` on the server.

```sh
# 1.1. Generate CA
openssl genrsa -out D:\DockerConfig\Certs\ca.key 4096
openssl req -x509 -new -nodes -key D:\DockerConfig\Certs\ca.key -sha256 -days 3650 -out D:\DockerConfig\Certs\ca.crt -subj "/CN=MyRedisTestCA"

# 1.2. Generate Redis server certificate
openssl genrsa -out D:\DockerConfig\Certs\redis.key 2048
openssl req -new -key D:\DockerConfig\Certs\redis.key -out D:\DockerConfig\Certs\redis.csr -subj "/CN=redis-server"
openssl x509 -req -in D:\DockerConfig\Certs\redis.csr -CA D:\DockerConfig\Certs\ca.crt -CAkey D:\DockerConfig\Certs\ca.key -CAcreateserial -out D:\DockerConfig\Certs\redis.crt -days 3650 -sha256

# 1.3. Generate client certificate
openssl genrsa -out D:\DockerConfig\Certs\client.key 2048
openssl req -new -key D:\DockerConfig\Certs\client.key -out D:\DockerConfig\Certs\client.csr -subj "/CN=redis-client"
openssl x509 -req -in D:\DockerConfig\Certs\client.csr -CA D:\DockerConfig\Certs\ca.crt -CAkey D:\DockerConfig\Certs\ca.key -CAcreateserial -out D:\DockerConfig\Certs\client.crt -days 3650 -sha256

# 1.4. Convert client certificate to PFX
openssl pkcs12 -export -out D:\DockerConfig\Certs\client.pfx -inkey D:\DockerConfig\Certs\client.key -in D:\DockerConfig\Certs\client.crt -certfile D:\DockerConfig\Certs\ca.crt
```
> Set a password for the PFX file. This will be used in your .NET app settings.

## 2. Redis Configuration (redis.conf)
Example (sensitive values replaced):
```
# TLS settings
tls-cert-file /usr/local/etc/redis/Certs/redis.crt
tls-key-file /usr/local/etc/redis/Certs/redis.key
tls-ca-cert-file /usr/local/etc/redis/Certs/ca.crt

tls-auth-clients yes
port 0
tls-port 6380
tls-protocols "TLSv1.2 TLSv1.3"

# Password for default user
user default on >your_redis_password ~* +@all
```

## 3. Running Redis with Docker
```sh
docker run --name redis-secure -d -p 6380:6380 -v D:/DockerConfig:/usr/local/etc/redis redis redis-server /usr/local/etc/redis/redis.conf
```

## 4. .NET Application Integration
- Copy only `ca.crt` and `client.pfx` from the server to `src/HbysApi.WebApi/Config/Certs/` in your project.
- Do NOT include private keys or server certificates in your repository.
- All sensitive values (host, port, password, pfx password) should be set in `appsettings.Development.json` (not committed to version control). Example structure is already present in `appsettings.json`.

---
