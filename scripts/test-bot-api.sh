#!/bin/bash

# Скрипт для проверки Bot API
TOKEN="999999999:ABCdefGHIjklMNOpqrsTUVwxyz123456"
BASE_URL="http://localhost:8081/bot$TOKEN"

echo "Testing MyTelegram Bot API"
echo "================================"

echo ""
echo "1. Testing getMe..."
curl -s "$BASE_URL/getMe" | jq .

echo ""
echo "2. Testing getUpdates..."
curl -s "$BASE_URL/getUpdates" | jq .

echo ""
echo "3. Testing getWebhookInfo..."
curl -s "$BASE_URL/getWebhookInfo" | jq .

echo ""
echo "4. Testing sendMessage..."
curl -s -X POST "$BASE_URL/sendMessage" \
  -H "Content-Type: application/json" \
  -d '{"chat_id": 2010001, "text": "Hello from test bot!"}' | jq .

echo ""
echo "Tests completed"
