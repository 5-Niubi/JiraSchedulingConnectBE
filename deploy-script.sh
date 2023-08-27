cd BE_REPO 
git checkout $branch_name && git pull 
BACKEND_REPOSITORY="pham2604/5niubi_backend" \
GATEWAY_REPOSITORY="pham2604/gateway_service" \
ADMIN_REPOSITORY="pham2604/admin_webapp" \
ASPNETCORE_ENVIRONMENT="Production" \
VERSION="dev-least" \
docker compose -f docker-compose.backend.yml up -d \
sudo docker image prune -a --force && exit"