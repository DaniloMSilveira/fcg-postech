# Dockerfile.prometheus
FROM prom/prometheus:latest

# Criar diretórios internos
RUN mkdir -p /etc/prometheus /prometheus

# Copiar arquivo de configuração
COPY prometheus-release.yaml /etc/prometheus/prometheus.yaml

# Definir diretório de trabalho
WORKDIR /etc/prometheus

# Comando padrão
CMD [ \
  "--config.file=/etc/prometheus/prometheus.yaml", \
  "--storage.tsdb.path=/prometheus", \
  "--web.enable-lifecycle" \
]
