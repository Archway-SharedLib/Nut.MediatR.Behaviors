using System;

namespace Nut.MediatR.ServiceLike;

/// <summary>
/// ログ出力するインターフェイスを定義します。
/// </summary>
public interface IServiceLikeLogger
{
    /// <summary>
    /// 情報レベルのログを出力します、
    /// </summary>
    /// <param name="message">出力するメッセージ</param>
    /// <param name="args">ログに出力する追加パラメーター</param>
    void Info(string message, params object[] args);

    /// <summary>
    /// エラーレベルのログを出力します。
    /// </summary>
    /// <param name="ex">発生した例外</param>
    /// <param name="message">出力するメッセージ</param>
    /// <param name="args">ログに出力する追加パラメーター</param>
    void Error(Exception ex, string message, params object[] args);

    /// <summary>
    /// トレースレベルのログを出力します、
    /// </summary>
    /// <param name="message">出力するメッセージ</param>
    /// <param name="args">ログに出力する追加パラメーター</param>
    void Trace(string message, params object[] args);

    /// <summary>
    /// トレースレベルのログ出力が有効かどうかを取得します。
    /// </summary>
    /// <returns>トレースレベルのログ出力が有効な場合は <see langword="true"/> 、そうでない場合は <see langword="false"/></returns>
    bool IsTraceEnabled();

    /// <summary>
    /// 情報レベルのログ出力が有効かどうかを取得します。
    /// </summary>
    /// <returns>情報レベルのログ出力が有効な場合は <see langword="true"/> 、そうでない場合は <see langword="false"/></returns>
    bool IsInfoEnabled();

    /// <summary>
    /// エラーレベルのログ出力が有効かどうかを取得します。
    /// </summary>
    /// <returns>エラーレベルのログ出力が有効な場合は <see langword="true"/> 、そうでない場合は <see langword="false"/></returns>
    bool IsErrorEnabled();
}
