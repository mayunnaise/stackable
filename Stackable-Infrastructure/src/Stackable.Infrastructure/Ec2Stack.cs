using Amazon.CDK.AWS.EC2;

namespace Stackable.Infrastructure;
internal class Ec2Stack : Construct
{
    public SecurityGroup BastionSecurityGroup { get; private set; }

    public Ec2Stack(Construct scope, string id, StackableSystemProvider systemProvider, Vpc vpc) : base(scope, id)
    {
        // 秘密鍵の保存名
        var keyPair = new KeyPair(this, systemProvider.GetId("ec2-kp"), new KeyPairProps
        {
            KeyPairName = systemProvider.GetId("ec2-kp"),
        });

        BastionSecurityGroup = new SecurityGroup(this, systemProvider.GetId("bastion-sg"), new SecurityGroupProps
        {
            Vpc = vpc,
            AllowAllOutbound = true,
        });

        // 指定したIPアドレスからのみ踏み台サーバーとへのSSHアクセスを許可
        BastionSecurityGroup.AddIngressRule(Peer.AnyIpv4(), Port.Tcp(22), "Allow SSH access from my IP");

        _ = new Instance_(this, systemProvider.GetId("bastion"), new InstanceProps
        {
            InstanceName = systemProvider.GetId("bastion"),
            Vpc = vpc,
            VpcSubnets = new SubnetSelection
            {
                SubnetType = SubnetType.PUBLIC,
            },
            SecurityGroup = BastionSecurityGroup,
            InstanceType = InstanceType.Of(InstanceClass.T2, InstanceSize.MICRO),
            MachineImage = new AmazonLinuxImage(new AmazonLinuxImageProps
            {
                Generation = AmazonLinuxGeneration.AMAZON_LINUX_2023,
                CpuType = AmazonLinuxCpuType.X86_64
            }),
            KeyPair = keyPair,
        });
    }
}
