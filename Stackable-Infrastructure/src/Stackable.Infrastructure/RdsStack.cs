using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.RDS;
using System.Collections.Generic;

namespace Stackable.Infrastructure;
internal class RdsStack : Construct
{
    public DatabaseInstance RdsInstance { get; private set; }

    public RdsStack(
        Construct scope,
        string id,
        StackableSystemProvider systemProvider,
        Vpc vpc,
        ISecurityGroup bastionSecurityGroup,
        List<ISecurityGroup> securityGroups) : base(scope, id)
    {
        var rdsSecurityGroup = new SecurityGroup(this, systemProvider.GetId("rds-sg"), new SecurityGroupProps
        {
            SecurityGroupName = systemProvider.GetId("rds-sg"),
            Vpc = vpc,
            AllowAllOutbound = true,
        });

        // 踏み台サーバーからRDSへのアクセスを許可
        rdsSecurityGroup.AddIngressRule(bastionSecurityGroup, Port.Tcp(3306), "Allow MySQL access from Bastion host");

        // VPC内の各サービスからのアクセスを許可
        foreach (var securityGroup in securityGroups)
        {
            rdsSecurityGroup.AddIngressRule(securityGroup, Port.Tcp(3306), $"Allow MySQL access from tasks");
        }

        RdsInstance = new DatabaseInstance(this, systemProvider.GetId("db"), new DatabaseInstanceProps
        {
            Engine = DatabaseInstanceEngine.Mysql(new MySqlInstanceEngineProps
            {
                Version = MysqlEngineVersion.VER_8_4_3
            }),
            InstanceType = Amazon.CDK.AWS.EC2.InstanceType.Of(InstanceClass.BURSTABLE4_GRAVITON, InstanceSize.MICRO),
            Vpc = vpc,
            VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PRIVATE_ISOLATED },
            SecurityGroups = [rdsSecurityGroup],
            MultiAz = false,
            AllocatedStorage = 20,
            StorageType = StorageType.GP2,
            DeletionProtection = false,
            DatabaseName = $"{systemProvider.ProjectName}_{systemProvider.Environment}_db",
            // DB接続情報のSecretsManagerの保存先
            Credentials = Credentials.FromGeneratedSecret("admin", new CredentialsBaseOptions
            {
                SecretName = $"{systemProvider.ProjectName}-db-secret",
            }),
            // スタック削除時にRDSインスタンスを削除する
            RemovalPolicy = RemovalPolicy.DESTROY,
        });
    }
}
