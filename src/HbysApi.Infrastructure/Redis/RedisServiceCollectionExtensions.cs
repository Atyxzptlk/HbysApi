using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

// RedisServiceCollectionExtensions: Adds Redis connection with TLS to DI container
// RedisServiceCollectionExtensions: TLS ile Redis bağlantısını DI container'a ekler
namespace HbysApi.Infrastructure.Redis
{
    public static class RedisServiceCollectionExtensions
    {
        // Registers Redis with TLS using configuration
        // Konfigürasyon ile TLS destekli Redis'i kaydeder
        public static IServiceCollection AddRedisWithTls(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConfig = configuration.GetSection("Redis");
            var redisHost = redisConfig["Host"];
            var redisPort = redisConfig["Port"];
            var redisPassword = redisConfig["Password"];
            var redisPfxPath = redisConfig["PfxPath"] ?? throw new ArgumentNullException("PfxPath", "Redis PFX path cannot be null.");
            var redisPfxPassword = redisConfig["PfxPassword"] ?? throw new ArgumentNullException("PfxPassword", "Redis PFX password cannot be null.");
            var redisCaPath = redisConfig["CaPath"] ?? throw new ArgumentNullException("CaPath", "CA certificate path cannot be null.");

            if (!File.Exists(redisCaPath) || !File.Exists(redisPfxPath))
                throw new FileNotFoundException("Redis CA veya PFX dosyası eksik.");

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
                ConnectTimeout = 5000,
                // SSL/TLS protocol versions to use
                // Kullanılacak SSL/TLS protokol sürümleri
                SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13
            };

#pragma warning disable SYSLIB0057
            options.CertificateSelection += (sender, targetHost, localCertificates, remoteCertificate, acceptableIssuers) =>
            {
                return new X509Certificate2(redisPfxPath, redisPfxPassword);
            };
            options.CertificateValidation += (sender, certificate, chain, sslPolicyErrors) =>
            {
                if (certificate == null) return false;
                var caCertificate = new X509Certificate2(redisCaPath);
                return certificate.Issuer == caCertificate.Subject;
            };
#pragma warning restore SYSLIB0057

            // Add IConnectionMultiplexer as singleton
            // IConnectionMultiplexer'ı singleton olarak ekler
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(options));
            return services;
        }
    }
}
