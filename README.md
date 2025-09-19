# rt-dashboard
Realtime dashboard demo


### Artemis container

The fastest, simplest way to get started is with this command which will create and start a detached container named mycontainer, 
expose the main messaging port (i.e. 61616) and HTTP port (i.e. 8161), and remove it when it terminates:

```
docker run --detach --name artemis -p 61616:61616 -p 8161:8161 --rm apache/activemq-artemis:latest-alpine
```

Once the broker starts you can open the web management console at http://localhost:8161 and log in with the default username & password artemis.

