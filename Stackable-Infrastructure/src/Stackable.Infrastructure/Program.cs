using Amazon.CDK;
using Stackable.Infrastructure;

var systemProvider = new StackableSystemProvider
{
    Environment = "dev",
    ProjectName = "stackable",
};

var app = new App();
_ = new StackableStack(app, systemProvider.GetId("stack"), new StackProps
{
    Env = new Environment
    {
        Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
        Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION"),
    },
}, systemProvider);
app.Synth();