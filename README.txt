DOCKER

Checking that it is running
docker ps 

Restart container
docker restart my-redis

Redis logs
docker logs my-redis
type C:\Users\nikos\source\AllinOne\appendonlydir\redis.log

Stop / delete container
docker stop my-redis
docker rm my-redis

docker run --name my-redis -p 6379:6379 -v C:\Users\nikos\source\AllinOne\appendonlydir:/data -d redis:latest redis-server /data/redis.conf

=====================================================
Migrations

-Package Manager Console

Add-Migration InitialCreate

Add-Migration UpdateDb20251011 

Update-Database

Get-Migration


-CLI
dotnet ef migrations add UpdateDb20251011

dotnet ef database update

