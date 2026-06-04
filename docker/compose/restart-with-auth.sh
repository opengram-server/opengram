#!/bin/bash
# Полный перезапуск MongoDB с включённой аутентификацией
# Скрипт корректно сбрасывает состояние MongoDB и поднимает его заново

echo "======================================"
echo "MongoDB Authentication Fix - Restart"
echo "======================================"
echo ""

cd "$(dirname "$0")"

echo "[1/5] Stopping all services..."
docker-compose down

echo "[2/5] Removing MongoDB data volume to reinitialize..."
if [ -d "./data/mongo" ]; then
    rm -rf "./data/mongo"
    echo "     MongoDB data directory removed"
else
    echo "     MongoDB data directory not found (this is fine)"
fi

echo "[3/5] Waiting a moment before starting services..."
sleep 2

echo "[4/5] Building and starting services with MongoDB authentication..."
docker-compose up --build -d

echo "[5/5] Waiting for MongoDB to initialize..."
sleep 10

echo ""
echo "======================================"
echo "Restart Complete!"
echo "======================================"
echo ""
echo "Next steps:"
echo "1. Check MongoDB initialization: docker-compose logs mongodb"
echo "2. Check data-seeder: docker-compose logs data-seeder"
echo "3. Check admin-api: docker-compose logs admin-api"
echo "4. Monitor bot-api: docker-compose logs bot-api-server"
echo ""
echo "MongoDB Authentication Details:"
echo "  Username: admin"
echo "  Auth Database: admin"
echo "  Target Database: tg"
echo ""
