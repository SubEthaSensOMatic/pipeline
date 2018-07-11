# pipeline
Asynchronous ASP.NET Core like pipeline implementation

### Usage

Create a context Class ...
```csharp
    public class MyPipelineContext 
    {
        public string SharedInformationBetweenItems { get; set; }
    }
```

Create items for your pipeline ...

```csharp
    public class MyPipelineItem: IPipelineItem<MyPipelineContext> 
    {
        public async Task run(MyPipelineContext ctx, Func<Task> next) 
        {
            await next();
            
            Console.WriteLine(ctx.SharedInformationBetweenItems);
        }
    }
```

Configure and run your pipeline ...

```csharp
  
  ...
  
  // without dependency injection
  // var builder = new PipelineBuilder();
 
  // or with dependency injection 
  var builder = new PipelineBuilder(itemType => container.Resolve(itemType));
  
  // Add delegate item to pipeline
  builder.Use(async (ctx, next) => {
    ctx.SharedInformationBetweenItems = "Some text...";
    
    // don't forget to call 
    await next();
  });
  
  // Use item object
  builder.Use<MyPipelineItem>();

  // Build pipeline ...
  var pipeline = builder.BuildPipeline();
  
  // ... and run
  var myContext = new MyPipelineContext();
  await pipeline(myContext);
  
  // ... run again with new context
  myContext = new MyPipelineContext();
  await pipeline(myContext);
  
  ...
```
