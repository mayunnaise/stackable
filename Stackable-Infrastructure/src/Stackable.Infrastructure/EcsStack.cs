using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.IAM;

namespace Stackable.Infrastructure;
internal class EcsStack : Construct
{
    public ISecurityGroup FargateSecurityGroup { get; private set; }

    public EcsStack(Construct scope, string id, StackableSystemProvider systemProvider, Vpc vpc) : base(scope, id)
    {
        FargateSecurityGroup = new SecurityGroup(this, systemProvider.GetId("fargate-sg"), new SecurityGroupProps
        {
            SecurityGroupName = systemProvider.GetId("fargate-sg"),
            Vpc = vpc,
            AllowAllOutbound = true,
        });

        var cluster = new Cluster(this, systemProvider.GetId("fargate-ecs-cluster"), new ClusterProps
        {
            ClusterName = systemProvider.GetId("fargate-ecs-cluster"),
            Vpc = vpc,
        });

        var appRepository = new Repository(this, systemProvider.GetId("app-ecr"), new RepositoryProps
        {
            RepositoryName = systemProvider.GetId("app-ecr"),
        });

        var taskRole = new Role(this, systemProvider.GetId("fargate-role"), new RoleProps
        {
            RoleName = systemProvider.GetId("fargate-role"),
            AssumedBy = new ServicePrincipal("ecs-tasks.amazonaws.com"),
        });

        // SecretsManagerからの取得を許可
        taskRole.AddToPolicy(new PolicyStatement(new PolicyStatementProps
        {
            Actions = ["secretsmanager:GetSecretValue"],
            Resources = ["*"],
        }));

        var taskDefinition = new FargateTaskDefinition(this, systemProvider.GetId("ecs-task"), new FargateTaskDefinitionProps
        {
            TaskRole = taskRole,
        });
        taskDefinition.AddContainer(systemProvider.GetId("app-task"), new ContainerDefinitionProps
        {
            ContainerName = systemProvider.GetId("app-task"),
            Image = ContainerImage.FromEcrRepository(appRepository),
            PortMappings = [new PortMapping
            {
                ContainerPort = 8080,
                Protocol = Amazon.CDK.AWS.ECS.Protocol.TCP,
            }],
            Logging = new AwsLogDriver(new AwsLogDriverProps
            {
                StreamPrefix = systemProvider.GetId("app-log"),
            }),
        });

        _ = new ApplicationLoadBalancedFargateService(this, systemProvider.GetId("alb"), new ApplicationLoadBalancedFargateServiceProps
        {
            ServiceName = systemProvider.GetId("alb"),
            Cluster = cluster,
            DesiredCount = 1,
            PublicLoadBalancer = true,
            MemoryLimitMiB = 512,
            TaskDefinition = taskDefinition,
            SecurityGroups = [FargateSecurityGroup],
        });
    }
}
