# Используем образ, который у вас ТОЧНО работает
FROM node:22-alpine

# Устанавливаем зависимости для запуска .NET (через стандартный репозиторий Alpine)
#RUN apk add --no-cache bash icu-libs krb5-libs libgcc libintl libssl3 libstdc++ zlib libstdc++
# Устанавливаем зависимости и САМ рантайм ASP.NET Core (в нем есть всё: и dotnet, и веб-фреймворк)
RUN apk add --no-cache bash icu-libs krb5-libs libgcc libintl libssl3 libstdc++ zlib \
    aspnetcore8-runtime --repository=https://dl-cdn.alpinelinux.org

WORKDIR /app

# Копируем файлы, собранные локально командой dotnet publish
COPY ./publish .

# Прокидываем порты
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Запуск через установленный dotnet
ENTRYPOINT ["dotnet", "OtusSocialNetwork.dll"]
