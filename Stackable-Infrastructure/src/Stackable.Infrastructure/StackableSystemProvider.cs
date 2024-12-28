namespace Stackable.Infrastructure;

internal class StackableSystemProvider
{
    /// <summary>
    /// プロジェクト名
    /// </summary>
    public string ProjectName { get; init; }

    /// <summary>
    /// 環境名
    /// </summary>
    public string Environment { get; init; }

    /// <summary>
    /// IDを取得する
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string GetId(string name)
    {
        return $"{ProjectName}-{Environment}-{name}";
    }
}
