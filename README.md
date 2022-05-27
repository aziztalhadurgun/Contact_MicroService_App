
# Proje Hakkında
Projeler .NET 6 Minimal API ile geliştirilmiştir.\
Report servisinden yeni bir rapor istenildiğinde REST ile Users servisi ile iletişime geçer.\
Rapor talepleri asenkron çalışmaktadır. Kullanıcı bir rapor talep ettiğinde, sistem 
arkaplanda bu çalışmayı rabbitmq kuyruğuna gönderir.\
Rapor tamamlandığında ise kullanıcı raporların listelendiği endpoint üzerinden 
raporun durumunu "Tamamlandı" olarak gözlemlemektedir.



## Kullanılan Teknolojiler

- .NET 6
- MassTransit
- RabbitMq
- Postgres
  
## Contact.Users Endpoints

```http
  GET /users                    => Tüm kişileri getir
  GET /userById/${id}           => Kişiyi gerir
  POST /userAdd                 => Yeni kişi ekle
  DELETE /userDelete/{id}       => Kişi sil
  POST /userDetailAdd/{id}      => Kişiye iletişim bilgisi ekle
  GET /userDetails              => Tüm kişiler iletişim bilgileriyle
  GET /userDetailById/{id}      => Kişiyi iletişim bilgileriyle getir
  DELETE /userDetailDelete/{id} => Kişinin iletişim bilgisini sil
```

## Contact.Report Endpoints

```http
  POST /createReport            => Yeni rapor oluşturma
  GET /reports                  => Raporları getir
```
