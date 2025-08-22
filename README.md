# YARP API Gateway + Refit Örneği — .NET 9 Demo


## 📌 Genel Bakış

Bu demo projesi, **.NET 9** ile bir API Gateway'in nasıl oluşturulacağını ve  
`YARP (Yet Another Reverse Proxy)` kullanarak gelen isteklerin tek bir noktadan tüm microservislere  nasıl yönlendirileceğini göstermektedir.  
Ayrıca `Refit` kütüphanesi kullanılarak typed HTTP clientlar ile güçlü ve sade bir API endpoint yaklaşımı sunulmaktadır.

---

## 📌 Yarp Api Gateway Avantajları Nelerdir

-  YARP (Yet Another Reverse Proxy), Microsoft tarafından geliştirilen yüksek performanslı bir reverse proxy ve API Gateway çözümüdür. ASP.NET Core üzerinde çalışır.
-  Clienttean gelen bir isteği, microservis mimarisi ile geliştirdiğimiz servislere tek bir entry point üzerinden dağıtan ve route eden bir mekanızmadır.
-  Ayrıca; Routing, Load Balancing, Authentication, Rate Limiting, Caching gibi ortak ihtiyaçları merkezi bir katmanda yönetmek için de güzel bir çözümdür.
-  Appsetting.json üzerinden kolayca routingler ve clusterlar konfigure edilebilir, kullanımı oldukça basittir.
-  https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/yarp/getting-started?view=aspnetcore-9.0

---

## 📌 Refit Avantajları Nelerdir

-  API endpointlerini çağırmak için HttpClient kodu yazmaya gerek kalmadan kolayca interface olarak tanımlayabilirsin, bu da size daha temiz, test edilebilir ve boilerplate kodlardan arındırılmış bir kod sağlar.
-  Her defasında Json Serialize/Deserialize yapmaya gerek kalmaz, çünkü Refit bunu otomatik yapar.
-  Test edilebilir olduğu için, unit testlerde kolayca mocklayabilirsiniz
-  Strongly-Type Api olduğu için runtimeda patlamadan compiletimeda hatayi yakalar.

---

## ✨ Öne Çıkan Özellikler

- **YARP ile API Gateway**: Gelen istekleri, tek bir entry point üzerinden `ConsumerApi` veya `ProductApi` gibi farklı microservislere yönlendiren reverse proxy.  
- **Refit ile Typed HTTP Client**: HTTP API çağrıları için arayüz tabanlı, bakımı kolay bir yapı.
- **ICarter ile Minimal Api Tasarımı**: Carter kütüphanesi ile yüksek trafikli uygulamarda daha hızlı çalışabilen, daha esnek, moduler, prototip ve fonksiyonel endpointler sunar. Daha minimal ve lighweighttir. Bakımı kolaydır. Refit ile kolayca minimal apiler geliştirebilirsiniz. Dotnet Core DI ile kolayca entegre edebilirsiniz ve geleneksel controllerlara göre daha hızlı response almanızı sağlar.
- **.NET 9 ile Modern Geliştirme**: En yeni .NET sürümüyle uyumluluk.  

---

## 📂 Proje Yapısı

/ApiGateway – YARP tabanlı API Gateway uygulaması
/ProductApi – Products servisi (örnek backend API)
/ConsumerApi – ProductsApi'ye Refit HttpClient kütüphanesi üzerinden erişen client servis (consumer API)

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
ICarterModule implemente eden, Proxy gibi çalışıp dış Api (ProductApi) ile haberleşen classımız.

```csharp
using Carter;
using ConsumerApi.Models;
using ConsumerApi.Services;

public class ProductProxyModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/productproxy", async (IProductApi api) =>
            Results.Ok(await api.GetProducts()));

        app.MapGet("/api/productproxy/{id}", async (Guid id, IProductApi api) =>
        {
            var product = await api.GetProduct(id);
            return product is not null ? Results.Ok(product) : Results.NotFound();
        });

        app.MapPost("/api/productproxy", async (Product product, IProductApi api) =>
        {
            var created = await api.PostProduct(product);
            return Results.Created($"/api/productproxy/{created.Id}", created);
        });
    }
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
