namespace Nut.MediatR;

/// <summary>
/// 認可結果を表します。
/// </summary>
public sealed class AuthorizationResult
{
    private AuthorizationResult()
    {
    }

    /// <summary>
    /// 認可が失敗した原因となる内容のメッセージを取得します。
    /// </summary>
    public string? FailureMessage { get; private set; }

    /// <summary>
    /// 認可が成功したかどうかを取得します。
    /// </summary>
    public bool Succeeded { get; private set; }

    /// <summary>
    /// 認可が失敗した場合の<see cref="AuthorizationResult"/>のインスタンスを返します。
    /// </summary>
    /// <param name="failureMessage">失敗した原因となる内容となるメッセージ</param>
    /// <returns>認可が失敗した場合の<see cref="AuthorizationResult"/>のインスタンス</returns>
    public static AuthorizationResult Failed(string failureMessage)
    {
        return new AuthorizationResult() { Succeeded = false, FailureMessage = failureMessage };
    }

    /// <summary>
    /// 認可が成功した場合の<see cref="AuthorizationResult"/>のインスタンスを返します。
    /// </summary>
    /// <returns>認可が成功した場合の<see cref="AuthorizationResult"/>のインスタンス</returns>
    public static AuthorizationResult Success()
    {
        return new AuthorizationResult() { Succeeded = true };
    }
}
