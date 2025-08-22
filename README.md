# YARP API Gateway + Refit Örneği — .NET 9 Demo


## 📌 Genel Bakış

Bu demo projesi, **.NET 9** ile bir API Gateway'in nasıl oluşturulacağını ve  
`YARP (Yet Another Reverse Proxy)` kullanarak gelen isteklerin tek bir noktadan tüm microservislere  
nasıl yönlendirileceğini göstermektedir.  

Ayrıca `Refit` kütüphanesi kullanılarak typed HTTP client’lar ile güçlü ve sade bir API tüketim yaklaşımı sunulmaktadır.

---

## ✨ Öne Çıkan Özellikler

- **YARP ile API Gateway**: Gelen istekleri `ConsumerApi` veya `ProductApi` gibi farklı microservislere yönlendirir.  
- **Refit ile Typed HTTP Client**: HTTP API çağrıları için arayüz tabanlı, bakımı kolay bir yapı.  
- **.NET 9 ile Modern Geliştirme**: En yeni .NET sürümüyle uyumluluk.  
- **Minimal Kod, Maksimum Anlaşılabilirlik**: Öğrenmesi ve genişletmesi kolay bir demo.

---

## 📂 Proje Yapısı

/ApiGateway – YARP tabanlı API Gateway uygulaması
/ProductApi – Products servisi (örnek backend API)
/ConsumerApi – Products Api'ye Refit HttpClient kütüphanesi üzerinden erişen client servis (consumer API)

- ApiGateway -> localhost:5000
- ProductApi -> localhost:5001
- ConsumerApi -> localhost:5002

Örnek api isteği :
- http://localhost:5000/productapi/api/products
- http://localhost:5000/consumerapi/api/productproxy

## 🔄 Projenin Akışı

İstemci, ApiGateway üzerinden API çağrısı yapar.

YARP, gelen isteği doğru backend servisine yönlendirir.

Backend servisi (ProductApi veya ConsumerApi) isteği işler.

Yanıt API Gateway aracılığıyla istemciye döner.


## ⚙️ Refit & YARP Konfigürasyonu

# YARP (ApiGateway appsettings.json)

```json
"ReverseProxy": {
    "Routes": {
        "ProductApi": {
            "ClusterId": "productApiCluster",
            "Match": {
                "Path": "/productapi/{**catch-all}"
            },
            "Transforms": [
                { "PathRemovePrefix": "/productapi" }
            ]
        },
        "ConsumerApi": {
            "ClusterId": "consumerApiCluster",
            "Match": {
                "Path": "/consumerapi/{**catch-all}"
            },
            "Transforms": [
                { "PathRemovePrefix": "/consumerapi" }
            ]
        }
    },
    "Clusters": {
        "productApiCluster": {
            "Destinations": {
                "productApiDestination": {
                    "Address": "http://localhost:5001/"
                }
            }
        },
        "consumerApiCluster": {
            "Destinations": {
                "consumerApiDestination": {
                    "Address": "http://localhost:5002/"
                }
            }
        }
    }
}
```

#🔹 Refit Kullanımı

Arayüz:

```csharp
public interface IProductApi
{
    [Get("/api/products")]
    Task<List<Product>> GetProducts();

    [Get("/api/products/{id}")]
    Task<Product> GetProduct(int id);

    [Post("/api/products")]
    Task<Product> PostProduct([Body] Product product);
}
```

Consumer Api (Program.cs):

```csharp
// Register Refit client for IProductApi
builder.Services.AddRefitClient<IProductApi>() // Ensure Refit.HttpClientFactory package is installed
   .ConfigureHttpClient(c =>
   {
       c.BaseAddress = new Uri("http://localhost:5001"); // ProductApi URL
   });

var app = builder.Build();
```

Yarp Api Gateway (Program.cs)
```csharp
// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();
app.MapReverseProxy();
```
