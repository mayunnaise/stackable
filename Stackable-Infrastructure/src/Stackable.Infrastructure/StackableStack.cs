namespace Stackable.Infrastructure;

public class StackableStack : Stack
{
    internal StackableStack(Construct scope, string id, IStackProps props, StackableSystemProvider systemProvider) : base(scope, id, props)
    {
        var s3Stack = new S3Stack(this, systemProvider.GetId("s3-stack"), systemProvider);

        var vpcStack = new VpcStack(this, systemProvider.GetId("vpc-stack"), systemProvider);

        var bastionStack = new Ec2Stack(this, systemProvider.GetId("bastion-stack"), systemProvider, vpcStack.Vpc);

        var rdsStack = new RdsStack(this, systemProvider.GetId("rds-stack"), systemProvider, vpcStack.Vpc, bastionStack.BastionSecurityGroup);
    }
}
