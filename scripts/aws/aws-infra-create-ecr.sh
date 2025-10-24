#!/bin/bash
set -e

# ==============================
# Configurações
# ==============================
REGION="us-east-1"
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)


# ==============================
# 1️⃣ Criar repositorios ECR
# ==============================
echo "📦 Criando repositório ECR para API..."
aws ecr create-repository \
    --repository-name fcg-api \
    --region $REGION \
    --tags Key=Project,Value=FiapCloudGames || echo "ECR já existe"

echo "📦 Autenticando repositório ECR..."
aws ecr get-login-password --region $REGION | docker login --username AWS --password-stdin $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/fcg-api

echo "📦 Buildando e publicando imagem..."
docker build -t $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/fcg-api:1.0 -f ./src/FCG.API/Dockerfile ./src
docker push $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/fcg-api:1.0

##########################################

echo "📦 Criando repositório ECR para Prometheus..."
aws ecr create-repository \
    --repository-name fcg-prometheus \
    --region $REGION \
    --tags Key=Project,Value=FiapCloudGames || echo "ECR já existe"

echo "📦 Autenticando repositório ECR..."
aws ecr get-login-password --region $REGION | docker login --username AWS --password-stdin $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/fcg-prometheus

echo "📦 Buildando e publicando imagem..."
docker build -t $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/fcg-prometheus:1.0 -f ./monitoring/prometheus/Dockerfile ./monitoring/prometheus
docker push $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/fcg-prometheus:1.0

##########################################

echo "📦 Criando repositório ECR para Grafana..."
aws ecr create-repository \
    --repository-name fcg-grafana \
    --region $REGION \
    --tags Key=Project,Value=FiapCloudGames || echo "ECR já existe"

echo "📦 Autenticando repositório ECR..."
aws ecr get-login-password --region $REGION | docker login --username AWS --password-stdin $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/fcg-grafana

echo "📦 Buildando e publicando imagem..."
docker build -t $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/fcg-grafana:1.0 -f ./monitoring/grafana/Dockerfile ./monitoring/grafana
docker push $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/fcg-grafana:1.0
