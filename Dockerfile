# filepath: c:\Users\Ahmet's Monster\Desktop\KonyaTeknikÜniversitesi-IMEOtomasyonu\Dockerfile
# Build aşaması: SDK imajı ile uygulamayı derle
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Sadece proje dosyalarını kopyala ve bağımlılıkları yükle
# Bu katman, kod değişmediği sürece tekrar kullanılacaktır.
COPY *.csproj .
RUN dotnet restore

# Tüm proje kaynak kodunu kopyala
COPY . .

# Uygulamayı yayınla (publish)
RUN dotnet publish -c Release -o /app/publish

# Final aşaması: Sadece runtime içeren küçük imajı kullan
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Derlenmiş dosyaları build aşamasından kopyala
COPY --from=build /app/publish .

# Uygulamanın başlangıç noktası
ENTRYPOINT ["dotnet", "KonyaTeknikÜniversitesi-IMEOtomasyonu.dll"]