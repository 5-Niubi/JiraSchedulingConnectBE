# Docker build
docker build -f AlgorithmService/Dockerfile -t rcpsp-estimator-0.0.1 .

# Docker tag
docker tag rcpsp-estimator-0.0.1 pham2604/estimate-rcpsp:0.0.1

# Docker push
sudo docker tag rcpsp-estimator-0.0.1 pham2604/estimate-rcpsp:0.0.1

# Docker run
docker run -p 8000:80 -e ASPNETCORE_URLS=http://+:80 -e ASPNETCORE_ENVIRONMENT=Development  rcpsp-estimator-0.0.1


#sudo docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=5Niubipass" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
