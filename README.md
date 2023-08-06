#Apartment Management Api

Geliştirici: Bahar Irmak

Bir sitede yer alan dairelerin aidat ve ortak kullanım(elektrik, su, doğalgaz) faturalarının yönetimini yapmayı sağlayan bir web apisidir.

Web api koleksiyonunu aşağıdaki linkten indirerek postmanda dökümantize bir şekilde görüntüleyebilirsiniz.

**! Postmanda baseurl i collection variable'dan değiştirebilirsiniz**

[![https://github.com/baharbubbles/.net_final_project/blob/master/Apartment%20Management%20Api.postman_collection.json](https://img.shields.io/badge/Postman-FF6C37?style=for-the-badge&logo=postman&logoColor=white)]("")

## Proje çalıştırma gereksinimleri
- .Net 6.0
- MSSQL

*Proje vscode ile geliştirildi. Dolayısıyla .vscode içerisinde bulunan debug ayarları ile ayağa kalkabilir.*

## Solution Projeleri
| Proje      | Açıklama |
| ----------- | ----------- |
| ApartmentManagement.Base      | Uygulama temel gereksinimleri burada tanımlanır       |
| ApartmentManagement.Data   | Veritabanı bağlantısı ve tablo tanımları burada barınır        |
| ApartmentManagement.Schema   | İş katmanı ve Veritabanı arasındaki transferde gerekli objeler burada tanımlanır        |
| ApartmentManagement.Business   | İş katmanı burada yer alır        |
| ApartmentManagement.Service   | Web api burada yer alır        |
| ApartmentManagement.PaymentService   | Ödeme servisi web apisi burada yer alır        |

## Uygulama Ayağa Kalkarken
Veritabanı olarak MSSQL kullanılmıştır. Veritabanı bağlantısı için appsettings.json dosyasında bulunan ConnectionStrings alanını değiştirmeniz yeterlidir.

Uygulama başladığı anda veritabanı tabloları oluşturulur ve admin kullanıcısı appsettings deki AdminUser alanından alınır.

Varsayılan Admin kullanıcı bilgisi:
```
{
  "AdminUser": {
    "Name": "Bahar Irmak",
    "Email": "admin@apartmng.com",
    "Password": "123456"
  }
}
```

PaymentService içeride bir HttpClient objesi oluşturarak ApartmentManagement.Service web apisine istek atar. Bu yüzden ApartmentManagement.Service web apisini ayağa kaldırmadan PaymentService'i çalıştırınız.

appsettings de PaymentService baseurl i değiştirmeniz yeterlidir.
```
{
  "PaymentServiceBaseUrl": "http://localhost:2780",
}
``` 
Varsayılan olarak PaymentService portu 2780'dir.

### Uygulamayı Çalıştırmak için

İlk olarak PaymentService'i çalıştırınız.
```
dotnet run --project .\ApartmentManagement.PaymentService\ApartmentManagement.PaymentService.csproj --urls="http://localhost:2780"
```
Daha sonra ApartmentManagement.Service'i çalıştırınız.
```
dotnet run --project .\ApartmentManagement.Service\ApartmentManagement.Service.csproj --urls="http://localhost:5001"
```

## Gereksinimler ve Uygulamaları

#### Uygulama yönetici ve daire kullanıcıları için iki farklı rolde bir rol yönetimi sağlamaktadır.
```
new Claim(ClaimTypes.Role, user.Type == Base.Enums.Enum_UserType.Admin ? "admin" : "user")
```
Kodu ile token üretimi esnasında rol bilgisi token claim'ine yazılmaktadır.

Uygulama AuthorizeAttribute kullanarak bu rol bilgisini kontrol etmektedir.
```
    [Authorize(Roles = "admin")]
```

#### Yönetici daire bilgilerini girer
`/apartmng/api/apartment` [POST]

#### Kullanıcı bilgilerini girer. Giriş yapması için otomatik bir şifre oluşturulur. Kullanıcıları dairelere atar.
`/apartmng/api/user` [POST] ile normal bir kullanıcı eklenilir.
Otomatik bir şifre PasswordGenerator.Get ile üretilir ve Hashlenerek veritabanında tutulur.
Kullanıcı `/apartmng/api/token/login `[POST] ile giriş yapar. Giriş yaptıktan sonra token üretilir ve geri döndürülür.
`/apartmng/api/user/AssignToApartment` ile kullanıcı daireye atanır.

#### Ay bazlı aidat ve fatura bilgilerini girer
`/apartmng/api/transaction/BulkMonthlyInsert` [POST] ile bir ayın aidat ve fatura bilgileri her bir daire için farklı olacak şekilde girebilir.
`/apartmng/api/transaction/BulkMonthlyInsertAll` [POST] ile bir ayın aidat ve fatura bilgileri tüm daireler için aynı olacak şekilde girebilir.

#### Kullanıcıların kredi kartı ile ödeme yapabilmesi için ayrı bir servis yazılacak.

PaymentService projesi bir ödeme süreci başlatmak ve bunu MemoryStorage ile takip etmektedir.
`/apartmng/api/transaction/StartPayment/{id}` ile ilgili fatura için ödeme aşaması başlatılır ve paymentservice den bir referans numarası döner. Bu numara transaction tablosuna kaydedilir.

Ödeme sürecini baştan sonra yönetecek kodu yazmadım. Sadece girdi ve çıktı yı sağlayacak deneme bir servis yazdım.

`/apartmng/api/transaction/CompletePayment/{referenceNumber}` bu uç ise payment servisinden geri beklenen sonucu karşılamak için kullanılmaktadır. Bunu paymentservice in iletmesi bekleniyor. Bu iletim sonucu ödeme işlemi tamamlanarak Transaction tablosundaki Status verisi "Paid" olarak güncellenir.