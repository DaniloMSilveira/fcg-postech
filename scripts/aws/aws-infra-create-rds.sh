#!/bin/bash
set -e

# =====================================
# 💰 CONFIGURAÇÕES OTIMIZADAS PARA BAIXO CUSTO (ACESSO PÚBLICO)
# =====================================
REGION="us-east-1"
DB_INSTANCE_ID="fcg-sqlserver"
DB_INSTANCE_CLASS="db.t3.micro"
ALLOCATED_STORAGE=20
ENGINE="sqlserver-ex"

SG_NAME="rds-sg-fcg-sqlserver-public"

DB_NAME="FiapCloudGames"
DB_USERNAME="admin"
DB_PASSWORD=""


echo "🚀 Criando instância RDS SQL Server..."

# =====================================
# 1️⃣ Cria Security Group
# =====================================
VPC_ID=$(aws ec2 describe-vpcs --query "Vpcs[0].VpcId" --output text --region $REGION)

echo "🔐 Criando Security Group público..."
RDS_SG_ID=$(aws ec2 create-security-group \
  --group-name $SG_NAME \
  --description "SG publico para RDS FiapCloudGames" \
  --vpc-id $VPC_ID \
  --query 'GroupId' \
  --output text \
  --region $REGION \
  --tag-specifications "ResourceType=security-group,Tags=[{Key=Project,Value=FiapCloudGames}]")

# =====================================
# 2️⃣ Libera acesso público (0.0.0.0/0)
# =====================================
echo "🌍 Liberando acesso público (porta 1433)..."
aws ec2 authorize-security-group-ingress \
  --group-id $RDS_SG_ID \
  --protocol tcp \
  --port 1433 \
  --cidr 0.0.0.0/0 \
  --region $REGION

# =====================================
# 3️⃣ Cria a instância RDS
# =====================================
echo "🗄️ Criando instância RDS..."

aws rds create-db-instance \
  --db-instance-identifier $DB_INSTANCE_ID \
  --engine $ENGINE \
  --master-username $DB_USERNAME \
  --master-user-password $DB_PASSWORD \
  --allocated-storage $ALLOCATED_STORAGE \
  --db-instance-class $DB_INSTANCE_CLASS \
  --publicly-accessible \
  --storage-type gp2 \
  --no-multi-az \
  --backup-retention-period 0 \
  --vpc-security-group-ids $RDS_SG_ID \
  --tags "Key=Name,Value=FiapCloudGames-RDS-Public" "Key=Project,Value=FiapCloudGames" \
  --region $REGION

echo "⏳ Aguardando o RDS ficar disponível (10–15 minutos)..."
aws rds wait db-instance-available \
  --db-instance-identifier $DB_INSTANCE_ID \
  --region $REGION

# =====================================
# 4️⃣ Exibe o resultado final
# =====================================
DB_ENDPOINT=$(aws rds describe-db-instances \
  --db-instance-identifier $DB_INSTANCE_ID \
  --query "DBInstances[0].Endpoint.Address" \
  --output text \
  --region $REGION)

echo ""
echo "✅ RDS criado com sucesso e público!"
echo "----------------------------------------"
echo "Endpoint: $DB_ENDPOINT"
echo "Usuário:  $DB_USERNAME"
echo "Senha:    $DB_PASSWORD"
echo "----------------------------------------"
echo "💡 Dica: conecte via Azure Data Studio ou DBeaver com:"
echo "   Server: $DB_ENDPOINT,1433"
echo "   User:   $DB_USERNAME"
echo "   Pass:   $DB_PASSWORD"
echo ""
echo "⚠️ AVISO: Esse banco está acessível publicamente (0.0.0.0/0)."
echo "   Use apenas para testes temporários."
echo ""
echo "🧹 Para deletar:"
echo "   aws rds delete-db-instance --db-instance-identifier $DB_INSTANCE_ID --skip-final-snapshot --region $REGION"
