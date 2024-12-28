using Amazon.CDK.AWS.EC2;

namespace Stackable.Infrastructure;

internal class VpcStack : Construct
{
    public Vpc Vpc { get; private init; }

    public VpcStack(Construct scope, string id, StackableSystemProvider systemProvider) : base(scope, id)
    {
        Vpc = new Vpc(this, systemProvider.GetId("vpc"), new VpcProps
        {
            IpAddresses = IpAddresses.Cidr("10.0.0.0/16"),
            MaxAzs = 2,
            NatGateways = 1,
            SubnetConfiguration =
            [
                new SubnetConfiguration
                {
                    Name = systemProvider.GetId("public-subnet1"),
                    SubnetType = SubnetType.PUBLIC,
                    CidrMask = 24,

                },
                new SubnetConfiguration
                {
                    Name = systemProvider.GetId("private-subnet1"),
                    SubnetType = SubnetType.PRIVATE_WITH_EGRESS,
                    CidrMask = 24,
                },
                new SubnetConfiguration
                {
                    Name = systemProvider.GetId("private-subnet2"),
                    SubnetType = SubnetType.PRIVATE_ISOLATED,
                    CidrMask = 24,
                },
            ],
        });
    }
}
