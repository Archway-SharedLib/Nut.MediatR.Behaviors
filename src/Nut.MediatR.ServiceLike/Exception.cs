using System;
using MediatR;

namespace Nut.MediatR.ServiceLike;

/// <summary>
/// <see cref="Nut.MediatR.ServiceLike"/> で発生した例外のベースクラスを定義します。
/// </summary>
public class MediatRServiceLikeException : Exception
{
    /// <summary>
    /// インスタンスを初期化します。
    /// </summary>
    public MediatRServiceLikeException() : base()
    {
    }

    /// <summary>
    /// インスタンスを初期化します。
    /// </summary>
    /// <param name="message">エラーに関するメッセージ</param>
    public MediatRServiceLikeException(string message) : base(message)
    {
    }
}

/// <summary>
/// 指定されたパスに紐づいた<see cref="IRequest"/>が見つからない場合に発生します。
/// </summary>
public class RequestNotFoundException: MediatRServiceLikeException
{
    /// <summary>
    /// インスタンスを初期化します。
    /// </summary>
    /// <param name="requestPath">送信先のパス</param>
    public RequestNotFoundException(string requestPath) : base(Resources.Strings.MediatorRequestNotFound(requestPath))
    {
        RequestPath = requestPath;
    }

    /// <summary>
    /// インスタンスを初期化します。
    /// </summary>
    /// <param name="requestPath">送信先のパス</param>
    /// <param name="message">エラーに関するメッセージ</param>
    public RequestNotFoundException(string requestPath, string message) : base(message)
    {
        RequestPath = requestPath;
    }

    /// <summary>
    /// 送信先のパスを取得します。
    /// </summary>
    public string RequestPath { get; }
}
