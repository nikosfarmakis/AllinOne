-Checking that it is running
docker ps 

-Restart container
docker restart my-redis

-Redis logs
docker logs my-redis
type C:\Users\nikos\source\AllinOne\appendonlydir\redis.log

-Stop / delete container
docker stop my-redis
docker rm my-redis

docker run --name my-redis -p 6379:6379 -d ^
  -v C:\redis-data:/data ^
  redis:latest redis-server --appendonly yes --requirepass "MyPass123"


docker run --name my-redis -p 6379:6379 ^
  -v C:\Users\nikos\source\AllinOne\redis-data:/data ^
  -d redis:latest redis-server ^
    --appendonly yes ^
    --appendfilename appendonly.aof ^
    --dir /data ^
    --appendfsync always ^
    --requirepass "MyPass123"



-config
docker run --name my-redis -p 6379:6379 ^
  -v C:\Users\nikos\source\AllinOne\redis-data:/data ^
  -d redis:latest redis-server /data/redis.conf

-In container 
docker exec -it my-redis redis-cli -a MyPass123

-scan all records
scan 0

set user:1 "Nikos"

-add hash
hmset user:1 name "Nikos" age 30

-AOF/RDB infos
info persistence
