# Dockerfile.grafana
FROM grafana/grafana:latest

# Variáveis de ambiente padrão
ENV GF_SECURITY_ADMIN_PASSWORD=admin123
ENV GF_USERS_ALLOW_SIGN_UP=false

# Copiar provisioning e dashboards
COPY provisioning/ /etc/grafana/provisioning/
COPY dashboards/ /var/lib/grafana/dashboards/

# Definir diretório de trabalho
WORKDIR /var/lib/grafana

# Expor porta
EXPOSE 3000

# Comando padrão
CMD ["/run.sh"]
