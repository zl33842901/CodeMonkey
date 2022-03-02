FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY . ./
ENV TZ=Asia/Shanghai
RUN sed -i "s/TLSv1.2/TLSv1.0/g" /etc/ssl/openssl.cnf
ENTRYPOINT ["dotnet", "xLiAd.CodeMonkey.WebApp.dll"]