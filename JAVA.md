Some Java vs C# differences explained

# Project Structure
## Domain Objects
Maybe just me, but I've always found the deeply nested folders in java code annoying, we generally don't do so much of that in C#, but larger projects head in that direction.
### Java
quarkus-cafe-barista

    /src/main/java/com/redhat/quarkus/cafe/barista/domain/Barista.java

### C#
quarkus.cafe.barista

    /Domain/Barista.cs

## Services
KafkaService.java vs BaristaKafkaService.cs - I only deviated here because it's a pain when you're debugging multiple microservices and they use the same class name; it leads to unnecessary confusion.
### Java
quarkus-cafe-barista

    /src/main/java/com/redhat/quarkus/cafe/barista/infrastructure/KafkaService.java

### C#
quarkus.cafe.barista

    /Domain/Service/BaristaKafkaService.cs

## References
## Java
I guess this depends on your Java environment (Maven, Gradle, or something else), but your external packages and internal references are in these pom.xml files. 

quarkus-cafe-barista/pom.xml

## C#
In C#, these go into the .csproj file in the root of each project like:

dotnet.cafe.barista/dotnet.cafe.barista.csproj


If you're using an IDE, you pretty much never edit these by hand.

# Serialization
### Java
For Quarkus, we use the @RegisterForReflection annotation to tell the SAXParser how to serialize to JSON as per:
https://quarkus.io/guides/writing-native-applications-tips#alternative-with-registerforreflection
    
    @RegisterForReflection
    public class LineItem {
        public Item item;
        public String name;   
        ...  
### C#
In C#, we're using the relatively new System.Text.Json serializer/deserializer, which requires get/set for properties and behaves slightly differently from the Newtonsoft.Json library everyone used to use. Docs:
https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to

    public class LineItem
    {
        public Item item { get; set; }
        public String name { get; set; }
        ...

This makes the marshalling code really easy, and we do it throughout like:
        
        OrderInEvent orderIn = JsonSerializer.Deserialize<OrderInEvent>(message);
        
# Lambda Expressions
Glad you guys finally got these in Java 8, because we've been doing this in C# since 3.0 in 2007
### Java

    kitchen.make(orderIn).thenApply(o -> {

### C#
    _kitchen.make(orderIn).ContinueWith(async kitchenOutput => {

# Async
Each of these functions takes in an OrderInEvent, and returns a list of events wrapped in asynchronous code.

Note that CompletableFuture and Task are very similar, but a good writeup of the differences (if you want to go down the rabbit hole) is available here:
    https://softwareengineering.stackexchange.com/questions/393264/is-the-c-async-task-construct-equivalent-to-javas-executor-future
    
### Java
    public CompletableFuture<Collection<Event>> make(final OrderInEvent orderInEvent) {
### C#
    public async Task<List<Event>> make(OrderInEvent orderInEvent)