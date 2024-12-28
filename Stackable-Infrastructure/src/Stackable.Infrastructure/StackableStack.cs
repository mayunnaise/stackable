using Amazon.CDK;
using Constructs;

namespace Stackable.Infrastructure;

public class StackableStack : Stack
{
    internal StackableStack(Construct scope, string id, IStackProps props, StackableSystemProvider systemProvider) : base(scope, id, props)
    {
        var s3Stack = new S3Stack(this, systemProvider.GetId("s3-stack"), systemProvider);

    }
}
