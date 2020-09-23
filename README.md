# dotnet-cafe-demo

This is a port of the Java Quarkus application available here:
https://github.com/quarkuscoffeeshop/quarkuscoffeeshop-main

The structure is extremely similar, but there are some differences based on this being idiomatic C#. A guide to the coding differences for Java developers is available in [this doc](JAVA.md).


login to your openshift cluster

    oc login -u <USERNAME> -p <PASSWORD> --server=https://CLUSTER_API_DNS:6443

create openshift project
    
    oc new-project dotnet-cafe-demo

# create mongodb
create a mongodb ephemeral instance in the project

    oc new-app \
      -n dotnet-cafe-demo \
      --template=mongodb-ephemeral \
      -p MONGODB_USER=cafe-user \
      -p MONGODB_PASSWORD=redhat-20 \
      -p MONGODB_DATABASE=cafedb \
      -p MONGODB_ADMIN_PASSWORD=redhat-20 \
      -p DATABASE_SERVICE_NAME=dotnet-cafe-mongodb-service

# create kafka cluster
This requires that the "Red Hat Integration - AMQ Streams" operator be installed on the cluster.

from the root of the dotnet-cafe-demo folder, run the create-kafka.yml script on OpenShift

    oc create -f create-kafka.yml

# dotnet.cafe.kitchen
build and deploy the dotnet.cafe.kitchen service and dependencies

set the environment variables for your OCP services
DOTNET_CAFE_KAFKA_BOOTSTRAP

    oc new-app \
      -n dotnet-cafe-demo \
      --name=dotnet-cafe-kitchen dotnet:3.1~https://github.com/vincent-tsugranes/dotnet-cafe-demo.git \
      --build-env DOTNET_STARTUP_PROJECT=dotnet.cafe.kitchen/dotnet.cafe.kitchen.csproj \
      -e DOTNET_CAFE_KAFKA_BOOTSTRAP=cafe-cluster-kafka-bootstrap:9092 \
      -l app=dotnet-cafe-module
 

# dotnet.cafe.barista

    oc new-app \
      -n dotnet-cafe-demo \
      --name=dotnet-cafe-barista dotnet:3.1~https://github.com/vincent-tsugranes/dotnet-cafe-demo.git \
      --build-env DOTNET_STARTUP_PROJECT=dotnet.cafe.barista/dotnet.cafe.barista.csproj \
      -e DOTNET_CAFE_KAFKA_BOOTSTRAP=cafe-cluster-kafka-bootstrap:9092 \
      -l app=dotnet-cafe-module
      
  
 # dotnet.cafe.counter 
 Set the DOTNET_CAFE_MONGODB environment variable as well
 
     oc new-app \
       -n dotnet-cafe-demo \
       --name=dotnet-cafe-counter dotnet:3.1~https://github.com/vincent-tsugranes/dotnet-cafe-demo.git \
       --build-env DOTNET_STARTUP_PROJECT=dotnet.cafe.counter/dotnet.cafe.counter.csproj \
       -e DOTNET_CAFE_MONGODB=mongodb://cafe-user:redhat-20@dotnet-cafe-mongodb-service:27017/cafedb \
       -e DOTNET_CAFE_KAFKA_BOOTSTRAP=cafe-cluster-kafka-bootstrap:9092 \
       -l app=dotnet-cafe-module
       
       
  # dotnet.cafe.web 
  
      oc new-app \
        -n dotnet-cafe-demo \
        --name=dotnet-cafe-web dotnet:3.1~https://github.com/vincent-tsugranes/dotnet-cafe-demo.git \
        --build-env DOTNET_STARTUP_PROJECT=dotnet.cafe.web/dotnet.cafe.web.csproj \
        -e DOTNET_CAFE_KAFKA_BOOTSTRAP=cafe-cluster-kafka-bootstrap:9092 \
        -l app=dotnet-cafe-module
  
  
  Expose a route to the site
        
        oc expose svc/dotnet-cafe-web
  
  Browse the site at
  
        http://dotnet-cafe-web-dotnet-cafe-demo.apps.OPENSHIFT_DOMAIN/
                  
                  
  #Add annotations for pretty topology display
  
  <img src="https://github.com/vincent-tsugranes/dotnet-cafe-demo/blob/master/support/images/dotnet-cafe-topology.png?raw=true"></img>
   
      oc annotate deployment -l app=dotnet-cafe-module app.openshift.io/connects-to='["cafe-cluster-kafka",{"apiVersion":"apps/v1","kind":"StatefulSet","name":"cafe-cluster-kafka"}]'
      oc annotate deployment dotnet-cafe-counter app.openshift.io/connects-to='["cafe-cluster-kafka",{"apiVersion":"apps/v1","kind":"StatefulSet","name":"cafe-cluster-kafka"},{"apiVersion":"apps.openshift.io/v1","kind":"DeploymentConfig","name":"dotnet-cafe-mongodb-service"}]' --overwrite
      oc label dc dotnet-cafe-mongodb-service app.kubernetes.io/name=mongodb
      oc label deployment -l app=dotnet-cafe-module app.kubernetes.io/name=dotnet
      
   