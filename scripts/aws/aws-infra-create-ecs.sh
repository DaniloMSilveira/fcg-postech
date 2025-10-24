#!/bin/bash
set -e

# ==============================
# CONFIGURAÇÕES
# ==============================
REGION="us-east-1"
CLUSTER_NAME="fcg-ecs-cluster"
SEC_GROUP_NAME="fcg-sg-ecs"
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)

# ALB / TG
ALB_NAME="fcg-alb"
TG_API="fcg-tg-api"
TG_PROM="fcg-tg-prometheus"
TG_GRAF="fcg-tg-grafana"

# ==============================
# 1️⃣ Criar ECS Cluster
# ==============================
echo "🚀 Criando ECS Cluster..."
aws ecs create-cluster \
  --cluster-name $CLUSTER_NAME \
  --region $REGION \
  --tags key=Project,value=FiapCloudGames \
  >/dev/null 2>&1 || echo "ℹ️ Cluster já existe."

# ==============================
# 2️⃣ Criar Security Group
# ==============================
echo "🔐 Criando Security Group..."
VPC_ID=$(aws ec2 describe-vpcs --query "Vpcs[0].VpcId" --output text --region $REGION)

SG_ID=$(aws ec2 create-security-group \
  --group-name $SEC_GROUP_NAME \
  --description "SG ECS FCG POC" \
  --vpc-id $VPC_ID \
  --region $REGION \
  --tag-specifications 'ResourceType=security-group,Tags=[{Key=Project,Value=FiapCloudGames}]' \
  --query "GroupId" --output text) || \
  SG_ID=$(aws ec2 describe-security-groups --filters Name=group-name,Values=$SEC_GROUP_NAME --query "SecurityGroups[0].GroupId" --output text)

for port in 8080 3000 9090 80; do
  aws ec2 authorize-security-group-ingress --group-id $SG_ID --protocol tcp --port $port --cidr 0.0.0.0/0 --region $REGION 2>/dev/null || true
done

# ==============================
# 3️⃣ Criar Task Definitions
# ==============================

aws ecs register-task-definition \
  --cli-input-json file://fcg-api-task.json \
  --region $REGION \
  --tags key=Project,value=FiapCloudGames

aws ecs register-task-definition \
  --cli-input-json file://fcg-prometheus-task.json \
  --region $REGION \
  --tags key=Project,value=FiapCloudGames

aws ecs register-task-definition \
  --cli-input-json file://fcg-grafana-task.json \
  --region $REGION \
  --tags key=Project,value=FiapCloudGames

# ==============================
# 4️⃣ Criando Logs Groups
# ==============================

aws logs create-log-group \
  --log-group-name /ecs/fcg-api \
  --region $REGION \
  --tags key=Project,value=FiapCloudGames

aws logs create-log-group \
  --log-group-name /ecs/fcg-prometheus \
  --region $REGION \
  --tags key=Project,value=FiapCloudGames

aws logs create-log-group \
  --log-group-name /ecs/fcg-grafana \
  --region $REGION \
  --tags key=Project,value=FiapCloudGames

# ==============================
# 4️⃣ Criar Target Groups
# ==============================
echo "🎯 Criando Target Groups..."
TG_API_ARN=$(aws elbv2 create-target-group --name $TG_API --protocol HTTP --port 8080 --vpc-id $VPC_ID --target-type ip --query "TargetGroups[0].TargetGroupArn" --output text)

aws elbv2 add-tags \
  --resource-arns $TG_API_ARN \
  --tags Key=Project,Value=FiapCloudGames

TG_PROM_ARN=$(aws elbv2 create-target-group --name $TG_PROM --protocol HTTP --port 9090 --vpc-id $VPC_ID --target-type ip --query "TargetGroups[0].TargetGroupArn" --output text)

aws elbv2 add-tags \
  --resource-arns $TG_PROM_ARN \
  --tags Key=Project,Value=FiapCloudGames

TG_GRAF_ARN=$(aws elbv2 create-target-group --name $TG_GRAF --protocol HTTP --port 3000 --vpc-id $VPC_ID --target-type ip --query "TargetGroups[0].TargetGroupArn" --output text)

