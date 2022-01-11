using System;

namespace Nut.MediatR;

/// <summary>
/// 認可が失敗したことを表す例外を定義します。
/// </summary>
public class UnauthorizedException : Exception
{
    /// <summary>
    /// メッセージを指定してインスタンスを初期化します。
    /// </summary>
    /// <param name="message">認可が失敗した理由を表すメッセージ</param>
    public UnauthorizedException(string message) : base(message)
    {
    }
}
