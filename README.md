# dotnet-cafe-demo

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
from the root of the dotnet-cafe-demo folder, run the create-kafka.yml script on OpenShift

    oc create -f create-kafka.yml

# dotnet.cafe.core
build and deploy the dotnet.cafe.counter service and dependencies

    oc new-app \
      -n dotnet-cafe-demo \
      --name=dotnet-cafe-core dotnet:3.1~https://github.com/vincent-tsugranes/dotnet-cafe-demo.git \
      --build-env DOTNET_STARTUP_PROJECT=dotnet.cafe.core/dotnet.cafe.counter.csproj
 