aws elbv2 add-tags \
  --resource-arns $TG_GRAF_ARN \
  --tags Key=Project,Value=FiapCloudGames

# ==============================
# 5️⃣ Criar ALB
# ==============================
echo "🌐 Criando ALB..."
SUBNETS=$(aws ec2 describe-subnets --query "Subnets[0:2].SubnetId" --output text)
ALB_ARN=$(aws elbv2 create-load-balancer --name $ALB_NAME --subnets $SUBNETS --security-groups $SG_ID --scheme internet-facing --type application --query "LoadBalancers[0].LoadBalancerArn" --output text)

aws elbv2 add-tags \
  --resource-arns $ALB_ARN \
  --tags Key=Project,Value=FiapCloudGames

ALB_DNS=$(aws elbv2 describe-load-balancers --names $ALB_NAME --query "LoadBalancers[0].DNSName" --output text)
echo "✅ ALB criado: $ALB_DNS"

# ==============================
# 6️⃣ Criar Listeners
# ==============================
LISTENER_API_ARN=$(aws elbv2 create-listener \
  --load-balancer-arn $ALB_ARN \
  --protocol HTTP \
  --port 8080 \
  --default-actions Type=forward,TargetGroupArn=$TG_API_ARN \
  --query "Listeners[0].ListenerArn" \
  --output text)

aws elbv2 add-tags \
  --resource-arns $LISTENER_API_ARN \
  --tags Key=Project,Value=FiapCloudGames

LISTENER_PROM_ARN=$(aws elbv2 create-listener \
  --load-balancer-arn $ALB_ARN \
  --protocol HTTP \
  --port 9090 \
  --default-actions Type=forward,TargetGroupArn=$TG_PROM_ARN \
  --query "Listeners[0].ListenerArn" \
  --output text)

aws elbv2 add-tags \
  --resource-arns $LISTENER_PROM_ARN \
  --tags Key=Project,Value=FiapCloudGames

LISTENER_GRAF_ARN=$(aws elbv2 create-listener \
  --load-balancer-arn $ALB_ARN \
  --protocol HTTP \
  --port 3000 \
  --default-actions Type=forward,TargetGroupArn=$TG_GRAF_ARN \
  --query "Listeners[0].ListenerArn" \
  --output text)

aws elbv2 add-tags \
  --resource-arns $LISTENER_GRAF_ARN \
  --tags Key=Project,Value=FiapCloudGames

# ==============================
# 7️⃣ Criar ECS Services
# ==============================
SUBNETS_ECS=$(echo $SUBNETS | tr '\t' ',' | tr ' ' ',')
echo "🚀 Criando ECS Services..."
for SERVICE in \
  "fcg-api-service:fcg-api-task:8080:$TG_API_ARN" \
  "fcg-prometheus-service:fcg-prometheus-task:9090:$TG_PROM_ARN" \
  "fcg-grafana-service:fcg-grafana-task:3000:$TG_GRAF_ARN"
do
  IFS=":" read SERVICE_NAME TASK_DEF PORT TG_ARN <<< "$SERVICE"

  echo "➡️ Criando serviço $SERVICE_NAME..."
  SERVICE_ARN=$(aws ecs create-service \
    --cluster $CLUSTER_NAME \
    --service-name $SERVICE_NAME \
    --task-definition $TASK_DEF \
    --desired-count 1 \
    --launch-type FARGATE \
    --network-configuration "awsvpcConfiguration={subnets=[$SUBNETS_ECS],securityGroups=[$SG_ID],assignPublicIp=ENABLED}" \
    --load-balancers "targetGroupArn=$TG_ARN,containerName=${TASK_DEF%-task},containerPort=$PORT" \
    --region $REGION \
    --query "service.serviceArn" \
    --output text)

  aws ecs tag-resource \
    --resource-arn $SERVICE_ARN \
    --tags key=Project,value=FiapCloudGames

  echo "✅ Serviço $SERVICE_NAME criado e tagueado com sucesso!"
done

echo "🎉 Todos os serviços estão no ar!"
echo "API:        http://$ALB_DNS:8080/swagger"
echo "Prometheus: http://$ALB_DNS:9090"
echo "Grafana:    http://$ALB_DNS:3000"
