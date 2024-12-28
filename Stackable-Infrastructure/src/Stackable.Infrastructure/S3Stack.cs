using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using Constructs;

namespace Stackable.Infrastructure;

internal class S3Stack : Construct
{
    public S3Stack(Construct scope, string id, StackableSystemProvider systemProvider) : base(scope, id)
    {
        var bucket = new Bucket(this, systemProvider.GetId("test-bucket"), new BucketProps
        {
            BucketName = systemProvider.GetId("test-bucket"),
            Versioned = true,
            RemovalPolicy = RemovalPolicy.DESTROY,
        });
    }
}
