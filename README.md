# dotnet-cafe-demo

This is a port of the Java Quarkus application available here:
https://github.com/quarkuscoffeeshop/quarkuscoffeeshop-main

The structure is extremely similar, but there are some differences based on this being idiomatic C#. A guide to the coding differences for Java developers is available in [this doc](JAVA.md).


#Building and Deploying

This uses an OpenShift tool called S2I (Source-to-Image). That basically means to deploy, you check your code into source control, then tell OpenShift to pull the code, build it, and deploy it automatically. This is as simple as it gets; no configuration files, no dockerfile, nothing but this oc new-app command, and everything gets created. You can then start subsequent build/deploys directly from the OpenShift web console.

There are some internal links between services, like connecting to Kafka or MongoDB, and that's all done through environment variables based on the default object names; it will work the exact same way in your environment unless you explicitly change something.



To get started, login to your openshift cluster

    oc login -u <USERNAME> -p <PASSWORD> --server=https://CLUSTER_API_DNS:6443

then create the openshift project - if you change the name here, you'll have to change all of the resource links in subsequent steps, you should only do that if you want to go off the reservation.
    
    oc new-project dotnet-cafe-demo

# create mongodb
create a mongodb ephemeral instance in the project. This sets the service name, credentials, and a DB for the project that are used in the connection strings later.

    oc new-app \
      -n dotnet-cafe-demo \
      --template=mongodb-ephemeral \
      -p MONGODB_USER=cafe-user \
      -p MONGODB_PASSWORD=redhat-20 \
      -p MONGODB_DATABASE=cafedb \
      -p MONGODB_ADMIN_PASSWORD=redhat-20 \
      -p DATABASE_SERVICE_NAME=dotnet-cafe-mongodb-service


*This will not work if you're on OpenShift 4.6+ because the mongodb-ephemeral template has been removed. It is included in this repo, and you can reinstall it by running:

    oc create -f mongodb-ephemeral-template.json

You can also get it from the source at: https://github.com/openshift/origin/blob/master/examples/db-templates/mongodb-ephemeral-template.json

    
# create kafka cluster
*This requires that the "Red Hat Integration - AMQ Streams" operator be installed on the cluster. That operator will take the configuration from this yaml file, and deploy a 3-node kafka cluster.

from the root of the dotnet-cafe-demo folder, run the create-kafka.yml script on OpenShift

    oc create -f create-kafka.yml

# dotnet.cafe.kitchen
build and deploy the dotnet.cafe.kitchen service and dependencies. 

The DOTNET_STARTUP_PROJECT parameter tells the build command which project should be build and run since multiple services are in the same solution, and then builds the dependencies as well. It also sets the DOTNET_CAFE_KAFKA_BOOTSTRAP runtime parameter to link to the kafka cluster we created above.

If you fork the project, you'll need to change the git link

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
      
   