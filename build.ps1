# build and run containers for kafka and mssql
docker compose -f .\docker-compose.yml up -d

# give broker some time to get ready for processing
sleep 5

# create kafka topic in broker container to produce and consume messages
# broker is the name of the container from docker-compose file
docker exec -it broker kafka-topics --bootstrap-server broker:9092 --create --topic 'RANDOM_NUMBER_DATA'

# clean, restore nuget packages, build dependencies
dotnet clean
dotnet restore
dotnet build
