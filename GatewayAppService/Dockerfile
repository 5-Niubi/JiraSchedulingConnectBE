FROM nginx:stable-alpine

EXPOSE 80
EXPOSE 443

# for htpasswd command
RUN apk add --no-cache --update \
      apache2-utils
RUN rm -f /etc/nginx/conf.d/*

COPY ./ai4cert.com.pem /etc/ssl/certs/ai4cert.com.pem
COPY ./ai4cert.com.key /etc/ssl/private/ai4cert.com.key

COPY ./nginx.conf /etc/nginx/conf.d/default.conf