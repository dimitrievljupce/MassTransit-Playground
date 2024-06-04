# Project Setup and Monitoring Guide

This document provides step-by-step instructions for setting up the project, configuring ElasticSearch and Kibana, and running the necessary services. By the end of this guide, you should be able to observe logs in Kibana.

### 1. Start Logging Services

Navigate to the logging directory and start the necessary services using Docker Compose.

```
  cd logging
  docker compose up -d
```

### 2. Verify ElasticSearch and Kibana

Ensure that Elastic Search and Kibana are running. You can verify this by accessing the Kibana UI in your browser.

Elastic Search: [http://localhost:9200](http://localhost:9200/)

Kibana: [http://localhost:5601](http://localhost:5601/)

You can check if these services are running by using the following command:

```
docker ps
```

Look for containers named elasticsearch and kibana in the output.

### 3. Configure Indexes/Templates in Kibana

Open Kibana in your web browser: [http://localhost:5601](http://localhost:5601/)

Navigate to Stack Management > Index Management.

Create the necessary index patterns for your logs:

- Go to Index Patterns and click on Create Index Pattern.
- Enter the index pattern that matches the indices (producer-* / consumer-*).
- Save the index pattern.

### 4. Start Application Services

Navigate to the root directory of the project and start the Producer, Consumer, and MassTransit.RabbitMq services using docker compose.

```
cd ..
docker compose up -d
```

Ensure that all the necessary containers are running by using the `docker ps` command.

### 5. Observe Logs in Kibana

Open Kibana in your web browser: [http://localhost:5601](http://localhost:5601/)

Navigate to Discover.

Select the index pattern you configured earlier (e.g., producer/consumer).

Observe the logs being ingested and indexed by ElasticSearch.

### Troubleshooting

If ElasticSearch or Kibana fails to start, check the Docker logs for errors:

```
docker logs <container_id>
```

Ensure there are no port conflicts on 9200 (ElasticSearch) and 5601 (Kibana).

Verify that your index patterns in Kibana match the indices producer / consumer.
