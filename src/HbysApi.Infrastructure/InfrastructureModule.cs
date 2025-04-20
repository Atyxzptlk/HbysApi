using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Security.Cryptography.X509Certificates;

namespace HbysApi.Infrastructure;

// InfrastructureModule: Registers infrastructure services and dependencies
// InfrastructureModule: Altyapı servislerini ve bağımlılıklarını kaydeder
public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redisConfig = configuration.GetSection("Redis");
            var redisHost = redisConfig["Host"];
            var redisPort = redisConfig["Port"];
            var redisCertPath = redisConfig["CertPath"];
            var redisKeyPath = redisConfig["KeyPath"];
            var redisCaPath = redisConfig["CaPath"];
            var redisPassword = redisConfig["Password"];

            var options = new ConfigurationOptions
            {
                // Redis endpoint(s) to connect
                // Bağlanılacak Redis endpoint(leri)
                EndPoints = { $"{redisHost}:{redisPort}" },
                // Enable SSL/TLS for secure connection
                // Güvenli bağlantı için SSL/TLS'i etkinleştir
                Ssl = true,
                // Hostname for SSL validation
                // SSL doğrulaması için hostname
                SslHost = redisHost,
                // Redis password for authentication
                // Kimlik doğrulama için Redis şifresi
                Password = redisPassword,
                // Do not abort, keep retrying on connection failure
                // Bağlantı başarısız olursa durma, tekrar dene
                AbortOnConnectFail = false,
                // Number of connection retry attempts
                // Bağlantı için yeniden deneme sayısı
                ConnectRetry = 5,
                // Connection timeout in milliseconds
                // Bağlantı zaman aşımı süresi (ms)
                ConnectTimeout = 5000
            };

            // Add certificate selection event handler
            // Sertifika seçimi için olay işleyicisi ekle
            options.CertificateSelection += (sender, targetHost, localCertificates, remoteCertificate, acceptableIssuers) =>
            {
                return X509Certificate2.CreateFromPemFile(redisCertPath, redisKeyPath);
            };

            // Add certificate validation event handler
            // Sertifika doğrulama için olay işleyicisi ekle
            options.CertificateValidation += (sender, certificate, chain, sslPolicyErrors) =>
            {
                return true; // Customize certificate validation
                // Sertifika doğrulama işlemini özelleştirin
            };

            return ConnectionMultiplexer.Connect(options);
        });

        return services;
    }
}