using System;
using System.Threading.Tasks;

namespace Nut.MediatR.ServiceLike;

/// <summary>
/// <see cref="MediatR"/> を利用してメッセージを送信するクライアントのインターフェイスを定義します。
/// </summary>
public interface IMediatorClient
{
    /// <summary>
    /// メッセージを送信します。
    /// </summary>
    /// <typeparam name="TResult">結果の型</typeparam>
    /// <param name="path">送信する先のパス</param>
    /// <param name="request">送信するデータ</param>
    /// <returns>送信先が返した結果を含む非同期オブジェクト</returns>
    Task<TResult?> SendAsync<TResult>(string path, object request) where TResult : class;

    /// <summary>
    /// メッセージを送信します。
    /// </summary>
    /// <param name="path">送信する先のパス</param>
    /// <param name="request">送信するデータ</param>
    /// <returns>非同期オブジェクト</returns>
    Task SendAsync(string path, object request);

    /// <summary>
    /// メッセージを発行します。
    /// </summary>
    /// <param name="key">メッセージの名前</param>
    /// <param name="eventData">発行するデータ</param>
    /// <returns>非同期オブジェクト</returns>
    Task PublishAsync(string key, object @eventData);

    /// <summary>
    /// オプションを指定してメッセージを発行します。
    /// </summary>
    /// <param name="key">メッセージの名前</param>
    /// <param name="eventData">発行するデータ</param>
    /// <param name="options">発行するメッセージのオプション</param>
    /// <returns>非同期オブジェクト</returns>
    Task PublishAsync(string key, object @eventData, PublishOptions options);

    /// <summary>
    /// オプションを指定してメッセージを発行します。
    /// </summary>
    /// <param name="key">メッセージの名前</param>
    /// <param name="eventData">発行するデータ</param>
    /// <param name="options">発行するメッセージのオプションを設定する処理</param>
    /// <returns>非同期オブジェクト</returns>
    Task PublishAsync(string key, object @eventData, Action<PublishOptions> options);
}
