# HbysApi Project

## Overview (English)
This project is designed using the **Clean Architecture** principles to ensure scalability, maintainability, and testability. The project is structured into multiple layers, each with a specific responsibility.

### Layers
1. **Domain**:
   - Contains core business logic and entities.
   - Independent of other layers.
   - Example: `IEntity`, `BaseEntity`, `Contact`.

2. **Application**:
   - Contains service interfaces, DTOs, and application-specific logic.
   - Depends on the Domain layer.
   - Example: `IService`, `BaseService`, `ContactDto`.

3. **Infrastructure**:
   - Handles database access and external dependencies.
   - Implements repository patterns.
   - Example: `Repository` for Oracle database.

4. **WebAPI**:
   - The entry point of the application.
   - Handles HTTP requests and responses.
   - Example: `Program.cs` with endpoints like `/api/sample`.

### Workflow
1. **Request**: A client sends an HTTP request to the WebAPI layer.
2. **Service Call**: The WebAPI layer calls the Application layer services.
3. **Business Logic**: The Application layer interacts with the Domain layer for business rules.
4. **Data Access**: The Application layer uses the Infrastructure layer to fetch or persist data.
5. **Response**: The WebAPI layer returns the response to the client.

---

## Genel Bakış (Türkçe)
Bu proje, ölçeklenebilirlik, sürdürülebilirlik ve test edilebilirlik sağlamak için **Clean Architecture** prensiplerine göre tasarlanmıştır. Proje, her biri belirli bir sorumluluğa sahip birden fazla katmana ayrılmıştır.

### Katmanlar
1. **Domain**:
   - Temel iş mantığı ve varlıkları içerir.
   - Diğer katmanlardan bağımsızdır.
   - Örnek: `IEntity`, `BaseEntity`, `Contact`.

2. **Application**:
   - Servis arayüzlerini, DTO'ları ve uygulamaya özgü mantığı içerir.
   - Domain katmanına bağımlıdır.
   - Örnek: `IService`, `BaseService`, `ContactDto`.

3. **Infrastructure**:
   - Veritabanı erişimi ve dış bağımlılıkları yönetir.
   - Repository desenlerini uygular.
   - Örnek: Oracle veritabanı için `Repository`.

4. **WebAPI**:
   - Uygulamanın giriş noktasıdır.
   - HTTP isteklerini ve yanıtlarını işler.
   - Örnek: `/api/sample` gibi endpoint'leri içeren `Program.cs`.

### Döngü
1. **İstek**: Bir istemci, WebAPI katmanına bir HTTP isteği gönderir.
2. **Servis Çağrısı**: WebAPI katmanı, Application katmanındaki servisleri çağırır.
3. **İş Mantığı**: Application katmanı, iş kuralları için Domain katmanıyla etkileşime girer.
4. **Veri Erişimi**: Application katmanı, Infrastructure katmanını kullanarak veri alır veya kaydeder.
5. **Yanıt**: WebAPI katmanı, istemciye yanıt döner.

---

## How to Run (Nasıl Çalıştırılır)
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

## Notes (Notlar)
- Ensure `appsettings.json` and `appsettings.Development.json` are configured correctly.
- Sensitive data like connection strings are excluded from Git using `.gitignore`.
